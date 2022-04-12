using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using SME.SR.Data.Models;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class MontarBoletinsQueryHandler : IRequestHandler<MontarBoletinsQuery, List<RelatorioBoletimSimplesEscolarDto>>
    {
        private const string FREQUENCIA_100 = "100";
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
                    if (turmas.Any(t => aluno.Any(a => a.CodigoTurma.ToString() == t.Codigo)))
                    {
                        var turma = turmas.First(t => aluno.Any(a => a.CodigoTurma.ToString() == t.Codigo));

                        var conselhoClassBimestres = await mediator.Send(new AlunoConselhoClasseCadastradoBimestresQuery(aluno.Key, turma.AnoLetivo, turma.ModalidadeCodigo, turma.Semestre));

                        if (conselhoClassBimestres != null && conselhoClassBimestres.Any())
                        {
                            var boletimAluno = new RelatorioBoletimSimplesEscolarDto();

                            if (componentesCurriculares.FirstOrDefault(c => c.Key == aluno.Key) == null)
                                throw new NegocioException($"Aluno: {aluno.Key} não possui componente curricular para gerar o boletim.");

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

                            if (notasAluno != null && notasAluno.Any())
                                await SetarNotasFrequencia(boletimAluno,
                                                        notasAluno,
                                                        frequenciasAluno,
                                                        frequenciasTurma,
                                                        mediasFrequencia,
                                                        conselhoClassBimestres,
                                                        registroFrequencia,
                                                        periodoAtual,
                                                        aulasPrevistas);

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
                                                                                        $"{percentualFrequenciaGlobal}%");

                            boletimAluno.ParecerConclusivo = parecerConclusivo?.ParecerConclusivo;

                            relatorioBoletimSimplesEscolar.Add(boletimAluno);
                        }
                    }
                }

                if (!relatorioBoletimSimplesEscolar.Any())
                    throw new NegocioException("Não foram encontradas informações para geração do boletim");

                OrdenarBoletins(relatorioBoletimSimplesEscolar);

                return relatorioBoletimSimplesEscolar;
            }
            catch (Exception ex)
            {
                throw new Exception($"Não foi possível montar boletim - Motivo: {ex.Message}");
            }
        }

        private List<RelatorioBoletimSimplesEscolarDto> OrdenarBoletins(List<RelatorioBoletimSimplesEscolarDto> boletinsAlunos)
        {
            var boletinsOrdenados = new List<RelatorioBoletimSimplesEscolarDto>();
            var turmas = boletinsAlunos.Select(b => b.Cabecalho.NomeTurma).Distinct();

            foreach (string turma in turmas.OrderBy(t => t))
            {
                var alunosTurma = boletinsAlunos.Where(a => a.Cabecalho.NomeTurma == turma).OrderBy(a => a.Cabecalho.NomeAlunoOrdenacao).ToList();
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
                CodigoEol = alunoCodigo,
                Aluno = nome,
                NomeAlunoOrdenacao = nomeAlunoOrdenacao,
                AnoLetivo = turma.AnoLetivo.ToString(),
                FrequenciaGlobal = frequenciaGlobal
            };
        }

        private void MapearComponentesOrdenadosGrupo(IEnumerable<ComponenteCurricularPorTurma> componentesCurricularesPorTurma, RelatorioBoletimSimplesEscolarDto boletim)
        {
            var componentesTurmaComArea = componentesCurricularesPorTurma.Where(w => w.AreaDoConhecimento != null).OrderBy(a => a.AreaDoConhecimento.Ordem).ThenBy(a => a.GrupoMatriz.Id).ThenBy(a => a.AreaDoConhecimento.Id).ThenBy(b => b.Disciplina).ToList();
            var componentesTurmaSemArea = componentesCurricularesPorTurma.Where(w => w.AreaDoConhecimento == null).OrderBy(a => a.GrupoMatriz.Id).ThenBy(b => b.Disciplina).ToList();
            var componentesTurma = componentesTurmaComArea.Concat(componentesTurmaSemArea);
            
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
                    if (boletim.ComponentesCurriculares == null)
                        boletim.ComponentesCurriculares = new List<ComponenteCurricularDto>();

                    if (!boletim.ComponentesCurriculares.Any(g => g.Codigo == componente.CodDisciplina.ToString()))
                        boletim.ComponentesCurriculares.Add(
                            new ComponenteCurricularDto()
                            {
                                Codigo = componente.CodDisciplina.ToString(),
                                Nome = componente.Disciplina,
                                Nota = componente.LancaNota,
                                Frequencia = componente.Frequencia,
                                Grupo = componente.GrupoMatriz.Id
                            });
                }
            }
        }

        private async Task SetarNotasFrequencia(RelatorioBoletimSimplesEscolarDto boletim, IEnumerable<NotasAlunoBimestreBoletimSimplesDto> notas, IEnumerable<FrequenciaAluno> frequenciasAluno, IEnumerable<FrequenciaAluno> frequenciasTurma,
            IEnumerable<MediaFrequencia> mediasFrequencia, IEnumerable<int> conselhoClasseBimestres, IEnumerable<TurmaComponenteQtdAulasDto> registroFrequencia, int periodoAtual, IEnumerable<TurmaComponenteQuantidadeAulasDto> aulasPrevistas)
        {
            var aulasPrevistasTurma = aulasPrevistas.Where(a => a.TurmaCodigo == frequenciasTurma.FirstOrDefault().TurmaId);

            if (boletim.ComponenteCurricularRegencia != null)
            {
                boletim.EhRegencia = true;

                if (boletim.ComponenteCurricularRegencia.Frequencia)
                {
                    var frequenciasAlunoRegencia = frequenciasAluno?.Where(f => f.DisciplinaId == boletim.ComponenteCurricularRegencia.Codigo);
                    var frequenciasTurmaRegencia = frequenciasTurma?.Where(f => f.DisciplinaId == boletim.ComponenteCurricularRegencia.Codigo);
                    var aulasCadastradas = registroFrequencia?.Where(f => f.ComponenteCurricularCodigo == boletim.ComponenteCurricularRegencia.Codigo);
                    var aulasPrevistasComponente = aulasPrevistasTurma.Where(a => a.ComponenteCurricularCodigo == boletim.ComponenteCurricularRegencia.Codigo);

                    boletim.ComponenteCurricularRegencia.FrequenciaBimestre1 = ObterFrequenciaBimestre(conselhoClasseBimestres, frequenciasAlunoRegencia, frequenciasTurmaRegencia, 1, aulasCadastradas, periodoAtual);
                    boletim.ComponenteCurricularRegencia.FrequenciaBimestre2 = ObterFrequenciaBimestre(conselhoClasseBimestres, frequenciasAlunoRegencia, frequenciasTurmaRegencia, 2, aulasCadastradas, periodoAtual);
                    boletim.ComponenteCurricularRegencia.FrequenciaBimestre3 = ObterFrequenciaBimestre(conselhoClasseBimestres, frequenciasAlunoRegencia, frequenciasTurmaRegencia, 3, aulasCadastradas, periodoAtual);
                    boletim.ComponenteCurricularRegencia.FrequenciaBimestre4 = ObterFrequenciaBimestre(conselhoClasseBimestres, frequenciasAlunoRegencia, frequenciasTurmaRegencia, 4, aulasCadastradas, periodoAtual);

                    boletim.ComponenteCurricularRegencia.FrequenciaFinal = await ObterFrequenciaFinalAluno(frequenciasAlunoRegencia, conselhoClasseBimestres, registroFrequencia.Where(rf => rf.ComponenteCurricularCodigo == boletim.ComponenteCurricularRegencia.Codigo));
                }

                foreach (var componenteCurricular in boletim.ComponenteCurricularRegencia.ComponentesCurriculares)
                {
                    if (componenteCurricular.Nota)
                    {
                        var notaFrequenciaComponente = notas?.Where(nf => nf.CodigoComponenteCurricular == componenteCurricular.Codigo);

                        componenteCurricular.NotaBimestre1 = ObterNotaBimestre(conselhoClasseBimestres, notaFrequenciaComponente, 1, periodoAtual);
                        componenteCurricular.NotaBimestre2 = ObterNotaBimestre(conselhoClasseBimestres, notaFrequenciaComponente, 2, periodoAtual);
                        componenteCurricular.NotaBimestre3 = ObterNotaBimestre(conselhoClasseBimestres, notaFrequenciaComponente, 3, periodoAtual);
                        componenteCurricular.NotaBimestre4 = ObterNotaBimestre(conselhoClasseBimestres, notaFrequenciaComponente, 4, periodoAtual);

                        componenteCurricular.NotaFinal = ObterNotaBimestre(conselhoClasseBimestres, notaFrequenciaComponente, 0, periodoAtual);
                    }
                }
            }

            if (boletim.ComponentesCurriculares != null && boletim.ComponentesCurriculares.Any())
            {
                foreach (var componenteCurricular in boletim.ComponentesCurriculares)
                {
                    var frequenciasAlunoComponente = frequenciasAluno?.Where(f => f.DisciplinaId == componenteCurricular.Codigo);
                    var frequenciasTurmaComponente = frequenciasTurma?.Where(f => f.DisciplinaId == componenteCurricular.Codigo);

                    if (componenteCurricular.Nota)
                    {
                        var notasComponente = notas?.Where(n => n.CodigoComponenteCurricular == componenteCurricular.Codigo) ?? null;

                        componenteCurricular.NotaBimestre1 = ObterNotaBimestre(conselhoClasseBimestres, notasComponente, 1, periodoAtual);
                        componenteCurricular.NotaBimestre2 = ObterNotaBimestre(conselhoClasseBimestres, notasComponente, 2, periodoAtual);
                        componenteCurricular.NotaBimestre3 = ObterNotaBimestre(conselhoClasseBimestres, notasComponente, 3, periodoAtual);
                        componenteCurricular.NotaBimestre4 = ObterNotaBimestre(conselhoClasseBimestres, notasComponente, 4, periodoAtual);

                        componenteCurricular.NotaFinal = ObterNotaBimestre(conselhoClasseBimestres, notasComponente, 0, periodoAtual);
                    }
                    else
                    {
                        componenteCurricular.NotaBimestre1 = ObterSintese(frequenciasAlunoComponente, mediasFrequencia, false, false, periodoAtual, 1);
                        componenteCurricular.NotaBimestre2 = ObterSintese(frequenciasAlunoComponente, mediasFrequencia, false, false, periodoAtual, 2);
                        componenteCurricular.NotaBimestre3 = ObterSintese(frequenciasAlunoComponente, mediasFrequencia, false, false, periodoAtual, 3);
                        componenteCurricular.NotaBimestre4 = ObterSintese(frequenciasAlunoComponente, mediasFrequencia, false, false, periodoAtual, 4);

                        componenteCurricular.NotaFinal = ObterSintese(frequenciasAlunoComponente, mediasFrequencia, false, false, periodoAtual, 0);
                    }

                    if (componenteCurricular.Frequencia)
                    {
                        var aulasCadastradas = registroFrequencia?.Where(f => f.ComponenteCurricularCodigo == componenteCurricular.Codigo);
                        componenteCurricular.FrequenciaBimestre1 = ObterFrequenciaBimestre(conselhoClasseBimestres, frequenciasAlunoComponente, frequenciasTurmaComponente, 1, aulasCadastradas, periodoAtual);
                        componenteCurricular.FrequenciaBimestre2 = ObterFrequenciaBimestre(conselhoClasseBimestres, frequenciasAlunoComponente, frequenciasTurmaComponente, 2, aulasCadastradas, periodoAtual);
                        componenteCurricular.FrequenciaBimestre3 = ObterFrequenciaBimestre(conselhoClasseBimestres, frequenciasAlunoComponente, frequenciasTurmaComponente, 3, aulasCadastradas, periodoAtual);
                        componenteCurricular.FrequenciaBimestre4 = ObterFrequenciaBimestre(conselhoClasseBimestres, frequenciasAlunoComponente, frequenciasTurmaComponente, 4, aulasCadastradas, periodoAtual);

                        var registroFrequenciaComponenteCurricular = registroFrequencia.Where(rf => rf.ComponenteCurricularCodigo == componenteCurricular.Codigo);
                        var frequenciaFinal = await ObterFrequenciaFinalAluno(frequenciasAlunoComponente, conselhoClasseBimestres, registroFrequenciaComponenteCurricular);
                        componenteCurricular.FrequenciaFinal = String.IsNullOrEmpty(frequenciaFinal) ? "-" : frequenciaFinal;
                    }
                }
            }
        }

        private string ObterNotaBimestre(IEnumerable<int> conselhoClassBimestres, IEnumerable<NotasAlunoBimestreBoletimSimplesDto> notasComponente, int bimestre, int periodoAtual)
        {
            var retorno = !VerificaPossuiConselho(conselhoClassBimestres, bimestre) ? "" :
                notasComponente?.FirstOrDefault(nc => nc.Bimestre == bimestre)?.NotaConceito;

            if ((bimestre == 0 || bimestre > periodoAtual) && String.IsNullOrEmpty(retorno))
                retorno = "-";

            return retorno;
        }

        private bool VerificaPossuiConselho(IEnumerable<int> conselhoClassBimestres, int bimestre)
        {
            return conselhoClassBimestres.Any(a => a == bimestre);
        }

        private string ObterFrequenciaBimestre(IEnumerable<int> conselhoClassBimestres, IEnumerable<FrequenciaAluno> frequenciasAlunoComponente, IEnumerable<FrequenciaAluno> frequenciasTurmaComponente, int bimestre, IEnumerable<TurmaComponenteQtdAulasDto> aulasCadastradas, int periodoAtual)
        {
            var possuiFrequenciaTurma = aulasCadastradas?.Any(nf => nf.Bimestre == bimestre);

            var frequencia = !VerificaPossuiConselho(conselhoClassBimestres, bimestre) ? "" :
                frequenciasAlunoComponente?.FirstOrDefault(nf => nf.Bimestre == bimestre)?.PercentualFrequencia.ToString() ??
                (possuiFrequenciaTurma.HasValue && possuiFrequenciaTurma.Value ? FREQUENCIA_100 : string.Empty);

            if (bimestre > periodoAtual && String.IsNullOrEmpty(frequencia))
                frequencia = "-";

            return frequencia;
        }

        private async Task<string> ObterFrequenciaFinalAluno(IEnumerable<FrequenciaAluno> frequenciasAluno, IEnumerable<int> conselhoClassBimestres, IEnumerable<TurmaComponenteQtdAulasDto> registroFrequencia)
        {
            if (!conselhoClassBimestres.Any(a => a == 0) || frequenciasAluno == null || !frequenciasAluno.Any())
                return "";
            else if (frequenciasAluno.FirstOrDefault(nf => nf.PeriodoEscolarId == null) != null)
                return frequenciasAluno.FirstOrDefault(nf => nf.PeriodoEscolarId == null).PercentualFrequencia.ToString();
            else
            {
                var bimestresAlunoComFrequencia = frequenciasAluno.Select(fa => fa.Bimestre).Distinct();
                var somaAulasBimestresSemFrequencia = registroFrequencia.Where(rf => !bimestresAlunoComFrequencia.Contains(rf.Bimestre)).Sum(rf => rf.AulasQuantidade);
                var frequenciaFinal = new FrequenciaAluno()
                {
                    TotalAulas = frequenciasAluno.Sum(f => f.TotalAulas) + somaAulasBimestresSemFrequencia,
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

                    return frequenciaFinal.PercentualFrequenciaFinal.ToString();
                }

                return frequenciaFinal.PercentualFrequencia.ToString();
            }
        }

        private string ObterSintese(IEnumerable<FrequenciaAluno> frequenciasComponente, IEnumerable<MediaFrequencia> mediaFrequencias, bool regencia, bool lancaNota)
        {
            var sintese = string.Empty;
            var frequencias = bimestre.HasValue ? frequenciasComponente.Where(w => w.Bimestre == bimestre.Value) : frequenciasComponente;

            var percentualFrequencia = ObterPercentualDeFrequencia(frequencias);

            if (bimestre == 0)
                sintese = percentualFrequencia == null ? "NF" : percentualFrequencia >= ObterFrequenciaMedia(mediaFrequencias, regencia, lancaNota) ? "F" : "NF";
            else
                sintese = percentualFrequencia.HasValue ? percentualFrequencia >= ObterFrequenciaMedia(mediaFrequencias, regencia, lancaNota) ? "F" : "NF" : "";

            if (string.IsNullOrEmpty(sintese))
                sintese = "-";

            return sintese;
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
            if (regencia)
                return mediaFrequencias.FirstOrDefault(mf => mf.Tipo == TipoParametroSistema.CompensacaoAusenciaPercentualRegenciaClasse).Media;
            else
                return mediaFrequencias.FirstOrDefault(mf => mf.Tipo == TipoParametroSistema.CompensacaoAusenciaPercentualFund2).Media;
        }
    }
}