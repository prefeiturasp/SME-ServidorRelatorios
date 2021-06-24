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

                    var componentesAluno = componentesCurriculares.First(c => c.Key == aluno.Key);
                    foreach (var turmaAluno in aluno)
                    {
                        MapearGruposEComponentes(componentesAluno.Where(cc => cc.CodigoTurma == turmaAluno.CodigoTurma.ToString()), boletimEscolarAlunoDto.Grupos);
                    }

                    var notasAluno = notas.FirstOrDefault(t => t.Key == aluno.First().CodigoAluno.ToString());
                    var frequenciasAluno = frequencia?.FirstOrDefault(t => t.Key == aluno.First().CodigoAluno.ToString());

                    if (notasAluno != null && notasAluno.Any())
                        SetarNotasFrequencia(boletimEscolarAlunoDto.Grupos, notasAluno, frequenciasAluno, mediasFrequencia, conselhoClassBimestres);

                    var frequeciaGlobal = frequenciasGlobal?.FirstOrDefault(t => t.Key == aluno.First().CodigoAluno.ToString());
                    var percentualFrequenciaGlobal = frequeciaGlobal != null ? frequeciaGlobal.First().PercentualFrequencia : 100;
                    var parecerConclusivo = pareceresConclusivos.FirstOrDefault(c => c.TurmaId.ToString() == turma.Codigo && c.AlunoCodigo.ToString() == aluno.Key);

                    boletimEscolarAlunoDto.Cabecalho = ObterCabecalhoInicial(dre, ue, turma, aluno.First().CodigoAluno.ToString(), aluno.First().NomeRelatorio, $"{percentualFrequenciaGlobal}%");
                    boletimEscolarAlunoDto.ParecerConclusivo = conselhoClassBimestres.Any(b => b == 0) ? parecerConclusivo?.ParecerConclusivo : null;
                    boletinsAlunos.Add(boletimEscolarAlunoDto);
                }
            }

            return await Task.FromResult(new BoletimEscolarDto(boletinsAlunos));
        }

        private BoletimEscolarCabecalhoDto ObterCabecalhoInicial(Dre dre, Ue ue, Turma turma, string alunoCodigo, string nome, string frequenciaGlobal)
        {
            return new BoletimEscolarCabecalhoDto()
            {
                Data = DateTime.Now.ToString("dd/MM/yyyy"),
                NomeDre = dre.Abreviacao,
                NomeUe = ue.NomeRelatorio,
                NomeTurma = turma.NomeRelatorio,
                CodigoEol = alunoCodigo,
                Aluno = nome,
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

        private void SetarNotasFrequencia(List<GrupoMatrizComponenteCurricularDto> gruposMatriz, IEnumerable<NotasAlunoBimestre> notas, IEnumerable<FrequenciaAluno> frequencia, IEnumerable<MediaFrequencia> mediasFrequencia, IEnumerable<int> conselhoClasseBimestres)
        {
            foreach (var grupoMatriz in gruposMatriz)
            {
                if (grupoMatriz.ComponenteCurricularRegencia != null)
                {
                    if (grupoMatriz.ComponenteCurricularRegencia.Frequencia)
                    {
                        var frequenciasRegencia = frequencia?.Where(f => f.DisciplinaId == grupoMatriz.ComponenteCurricularRegencia.Codigo);

                        grupoMatriz.ComponenteCurricularRegencia.FrequenciaBimestre1 = ObterFrequenciaBimestre(conselhoClasseBimestres, frequenciasRegencia, 1);
                        grupoMatriz.ComponenteCurricularRegencia.FrequenciaBimestre2 = ObterFrequenciaBimestre(conselhoClasseBimestres, frequenciasRegencia, 2);
                        grupoMatriz.ComponenteCurricularRegencia.FrequenciaBimestre3 = ObterFrequenciaBimestre(conselhoClasseBimestres, frequenciasRegencia, 3);
                        grupoMatriz.ComponenteCurricularRegencia.FrequenciaBimestre4 = ObterFrequenciaBimestre(conselhoClasseBimestres, frequenciasRegencia, 4);

                        grupoMatriz.ComponenteCurricularRegencia.FrequenciaFinal = ObterFrequenciaFinalAluno(frequenciasRegencia, conselhoClasseBimestres);
                    }

                    foreach (var componenteCurricular in grupoMatriz.ComponenteCurricularRegencia.ComponentesCurriculares)
                    {
                        if (componenteCurricular.Nota)
                        {
                            var notaFrequenciaComponente = notas?.Where(nf => nf.CodigoComponenteCurricular == componenteCurricular.Codigo);

                            componenteCurricular.NotaBimestre1 = ObterNotaBimestre(conselhoClasseBimestres, notaFrequenciaComponente, 1);
                            componenteCurricular.NotaBimestre2 = ObterNotaBimestre(conselhoClasseBimestres, notaFrequenciaComponente, 2);
                            componenteCurricular.NotaBimestre3 = ObterNotaBimestre(conselhoClasseBimestres, notaFrequenciaComponente, 3);
                            componenteCurricular.NotaBimestre4 = ObterNotaBimestre(conselhoClasseBimestres, notaFrequenciaComponente, 4);

                            componenteCurricular.NotaFinal = notaFrequenciaComponente?.FirstOrDefault(nf => nf.PeriodoEscolar == null)?.NotaConceito?.NotaConceito;
                        }
                    }
                }

                if (grupoMatriz.ComponentesCurriculares != null && grupoMatriz.ComponentesCurriculares.Any())
                {
                    foreach (var componenteCurricular in grupoMatriz.ComponentesCurriculares)
                    {
                        var frequenciasComponente = frequencia?.Where(f => f.DisciplinaId == componenteCurricular.Codigo);

                        if (componenteCurricular.Nota)
                        {
                            var notasComponente = notas?.Where(n => n.CodigoComponenteCurricular == componenteCurricular.Codigo) ?? null;

                            componenteCurricular.NotaBimestre1 = ObterNotaBimestre(conselhoClasseBimestres, notasComponente, 1);
                            componenteCurricular.NotaBimestre2 = ObterNotaBimestre(conselhoClasseBimestres, notasComponente, 2);
                            componenteCurricular.NotaBimestre3 = ObterNotaBimestre(conselhoClasseBimestres, notasComponente, 3);
                            componenteCurricular.NotaBimestre4 = ObterNotaBimestre(conselhoClasseBimestres, notasComponente, 4);

                            componenteCurricular.NotaFinal = notasComponente?.FirstOrDefault(nf => nf.PeriodoEscolar == null)?.NotaConceito?.NotaConceito;
                        }
                        else
                            componenteCurricular.NotaFinal = ObterSintese(frequenciasComponente, mediasFrequencia, false, false);


                        if (componenteCurricular.Frequencia)
                        {

                            componenteCurricular.FrequenciaBimestre1 = ObterFrequenciaBimestre(conselhoClasseBimestres, frequenciasComponente, 1);
                            componenteCurricular.FrequenciaBimestre2 = ObterFrequenciaBimestre(conselhoClasseBimestres, frequenciasComponente, 2);
                            componenteCurricular.FrequenciaBimestre3 = ObterFrequenciaBimestre(conselhoClasseBimestres, frequenciasComponente, 3);
                            componenteCurricular.FrequenciaBimestre4 = ObterFrequenciaBimestre(conselhoClasseBimestres, frequenciasComponente, 4);

                            componenteCurricular.FrequenciaFinal = ObterFrequenciaFinalAluno(frequenciasComponente, conselhoClasseBimestres); ;

                            if (!componenteCurricular.Nota)
                                componenteCurricular.NotaFinal = ObterSintese(frequenciasComponente, mediasFrequencia, false, false);
                        }
                    }
                }
            }
        }

        private string ObterNotaBimestre(IEnumerable<int> conselhoClassBimestres, IEnumerable<NotasAlunoBimestre> notasComponente, int bimestre)
        {
            return !VerificaPossuiConselho(conselhoClassBimestres, bimestre) ? "" :
                notasComponente?.FirstOrDefault(nf => nf.PeriodoEscolar != null && nf.PeriodoEscolar.Bimestre == bimestre)?.NotaConceito?.NotaConceito;
        }

        private bool VerificaPossuiConselho(IEnumerable<int> conselhoClassBimestres, int bimestre)
        {
            return conselhoClassBimestres.Any(a => a == bimestre);
        }

        private string ObterFrequenciaBimestre(IEnumerable<int> conselhoClassBimestres, IEnumerable<FrequenciaAluno> frequenciasComponente, int bimestre)
        {
            return !VerificaPossuiConselho(conselhoClassBimestres, bimestre) ? "" :
                frequenciasComponente?.FirstOrDefault(nf => nf.Bimestre == bimestre)?.PercentualFrequencia.ToString() ?? FREQUENCIA_100;

        }

        private string ObterFrequenciaRegenciaBimestre(bool nota, IEnumerable<int> conselhoClassBimestres, IEnumerable<FrequenciaAluno> frequenciasComponente, int bimestre)
        {
            return !nota ? "" :
                !VerificaPossuiConselho(conselhoClassBimestres, bimestre) ? "" :
                frequenciasComponente?.FirstOrDefault(nf => nf.Bimestre == bimestre)?.PercentualFrequencia.ToString() ?? FREQUENCIA_100;

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
