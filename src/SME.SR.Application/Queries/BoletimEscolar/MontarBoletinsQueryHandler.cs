using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Collections;
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
            var notasFrequencia = request.NotasFrequencia;

            var boletinsAlunos = new List<BoletimEscolarAlunoDto>();

            foreach (var turma in turmas)
            {
                var boletimEscolarAlunoDto = new BoletimEscolarAlunoDto();

                boletimEscolarAlunoDto.Cabecalho = ObterCabecalhoInicial(dre, ue, turma);

                boletimEscolarAlunoDto.Grupos = MapearGruposEComponentes(componentesCurriculares.FirstOrDefault(cc => cc.Key == turma.Codigo));

                foreach (var aluno in alunos.FirstOrDefault(a => a.Key == turma.Codigo))
                {
                    var notasAluno = notasFrequencia.FirstOrDefault(nf => nf.Key == turma.Codigo).Where(t => t.CodigoAluno == aluno.CodigoAluno.ToString());



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
                NomeUe = ue.NomeRelatorio
            };
        }

        private List<GrupoMatrizComponenteCurricularDto> MapearGruposEComponentes(IEnumerable<ComponenteCurricularPorTurma> componentesCurricularesPorTurma)
        {
            var gruposMatrizes = componentesCurricularesPorTurma.GroupBy(cc => cc.GrupoMatriz);

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
                    if (componente.Regencia)
                    {
                        grupoParaAdd.ComponenteCurricularRegencia = new ComponenteCurricularRegenciaDto();

                        foreach (var componenteRegencia in componente.ComponentesCurricularesRegencia)
                        {
                            grupoParaAdd.ComponenteCurricularRegencia.ComponentesCurriculares.Add(
                                new ComponenteCurricularRegenciaNotaDto()
                                {
                                    Codigo = componenteRegencia.CodDisciplina.ToString(),
                                    Nome = componenteRegencia.Disciplina
                                });
                        }
                    }
                    else
                    {
                        if (grupoParaAdd.ComponentesCurriculares == null)
                            grupoParaAdd.ComponentesCurriculares = new List<ComponenteCurricularDto>();

                        grupoParaAdd.ComponentesCurriculares.Add(
                            new ComponenteCurricularDto()
                            {
                                Codigo = componente.CodDisciplina.ToString(),
                                Nome = componente.Disciplina
                            });
                    }

                }

                gruposRetorno.Add(grupoParaAdd);
            }

            return gruposRetorno;
        }

        private void SetarNotasFrequencia(List<GrupoMatrizComponenteCurricularDto> gruposMatriz, IEnumerable<NotasFrequenciaAlunoBimestre> notasFrequencia)
        {
            foreach (var grupoMatriz in gruposMatriz)
            {
                if(grupoMatriz.ComponenteCurricularRegencia != null)
                {
                    grupoMatriz.ComponenteCurricularRegencia.FrequenciaBimestre1 = notaFrequenciaComponente.FirstOrDefault(nf => nf.PeriodoEscolar.Bimestre == 1)?.FrequenciaAluno.PercentualFrequencia;
                    grupoMatriz.ComponenteCurricularRegencia.FrequenciaBimestre2 = notaFrequenciaComponente.FirstOrDefault(nf => nf.PeriodoEscolar.Bimestre == 2)?.FrequenciaAluno.PercentualFrequencia;
                    grupoMatriz.ComponenteCurricularRegencia.FrequenciaBimestre3 = notaFrequenciaComponente.FirstOrDefault(nf => nf.PeriodoEscolar.Bimestre == 3)?.FrequenciaAluno.PercentualFrequencia;
                    grupoMatriz.ComponenteCurricularRegencia.FrequenciaBimestre4 = notaFrequenciaComponente.FirstOrDefault(nf => nf.PeriodoEscolar.Bimestre == 4)?.FrequenciaAluno.PercentualFrequencia;

                    foreach (var componenteCurricular in grupoMatriz.ComponenteCurricularRegencia.ComponentesCurriculares)
                    {
                        var notaFrequenciaComponente = notasFrequencia.Where(nf => nf.CodigoComponenteCurricular == componenteCurricular.Codigo);

                        componenteCurricular.NotaBimestre1 = notaFrequenciaComponente.FirstOrDefault(nf => nf.PeriodoEscolar.Bimestre == 1)?.NotaConceito.NotaConceito;
                        componenteCurricular.NotaBimestre2 = notaFrequenciaComponente.FirstOrDefault(nf => nf.PeriodoEscolar.Bimestre == 2)?.NotaConceito.NotaConceito;
                        componenteCurricular.NotaBimestre3 = notaFrequenciaComponente.FirstOrDefault(nf => nf.PeriodoEscolar.Bimestre == 3)?.NotaConceito.NotaConceito;
                        componenteCurricular.NotaBimestre4 = notaFrequenciaComponente.FirstOrDefault(nf => nf.PeriodoEscolar.Bimestre == 4)?.NotaConceito.NotaConceito;
                    }
                }

                if (grupoMatriz.ComponentesCurriculares != null && grupoMatriz.ComponentesCurriculares.Any())
                {
                    foreach (var componenteCurricular in grupoMatriz.ComponentesCurriculares)
                    {
                        var notaFrequenciaComponente = notasFrequencia.Where(nf => nf.CodigoComponenteCurricular == componenteCurricular.Codigo);

                        componenteCurricular.NotaBimestre1 = notaFrequenciaComponente.FirstOrDefault(nf => nf.PeriodoEscolar.Bimestre == 1)?.NotaConceito.NotaConceito;
                        componenteCurricular.NotaBimestre2 = notaFrequenciaComponente.FirstOrDefault(nf => nf.PeriodoEscolar.Bimestre == 2)?.NotaConceito.NotaConceito;
                        componenteCurricular.NotaBimestre3 = notaFrequenciaComponente.FirstOrDefault(nf => nf.PeriodoEscolar.Bimestre == 3)?.NotaConceito.NotaConceito;
                        componenteCurricular.NotaBimestre4 = notaFrequenciaComponente.FirstOrDefault(nf => nf.PeriodoEscolar.Bimestre == 4)?.NotaConceito.NotaConceito;

                        componenteCurricular.FrequenciaBimestre1 = notaFrequenciaComponente.FirstOrDefault(nf => nf.PeriodoEscolar.Bimestre == 1)?.FrequenciaAluno.PercentualFrequencia;
                        componenteCurricular.FrequenciaBimestre2 = notaFrequenciaComponente.FirstOrDefault(nf => nf.PeriodoEscolar.Bimestre == 2)?.FrequenciaAluno.PercentualFrequencia;
                        componenteCurricular.FrequenciaBimestre3 = notaFrequenciaComponente.FirstOrDefault(nf => nf.PeriodoEscolar.Bimestre == 3)?.FrequenciaAluno.PercentualFrequencia;
                        componenteCurricular.FrequenciaBimestre4 = notaFrequenciaComponente.FirstOrDefault(nf => nf.PeriodoEscolar.Bimestre == 4)?.FrequenciaAluno.PercentualFrequencia;
                    }
                }

            }
        }
    }
}
