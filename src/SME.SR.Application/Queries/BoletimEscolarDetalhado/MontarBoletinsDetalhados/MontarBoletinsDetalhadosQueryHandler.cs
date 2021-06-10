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
    public class MontarBoletinsDetalhadosQueryHandler : IRequestHandler<MontarBoletinsDetalhadosQuery, BoletimEscolarDetalhadoDto>
    {
        private const string FREQUENCIA_100 = "100";
        private readonly IMediator mediator;
        private readonly ITipoCalendarioRepository tipoCalendarioRepository;

        public MontarBoletinsDetalhadosQueryHandler(IMediator mediator,
                                          ITipoCalendarioRepository tipoCalendarioRepository)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this.tipoCalendarioRepository = tipoCalendarioRepository ?? throw new ArgumentNullException(nameof(tipoCalendarioRepository));
        }

        public async Task<BoletimEscolarDetalhadoDto> Handle(MontarBoletinsDetalhadosQuery request, CancellationToken cancellationToken)
        {
            var turmas = request.Turmas;
            var dre = request.Dre;
            var ue = request.Ue;
            var ultimoBimestrePeriodoFechamento = request.UltimoBimestrePeriodoFechamento;
            var ciclos = request.TiposCiclo;
            var componentesCurriculares = request.ComponentesCurriculares;
            var alunos = request.AlunosPorTuma;
            var fotos = request.AlunosFoto;
            var notas = request.Notas;
            var frequencia = request.Frequencias;
            var frequenciasGlobal = request.FrequenciasGlobal;
            var tiposNota = request.TiposNota;
            var pareceresConclusivos = request.PareceresConclusivos;
            var mediasFrequencia = request.MediasFrequencia;
            var recomendacoes = request.RecomendacoesAlunos;

            var boletinsAlunos = new List<BoletimEscolarDetalhadoAlunoDto>();

            foreach (var aluno in alunos)
            {
                // verifica PossuiConselho bimestre
                var turma = turmas.First(t => aluno.Any(a => a.CodigoTurma.ToString() == t.Codigo));

                var conselhoClassBimestres = await mediator.Send(new AlunoConselhoClasseCadastradoBimestresQuery(aluno.Key, turma.AnoLetivo, turma.ModalidadeCodigo, turma.Semestre));

                if (conselhoClassBimestres != null && conselhoClassBimestres.Any())
                {
                    var tipoNota = tiposNota[turma.Codigo];
                    var boletimEscolarAlunoDto = new BoletimEscolarDetalhadoAlunoDto()
                    {
                        TipoNota = tipoNota
                    };

                    var componentesAluno = componentesCurriculares.First(c => c.Key == aluno.Key);
                    foreach (var turmaAluno in aluno)
                    {
                        await MapearGruposEComponentes(componentesAluno.Where(cc => cc.CodigoTurma == turmaAluno.CodigoTurma.ToString()), boletimEscolarAlunoDto);
                    }

                    var notasAluno = notas.FirstOrDefault(t => t.Key == aluno.First().CodigoAluno.ToString());
                    var frequenciasAluno = frequencia?.FirstOrDefault(t => t.Key == aluno.First().CodigoAluno.ToString());

                    if (notasAluno != null && notasAluno.Any())
                        SetarNotasFrequencia(boletimEscolarAlunoDto, notasAluno, frequenciasAluno, mediasFrequencia, conselhoClassBimestres, ultimoBimestrePeriodoFechamento);

                    var frequeciaGlobal = frequenciasGlobal?.FirstOrDefault(t => t.Key == aluno.First().CodigoAluno.ToString());
                    var percentualFrequenciaGlobal = frequeciaGlobal != null ? frequeciaGlobal.First().PercentualFrequencia : 100;
                    var parecerConclusivo = pareceresConclusivos.FirstOrDefault(c => c.TurmaId.ToString() == turma.Codigo && c.AlunoCodigo.ToString() == aluno.Key);
                    var recomendacao = recomendacoes?.FirstOrDefault(r => r.TurmaCodigo == turma.Codigo && r.AlunoCodigo == aluno.Key);
                    var ciclo = ciclos.FirstOrDefault(c => c.Modalidade == turma.ModalidadeCodigo && c.Ano == Convert.ToInt32(turma.Ano));
                    var foto = fotos.FirstOrDefault(c => c.CodigoAluno.ToString() == aluno.Key);

                    boletimEscolarAlunoDto.Cabecalho = ObterCabecalhoInicial(dre, ue, ciclo, turma, aluno.Key, foto, aluno.FirstOrDefault().NomeRelatorio, $"{percentualFrequenciaGlobal}%", request.AnoLetivo);
                    boletimEscolarAlunoDto.ParecerConclusivo = conselhoClassBimestres.Any(b => b == 0) ? (parecerConclusivo?.ParecerConclusivo ?? "") : null;
                    boletimEscolarAlunoDto.RecomendacoesEstudante = recomendacao?.RecomendacoesAluno;
                    boletimEscolarAlunoDto.RecomendacoesFamilia = recomendacao?.RecomendacoesFamilia;
                    boletinsAlunos.Add(boletimEscolarAlunoDto);
                }
            }

            return await Task.FromResult(new BoletimEscolarDetalhadoDto(boletinsAlunos));
        }

        private BoletimEscolarDetalhadoCabecalhoDto ObterCabecalhoInicial(Dre dre, Ue ue, TipoCiclo ciclo, Turma turma, string alunoCodigo, AlunoFotoArquivoDto foto, string nome, string frequenciaGlobal, int anoLetivo)
        {
            return new BoletimEscolarDetalhadoCabecalhoDto()
            {
                Data = DateTime.Now.ToString("dd/MM/yyyy"),
                NomeDre = dre.Nome,
                NomeUe = ue.TituloTipoEscolaNome,
                NomeTurma = turma.NomeRelatorio,
                CodigoEol = alunoCodigo,
                Aluno = nome,
                FrequenciaGlobal = frequenciaGlobal,
                Ciclo = ciclo.Descricao,
                Foto = foto?.FotoBase64,
                AnoLetivo = anoLetivo
            };
        }

        private async Task MapearGruposEComponentes(IEnumerable<ComponenteCurricularPorTurma> componentesCurricularesPorTurma, BoletimEscolarDetalhadoAlunoDto boletim)
        {
            var componentesOrdenados = await mediator.Send(new OrdenarComponentesPorGrupoMatrizAreaConhecimentoQuery(componentesCurricularesPorTurma));

            var gruposMatrizes = componentesCurricularesPorTurma.GroupBy(cc => cc.GrupoMatriz).ToList();

            var grupos = boletim.Grupos;

            var gruposRetorno = new List<GrupoMatrizComponenteCurricularDto>();

            foreach (var grupoMatriz in gruposMatrizes)
            {
                GrupoMatrizComponenteCurricularDto grupo = null;

                if (grupos.Any(g => g.Id == (int)grupoMatriz.Key.Id))
                    grupo = grupos.FirstOrDefault(g => g.Id == (int)grupoMatriz.Key.Id);
                else
                {
                    grupo = new GrupoMatrizComponenteCurricularDto()
                    {
                        Id = (int)grupoMatriz.Key.Id,
                        Nome = $"GRUPO {gruposRetorno.Count() + 1}",
                        Descricao = grupoMatriz.Key.Nome
                    };

                    grupos.Add(grupo);
                }

                foreach (var componente in grupoMatriz.OrderBy(a => a.Disciplina))
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
                        if (grupo.ComponentesCurriculares == null)
                            grupo.ComponentesCurriculares = new List<ComponenteCurricularDto>();

                        if (!grupo.ComponentesCurriculares.Any(g => g.Codigo == componente.CodDisciplina.ToString()))
                            grupo.ComponentesCurriculares.Add(
                                new ComponenteCurricularDto()
                                {
                                    Codigo = componente.CodDisciplina.ToString(),
                                    Nome = componente.Disciplina,
                                    Nota = componente.LancaNota,
                                    Frequencia = componente.Frequencia
                                });
                    }
                }
                if (boletim.ComponenteCurricularRegencia != null)
                    boletim.ComponenteCurricularRegencia.ComponentesCurriculares = boletim.ComponenteCurricularRegencia.ComponentesCurriculares.OrderBy(c => c.Nome).ToList();

                if (grupo.ComponentesCurriculares != null && grupo.ComponentesCurriculares.Any())
                    grupo.ComponentesCurriculares = grupo.ComponentesCurriculares.OrderBy(c => c.Nome).ToList();
            }
        }

        private void SetarNotasFrequencia(BoletimEscolarDetalhadoAlunoDto boletimEscolar, IEnumerable<NotasAlunoBimestre> notas, IEnumerable<FrequenciaAluno> frequencia, IEnumerable<MediaFrequencia> mediasFrequencia, IEnumerable<int> conselhoClasseBimestres, int ultimoBimestrePeriodoFechamento)
        {
            if (boletimEscolar.ComponenteCurricularRegencia != null)
            {
                if (boletimEscolar.ComponenteCurricularRegencia.Frequencia)
                {
                    var frequenciasRegencia = frequencia?.Where(f => f.DisciplinaId == boletimEscolar.ComponenteCurricularRegencia.Codigo);

                    boletimEscolar.ComponenteCurricularRegencia.FrequenciaBimestre1 = ObterFrequenciaBimestre(conselhoClasseBimestres, frequenciasRegencia, 1);
                    boletimEscolar.ComponenteCurricularRegencia.FrequenciaBimestre2 = ObterFrequenciaBimestre(conselhoClasseBimestres, frequenciasRegencia, 2);
                    boletimEscolar.ComponenteCurricularRegencia.FrequenciaBimestre3 = ObterFrequenciaBimestre(conselhoClasseBimestres, frequenciasRegencia, 3);
                    boletimEscolar.ComponenteCurricularRegencia.FrequenciaBimestre4 = ObterFrequenciaBimestre(conselhoClasseBimestres, frequenciasRegencia, 4);

                    boletimEscolar.ComponenteCurricularRegencia.FrequenciaFinal = ObterFrequenciaFinalAluno(frequenciasRegencia, conselhoClasseBimestres);
                }

                foreach (var componenteCurricular in boletimEscolar.ComponenteCurricularRegencia.ComponentesCurriculares)
                {
                    if (componenteCurricular.Nota)
                    {
                        var notaFrequenciaComponente = notas?.Where(nf => nf.CodigoComponenteCurricular == componenteCurricular.Codigo);

                        componenteCurricular.NotaBimestre1 = ObterNotaBimestre(ultimoBimestrePeriodoFechamento, notaFrequenciaComponente, 1);
                        componenteCurricular.NotaBimestre2 = ObterNotaBimestre(ultimoBimestrePeriodoFechamento, notaFrequenciaComponente, 2);
                        componenteCurricular.NotaBimestre3 = ObterNotaBimestre(ultimoBimestrePeriodoFechamento, notaFrequenciaComponente, 3);
                        componenteCurricular.NotaBimestre4 = ObterNotaBimestre(ultimoBimestrePeriodoFechamento, notaFrequenciaComponente, 4);

                        componenteCurricular.NotaFinal = ObterNotaBimestreFinal(conselhoClasseBimestres, notaFrequenciaComponente);
                    }
                }
            }

            foreach (var grupoMatriz in boletimEscolar.Grupos)
            {
                if (grupoMatriz.ComponentesCurriculares != null && grupoMatriz.ComponentesCurriculares.Any())
                {
                    foreach (var componenteCurricular in grupoMatriz.ComponentesCurriculares)
                    {
                        var frequenciasComponente = frequencia?.Where(f => f.DisciplinaId == componenteCurricular.Codigo);

                        if (componenteCurricular.Nota)
                        {
                            var notasComponente = notas?.Where(n => n.CodigoComponenteCurricular == componenteCurricular.Codigo) ?? null;

                            componenteCurricular.NotaBimestre1 = ObterNotaBimestre(ultimoBimestrePeriodoFechamento, notasComponente, 1);
                            componenteCurricular.NotaBimestre2 = ObterNotaBimestre(ultimoBimestrePeriodoFechamento, notasComponente, 2);
                            componenteCurricular.NotaBimestre3 = ObterNotaBimestre(ultimoBimestrePeriodoFechamento, notasComponente, 3);
                            componenteCurricular.NotaBimestre4 = ObterNotaBimestre(ultimoBimestrePeriodoFechamento, notasComponente, 4);

                            componenteCurricular.NotaFinal = ObterNotaBimestreFinal(conselhoClasseBimestres, notasComponente);
                        }
                        else
                            componenteCurricular.NotaFinal = ObterSintese(frequenciasComponente, mediasFrequencia, false, false);


                        if (componenteCurricular.Frequencia)
                        {
                            componenteCurricular.FrequenciaBimestre1 = ObterFrequenciaBimestre(conselhoClasseBimestres, frequenciasComponente, 1);
                            componenteCurricular.FrequenciaBimestre2 = ObterFrequenciaBimestre(conselhoClasseBimestres, frequenciasComponente, 2);
                            componenteCurricular.FrequenciaBimestre3 = ObterFrequenciaBimestre(conselhoClasseBimestres, frequenciasComponente, 3);
                            componenteCurricular.FrequenciaBimestre4 = ObterFrequenciaBimestre(conselhoClasseBimestres, frequenciasComponente, 4);

                            componenteCurricular.FrequenciaFinal = ObterFrequenciaFinalAluno(frequenciasComponente, conselhoClasseBimestres);

                            if (!componenteCurricular.Nota)
                                componenteCurricular.NotaFinal = ObterSintese(frequenciasComponente, mediasFrequencia, false, false);
                        }
                        else
                        {
                            componenteCurricular.FrequenciaBimestre1 = "-";
                            componenteCurricular.FrequenciaBimestre2 = "-";
                            componenteCurricular.FrequenciaBimestre3 = "-";
                            componenteCurricular.FrequenciaBimestre4 = "-";
                            componenteCurricular.FrequenciaFinal = "-";
                        }
                    }
                }
            }
        }

        private string ObterNotaBimestre(int ultimoBimestrePeriodoFechamento, IEnumerable<NotasAlunoBimestre> notasComponente, int bimestre)
        {
            if (bimestre > ultimoBimestrePeriodoFechamento)
                return "";

            var nota = notasComponente?.FirstOrDefault(nf => nf.PeriodoEscolar != null && nf.PeriodoEscolar.Bimestre == bimestre)?.NotaConceito?.NotaConceito;
            return !string.IsNullOrEmpty(nota) ? nota : "-";
        }

        private string ObterNotaBimestreFinal(IEnumerable<int> conselhoClassBimestres, IEnumerable<NotasAlunoBimestre> notasComponente)
        {
            if (!VerificaPossuiConselho(conselhoClassBimestres, 0))
                return "";

            var nota = notasComponente?.FirstOrDefault(nf => nf.PeriodoEscolar == null)?.NotaConceito?.NotaConceito; 
            return !string.IsNullOrEmpty(nota) ? nota : "-";
        }

        private bool VerificaPossuiConselho(IEnumerable<int> conselhoClassBimestres, int bimestre)
        {
            return conselhoClassBimestres.Any(a => a == bimestre);
        }

        private string ObterFrequenciaBimestre(IEnumerable<int> conselhoClassBimestres, IEnumerable<FrequenciaAluno> frequenciasComponente, int bimestre)
        {
            var frequencia = !VerificaPossuiConselho(conselhoClassBimestres, bimestre) ? "" :
                frequenciasComponente?.FirstOrDefault(nf => nf.Bimestre == bimestre)?.PercentualFrequencia.ToString() ?? FREQUENCIA_100;
            return !string.IsNullOrEmpty(frequencia) ? frequencia + "%" : frequencia;

        }

        private string ObterFrequenciaFinalAluno(IEnumerable<FrequenciaAluno> frequencias, IEnumerable<int> conselhoClassBimestres)
        {
            if (!conselhoClassBimestres.Any(a => a == 0))
                return "";
            else if (frequencias == null || !frequencias.Any())
                return FREQUENCIA_100;
            else if (frequencias.FirstOrDefault(nf => nf.PeriodoEscolarId == null) != null)
                return frequencias.FirstOrDefault(nf => nf.PeriodoEscolarId == null).PercentualFrequencia.ToString();
            else
            {
                var frequenciaFinal = new FrequenciaAluno()
                {
                    TotalAulas = frequencias.Sum(f => f.TotalAulas),
                    TotalAusencias = frequencias.Sum(f => f.TotalAusencias),
                    TotalCompensacoes = frequencias.Sum(f => f.TotalCompensacoes)
                };

                //Particularidade de cálculo de frequência para 2020.
                if (frequencias.First().AnoTurma.Equals(2020))
                {
                    var idTipoCalendario = tipoCalendarioRepository.ObterPorAnoLetivoEModalidade(frequencias.First().AnoTurma, frequencias.First().ModalidadeTurma).Result;
                    var periodos = mediator.Send(new ObterPeriodosEscolaresPorTipoCalendarioQuery(idTipoCalendario)).Result;

                    periodos.ToList().ForEach(p =>
                    {
                        var frequencia = frequencias.SingleOrDefault(f => f.Bimestre.Equals(p.Bimestre));
                        frequenciaFinal.AdicionarFrequenciaBimestre(p.Bimestre, frequencia != null ? frequencia.PercentualFrequencia : 100);
                    });

                    return frequenciaFinal.PercentualFrequenciaFinal.ToString();
                }

                return frequenciaFinal.PercentualFrequencia.ToString();
            }

        }

        private string ObterSintese(IEnumerable<FrequenciaAluno> frequenciasComponente, IEnumerable<MediaFrequencia> mediaFrequencias, bool regencia, bool lancaNota)
        {
            var percentualFrequencia = ObterPercentualDeFrequencia(frequenciasComponente);

            var sintese = percentualFrequencia >= ObterFrequenciaMedia(mediaFrequencias, regencia, lancaNota) ?
                          "F" : "NF";

            return sintese;
        }

        private double ObterPercentualDeFrequencia(IEnumerable<FrequenciaAluno> frequenciaDisciplina)
        {
            return frequenciaDisciplina != null && frequenciaDisciplina.Any() ? frequenciaDisciplina.Sum(x => x.PercentualFrequencia) / frequenciaDisciplina.Count() : 100;
        }

        private double ObterFrequenciaMedia(IEnumerable<MediaFrequencia> mediaFrequencias, bool regencia, bool lancaNota)
        {
            if (regencia || !lancaNota)
                return mediaFrequencias.FirstOrDefault(mf => mf.Tipo == TipoParametroSistema.CompensacaoAusenciaPercentualRegenciaClasse).Media;
            else
                return mediaFrequencias.FirstOrDefault(mf => mf.Tipo == TipoParametroSistema.CompensacaoAusenciaPercentualFund2).Media;
        }
    }
}
