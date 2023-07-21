using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using SME.SR.Data.Models;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class MontarBoletinsQueryHandler : IRequestHandler<MontarBoletinsQuery, List<RelatorioBoletimSimplesEscolarDto>>
    {
        private const double FREQUENCIA_100 = 100;
        private const int PERCENTUAL_FREQUENCIA_PRECISAO = 2;

        private const int BIMESTRE_1 = 1;
        private const int BIMESTRE_2 = 2;
        private const int BIMESTRE_3 = 3;
        private const int BIMESTRE_4 = 4;
        private const int BIMESTRE_FINAL = 0;
        private readonly IMediator mediator;
        private readonly ITipoCalendarioRepository tipoCalendarioRepository;

        public MontarBoletinsQueryHandler(IMediator mediator,
                                          ITipoCalendarioRepository tipoCalendarioRepository)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this.tipoCalendarioRepository = tipoCalendarioRepository ?? throw new ArgumentNullException(nameof(tipoCalendarioRepository));
        }

        public async Task<List<RelatorioBoletimSimplesEscolarDto>> Handle(MontarBoletinsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var turmas = request.Turmas;
                var dre = request.Dre;
                var ue = request.Ue;
                var componentesCurriculares = request.ComponentesCurriculares;
                var alunos = request.AlunosPorTuma;
                var notas = request.Notas;
                var frequencia = request.Frequencias;
                var frequenciasGlobal = request.FrequenciasGlobal;
                var tiposNota = request.TiposNota;
                var mediasFrequencia = request.MediasFrequencia;
                var pareceresConclusivos = request.PareceresConclusivos;
                var aulasPrevistas = request.AulasPrevistas;

                var registroFrequencia = await mediator.Send(new ObterTotalAulasTurmaEBimestreEComponenteCurricularQuery(turmas.Select(a => a.Codigo).ToArray(), 0, new string[] { }, new int[] { 1, 2, 3, 4 }));
                var periodoAtual = await mediator.Send(new ObterBimestrePeriodoFechamentoAtualQuery(request.Turmas.Select(a => a.AnoLetivo).FirstOrDefault()));

                var relatorioBoletimSimplesEscolar = new List<RelatorioBoletimSimplesEscolarDto>();

                foreach (var aluno in alunos)
                {
                    if (turmas.Any(t => aluno.Any(a => a.CodigoTurma.ToString() == t.Codigo || a.CodigoTurma.ToString() == t.RegularCodigo)))
                    {
                        var turma = turmas.First(t => aluno.Any(a => a.CodigoTurma.ToString() == t.Codigo));

                        var conselhoClasseBimestres = await mediator.Send(new AlunoConselhoClasseCadastradoBimestresQuery(aluno.Key, turma.AnoLetivo, turma.ModalidadeCodigo, turma.Semestre));

                        if (conselhoClasseBimestres != null && conselhoClasseBimestres.Any())
                        {
                            var boletimAluno = new RelatorioBoletimSimplesEscolarDto();

                            if (!componentesCurriculares.Any(c => c.Key == aluno.Key))
                                continue;

                            var componentesAluno = componentesCurriculares.First(c => c.Key == aluno.Key);

                            foreach (var turmaAluno in componentesAluno.Select(s => s.CodigoTurma).Distinct())
                                MapearComponentesOrdenadosGrupo(componentesAluno.Where(cc => cc.CodigoTurma == turmaAluno.ToString()), boletimAluno);

                            var notasAluno = notas.FirstOrDefault(t => t.Key == aluno.First().CodigoAluno.ToString());

                            boletimAluno.ModalidadeTurma = turma.ModalidadeCodigo;

                            var frequenciasAluno = frequencia?
                                .Where(t => t.Key == aluno.First().CodigoAluno.ToString())
                                .SelectMany(f => f);

                            var frequenciasTurma = frequencia?
                                .SelectMany(a => a)
                                .Where(f => f.TurmaId == turma.Codigo);

                            var anoLetivo = turma.AnoLetivo;
                            var mediasFrequenciaAnoLetivo = mediasFrequencia?.Where(c => c.Ano == anoLetivo).Select(c => c);

                            if (notasAluno != null && notasAluno.Any())
                            {
                                await SetarNotasFrequencia(boletimAluno,
                                    notasAluno,
                                    frequenciasAluno,
                                    frequenciasTurma,
                                    mediasFrequenciaAnoLetivo ?? mediasFrequencia,
                                    conselhoClasseBimestres,
                                    registroFrequencia,
                                    periodoAtual,
                                    aulasPrevistas);
                            }

                            var frequenciaGlobal = frequenciasGlobal?
                                .FirstOrDefault(t => t.Key == aluno.First().CodigoAluno.ToString());

                            var percentualFrequenciaGlobal = frequenciaGlobal != null ? frequenciaGlobal.First().PercentualFrequencia : 100;
                            var parecerConclusivo = pareceresConclusivos.FirstOrDefault(c => c.TurmaId.ToString() == turma.Codigo && c.AlunoCodigo.ToString() == aluno.Key);

                            boletimAluno.Cabecalho = ObterCabecalhoInicial(dre,
                                ue,
                                turma,
                                aluno.First().CodigoAluno.ToString(),
                                aluno.OrderBy(a => a.DataSituacao).Last().NomeRelatorio,
                                aluno.First().ObterNomeFinal(),
                                $"{percentualFrequenciaGlobal.ToString($"N{PERCENTUAL_FREQUENCIA_PRECISAO}", CultureInfo.CurrentCulture)}%");

                            boletimAluno.ParecerConclusivo = parecerConclusivo?.ParecerConclusivo;

                            relatorioBoletimSimplesEscolar.Add(boletimAluno);
                        }
                    }
                }

                if (!relatorioBoletimSimplesEscolar.Any())
                    throw new NegocioException("Não foram encontradas informações para geração do boletim");

                return OrdenarBoletins(relatorioBoletimSimplesEscolar);
            }
            catch (Exception ex)
            {
                var mensagem = $"Não foi possível montar boletim - Motivo: {ex.Message}";

                if (ex is NegocioException)
                    throw new NegocioException(mensagem);

                throw new Exception(mensagem);
            }
        }

        private List<RelatorioBoletimSimplesEscolarDto> OrdenarBoletins(List<RelatorioBoletimSimplesEscolarDto> boletinsAlunos)
        {
            var boletinsOrdenados = new List<RelatorioBoletimSimplesEscolarDto>();
            var turmas = boletinsAlunos.Select(b => b.Cabecalho.NumeroTurma).Distinct();

            foreach (string turma in turmas.OrderBy(t => t))
            {
                var alunosTurma = boletinsAlunos.Where(a => a.Cabecalho.NumeroTurma == turma).OrderBy(a => a.Cabecalho.NomeAlunoOrdenacao).ToList();
                boletinsOrdenados.AddRange(alunosTurma);
            }

            return boletinsOrdenados;
        }

        private BoletimEscolarCabecalhoDto ObterCabecalhoInicial(Dre dre, Ue ue, Turma turma, string alunoCodigo, string nome, string nomeAlunoOrdenacao, string frequenciaGlobal)
        {
            return new BoletimEscolarCabecalhoDto()
            {
                Data = DateTime.Now.ToString("dd/MM/yyyy"),
                NomeDre = dre.Abreviacao,
                NomeUe = ue.NomeRelatorio,
                NomeTurma = turma.NomeRelatorio,
                NumeroTurma = turma.Nome,
                CodigoEol = alunoCodigo,
                Aluno = nome,
                NomeAlunoOrdenacao = nomeAlunoOrdenacao,
                AnoLetivo = turma.AnoLetivo.ToString(),
                FrequenciaGlobal = frequenciaGlobal
            };
        }

        private void MapearComponentesOrdenadosGrupo(IEnumerable<ComponenteCurricularPorTurma> componentesCurricularesPorTurma, RelatorioBoletimSimplesEscolarDto boletim)
        {
            var componentesTurmaComAreaOrdem = componentesCurricularesPorTurma.Where(w => w.AreaDoConhecimento != null && !w.TerritorioSaber && w.AreaDoConhecimento.Ordem.HasValue).OrderBy(c => c.GrupoMatriz.Id).ThenBy(d => d.Disciplina).ToList();
            var componentesTurmaComAreaSemOrdem = componentesCurricularesPorTurma.Where(w => w.AreaDoConhecimento != null && !w.TerritorioSaber && !w.AreaDoConhecimento.Ordem.HasValue).OrderBy(c => c.GrupoMatriz.Id).ThenBy(b => b.AreaDoConhecimento.Nome).ThenBy(b => b.Disciplina).ToList();

            var componentesTurmaSemArea = componentesCurricularesPorTurma.Where(w => w.AreaDoConhecimento == null && !w.TerritorioSaber).OrderBy(a => a.GrupoMatriz.Id).ThenBy(b => b.Disciplina).ToList();
            var componentesTerritorioSaber = componentesCurricularesPorTurma.Where(w => w.TerritorioSaber).OrderBy(c=> c.Disciplina).ThenBy(o => o.OrdemTerritorioSaber).ToList();
            var componentesTurma = componentesTurmaComAreaOrdem.Concat(componentesTurmaSemArea).Concat(componentesTerritorioSaber).Concat(componentesTurmaComAreaSemOrdem);

            foreach (var componente in componentesTurma)
            {
                if (componente.Regencia && componente.ComponentesCurricularesRegencia != null && componente.ComponentesCurricularesRegencia.Any())
                {
                    boletim.ComponenteCurricularRegencia = new ComponenteCurricularRegenciaDto()
                    {
                        Codigo = componente.CodDisciplina.ToString(),
                        Frequencia = componente.Frequencia
                    };

                    foreach (var componenteRegencia in componente.ComponentesCurricularesRegencia.OrderBy(a => a.Disciplina))
                    {
                        if (!boletim.ComponenteCurricularRegencia.ComponentesCurriculares.Any(g => g.Codigo == componenteRegencia.CodDisciplina.ToString()))
                            boletim.ComponenteCurricularRegencia.ComponentesCurriculares.Add(
                                new ComponenteCurricularRegenciaNotaDto()
                                {
                                    Codigo = componenteRegencia.CodDisciplina.ToString(),
                                    Nome = componenteRegencia.Disciplina,
                                    Nota = componenteRegencia.LancaNota
                                });
                    }
                }
                else if (!componente.Regencia)
                {
                    boletim.ComponentesCurriculares ??= new List<ComponenteCurricularDto>();

                    if (!boletim.ComponentesCurriculares.Any(g => !componente.TerritorioSaber && g.Codigo == componente.CodDisciplina.ToString()) ||
                        !boletim.ComponentesCurriculares.Any(g => componente.TerritorioSaber && g.Codigo == componente.CodDisciplina.ToString() && g.Professor == componente.Professor))
                    {
                        boletim.ComponentesCurriculares.Add(
                            new ComponenteCurricularDto()
                            {
                                Codigo = componente.CodDisciplina.ToString(),
                                CodigoTerritorioSaber = componente.CodigoTerritorioSaber.ToString(),
                                Nome = componente.Disciplina,
                                Nota = componente.LancaNota,
                                Frequencia = componente.Frequencia,
                                Grupo = componente.GrupoMatriz.Id,
                                Professor = componente.Professor,
                                TerritorioSaber = componente.TerritorioSaber
                            });
                    }
                }
            }
        }

        private async Task SetarNotasFrequencia(RelatorioBoletimSimplesEscolarDto boletim, IEnumerable<NotasAlunoBimestreBoletimSimplesDto> notas, IEnumerable<FrequenciaAluno> frequenciasAluno, IEnumerable<FrequenciaAluno> frequenciasTurma,
            IEnumerable<MediaFrequencia> mediasFrequencia, IEnumerable<int> conselhoClasseBimestres, IEnumerable<TurmaComponenteQtdAulasDto> registroFrequencia, int periodoAtual, IEnumerable<TurmaComponenteQuantidadeAulasDto> aulasPrevistas)
        {
            var aulasPrevistasTurma = aulasPrevistas.Where(a => a.TurmaCodigo == frequenciasTurma.FirstOrDefault().TurmaId);

            var transformarNotaEmConceito = !notas.Where(n => !string.IsNullOrEmpty(n.NotaConceito))
                .All(n => n.NotaConceito.ToCharArray().Where(c => c != '.').All(c => char.IsDigit(c)));

            if (boletim.ComponenteCurricularRegencia != null)
            {
                boletim.EhRegencia = true;

                if (boletim.ComponenteCurricularRegencia.Frequencia)
                {
                    var frequenciasAlunoRegencia = frequenciasAluno?.Where(f => f.DisciplinaId == boletim.ComponenteCurricularRegencia.Codigo);

                    boletim.ComponenteCurricularRegencia.FrequenciaBimestre1 = ObterFrequenciaBimestre(conselhoClasseBimestres,frequenciasAlunoRegencia, BIMESTRE_1);
                    boletim.ComponenteCurricularRegencia.FrequenciaBimestre2 = ObterFrequenciaBimestre(conselhoClasseBimestres, frequenciasAlunoRegencia, BIMESTRE_2);
                    boletim.ComponenteCurricularRegencia.FrequenciaBimestre3 = ObterFrequenciaBimestre(conselhoClasseBimestres, frequenciasAlunoRegencia, BIMESTRE_3);
                    boletim.ComponenteCurricularRegencia.FrequenciaBimestre4 = ObterFrequenciaBimestre(conselhoClasseBimestres, frequenciasAlunoRegencia, BIMESTRE_4);

                    boletim.ComponenteCurricularRegencia.FrequenciaFinal = await ObterFrequenciaFinalAluno(frequenciasAlunoRegencia, BIMESTRE_FINAL, conselhoClasseBimestres);
                }

                foreach (var componenteCurricular in boletim.ComponenteCurricularRegencia.ComponentesCurriculares)
                {
                    if (componenteCurricular.Nota)
                    {
                        var notaFrequenciaComponente = notas?.Where(nf => nf.CodigoComponenteCurricular == componenteCurricular.Codigo);

                        componenteCurricular.NotaBimestre1 = ObterNotaBimestre(conselhoClasseBimestres, notaFrequenciaComponente, BIMESTRE_1, transformarNotaEmConceito);
                        componenteCurricular.NotaBimestre2 = ObterNotaBimestre(conselhoClasseBimestres, notaFrequenciaComponente, BIMESTRE_2, transformarNotaEmConceito);
                        componenteCurricular.NotaBimestre3 = ObterNotaBimestre(conselhoClasseBimestres, notaFrequenciaComponente, BIMESTRE_3, transformarNotaEmConceito);
                        componenteCurricular.NotaBimestre4 = ObterNotaBimestre(conselhoClasseBimestres, notaFrequenciaComponente, BIMESTRE_4, transformarNotaEmConceito);

                        componenteCurricular.NotaFinal = ObterNotaBimestre(conselhoClasseBimestres, notaFrequenciaComponente, BIMESTRE_FINAL, transformarNotaEmConceito);
                    }
                }
            }

            if (boletim.ComponentesCurriculares != null && boletim.ComponentesCurriculares.Any())
            {
                foreach (var componenteCurricular in boletim.ComponentesCurriculares)
                {
                    var frequenciasAlunoComponente =
                         frequenciasAluno?.Where(f => f.DisciplinaId == componenteCurricular.Codigo || f.DisciplinaId == componenteCurricular.CodigoTerritorioSaber
                         && (!componenteCurricular.TerritorioSaber
                              || (componenteCurricular.TerritorioSaber && !string.IsNullOrEmpty(componenteCurricular.Professor) ? f.Professor == componenteCurricular.Professor : true)));

                    var frequenciaFinal = await ObterFrequenciaFinalAluno(frequenciasAlunoComponente, BIMESTRE_FINAL, conselhoClasseBimestres);

                    if (componenteCurricular.Nota)
                    {
                        var notasComponente = notas?.Where(n => n.CodigoComponenteCurricular == componenteCurricular.Codigo) ?? null;

                        componenteCurricular.NotaBimestre1 = ObterNotaBimestre(conselhoClasseBimestres, notasComponente, BIMESTRE_1, transformarNotaEmConceito);
                        componenteCurricular.NotaBimestre2 = ObterNotaBimestre(conselhoClasseBimestres, notasComponente, BIMESTRE_2, transformarNotaEmConceito);
                        componenteCurricular.NotaBimestre3 = ObterNotaBimestre(conselhoClasseBimestres, notasComponente, BIMESTRE_3, transformarNotaEmConceito);
                        componenteCurricular.NotaBimestre4 = ObterNotaBimestre(conselhoClasseBimestres, notasComponente, BIMESTRE_4, transformarNotaEmConceito);

                        componenteCurricular.NotaFinal = ObterNotaBimestre(conselhoClasseBimestres, notasComponente, BIMESTRE_FINAL, transformarNotaEmConceito);
                    }
                    else
                    {
                        componenteCurricular.NotaFinal = !string.IsNullOrEmpty(frequenciaFinal) &&
                            frequenciaFinal.ToCharArray()
                                .Where(c => c != ',' && c != '.').All(f => char.IsDigit(f)) ? ObterSintese(double.Parse(frequenciaFinal ?? "0"), mediasFrequencia, false, false, BIMESTRE_FINAL, conselhoClasseBimestres) : frequenciaFinal;
                    }


                    if (componenteCurricular.Frequencia)
                    {
                        var aulasCadastradas = registroFrequencia?
                            .Where(f => f.ComponenteCurricularCodigo == componenteCurricular.Codigo &&
                                (!componenteCurricular.TerritorioSaber || (componenteCurricular.TerritorioSaber && f.Professor == componenteCurricular.Professor)));

                        componenteCurricular.FrequenciaBimestre1 = ObterFrequenciaBimestre(conselhoClasseBimestres, frequenciasAlunoComponente, BIMESTRE_1);
                        componenteCurricular.FrequenciaBimestre2 = ObterFrequenciaBimestre(conselhoClasseBimestres, frequenciasAlunoComponente, BIMESTRE_2);
                        componenteCurricular.FrequenciaBimestre3 = ObterFrequenciaBimestre(conselhoClasseBimestres, frequenciasAlunoComponente, BIMESTRE_3);
                        componenteCurricular.FrequenciaBimestre4 = ObterFrequenciaBimestre(conselhoClasseBimestres, frequenciasAlunoComponente, BIMESTRE_4);
                        componenteCurricular.FrequenciaFinal = frequenciaFinal ?? string.Empty;
                    }
                }
            }
        }

        private string ObterNotaBimestre(IEnumerable<int> conselhoClasseBimestres, IEnumerable<NotasAlunoBimestreBoletimSimplesDto> notasComponente, int bimestre, bool transformarNotaEmConceito = false)
        {
            var retorno = !VerificaPossuiConselho(conselhoClasseBimestres, bimestre) ? "" :
                notasComponente?.FirstOrDefault(nc => nc.Bimestre == bimestre && nc.NotaConceito != null)?.NotaConceito;

            retorno = transformarNotaEmConceito && decimal.TryParse(retorno, out decimal valor) ?
                TransformarNotaEmConceito(decimal.Parse(retorno, CultureInfo.InvariantCulture)) : retorno;

            return String.IsNullOrEmpty(retorno) ? String.Empty : retorno;
        }

        private bool VerificaPossuiConselho(IEnumerable<int> conselhoClasseBimestres, int bimestre)
        {
            return conselhoClasseBimestres.Any(a => a == bimestre);
        }  

        private string ObterFrequenciaBimestre(IEnumerable<int> conselhoClasseBimestres, IEnumerable<FrequenciaAluno> frequenciasAlunoComponente, int bimestre)
        {
            var frequencia = !VerificaPossuiConselho(conselhoClasseBimestres, bimestre) ? "" :
                frequenciasAlunoComponente?.FirstOrDefault(nf => nf.Bimestre == bimestre)?.PercentualFrequenciaFormatado ?? string.Empty;

            return String.IsNullOrEmpty(frequencia) ? String.Empty : frequencia;
        }

        private async Task<string> ObterFrequenciaFinalAluno(IEnumerable<FrequenciaAluno> frequenciasAluno, int bimestre, IEnumerable<int> conselhoClasseBimestres)
        {
            if (frequenciasAluno == null || !frequenciasAluno.Any())
                return "-";
            else if (frequenciasAluno.FirstOrDefault(nf => nf.PeriodoEscolarId == null) != null)
                return frequenciasAluno.FirstOrDefault(nf => nf.PeriodoEscolarId == null).PercentualFrequenciaFormatado;
            else
            {
                var frequenciaFinal = new FrequenciaAluno()
                {
                    TotalAulas = frequenciasAluno.Sum(f => f.TotalAulas),
                    TotalAusencias = frequenciasAluno.Sum(f => f.TotalAusencias),
                    TotalCompensacoes = frequenciasAluno.Sum(f => f.TotalCompensacoes)
                };

                //Particularidade de cálculo de frequência para 2020.
                if (frequenciasAluno.First().AnoTurma.Equals(2020))
                {
                    var idTipoCalendario = tipoCalendarioRepository.ObterPorAnoLetivoEModalidade(frequenciasAluno.First().AnoTurma, frequenciasAluno.First().ModalidadeTurma).Result;
                    var periodos = await mediator.Send(new ObterPeriodosEscolaresPorTipoCalendarioQuery(idTipoCalendario));

                    periodos.ToList().ForEach(p =>
                    {
                        var frequencia = frequenciasAluno.SingleOrDefault(f => f.Bimestre.Equals(p.Bimestre));
                        frequenciaFinal.AdicionarFrequenciaBimestre(p.Bimestre, frequencia != null ? frequencia.PercentualFrequencia : (double?)null);
                    });

                    return frequenciaFinal.PercentualFrequenciaFinal?.ToString($"N{PERCENTUAL_FREQUENCIA_PRECISAO}", CultureInfo.CurrentCulture);
                }
                if (!VerificaPossuiConselho(conselhoClasseBimestres, bimestre))
                    return "-";
                return frequenciaFinal.PercentualFrequenciaFormatado;
            }
        }

        private string ObterSintese(double? frequenciaFinal, IEnumerable<MediaFrequencia> mediaFrequencias, bool regencia, bool lancaNota, int bimestre, IEnumerable<int> conselhoClasseBimestres)
        {
            if (!VerificaPossuiConselho(conselhoClasseBimestres, bimestre))
                return "";
            var frequenciaMedia = ObterFrequenciaMedia(mediaFrequencias, regencia, lancaNota);
            return frequenciaFinal.HasValue ? frequenciaFinal >= frequenciaMedia ? "F" : "NF" : "";
        }

        private bool TemFrequencia(IEnumerable<FrequenciaAluno> frequencias)
        {
            return (frequencias != null && frequencias.Any());
        }

        private double? ObterPercentualDeFrequencia(IEnumerable<FrequenciaAluno> frequenciaDisciplina)
        {
            return TemFrequencia(frequenciaDisciplina) ? frequenciaDisciplina.Sum(x => x.PercentualFrequencia) / frequenciaDisciplina.Count() : (double?)null;
        }

        private double ObterFrequenciaMedia(IEnumerable<MediaFrequencia> mediaFrequencias, bool regencia, bool lancaNota)
        {
            if (regencia || !lancaNota)
                return mediaFrequencias.FirstOrDefault(mf => mf.Tipo == TipoParametroSistema.CompensacaoAusenciaPercentualRegenciaClasse).Media;
            else
                return mediaFrequencias.FirstOrDefault(mf => mf.Tipo == TipoParametroSistema.CompensacaoAusenciaPercentualFund2).Media;
        }

        private string TransformarNotaEmConceito(decimal valorNota)
        {
            if (valorNota < 5)
                return "NS";

            if (valorNota < 7)
                return "S";

            return "P";
        }
    }
}