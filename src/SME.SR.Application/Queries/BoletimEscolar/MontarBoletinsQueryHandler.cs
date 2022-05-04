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
    public class MontarBoletinsQueryHandler : IRequestHandler<MontarBoletinsQuery, BoletimEscolarDto>
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

        public async Task<BoletimEscolarDto> Handle(MontarBoletinsQuery request, CancellationToken cancellationToken)
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

                var boletinsAlunos = new List<BoletimEscolarAlunoDto>();

                foreach (var aluno in alunos)
                {
                    // verifica PossuiConselho bimestre
                    var turma = turmas.First(t => aluno.Any(a => a.CodigoTurma.ToString() == t.Codigo));

                    var conselhoClassBimestres = await mediator.Send(new AlunoConselhoClasseCadastradoBimestresQuery(aluno.Key, turma.AnoLetivo, turma.ModalidadeCodigo, turma.Semestre));

                    if (conselhoClassBimestres != null && conselhoClassBimestres.Any())
                    {
                        var tipoNota = tiposNota[turma.Codigo];
                        var boletimEscolarAlunoDto = new BoletimEscolarAlunoDto()
                        {
                            TipoNota = tipoNota
                        };

                        if (componentesCurriculares.FirstOrDefault(c => c.Key == aluno.Key) == null)
                            throw new NegocioException($"Aluno: {aluno.Key} não possui componente curricular para gerar o boletim.");

                        var componentesAluno = componentesCurriculares.First(c => c.Key == aluno.Key);
                        //foreach (var turmaAluno in aluno)
                        foreach (var turmaAluno in componentesAluno.Select(s => s.CodigoTurma).Distinct())
                            MapearGruposEComponentes(componentesAluno.Where(cc => cc.CodigoTurma == turmaAluno.ToString()), boletimEscolarAlunoDto.Grupos);

                        var notasAluno = notas.FirstOrDefault(t => t.Key == aluno.First().CodigoAluno.ToString());

                        var frequenciasAluno = frequencia?
                            .Where(t => t.Key == aluno.First().CodigoAluno.ToString())
                            .SelectMany(f => f);

                        var frequenciasTurma = frequencia?
                            .SelectMany(a => a)
                            .Where(f => f.TurmaId == turma.Codigo);

                        if (notasAluno != null && notasAluno.Any())
                            SetarNotasFrequencia(boletimEscolarAlunoDto.Grupos,
                                                    notasAluno,
                                                    frequenciasAluno,
                                                    frequenciasTurma,
                                                    mediasFrequencia,
                                                    conselhoClassBimestres,
                                                    registroFrequencia,
                                                    periodoAtual,
                                                    aulasPrevistas);

                        var frequeciaGlobal = frequenciasGlobal?
                            .FirstOrDefault(t => t.Key == aluno.First().CodigoAluno.ToString());

                        var percentualFrequenciaGlobal = frequeciaGlobal != null ? frequeciaGlobal.First().PercentualFrequencia : 100;
                        var parecerConclusivo = pareceresConclusivos.FirstOrDefault(c => c.TurmaId.ToString() == turma.Codigo && c.AlunoCodigo.ToString() == aluno.Key);

                        boletimEscolarAlunoDto.Cabecalho = ObterCabecalhoInicial(dre,
                                                                                    ue,
                                                                                    turma,
                                                                                    aluno.First().CodigoAluno.ToString(),
                                                                                    aluno.OrderBy(a => a.DataSituacao).Last().NomeRelatorio,
                                                                                    aluno.First().ObterNomeFinal(),
                                                                                    $"{percentualFrequenciaGlobal}%");

                        boletimEscolarAlunoDto.ParecerConclusivo = conselhoClassBimestres.Any(b => b == 0) ? parecerConclusivo?.ParecerConclusivo : null;

                        boletinsAlunos.Add(boletimEscolarAlunoDto);
                    }
                }

                if (!boletinsAlunos.Any())
                    throw new NegocioException("Não foram encontradas informações para geração do boletim");

                return await Task.FromResult(new BoletimEscolarDto(OrdenarBoletins(boletinsAlunos)));

            }
            catch (Exception ex)
            {
                throw new Exception($"Não foi possível montar boletim - Motivo: {ex.Message}");
            }
        }

        private List<BoletimEscolarAlunoDto> OrdenarBoletins(List<BoletimEscolarAlunoDto> boletinsAlunos)
        {
            var boletinsOrdenados = new List<BoletimEscolarAlunoDto>();
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

        private void MapearGruposEComponentes(IEnumerable<ComponenteCurricularPorTurma> componentesCurricularesPorTurma, List<GrupoMatrizComponenteCurricularDto> grupos)
        {
            var gruposMatrizes = componentesCurricularesPorTurma.GroupBy(cc => cc.GrupoMatriz).ToList();

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
                        grupo.ComponenteCurricularRegencia = new ComponenteCurricularRegenciaDto()
                        {
                            Codigo = componente.CodDisciplina.ToString(),
                            Frequencia = componente.Frequencia
                        };

                        foreach (var componenteRegencia in componente.ComponentesCurricularesRegencia.OrderBy(a => a.Disciplina))
                        {
                            if (!grupo.ComponenteCurricularRegencia.ComponentesCurriculares.Any(g => g.Codigo == componenteRegencia.CodDisciplina.ToString()))
                                grupo.ComponenteCurricularRegencia.ComponentesCurriculares.Add(
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
                if (grupo.ComponenteCurricularRegencia != null)
                    grupo.ComponenteCurricularRegencia.ComponentesCurriculares = grupo.ComponenteCurricularRegencia.ComponentesCurriculares.OrderBy(c => c.Nome).ToList();

                if (grupo.ComponentesCurriculares != null && grupo.ComponentesCurriculares.Any())
                    grupo.ComponentesCurriculares = grupo.ComponentesCurriculares.OrderBy(c => c.Nome).ToList();
            }
        }

        private void SetarNotasFrequencia(List<GrupoMatrizComponenteCurricularDto> gruposMatriz, IEnumerable<NotasAlunoBimestre> notas, IEnumerable<FrequenciaAluno> frequenciasAluno, IEnumerable<FrequenciaAluno> frequenciasTurma,
            IEnumerable<MediaFrequencia> mediasFrequencia, IEnumerable<int> conselhoClasseBimestres, IEnumerable<TurmaComponenteQtdAulasDto> registroFrequencia, int periodoAtual, IEnumerable<TurmaComponenteQuantidadeAulasDto> aulasPrevistas)
        {
            var aulasPrevistasTurma = aulasPrevistas.Where(a => a.TurmaCodigo == frequenciasTurma.FirstOrDefault().TurmaId);
            foreach (var grupoMatriz in gruposMatriz)
            {
                if (grupoMatriz.ComponenteCurricularRegencia != null)
                {
                    if (grupoMatriz.ComponenteCurricularRegencia.Frequencia)
                    {
                        var frequenciasAlunoRegencia = frequenciasAluno?.Where(f => f.DisciplinaId == grupoMatriz.ComponenteCurricularRegencia.Codigo);
                        var frequenciasTurmaRegencia = frequenciasTurma?.Where(f => f.DisciplinaId == grupoMatriz.ComponenteCurricularRegencia.Codigo);
                        var aulasCadastradas = registroFrequencia?.Where(f => f.ComponenteCurricularCodigo == grupoMatriz.ComponenteCurricularRegencia.Codigo);
                        var aulasPrevistasComponente = aulasPrevistasTurma.Where(a => a.ComponenteCurricularCodigo == grupoMatriz.ComponenteCurricularRegencia.Codigo);

                        grupoMatriz.ComponenteCurricularRegencia.FrequenciaBimestre1 = ObterFrequenciaBimestre(conselhoClasseBimestres, frequenciasAlunoRegencia, frequenciasTurmaRegencia, 1, aulasCadastradas, periodoAtual);
                        grupoMatriz.ComponenteCurricularRegencia.FrequenciaBimestre2 = ObterFrequenciaBimestre(conselhoClasseBimestres, frequenciasAlunoRegencia, frequenciasTurmaRegencia, 2, aulasCadastradas, periodoAtual);
                        grupoMatriz.ComponenteCurricularRegencia.FrequenciaBimestre3 = ObterFrequenciaBimestre(conselhoClasseBimestres, frequenciasAlunoRegencia, frequenciasTurmaRegencia, 3, aulasCadastradas, periodoAtual);
                        grupoMatriz.ComponenteCurricularRegencia.FrequenciaBimestre4 = ObterFrequenciaBimestre(conselhoClasseBimestres, frequenciasAlunoRegencia, frequenciasTurmaRegencia, 4, aulasCadastradas, periodoAtual);

                        grupoMatriz.ComponenteCurricularRegencia.FrequenciaFinal = ObterFrequenciaFinalAluno(frequenciasAlunoRegencia, frequenciasTurmaRegencia, conselhoClasseBimestres, registroFrequencia.Where(rf => rf.ComponenteCurricularCodigo == grupoMatriz.ComponenteCurricularRegencia.Codigo));
                    }

                    foreach (var componenteCurricular in grupoMatriz.ComponenteCurricularRegencia.ComponentesCurriculares)
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

                if (grupoMatriz.ComponentesCurriculares != null && grupoMatriz.ComponentesCurriculares.Any())
                {
                    foreach (var componenteCurricular in grupoMatriz.ComponentesCurriculares)
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
                return "-";
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
