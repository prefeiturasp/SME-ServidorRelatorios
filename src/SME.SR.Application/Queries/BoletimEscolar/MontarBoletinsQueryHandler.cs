using MediatR;
using SME.SR.Data;
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
        public async Task<BoletimEscolarDto> Handle(MontarBoletinsQuery request, CancellationToken cancellationToken)
        {
            var turmas = request.Turmas;
            var dre = request.Dre;
            var ue = request.Ue;
            var alunos = request.AlunosPorTuma;
            var componentesCurriculares = request.ComponentesCurricularesPorTurma;
            var notas = request.Notas;
            var frequencia = request.Frequencias;
            var tiposNota = request.TiposNota;
            var mediasFrequencia = request.MediasFrequencia;

            var boletinsAlunos = new List<BoletimEscolarAlunoDto>();

            foreach (var turma in turmas)
            {
                var tipoNota = tiposNota[turma.Codigo];              
                var notasTurma = notas.FirstOrDefault(nf => nf.Key == turma.Codigo);
                var frequenciasTurma = frequencia.FirstOrDefault(nf => nf.Key == turma.Codigo);

                foreach (var aluno in alunos.FirstOrDefault(a => a.Key == turma.Codigo))
                {
                    var grupoComponentesDoAluno = MapearGruposEComponentes(componentesCurriculares.FirstOrDefault(cc => cc.Key == turma.Codigo)); 

                    var boletimEscolarAlunoDto = new BoletimEscolarAlunoDto()
                    {
                        TipoNota = tipoNota,
                        Cabecalho = ObterCabecalhoInicial(dre, ue, turma),
                        Grupos = grupoComponentesDoAluno
                    };

                    boletimEscolarAlunoDto.Cabecalho.CodigoEol = aluno.CodigoAluno.ToString();
                    boletimEscolarAlunoDto.Cabecalho.Aluno = aluno.NomeRelatorio;

                    var notasAluno = notasTurma?.Where(t => t.CodigoAluno == aluno.CodigoAluno.ToString()).ToList();
                    var frequenciasAluno = frequenciasTurma?.Where(t => t.CodigoAluno == aluno.CodigoAluno.ToString()).ToList();

                    SetarNotasFrequencia(boletimEscolarAlunoDto.Grupos, notasAluno, frequenciasAluno, mediasFrequencia);

                    boletinsAlunos.Add(boletimEscolarAlunoDto);
                }
            }

            return await Task.FromResult(new BoletimEscolarDto(boletinsAlunos));
        }

        private BoletimEscolarCabecalhoDto ObterCabecalhoInicial(Dre dre, Ue ue, Turma turma)
        {
            return new BoletimEscolarCabecalhoDto()
            {
                Data = DateTime.Now.ToString("dd/MM/yyyy"),
                NomeDre = dre.Abreviacao,
                NomeUe = ue.NomeRelatorio,
                NomeTurma = turma.NomeRelatorio
            };
        }

        private List<GrupoMatrizComponenteCurricularDto> MapearGruposEComponentes(IEnumerable<ComponenteCurricularPorTurma> componentesCurricularesPorTurma)
        {
            var gruposMatrizes = componentesCurricularesPorTurma.GroupBy(cc => cc.GrupoMatriz).ToList();

            var gruposRetorno = new List<GrupoMatrizComponenteCurricularDto>();

            foreach (var grupoMatriz in gruposMatrizes)
            {
                var grupoParaAdd = new GrupoMatrizComponenteCurricularDto()
                {
                    Id = (int)grupoMatriz.Key.Id,
                    Nome = $"GRUPO {gruposRetorno.Count() + 1}",
                    Descricao = grupoMatriz.Key.Nome
                };

                foreach (var componente in grupoMatriz)
                {
                    if (componente.Regencia && componente.ComponentesCurricularesRegencia != null && componente.ComponentesCurricularesRegencia.Any())
                    {
                        grupoParaAdd.ComponenteCurricularRegencia = new ComponenteCurricularRegenciaDto()
                        {
                            Codigo = componente.CodDisciplina.ToString(),
                            Frequencia = componente.Frequencia
                        };

                        foreach (var componenteRegencia in componente.ComponentesCurricularesRegencia)
                        {
                            grupoParaAdd.ComponenteCurricularRegencia.ComponentesCurriculares.Add(
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
                        if (grupoParaAdd.ComponentesCurriculares == null)
                            grupoParaAdd.ComponentesCurriculares = new List<ComponenteCurricularDto>();

                        grupoParaAdd.ComponentesCurriculares.Add(
                            new ComponenteCurricularDto()
                            {
                                Codigo = componente.CodDisciplina.ToString(),
                                Nome = componente.Disciplina,
                                Nota = componente.LancaNota,
                                Frequencia = componente.Frequencia
                            });
                    }
                }

                gruposRetorno.Add(grupoParaAdd);
            }

            return gruposRetorno;
        }

        private void SetarNotasFrequencia(List<GrupoMatrizComponenteCurricularDto> gruposMatriz, IEnumerable<NotasAlunoBimestre> notas, IEnumerable<FrequenciaAluno> frequencia, IEnumerable<MediaFrequencia> mediasFrequencia)
        {
            foreach (var grupoMatriz in gruposMatriz)
            {
                if (grupoMatriz.ComponenteCurricularRegencia != null)
                {
                    if (grupoMatriz.ComponenteCurricularRegencia.Frequencia)
                    {
                        var frequenciasRegencia = frequencia?.Where(f => f.DisciplinaId == grupoMatriz.ComponenteCurricularRegencia.Codigo);

                        grupoMatriz.ComponenteCurricularRegencia.FrequenciaBimestre1 = frequenciasRegencia?.FirstOrDefault(f => f.Bimestre == 1)?.PercentualFrequencia.ToString() ?? "100";
                        grupoMatriz.ComponenteCurricularRegencia.FrequenciaBimestre2 = frequenciasRegencia?.FirstOrDefault(f => f.Bimestre == 2)?.PercentualFrequencia.ToString() ?? "100";
                        grupoMatriz.ComponenteCurricularRegencia.FrequenciaBimestre3 = frequenciasRegencia?.FirstOrDefault(f => f.Bimestre == 3)?.PercentualFrequencia.ToString() ?? "100";
                        grupoMatriz.ComponenteCurricularRegencia.FrequenciaBimestre4 = frequenciasRegencia?.FirstOrDefault(f => f.Bimestre == 4)?.PercentualFrequencia.ToString() ?? "100";

                        grupoMatriz.ComponenteCurricularRegencia.FrequenciaFinal = ObterFrequenciaFinalAluno(frequenciasRegencia);
                    }

                    foreach (var componenteCurricular in grupoMatriz.ComponenteCurricularRegencia.ComponentesCurriculares)
                    {
                        if (componenteCurricular.Nota)
                        {
                            var notaFrequenciaComponente = notas?.Where(nf => nf.CodigoComponenteCurricular == componenteCurricular.Codigo);

                            componenteCurricular.NotaBimestre1 = notaFrequenciaComponente?.FirstOrDefault(nf => nf.PeriodoEscolar != null && nf.PeriodoEscolar.Bimestre == 1)?.NotaConceito?.NotaConceito;
                            componenteCurricular.NotaBimestre2 = notaFrequenciaComponente?.FirstOrDefault(nf => nf.PeriodoEscolar != null && nf.PeriodoEscolar.Bimestre == 2)?.NotaConceito?.NotaConceito;
                            componenteCurricular.NotaBimestre3 = notaFrequenciaComponente?.FirstOrDefault(nf => nf.PeriodoEscolar != null && nf.PeriodoEscolar.Bimestre == 3)?.NotaConceito?.NotaConceito;
                            componenteCurricular.NotaBimestre4 = notaFrequenciaComponente?.FirstOrDefault(nf => nf.PeriodoEscolar != null && nf.PeriodoEscolar.Bimestre == 4)?.NotaConceito?.NotaConceito;

                            componenteCurricular.NotaFinal = notaFrequenciaComponente?.FirstOrDefault(nf => nf.PeriodoEscolar == null)?.NotaConceito?.NotaConceito;
                        }
                    }
                }

                if (grupoMatriz.ComponentesCurriculares != null && grupoMatriz.ComponentesCurriculares.Any())
                {
                    foreach (var componenteCurricular in grupoMatriz.ComponentesCurriculares)
                    {
                        if (componenteCurricular.Nota)
                        {
                            var notasComponente = notas?.Where(n => n.CodigoComponenteCurricular == componenteCurricular.Codigo) ?? null;

                            componenteCurricular.NotaBimestre1 = notasComponente?.FirstOrDefault(nf => nf.PeriodoEscolar != null && nf.PeriodoEscolar.Bimestre == 1)?.NotaConceito?.NotaConceito;
                            componenteCurricular.NotaBimestre2 = notasComponente?.FirstOrDefault(nf => nf.PeriodoEscolar != null && nf.PeriodoEscolar.Bimestre == 2)?.NotaConceito?.NotaConceito;
                            componenteCurricular.NotaBimestre3 = notasComponente?.FirstOrDefault(nf => nf.PeriodoEscolar != null && nf.PeriodoEscolar.Bimestre == 3)?.NotaConceito?.NotaConceito;
                            componenteCurricular.NotaBimestre4 = notasComponente?.FirstOrDefault(nf => nf.PeriodoEscolar != null && nf.PeriodoEscolar.Bimestre == 4)?.NotaConceito?.NotaConceito;

                            componenteCurricular.NotaFinal = notasComponente?.FirstOrDefault(nf => nf.PeriodoEscolar == null)?.NotaConceito?.NotaConceito;
                        }

                        if (componenteCurricular.Frequencia)
                        {
                            var frequenciasComponente = frequencia?.Where(f => f.DisciplinaId == componenteCurricular.Codigo);

                            componenteCurricular.FrequenciaBimestre1 = frequenciasComponente?.FirstOrDefault(nf => nf.Bimestre == 1)?.PercentualFrequencia.ToString() ?? "100";
                            componenteCurricular.FrequenciaBimestre2 = frequenciasComponente?.FirstOrDefault(nf => nf.Bimestre == 2)?.PercentualFrequencia.ToString() ?? "100";
                            componenteCurricular.FrequenciaBimestre3 = frequenciasComponente?.FirstOrDefault(nf => nf.Bimestre == 3)?.PercentualFrequencia.ToString() ?? "100";
                            componenteCurricular.FrequenciaBimestre4 = frequenciasComponente?.FirstOrDefault(nf => nf.Bimestre == 4)?.PercentualFrequencia.ToString() ?? "100";

                            componenteCurricular.FrequenciaFinal = ObterFrequenciaFinalAluno(frequenciasComponente);

                            if (!componenteCurricular.Nota)
                                componenteCurricular.NotaFinal = ObterSintese(frequenciasComponente, mediasFrequencia, false, false);
                        }
                    }
                }
            }
        }

        private string ObterFrequenciaFinalAluno(IEnumerable<FrequenciaAluno> frequencias)
        {
            if (frequencias == null || !frequencias.Any())
                return "100";
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
