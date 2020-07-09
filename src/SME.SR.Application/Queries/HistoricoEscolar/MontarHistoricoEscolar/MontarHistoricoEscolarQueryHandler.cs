using DocumentFormat.OpenXml.Office2010.ExcelAc;
using MediatR;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
    public class MontarHistoricoEscolarQueryHandler : IRequestHandler<MontarHistoricoEscolarQuery, IEnumerable<HistoricoEscolarDTO>>
    {
        public MontarHistoricoEscolarQueryHandler()
        {

        }

        public async Task<IEnumerable<HistoricoEscolarDTO>> Handle(MontarHistoricoEscolarQuery request, CancellationToken cancellationToken)
        {
            var listaRetorno = new List<HistoricoEscolarDTO>();

            foreach (var turmaCodigo in request.TurmasCodigo)
            {
                var componentesDaTurma = request.ComponentesCurricularesTurmas.FirstOrDefault(cc => cc.Key == turmaCodigo);

                var listaComponentesDaTurma = ObterGruposComponentesCurriculares(componentesDaTurma, request.AreasConhecimento);
                var notasTurma = request.Notas.FirstOrDefault(nf => nf.Key == turmaCodigo);
                var frequenciasTurma = request.Frequencias.FirstOrDefault(nf => nf.Key == turmaCodigo);

                foreach (var aluno in request.AlunosTurmas.Where(a => a.Turmas.Select(t => t.Codigo).Contains(turmaCodigo)))
                {
                    var historicoDto = new HistoricoEscolarDTO()
                    {
                        NomeDre = request.Dre.Nome,
                        Cabecalho = request.Cabecalho,
                        InformacoesAluno = aluno.Aluno,
                        GruposComponentesCurriculares = listaComponentesDaTurma
                    };

                    var notasAluno = notasTurma.Where(t => t.CodigoAluno == aluno.Aluno.Codigo.ToString());
                    var frequenciasAluno = frequenciasTurma?.Where(t => t.CodigoAluno == aluno.Aluno.Codigo.ToString());

                    SetarNotasFrequencia(turmaCodigo, historicoDto.GruposComponentesCurriculares, notasAluno, frequenciasAluno, request.MediasFrequencia);

                    listaRetorno.Add(historicoDto);
                }
            }

            return await Task.FromResult(listaRetorno);
        }

        private List<GruposComponentesCurricularesDto> ObterGruposComponentesCurriculares(IEnumerable<ComponenteCurricularPorTurma> componentesCurricularesDaTurma,
                                                                                          IEnumerable<AreaDoConhecimento> areasDoConhecimentos)
        {
            var gruposComponentes = new List<GruposComponentesCurricularesDto>();

            var componentesPorGrupoMatriz = componentesCurricularesDaTurma.GroupBy(cc => cc.GrupoMatriz);

            gruposComponentes.AddRange(componentesPorGrupoMatriz.Select(gp => new GruposComponentesCurricularesDto
            {
                Nome = gp.Key.Nome,
                AreasDeConhecimento = areasDoConhecimentos.Where(ac => gp.Select(c => c.CodDisciplina)
                                      .Contains(ac.CodigoComponenteCurricular)).Select(ac => new AreaDeConhecimentoDto()
                                      {
                                          Nome = ac.Nome,
                                          ComponentesCurriculares = gp.Select(cc => new ComponenteCurricularDto()
                                          {
                                              Nome = cc.Disciplina,
                                              Codigo = cc.CodDisciplina.ToString(),
                                              Frequencia = cc.Frequencia,
                                              Nota = cc.LancaNota
                                          }).ToList()
                                      }).ToList()
            }));

            return gruposComponentes;
        }

        private void SetarNotasFrequencia(string codigoTurma, List<GruposComponentesCurricularesDto> gruposMatriz, IEnumerable<NotasAlunoBimestre> notas, IEnumerable<FrequenciaAluno> frequencia, IEnumerable<MediaFrequencia> mediasFrequencia)
        {
            gruposMatriz.Select(gm =>
                gm.AreasDeConhecimento.Select(ac =>
                    ac.ComponentesCurriculares.Select(cc =>
                    {
                        if (cc.Nota)
                        {
                            var notasComponente = notas?.Where(n => n.CodigoComponenteCurricular == cc.Codigo && n.CodigoTurma == codigoTurma) ?? null;

                            cc.NotaBimestre1 = notasComponente?.FirstOrDefault(nf => nf.PeriodoEscolar != null && nf.PeriodoEscolar.Bimestre == 1)?.NotaConceito?.NotaConceito;
                            cc.NotaBimestre2 = notasComponente?.FirstOrDefault(nf => nf.PeriodoEscolar != null && nf.PeriodoEscolar.Bimestre == 2)?.NotaConceito?.NotaConceito;
                            cc.NotaBimestre3 = notasComponente?.FirstOrDefault(nf => nf.PeriodoEscolar != null && nf.PeriodoEscolar.Bimestre == 3)?.NotaConceito?.NotaConceito;
                            cc.NotaBimestre4 = notasComponente?.FirstOrDefault(nf => nf.PeriodoEscolar != null && nf.PeriodoEscolar.Bimestre == 4)?.NotaConceito?.NotaConceito;

                            cc.NotaFinal = notasComponente?.FirstOrDefault(nf => nf.PeriodoEscolar == null)?.NotaConceito?.NotaConceito;
                        }

                        if (cc.Frequencia)
                        {
                            var frequenciasComponente = frequencia?.Where(f => f.DisciplinaId == cc.Codigo && f.TurmaId == codigoTurma);

                            cc.FrequenciaBimestre1 = frequenciasComponente?.FirstOrDefault(nf => nf.Bimestre == 1)?.PercentualFrequencia.ToString() ?? "100";
                            cc.FrequenciaBimestre2 = frequenciasComponente?.FirstOrDefault(nf => nf.Bimestre == 2)?.PercentualFrequencia.ToString() ?? "100";
                            cc.FrequenciaBimestre3 = frequenciasComponente?.FirstOrDefault(nf => nf.Bimestre == 3)?.PercentualFrequencia.ToString() ?? "100";
                            cc.FrequenciaBimestre4 = frequenciasComponente?.FirstOrDefault(nf => nf.Bimestre == 4)?.PercentualFrequencia.ToString() ?? "100";

                            cc.FrequenciaFinal = frequenciasComponente?.FirstOrDefault(nf => nf.PeriodoEscolarId == null)?.PercentualFrequencia.ToString() ?? "100";

                            if (!cc.Nota)
                                cc.NotaFinal = ObterSintese(frequenciasComponente, mediasFrequencia, false, false);
                        }

                        return cc;

                    })));
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
