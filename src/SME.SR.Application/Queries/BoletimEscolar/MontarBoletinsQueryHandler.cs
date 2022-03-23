﻿using MediatR;
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
    public class MontarBoletinsQueryHandler : IRequestHandler<MontarBoletinsQuery, List<BoletimSimplesEscolarDto>>
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

        public async Task<List<BoletimSimplesEscolarDto>> Handle(MontarBoletinsQuery request, CancellationToken cancellationToken)
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
                var periodoAtual = await mediator.Send(new ObterBimestrePeriodoFechamentoAtualQuery(dre.Id, ue.Id, request.Turmas.Select(a => a.AnoLetivo).FirstOrDefault()));

                var relatorioBoletimSimplesEscolar = new List<BoletimSimplesEscolarDto>();

                foreach (var aluno in alunos)
                {
                    // verifica PossuiConselho bimestre
                    var turma = turmas.First(t => aluno.Any(a => a.CodigoTurma.ToString() == t.Codigo));

                    var conselhoClassBimestres = await mediator.Send(new AlunoConselhoClasseCadastradoBimestresQuery(aluno.Key, turma.AnoLetivo, turma.ModalidadeCodigo, turma.Semestre));

                    if (conselhoClassBimestres != null && conselhoClassBimestres.Any())
                    {
                        var boletimAluno = new BoletimSimplesEscolarDto();

                        if (componentesCurriculares.FirstOrDefault(c => c.Key == aluno.Key) == null)
                            throw new NegocioException($"Aluno: {aluno.Key} não possui componente curricular para gerar o boletim.");

                        var componentesAluno = componentesCurriculares.First(c => c.Key == aluno.Key);
                        //foreach (var turmaAluno in aluno)
                        foreach (var turmaAluno in componentesAluno.Select(s => s.CodigoTurma).Distinct())
                            MapearComponentesOrdenadosGrupo(componentesAluno.Where(cc => cc.CodigoTurma == turmaAluno.ToString()), boletimAluno);

                        var notasAluno = notas.FirstOrDefault(t => t.Key == aluno.First().CodigoAluno.ToString());

                        var frequenciasAluno = frequencia?
                            .Where(t => t.Key == aluno.First().CodigoAluno.ToString())
                            .SelectMany(f => f);

                        var frequenciasTurma = frequencia?
                            .SelectMany(a => a)
                            .Where(f => f.TurmaId == turma.Codigo);

                        if (notasAluno != null && notasAluno.Any())
                            SetarNotasFrequencia(boletimAluno,
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

                        boletimAluno.ParecerConclusivo = conselhoClassBimestres.Any(b => b == 0) ? parecerConclusivo?.ParecerConclusivo : null;

                        relatorioBoletimSimplesEscolar.Add(boletimAluno);
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

        private List<BoletimSimplesEscolarDto> OrdenarBoletins(List<BoletimSimplesEscolarDto> boletinsAlunos)
        {
            var boletinsOrdenados = new List<BoletimSimplesEscolarDto>();
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

        private void MapearComponentesOrdenadosGrupo(IEnumerable<ComponenteCurricularPorTurma> componentesCurricularesPorTurma, BoletimSimplesEscolarDto boletim)
        {
            foreach (var componente in componentesCurricularesPorTurma.OrderBy(a => a.GrupoMatriz).ThenBy(a=> a.Disciplina))
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
                                Frequencia = componente.Frequencia
                            });
                }
            }
            if (boletim.ComponenteCurricularRegencia != null)
                boletim.ComponenteCurricularRegencia.ComponentesCurriculares = boletim.ComponenteCurricularRegencia.ComponentesCurriculares.OrderBy(c => c.Nome).ToList();

            if (boletim.ComponentesCurriculares != null && boletim.ComponentesCurriculares.Any())
                boletim.ComponentesCurriculares = boletim.ComponentesCurriculares.OrderBy(c => c.Nome).ToList();
        }

        private void SetarNotasFrequencia(BoletimSimplesEscolarDto boletim, IEnumerable<NotasAlunoBimestre> notas, IEnumerable<FrequenciaAluno> frequenciasAluno, IEnumerable<FrequenciaAluno> frequenciasTurma,
            IEnumerable<MediaFrequencia> mediasFrequencia, IEnumerable<int> conselhoClasseBimestres, IEnumerable<TurmaComponenteQtdAulasDto> registroFrequencia, int periodoAtual, IEnumerable<TurmaComponenteQuantidadeAulasDto> aulasPrevistas)
        {
            var aulasPrevistasTurma = aulasPrevistas.Where(a => a.TurmaCodigo == frequenciasTurma.FirstOrDefault().TurmaId);

            if (boletim.ComponenteCurricularRegencia != null)
            {
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

                    boletim.ComponenteCurricularRegencia.FrequenciaFinal = ObterFrequenciaFinalAluno(frequenciasAlunoRegencia, frequenciasTurmaRegencia, conselhoClasseBimestres, registroFrequencia.Where(rf => rf.ComponenteCurricularCodigo == boletim.ComponenteCurricularRegencia.Codigo));
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

                        componenteCurricular.NotaFinal = notaFrequenciaComponente?.FirstOrDefault(nf => nf.PeriodoEscolar == null)?.NotaConceito?.NotaConceito;
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

                        var notaFinal = notasComponente?.FirstOrDefault(nf => nf.PeriodoEscolar == null)?.NotaConceito?.NotaConceito;
                        componenteCurricular.NotaFinal = String.IsNullOrEmpty(notaFinal) && (periodoAtual >= 1 && periodoAtual <= 4) ? "-" : notaFinal;
                    }
                    else
                        componenteCurricular.NotaFinal = ObterSintese(frequenciasAlunoComponente, mediasFrequencia, false, false);


                    if (componenteCurricular.Frequencia)
                    {
                        var aulasCadastradas = registroFrequencia?.Where(f => f.ComponenteCurricularCodigo == componenteCurricular.Codigo);
                        componenteCurricular.FrequenciaBimestre1 = ObterFrequenciaBimestre(conselhoClasseBimestres, frequenciasAlunoComponente, frequenciasTurmaComponente, 1, aulasCadastradas, periodoAtual);
                        componenteCurricular.FrequenciaBimestre2 = ObterFrequenciaBimestre(conselhoClasseBimestres, frequenciasAlunoComponente, frequenciasTurmaComponente, 2, aulasCadastradas, periodoAtual);
                        componenteCurricular.FrequenciaBimestre3 = ObterFrequenciaBimestre(conselhoClasseBimestres, frequenciasAlunoComponente, frequenciasTurmaComponente, 3, aulasCadastradas, periodoAtual);
                        componenteCurricular.FrequenciaBimestre4 = ObterFrequenciaBimestre(conselhoClasseBimestres, frequenciasAlunoComponente, frequenciasTurmaComponente, 4, aulasCadastradas, periodoAtual);

                        var frequenciaFinal = ObterFrequenciaFinalAluno(frequenciasAlunoComponente, frequenciasTurmaComponente, conselhoClasseBimestres, registroFrequencia.Where(rf => rf.ComponenteCurricularCodigo == componenteCurricular.Codigo));
                        componenteCurricular.FrequenciaFinal = String.IsNullOrEmpty(frequenciaFinal) && (periodoAtual >= 1 && periodoAtual <= 4) ? "-" : frequenciaFinal;

                        if (!componenteCurricular.Nota)
                            componenteCurricular.NotaFinal = ObterSintese(frequenciasAlunoComponente, mediasFrequencia, false, false);
                    }
                }
            }
        }

        private string ObterNotaBimestre(IEnumerable<int> conselhoClassBimestres, IEnumerable<NotasAlunoBimestre> notasComponente, int bimestre, int periodoAtual)
        {
            var retorno = !VerificaPossuiConselho(conselhoClassBimestres, bimestre) ? "" :
                notasComponente?.FirstOrDefault(nf => nf.PeriodoEscolar != null && nf.PeriodoEscolar.Bimestre == bimestre)?.NotaConceito?.NotaConceito;

            if (bimestre > periodoAtual && String.IsNullOrEmpty(retorno))
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

        private string ObterFrequenciaFinalAluno(IEnumerable<FrequenciaAluno> frequenciasAluno, IEnumerable<FrequenciaAluno> frequenciasTurma, IEnumerable<int> conselhoClassBimestres, IEnumerable<TurmaComponenteQtdAulasDto> registroFrequencia)
        {
            if (!conselhoClassBimestres.Any(a => a == 0) || frequenciasAluno == null || !frequenciasAluno.Any())
                return FREQUENCIA_100;
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
                    var periodos = mediator.Send(new ObterPeriodosEscolaresPorTipoCalendarioQuery(idTipoCalendario)).Result;

                    periodos.ToList().ForEach(p =>
                    {
                        var frequencia = frequenciasAluno.SingleOrDefault(f => f.Bimestre.Equals(p.Bimestre));
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
