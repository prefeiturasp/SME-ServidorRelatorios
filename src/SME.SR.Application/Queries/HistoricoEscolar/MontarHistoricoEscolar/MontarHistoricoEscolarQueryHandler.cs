using DocumentFormat.OpenXml.Office2010.ExcelAc;
using MediatR;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RazorEngine.Compilation.ImpromptuInterface;
using SME.SR.Data;
using SME.SR.Data.Models;
using SME.SR.Infra;
using SME.SR.Infra.Utilitarios;
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

            foreach (var aluno in request.AlunosTurmas)
            {
                var componentesDaTurma = request.ComponentesCurricularesTurmas.Where(cc => aluno.Turmas.Select(c => c.Codigo).Contains(cc.Key)).SelectMany(ct => ct).Distinct();
                var componentesPorGrupoMatriz = componentesDaTurma.GroupBy(cc => cc.GrupoMatriz);

                var baseNacionalComum = componentesPorGrupoMatriz.FirstOrDefault(cpm => cpm.Key.Id == 1)?.Select(b => b);
                var diversificados = componentesPorGrupoMatriz.FirstOrDefault(cpm => cpm.Key.Id == 2)?.Select(d => d);
                var enriquecimentos = componentesPorGrupoMatriz.FirstOrDefault(cpm => cpm.Key.Id == 3)?.Select(e => e);
                var projetos = componentesPorGrupoMatriz.FirstOrDefault(cpm => cpm.Key.Id == 4)?.Select(p => p);

                var diversificadosDto = ObterGruposDiversificado(diversificados, request.AreasConhecimento);
                var baseNacionalDto = ObterBaseNacionalComum(baseNacionalComum, request.AreasConhecimento);
                var enriquecimentoDto = ObterEnriquecimentos(enriquecimentos);
                var projetosDto = ObterProjetos(projetos);

                var historicoDto = new HistoricoEscolarDTO()
                {
                    NomeDre = request.Dre.Nome,
                    Cabecalho = request.Cabecalho,
                    InformacoesAluno = aluno.Aluno,
                    GruposComponentesCurriculares = diversificadosDto,
                    BaseNacionalComum = baseNacionalDto,
                    ProjetosAtividadesComplementares = projetosDto
                };

                var notasAluno = request.Notas.Where(n => aluno.Turmas.Select(t => t.Codigo).Contains(n.Key)).SelectMany(a => a).Where(w => w.CodigoAluno == aluno.Aluno.Codigo && w.PeriodoEscolar == null);
                var frequenciasAluno = request.Frequencias.Where(f => aluno.Turmas.Select(t => t.Codigo).Contains(f.Key)).SelectMany(a => a).Where(a => a.CodigoAluno == aluno.Aluno.Codigo);

                SetarNotasFrequencia(aluno.Turmas.ToArray(), historicoDto, notasAluno, frequenciasAluno, request.MediasFrequencia);

                listaRetorno.Add(historicoDto);
            }

            return await Task.FromResult(listaRetorno);
        }

        private List<GruposComponentesCurricularesDto> ObterGruposDiversificado(IEnumerable<ComponenteCurricularPorTurma> componentesCurricularesDaTurma,
                                                                                IEnumerable<AreaDoConhecimento> areasDoConhecimentos)
        {
            var gruposComponentes = new List<GruposComponentesCurricularesDto>();

            if (componentesCurricularesDaTurma != null && componentesCurricularesDaTurma.Any())
            {
                var componentesPorGrupoMatriz = componentesCurricularesDaTurma.GroupBy(cc => cc.GrupoMatriz);
                var areasConhecimento = areasDoConhecimentos.Where(a => componentesCurricularesDaTurma.Select(cc => cc.CodDisciplina).Contains(a.CodigoComponenteCurricular)).GroupBy(g => new { g.Nome, g.Id });

                gruposComponentes.AddRange(componentesPorGrupoMatriz.Select(gp => new GruposComponentesCurricularesDto
                {
                    Nome = gp.Key.Nome,
                    AreasDeConhecimento = areasConhecimento.Select(ac => new AreaDeConhecimentoDto()
                    {
                        Nome = ac.Key.Nome,
                        ComponentesCurriculares = componentesCurricularesDaTurma.Where(c => ac.Select(a => a.CodigoComponenteCurricular).Contains(c.CodDisciplina)).Select(cc => new ComponenteCurricularHistoricoEscolarDto()
                        {
                            Nome = cc.Disciplina,
                            Codigo = cc.CodDisciplina.ToString(),
                            Frequencia = cc.Frequencia,
                            Nota = cc.LancaNota
                        }).ToList()
                    }).ToList()
                }));
            }

            return gruposComponentes;
        }

        private List<EnriquecimentoCurricularDto> ObterEnriquecimentos(IEnumerable<ComponenteCurricularPorTurma> componentesCurricularesDaTurma)
        {
            var enriquecimentos = new List<EnriquecimentoCurricularDto>();

            if (componentesCurricularesDaTurma != null && componentesCurricularesDaTurma.Any())
            {
                enriquecimentos.AddRange(componentesCurricularesDaTurma.Select(gp => new EnriquecimentoCurricularDto
                {
                    Codigo = gp.CodDisciplina.ToString(),
                    Nome = gp.Disciplina,
                    Nota = gp.LancaNota,
                    Frequencia = gp.Frequencia
                }));
            }

            return enriquecimentos;
        }

        private List<ProjetosAtividadesComplementaresDto> ObterProjetos(IEnumerable<ComponenteCurricularPorTurma> componentesCurricularesDaTurma)
        {
            var projetos = new List<ProjetosAtividadesComplementaresDto>();

            if (componentesCurricularesDaTurma != null && componentesCurricularesDaTurma.Any())
            {
                var componentesPorGrupoMatriz = componentesCurricularesDaTurma.GroupBy(cc => cc.GrupoMatriz);

                projetos.AddRange(componentesCurricularesDaTurma.Select(gp => new ProjetosAtividadesComplementaresDto
                {
                    Codigo = gp.CodDisciplina.ToString(),
                    Nome = gp.Disciplina,
                    Nota = gp.LancaNota,
                    Frequencia = gp.Frequencia
                }));
            }

            return projetos;
        }

        private BaseNacionalComumDto ObterBaseNacionalComum(IEnumerable<ComponenteCurricularPorTurma> componentesCurricularesDaTurma,
                                                            IEnumerable<AreaDoConhecimento> areasDoConhecimentos)
        {
            BaseNacionalComumDto baseNacional = null;

            if (componentesCurricularesDaTurma != null && componentesCurricularesDaTurma.Any())
            {
                var areasConhecimento = areasDoConhecimentos.Where(a => componentesCurricularesDaTurma.Select(cc => cc.CodDisciplina).Contains(a.CodigoComponenteCurricular)).GroupBy(g => new { g.Nome, g.Id });

                baseNacional = new BaseNacionalComumDto()
                {
                    AreasDeConhecimento = areasConhecimento.Select(ac => new AreaDeConhecimentoDto()
                    {
                        Nome = ac.Key.Nome,
                        ComponentesCurriculares = componentesCurricularesDaTurma.Where(c => ac.Select(a => a.CodigoComponenteCurricular).Contains(c.CodDisciplina)).Select(cc => new ComponenteCurricularHistoricoEscolarDto()
                        {
                            Nome = cc.Disciplina,
                            Codigo = cc.CodDisciplina.ToString(),
                            Frequencia = cc.Frequencia,
                            Nota = cc.LancaNota
                        }).ToList()
                    }).ToList()
                };
            }

            return baseNacional;
        }

        private void SetarNotasFrequencia(Turma[] turmas, HistoricoEscolarDTO historicoEscolarDTO, IEnumerable<NotasAlunoBimestre> notas, IEnumerable<FrequenciaAluno> frequencia, IEnumerable<MediaFrequencia> mediasFrequencia)
        {
            foreach (var turma in turmas)
            {
                //Base Conhecimento
                if (historicoEscolarDTO.BaseNacionalComum != null)
                    historicoEscolarDTO.
                        BaseNacionalComum.AreasDeConhecimento.ForEach(ac =>
                            ac.ComponentesCurriculares.ForEach(cc =>
                            {
                                var notaComponente = notas?.FirstOrDefault(nf => nf.PeriodoEscolar == null && nf.CodigoTurma == turma.Codigo)?.NotaConceito?.NotaConceito;
                                var frequenciaComponente = frequencia?.FirstOrDefault(nf => nf.PeriodoEscolarId == null && nf.TurmaId == turma.Codigo)?.PercentualFrequencia.ToString() ?? "100";

                                if (cc.Nota)
                                {
                                    if (turma.Ano == 1)
                                        cc.NotaConceitoPrimeiroAno = notaComponente;
                                    else if (turma.Ano == 2)
                                        cc.NotaConceitoSegundoAno = notaComponente;
                                    else if (turma.Ano == 3)
                                        cc.NotaConceitoTerceiroAno = notaComponente;
                                    else if (turma.Ano == 4)
                                        cc.NotaConceitoQuartoAno = notaComponente;
                                    else if (turma.Ano == 5)
                                        cc.NotaConceitoQuintoAno = notaComponente;
                                    else if (turma.Ano == 6)
                                        cc.NotaConceitoSextoAno = notaComponente;
                                    else if (turma.Ano == 7)
                                        cc.NotaConceitoSetimoAno = notaComponente;
                                    else if (turma.Ano == 8)
                                        cc.NotaConceitoOitavoAno = notaComponente;
                                    else
                                        cc.NotaConceitoNonoAno = notaComponente;
                                }
                                else
                                {
                                    var frequenciasPorTurmaBimestre = frequencia?.Where(nf => nf.PeriodoEscolarId != null && nf.TurmaId == turma.Codigo);

                                    if (turma.Ano == 1)
                                        cc.NotaConceitoPrimeiroAno = ObterSintese(frequenciasPorTurmaBimestre, mediasFrequencia, false, false);
                                    else if (turma.Ano == 2)
                                        cc.NotaConceitoSegundoAno = ObterSintese(frequenciasPorTurmaBimestre, mediasFrequencia, false, false);
                                    else if (turma.Ano == 3)
                                        cc.NotaConceitoTerceiroAno = ObterSintese(frequenciasPorTurmaBimestre, mediasFrequencia, false, false);
                                    else if (turma.Ano == 4)
                                        cc.NotaConceitoQuartoAno = ObterSintese(frequenciasPorTurmaBimestre, mediasFrequencia, false, false);
                                    else if (turma.Ano == 5)
                                        cc.NotaConceitoQuintoAno = ObterSintese(frequenciasPorTurmaBimestre, mediasFrequencia, false, false);
                                    else if (turma.Ano == 6)
                                        cc.NotaConceitoSextoAno = ObterSintese(frequenciasPorTurmaBimestre, mediasFrequencia, false, false);
                                    else if (turma.Ano == 7)
                                        cc.NotaConceitoSetimoAno = ObterSintese(frequenciasPorTurmaBimestre, mediasFrequencia, false, false);
                                    else if (turma.Ano == 8)
                                        cc.NotaConceitoOitavoAno = ObterSintese(frequenciasPorTurmaBimestre, mediasFrequencia, false, false);
                                    else
                                        cc.NotaConceitoNonoAno = ObterSintese(frequenciasPorTurmaBimestre, mediasFrequencia, false, false);
                                }

                                if (cc.Frequencia)
                                {

                                    if (turma.Ano == 1)
                                        cc.FrequenciaPrimeiroAno = notaComponente;
                                    else if (turma.Ano == 2)
                                        cc.FrequenciaSegundoAno = notaComponente;
                                    else if (turma.Ano == 3)
                                        cc.FrequenciaTerceiroAno = notaComponente;
                                    else if (turma.Ano == 4)
                                        cc.FrequenciaQuartoAno = notaComponente;
                                    else if (turma.Ano == 5)
                                        cc.FrequenciaQuintoAno = notaComponente;
                                    else if (turma.Ano == 6)
                                        cc.FrequenciaSextoAno = notaComponente;
                                    else if (turma.Ano == 7)
                                        cc.FrequenciaSetimoAno = notaComponente;
                                    else if (turma.Ano == 8)
                                        cc.FrequenciaOitavoAno = notaComponente;
                                    else if (turma.Ano == 9)
                                        cc.FrequenciaNonoAno = notaComponente;
                                }
                            }));

                //Diversificada
                if (historicoEscolarDTO.GruposComponentesCurriculares != null && historicoEscolarDTO.GruposComponentesCurriculares.Any())
                    historicoEscolarDTO.
                    GruposComponentesCurriculares.ForEach(gc => gc.
                        AreasDeConhecimento.ForEach(ac =>
                        ac.ComponentesCurriculares.ForEach(cc =>
                        {
                            var notaComponente = notas?.FirstOrDefault(nf => nf.PeriodoEscolar == null && nf.CodigoTurma == turma.Codigo)?.NotaConceito?.NotaConceito;
                            var frequenciaComponente = frequencia?.FirstOrDefault(nf => nf.PeriodoEscolarId == null && nf.TurmaId == turma.Codigo)?.PercentualFrequencia.ToString() ?? "100";

                            if (cc.Nota)
                            {
                                if (turma.Ano == 1)
                                    cc.NotaConceitoPrimeiroAno = notaComponente;
                                else if (turma.Ano == 2)
                                    cc.NotaConceitoSegundoAno = notaComponente;
                                else if (turma.Ano == 3)
                                    cc.NotaConceitoTerceiroAno = notaComponente;
                                else if (turma.Ano == 4)
                                    cc.NotaConceitoQuartoAno = notaComponente;
                                else if (turma.Ano == 5)
                                    cc.NotaConceitoQuintoAno = notaComponente;
                                else if (turma.Ano == 6)
                                    cc.NotaConceitoSextoAno = notaComponente;
                                else if (turma.Ano == 7)
                                    cc.NotaConceitoSetimoAno = notaComponente;
                                else if (turma.Ano == 8)
                                    cc.NotaConceitoOitavoAno = notaComponente;
                                else
                                    cc.NotaConceitoNonoAno = notaComponente;
                            }
                            else
                            {
                                var frequenciasPorTurmaBimestre = frequencia?.Where(nf => nf.PeriodoEscolarId != null && nf.TurmaId == turma.Codigo);

                                if (turma.Ano == 1)
                                    cc.NotaConceitoPrimeiroAno = ObterSintese(frequenciasPorTurmaBimestre, mediasFrequencia, false, false);
                                else if (turma.Ano == 2)
                                    cc.NotaConceitoSegundoAno = ObterSintese(frequenciasPorTurmaBimestre, mediasFrequencia, false, false);
                                else if (turma.Ano == 3)
                                    cc.NotaConceitoTerceiroAno = ObterSintese(frequenciasPorTurmaBimestre, mediasFrequencia, false, false);
                                else if (turma.Ano == 4)
                                    cc.NotaConceitoQuartoAno = ObterSintese(frequenciasPorTurmaBimestre, mediasFrequencia, false, false);
                                else if (turma.Ano == 5)
                                    cc.NotaConceitoQuintoAno = ObterSintese(frequenciasPorTurmaBimestre, mediasFrequencia, false, false);
                                else if (turma.Ano == 6)
                                    cc.NotaConceitoSextoAno = ObterSintese(frequenciasPorTurmaBimestre, mediasFrequencia, false, false);
                                else if (turma.Ano == 7)
                                    cc.NotaConceitoSetimoAno = ObterSintese(frequenciasPorTurmaBimestre, mediasFrequencia, false, false);
                                else if (turma.Ano == 8)
                                    cc.NotaConceitoOitavoAno = ObterSintese(frequenciasPorTurmaBimestre, mediasFrequencia, false, false);
                                else
                                    cc.NotaConceitoNonoAno = ObterSintese(frequenciasPorTurmaBimestre, mediasFrequencia, false, false);
                            }

                            if (cc.Frequencia)
                            {

                                if (turma.Ano == 1)
                                    cc.FrequenciaPrimeiroAno = notaComponente;
                                else if (turma.Ano == 2)
                                    cc.FrequenciaSegundoAno = notaComponente;
                                else if (turma.Ano == 3)
                                    cc.FrequenciaTerceiroAno = notaComponente;
                                else if (turma.Ano == 4)
                                    cc.FrequenciaQuartoAno = notaComponente;
                                else if (turma.Ano == 5)
                                    cc.FrequenciaQuintoAno = notaComponente;
                                else if (turma.Ano == 6)
                                    cc.FrequenciaSextoAno = notaComponente;
                                else if (turma.Ano == 7)
                                    cc.FrequenciaSetimoAno = notaComponente;
                                else if (turma.Ano == 8)
                                    cc.FrequenciaOitavoAno = notaComponente;
                                else if (turma.Ano == 9)
                                    cc.FrequenciaNonoAno = notaComponente;
                            }
                        })));

                //Enriquecimento Curricular
                if (historicoEscolarDTO.EnriquecimentoCurricular != null && historicoEscolarDTO.EnriquecimentoCurricular.Any())
                    historicoEscolarDTO.
                    EnriquecimentoCurricular.ForEach(cc =>
                    {
                        var notaComponente = notas?.FirstOrDefault(nf => nf.PeriodoEscolar == null && nf.CodigoTurma == turma.Codigo)?.NotaConceito?.NotaConceito;
                        var frequenciaComponente = frequencia?.FirstOrDefault(nf => nf.PeriodoEscolarId == null && nf.TurmaId == turma.Codigo)?.PercentualFrequencia.ToString() ?? "100";

                        if (cc.Nota)
                        {
                            if (turma.Ano == 1)
                                cc.NotaConceitoPrimeiroAno = notaComponente;
                            else if (turma.Ano == 2)
                                cc.NotaConceitoSegundoAno = notaComponente;
                            else if (turma.Ano == 3)
                                cc.NotaConceitoTerceiroAno = notaComponente;
                            else if (turma.Ano == 4)
                                cc.NotaConceitoQuartoAno = notaComponente;
                            else if (turma.Ano == 5)
                                cc.NotaConceitoQuintoAno = notaComponente;
                            else if (turma.Ano == 6)
                                cc.NotaConceitoSextoAno = notaComponente;
                            else if (turma.Ano == 7)
                                cc.NotaConceitoSetimoAno = notaComponente;
                            else if (turma.Ano == 8)
                                cc.NotaConceitoOitavoAno = notaComponente;
                            else
                                cc.NotaConceitoNonoAno = notaComponente;
                        }
                        else
                        {
                            var frequenciasPorTurmaBimestre = frequencia?.Where(nf => nf.PeriodoEscolarId != null && nf.TurmaId == turma.Codigo);

                            if (turma.Ano == 1)
                                cc.NotaConceitoPrimeiroAno = ObterSintese(frequenciasPorTurmaBimestre, mediasFrequencia, false, false);
                            else if (turma.Ano == 2)
                                cc.NotaConceitoSegundoAno = ObterSintese(frequenciasPorTurmaBimestre, mediasFrequencia, false, false);
                            else if (turma.Ano == 3)
                                cc.NotaConceitoTerceiroAno = ObterSintese(frequenciasPorTurmaBimestre, mediasFrequencia, false, false);
                            else if (turma.Ano == 4)
                                cc.NotaConceitoQuartoAno = ObterSintese(frequenciasPorTurmaBimestre, mediasFrequencia, false, false);
                            else if (turma.Ano == 5)
                                cc.NotaConceitoQuintoAno = ObterSintese(frequenciasPorTurmaBimestre, mediasFrequencia, false, false);
                            else if (turma.Ano == 6)
                                cc.NotaConceitoSextoAno = ObterSintese(frequenciasPorTurmaBimestre, mediasFrequencia, false, false);
                            else if (turma.Ano == 7)
                                cc.NotaConceitoSetimoAno = ObterSintese(frequenciasPorTurmaBimestre, mediasFrequencia, false, false);
                            else if (turma.Ano == 8)
                                cc.NotaConceitoOitavoAno = ObterSintese(frequenciasPorTurmaBimestre, mediasFrequencia, false, false);
                            else
                                cc.NotaConceitoNonoAno = ObterSintese(frequenciasPorTurmaBimestre, mediasFrequencia, false, false);
                        }
                    });

                //Projetos
                if (historicoEscolarDTO.ProjetosAtividadesComplementares != null && historicoEscolarDTO.ProjetosAtividadesComplementares.Any())
                    historicoEscolarDTO.ProjetosAtividadesComplementares
                     .ForEach(cc =>
                     {
                         var notaComponente = notas?.FirstOrDefault(nf => nf.PeriodoEscolar == null && nf.CodigoTurma == turma.Codigo)?.NotaConceito?.NotaConceito;
                         var frequenciaComponente = frequencia?.FirstOrDefault(nf => nf.PeriodoEscolarId == null && nf.TurmaId == turma.Codigo)?.PercentualFrequencia.ToString() ?? "100";

                         if (cc.Nota)
                         {
                             if (turma.Ano == 1)
                                 cc.NotaConceitoPrimeiroAno = notaComponente;
                             else if (turma.Ano == 2)
                                 cc.NotaConceitoSegundoAno = notaComponente;
                             else if (turma.Ano == 3)
                                 cc.NotaConceitoTerceiroAno = notaComponente;
                             else if (turma.Ano == 4)
                                 cc.NotaConceitoQuartoAno = notaComponente;
                             else if (turma.Ano == 5)
                                 cc.NotaConceitoQuintoAno = notaComponente;
                             else if (turma.Ano == 6)
                                 cc.NotaConceitoSextoAno = notaComponente;
                             else if (turma.Ano == 7)
                                 cc.NotaConceitoSetimoAno = notaComponente;
                             else if (turma.Ano == 8)
                                 cc.NotaConceitoOitavoAno = notaComponente;
                             else
                                 cc.NotaConceitoNonoAno = notaComponente;
                         }
                         else
                         {
                             var frequenciasPorTurmaBimestre = frequencia?.Where(nf => nf.PeriodoEscolarId != null && nf.TurmaId == turma.Codigo);

                             if (turma.Ano == 1)
                                 cc.NotaConceitoPrimeiroAno = ObterSintese(frequenciasPorTurmaBimestre, mediasFrequencia, false, false);
                             else if (turma.Ano == 2)
                                 cc.NotaConceitoSegundoAno = ObterSintese(frequenciasPorTurmaBimestre, mediasFrequencia, false, false);
                             else if (turma.Ano == 3)
                                 cc.NotaConceitoTerceiroAno = ObterSintese(frequenciasPorTurmaBimestre, mediasFrequencia, false, false);
                             else if (turma.Ano == 4)
                                 cc.NotaConceitoQuartoAno = ObterSintese(frequenciasPorTurmaBimestre, mediasFrequencia, false, false);
                             else if (turma.Ano == 5)
                                 cc.NotaConceitoQuintoAno = ObterSintese(frequenciasPorTurmaBimestre, mediasFrequencia, false, false);
                             else if (turma.Ano == 6)
                                 cc.NotaConceitoSextoAno = ObterSintese(frequenciasPorTurmaBimestre, mediasFrequencia, false, false);
                             else if (turma.Ano == 7)
                                 cc.NotaConceitoSetimoAno = ObterSintese(frequenciasPorTurmaBimestre, mediasFrequencia, false, false);
                             else if (turma.Ano == 8)
                                 cc.NotaConceitoOitavoAno = ObterSintese(frequenciasPorTurmaBimestre, mediasFrequencia, false, false);
                             else
                                 cc.NotaConceitoNonoAno = ObterSintese(frequenciasPorTurmaBimestre, mediasFrequencia, false, false);
                         }
                     });
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
