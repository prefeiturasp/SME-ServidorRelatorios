using DocumentFormat.OpenXml.Office2010.ExcelAc;
using MediatR;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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

                var notasAluno = request.Notas.Where(n => aluno.Turmas.Select(t => t.Codigo).Contains(n.Key)).SelectMany(a => a).Where(w => w.CodigoAluno == aluno.Aluno.Codigo);
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

                gruposComponentes.AddRange(componentesPorGrupoMatriz.Select(gp => new GruposComponentesCurricularesDto
                {
                    Nome = gp.Key.Nome,
                    AreasDeConhecimento = areasDoConhecimentos.Where(ac => gp.Select(c => c.CodDisciplina)
                                          .Contains(ac.CodigoComponenteCurricular)).Select(ac => new AreaDeConhecimentoDto()
                                          {
                                              Nome = ac.Nome,
                                              ComponentesCurriculares = gp.Select(cc => new ComponenteCurricularHistoricoEscolarDto()
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
            BaseNacionalComumDto enriquecimentos = null;

            if (componentesCurricularesDaTurma != null && componentesCurricularesDaTurma.Any())
            {
                enriquecimentos = new BaseNacionalComumDto()
                {
                    AreasDeConhecimento = areasDoConhecimentos.Where(ac => componentesCurricularesDaTurma.Select(c => c.CodDisciplina)
                                            .Contains(ac.CodigoComponenteCurricular)).Select(ac => new AreaDeConhecimentoDto()
                                            {
                                                Nome = ac.Nome,
                                                ComponentesCurriculares = componentesCurricularesDaTurma.Select(cc => new ComponenteCurricularHistoricoEscolarDto()
                                                {
                                                    Nome = cc.Disciplina,
                                                    Codigo = cc.CodDisciplina.ToString(),
                                                    Frequencia = cc.Frequencia,
                                                    Nota = cc.LancaNota
                                                }).ToList()
                                            }).ToList()

                };
            }

            return enriquecimentos;
        }

        private void SetarNotasFrequencia(Turma[] turmas, HistoricoEscolarDTO historicoEscolarDTO, IEnumerable<NotasAlunoBimestre> notas, IEnumerable<FrequenciaAluno> frequencia, IEnumerable<MediaFrequencia> mediasFrequencia)
        {
            Turma turma1Ano = turmas.FirstOrDefault(t => t.Ano == 1);
            Turma turma2Ano = turmas.FirstOrDefault(t => t.Ano == 2);
            Turma turma3Ano = turmas.FirstOrDefault(t => t.Ano == 3);
            Turma turma4Ano = turmas.FirstOrDefault(t => t.Ano == 4);
            Turma turma5Ano = turmas.FirstOrDefault(t => t.Ano == 5);
            Turma turma6Ano = turmas.FirstOrDefault(t => t.Ano == 6);
            Turma turma7Ano = turmas.FirstOrDefault(t => t.Ano == 7);
            Turma turma8Ano = turmas.FirstOrDefault(t => t.Ano == 8);
            Turma turma9Ano = turmas.FirstOrDefault(t => t.Ano == 9);

            //Base Conhecimento
            if (historicoEscolarDTO.BaseNacionalComum != null)
                historicoEscolarDTO.
                    BaseNacionalComum.AreasDeConhecimento.Select(ac =>
                        ac.ComponentesCurriculares.Select(cc =>
                        {
                            var notasComponente = notas?.Where(n => n.CodigoComponenteCurricular == cc.Codigo);
                            var frequenciasComponente = frequencia?.Where(f => f.DisciplinaId == cc.Codigo);

                            if (cc.Nota)
                            {
                                if (turma1Ano != null)
                                    cc.NotaConceitoPrimeiroAno = notasComponente?.FirstOrDefault(nf => nf.PeriodoEscolar == null && nf.CodigoTurma == turma1Ano.Codigo)?.NotaConceito?.NotaConceito;

                                if (turma2Ano != null)
                                    cc.NotaConceitoSegundoAno = notasComponente?.FirstOrDefault(nf => nf.PeriodoEscolar == null && nf.CodigoTurma == turma2Ano.Codigo)?.NotaConceito?.NotaConceito;

                                if (turma3Ano != null)
                                    cc.NotaConceitoTerceiroAno = notasComponente?.FirstOrDefault(nf => nf.PeriodoEscolar == null && nf.CodigoTurma == turma3Ano.Codigo)?.NotaConceito?.NotaConceito;

                                if (turma4Ano != null)
                                    cc.NotaConceitoQuartoAno = notasComponente?.FirstOrDefault(nf => nf.PeriodoEscolar == null && nf.CodigoTurma == turma4Ano.Codigo)?.NotaConceito?.NotaConceito;

                                if (turma5Ano != null)
                                    cc.NotaConceitoQuintoAno = notasComponente?.FirstOrDefault(nf => nf.PeriodoEscolar == null && nf.CodigoTurma == turma5Ano.Codigo)?.NotaConceito?.NotaConceito;

                                if (turma6Ano != null)
                                    cc.NotaConceitoSextoAno = notasComponente?.FirstOrDefault(nf => nf.PeriodoEscolar == null && nf.CodigoTurma == turma6Ano.Codigo)?.NotaConceito?.NotaConceito;

                                if (turma7Ano != null)
                                    cc.NotaConceitoSetimoAno = notasComponente?.FirstOrDefault(nf => nf.PeriodoEscolar == null && nf.CodigoTurma == turma7Ano.Codigo)?.NotaConceito?.NotaConceito;

                                if (turma8Ano != null)
                                    cc.NotaConceitoOitavoAno = notasComponente?.FirstOrDefault(nf => nf.PeriodoEscolar == null && nf.CodigoTurma == turma8Ano.Codigo)?.NotaConceito?.NotaConceito;

                                if (turma9Ano != null)
                                    cc.NotaConceitoNonoAno = notasComponente?.FirstOrDefault(nf => nf.PeriodoEscolar == null && nf.CodigoTurma == turma9Ano.Codigo)?.NotaConceito?.NotaConceito;
                            }
                            else
                            {
                                if (turma1Ano != null)
                                    cc.NotaConceitoPrimeiroAno = ObterSintese(frequenciasComponente.Where(fc => fc.TurmaId == turma1Ano.Codigo), mediasFrequencia, false, false);

                                if (turma2Ano != null)
                                    cc.NotaConceitoSegundoAno = ObterSintese(frequenciasComponente.Where(fc => fc.TurmaId == turma2Ano.Codigo), mediasFrequencia, false, false);

                                if (turma3Ano != null)
                                    cc.NotaConceitoTerceiroAno = ObterSintese(frequenciasComponente.Where(fc => fc.TurmaId == turma3Ano.Codigo), mediasFrequencia, false, false);

                                if (turma4Ano != null)
                                    cc.NotaConceitoQuartoAno = ObterSintese(frequenciasComponente.Where(fc => fc.TurmaId == turma4Ano.Codigo), mediasFrequencia, false, false);

                                if (turma5Ano != null)
                                    cc.NotaConceitoQuintoAno = ObterSintese(frequenciasComponente.Where(fc => fc.TurmaId == turma5Ano.Codigo), mediasFrequencia, false, false);

                                if (turma6Ano != null)
                                    cc.NotaConceitoSextoAno = ObterSintese(frequenciasComponente.Where(fc => fc.TurmaId == turma6Ano.Codigo), mediasFrequencia, false, false);

                                if (turma7Ano != null)
                                    cc.NotaConceitoSetimoAno = ObterSintese(frequenciasComponente.Where(fc => fc.TurmaId == turma7Ano.Codigo), mediasFrequencia, false, false);

                                if (turma8Ano != null)
                                    cc.NotaConceitoOitavoAno = ObterSintese(frequenciasComponente.Where(fc => fc.TurmaId == turma8Ano.Codigo), mediasFrequencia, false, false);

                                if (turma9Ano != null)
                                    cc.NotaConceitoNonoAno = ObterSintese(frequenciasComponente.Where(fc => fc.TurmaId == turma9Ano.Codigo), mediasFrequencia, false, false);
                            }

                            if (cc.Frequencia)
                            {
                                if (turma1Ano != null)
                                    cc.FrequenciaPrimeiroAno = frequenciasComponente?.FirstOrDefault(nf => nf.PeriodoEscolarId == null && nf.TurmaId == turma1Ano.Codigo)?.PercentualFrequencia.ToString() ?? "100";

                                if (turma2Ano != null)
                                    cc.FrequenciaSegundoAno = frequenciasComponente?.FirstOrDefault(nf => nf.PeriodoEscolarId == null && nf.TurmaId == turma2Ano.Codigo)?.PercentualFrequencia.ToString() ?? "100";

                                if (turma3Ano != null)
                                    cc.FrequenciaTerceiroAno = frequenciasComponente?.FirstOrDefault(nf => nf.PeriodoEscolarId == null && nf.TurmaId == turma3Ano.Codigo)?.PercentualFrequencia.ToString() ?? "100";

                                if (turma4Ano != null)
                                    cc.FrequenciaQuartoAno = frequenciasComponente?.FirstOrDefault(nf => nf.PeriodoEscolarId == null && nf.TurmaId == turma4Ano.Codigo)?.PercentualFrequencia.ToString() ?? "100";

                                if (turma5Ano != null)
                                    cc.FrequenciaQuintoAno = frequenciasComponente?.FirstOrDefault(nf => nf.PeriodoEscolarId == null && nf.TurmaId == turma5Ano.Codigo)?.PercentualFrequencia.ToString() ?? "100";

                                if (turma6Ano != null)
                                    cc.FrequenciaSextoAno = frequenciasComponente?.FirstOrDefault(nf => nf.PeriodoEscolarId == null && nf.TurmaId == turma6Ano.Codigo)?.PercentualFrequencia.ToString() ?? "100";

                                if (turma7Ano != null)
                                    cc.FrequenciaSetimoAno = frequenciasComponente?.FirstOrDefault(nf => nf.PeriodoEscolarId == null && nf.TurmaId == turma7Ano.Codigo)?.PercentualFrequencia.ToString() ?? "100";

                                if (turma8Ano != null)
                                    cc.FrequenciaOitavoAno = frequenciasComponente?.FirstOrDefault(nf => nf.PeriodoEscolarId == null && nf.TurmaId == turma8Ano.Codigo)?.PercentualFrequencia.ToString() ?? "100";

                                if (turma9Ano != null)
                                    cc.FrequenciaNonoAno = frequenciasComponente?.FirstOrDefault(nf => nf.PeriodoEscolarId == null && nf.TurmaId == turma9Ano.Codigo)?.PercentualFrequencia.ToString() ?? "100";
                            }

                            return cc;

                        }));

            //Diversificada
            if (historicoEscolarDTO.GruposComponentesCurriculares != null && historicoEscolarDTO.GruposComponentesCurriculares.Any())
                historicoEscolarDTO.
                GruposComponentesCurriculares.Select(gc => gc.
                    AreasDeConhecimento.Select(ac =>
                    ac.ComponentesCurriculares.Select(cc =>
                    {
                        var notasComponente = notas?.Where(n => n.CodigoComponenteCurricular == cc.Codigo);
                        var frequenciasComponente = frequencia?.Where(f => f.DisciplinaId == cc.Codigo);

                        if (cc.Nota)
                        {
                            if (turma1Ano != null)
                                cc.NotaConceitoPrimeiroAno = notasComponente?.FirstOrDefault(nf => nf.PeriodoEscolar == null && nf.CodigoTurma == turma1Ano.Codigo)?.NotaConceito?.NotaConceito;

                            if (turma2Ano != null)
                                cc.NotaConceitoSegundoAno = notasComponente?.FirstOrDefault(nf => nf.PeriodoEscolar == null && nf.CodigoTurma == turma2Ano.Codigo)?.NotaConceito?.NotaConceito;

                            if (turma3Ano != null)
                                cc.NotaConceitoTerceiroAno = notasComponente?.FirstOrDefault(nf => nf.PeriodoEscolar == null && nf.CodigoTurma == turma3Ano.Codigo)?.NotaConceito?.NotaConceito;

                            if (turma4Ano != null)
                                cc.NotaConceitoQuartoAno = notasComponente?.FirstOrDefault(nf => nf.PeriodoEscolar == null && nf.CodigoTurma == turma4Ano.Codigo)?.NotaConceito?.NotaConceito;

                            if (turma5Ano != null)
                                cc.NotaConceitoQuintoAno = notasComponente?.FirstOrDefault(nf => nf.PeriodoEscolar == null && nf.CodigoTurma == turma5Ano.Codigo)?.NotaConceito?.NotaConceito;

                            if (turma6Ano != null)
                                cc.NotaConceitoSextoAno = notasComponente?.FirstOrDefault(nf => nf.PeriodoEscolar == null && nf.CodigoTurma == turma6Ano.Codigo)?.NotaConceito?.NotaConceito;

                            if (turma7Ano != null)
                                cc.NotaConceitoSetimoAno = notasComponente?.FirstOrDefault(nf => nf.PeriodoEscolar == null && nf.CodigoTurma == turma7Ano.Codigo)?.NotaConceito?.NotaConceito;

                            if (turma8Ano != null)
                                cc.NotaConceitoOitavoAno = notasComponente?.FirstOrDefault(nf => nf.PeriodoEscolar == null && nf.CodigoTurma == turma8Ano.Codigo)?.NotaConceito?.NotaConceito;

                            if (turma9Ano != null)
                                cc.NotaConceitoNonoAno = notasComponente?.FirstOrDefault(nf => nf.PeriodoEscolar == null && nf.CodigoTurma == turma9Ano.Codigo)?.NotaConceito?.NotaConceito;
                        }
                        else
                        {
                            if (turma1Ano != null)
                                cc.NotaConceitoPrimeiroAno = ObterSintese(frequenciasComponente.Where(fc => fc.TurmaId == turma1Ano.Codigo), mediasFrequencia, false, false);

                            if (turma2Ano != null)
                                cc.NotaConceitoSegundoAno = ObterSintese(frequenciasComponente.Where(fc => fc.TurmaId == turma2Ano.Codigo), mediasFrequencia, false, false);

                            if (turma3Ano != null)
                                cc.NotaConceitoTerceiroAno = ObterSintese(frequenciasComponente.Where(fc => fc.TurmaId == turma3Ano.Codigo), mediasFrequencia, false, false);

                            if (turma4Ano != null)
                                cc.NotaConceitoQuartoAno = ObterSintese(frequenciasComponente.Where(fc => fc.TurmaId == turma4Ano.Codigo), mediasFrequencia, false, false);

                            if (turma5Ano != null)
                                cc.NotaConceitoQuintoAno = ObterSintese(frequenciasComponente.Where(fc => fc.TurmaId == turma5Ano.Codigo), mediasFrequencia, false, false);

                            if (turma6Ano != null)
                                cc.NotaConceitoSextoAno = ObterSintese(frequenciasComponente.Where(fc => fc.TurmaId == turma6Ano.Codigo), mediasFrequencia, false, false);

                            if (turma7Ano != null)
                                cc.NotaConceitoSetimoAno = ObterSintese(frequenciasComponente.Where(fc => fc.TurmaId == turma7Ano.Codigo), mediasFrequencia, false, false);

                            if (turma8Ano != null)
                                cc.NotaConceitoOitavoAno = ObterSintese(frequenciasComponente.Where(fc => fc.TurmaId == turma8Ano.Codigo), mediasFrequencia, false, false);

                            if (turma9Ano != null)
                                cc.NotaConceitoNonoAno = ObterSintese(frequenciasComponente.Where(fc => fc.TurmaId == turma9Ano.Codigo), mediasFrequencia, false, false);
                        }

                        if (cc.Frequencia)
                        {
                            if (turma1Ano != null)
                                cc.FrequenciaPrimeiroAno = frequenciasComponente?.FirstOrDefault(nf => nf.PeriodoEscolarId == null && nf.TurmaId == turma1Ano.Codigo)?.PercentualFrequencia.ToString() ?? "100";

                            if (turma2Ano != null)
                                cc.FrequenciaSegundoAno = frequenciasComponente?.FirstOrDefault(nf => nf.PeriodoEscolarId == null && nf.TurmaId == turma2Ano.Codigo)?.PercentualFrequencia.ToString() ?? "100";

                            if (turma3Ano != null)
                                cc.FrequenciaTerceiroAno = frequenciasComponente?.FirstOrDefault(nf => nf.PeriodoEscolarId == null && nf.TurmaId == turma3Ano.Codigo)?.PercentualFrequencia.ToString() ?? "100";

                            if (turma4Ano != null)
                                cc.FrequenciaQuartoAno = frequenciasComponente?.FirstOrDefault(nf => nf.PeriodoEscolarId == null && nf.TurmaId == turma4Ano.Codigo)?.PercentualFrequencia.ToString() ?? "100";

                            if (turma5Ano != null)
                                cc.FrequenciaQuintoAno = frequenciasComponente?.FirstOrDefault(nf => nf.PeriodoEscolarId == null && nf.TurmaId == turma5Ano.Codigo)?.PercentualFrequencia.ToString() ?? "100";

                            if (turma6Ano != null)
                                cc.FrequenciaSextoAno = frequenciasComponente?.FirstOrDefault(nf => nf.PeriodoEscolarId == null && nf.TurmaId == turma6Ano.Codigo)?.PercentualFrequencia.ToString() ?? "100";

                            if (turma7Ano != null)
                                cc.FrequenciaSetimoAno = frequenciasComponente?.FirstOrDefault(nf => nf.PeriodoEscolarId == null && nf.TurmaId == turma7Ano.Codigo)?.PercentualFrequencia.ToString() ?? "100";

                            if (turma8Ano != null)
                                cc.FrequenciaOitavoAno = frequenciasComponente?.FirstOrDefault(nf => nf.PeriodoEscolarId == null && nf.TurmaId == turma8Ano.Codigo)?.PercentualFrequencia.ToString() ?? "100";

                            if (turma9Ano != null)
                                cc.FrequenciaNonoAno = frequenciasComponente?.FirstOrDefault(nf => nf.PeriodoEscolarId == null && nf.TurmaId == turma9Ano.Codigo)?.PercentualFrequencia.ToString() ?? "100";
                        }

                        return cc;

                    })));

            //Enriquecimento Curricular
            if (historicoEscolarDTO.EnriquecimentoCurricular != null && historicoEscolarDTO.EnriquecimentoCurricular.Any())
                historicoEscolarDTO.
                EnriquecimentoCurricular.Select(e =>
                    {
                        var notasComponente = notas?.Where(n => n.CodigoComponenteCurricular == e.Codigo);
                        var frequenciasComponente = frequencia?.Where(f => f.DisciplinaId == e.Codigo);

                        if (e.Nota)
                        {
                            if (turma1Ano != null)
                                e.NotaConceitoPrimeiroAno = notasComponente?.FirstOrDefault(nf => nf.PeriodoEscolar == null && nf.CodigoTurma == turma1Ano.Codigo)?.NotaConceito?.NotaConceito;

                            if (turma2Ano != null)
                                e.NotaConceitoSegundoAno = notasComponente?.FirstOrDefault(nf => nf.PeriodoEscolar == null && nf.CodigoTurma == turma2Ano.Codigo)?.NotaConceito?.NotaConceito;

                            if (turma3Ano != null)
                                e.NotaConceitoTerceiroAno = notasComponente?.FirstOrDefault(nf => nf.PeriodoEscolar == null && nf.CodigoTurma == turma3Ano.Codigo)?.NotaConceito?.NotaConceito;

                            if (turma4Ano != null)
                                e.NotaConceitoQuartoAno = notasComponente?.FirstOrDefault(nf => nf.PeriodoEscolar == null && nf.CodigoTurma == turma4Ano.Codigo)?.NotaConceito?.NotaConceito;

                            if (turma5Ano != null)
                                e.NotaConceitoQuintoAno = notasComponente?.FirstOrDefault(nf => nf.PeriodoEscolar == null && nf.CodigoTurma == turma5Ano.Codigo)?.NotaConceito?.NotaConceito;

                            if (turma6Ano != null)
                                e.NotaConceitoSextoAno = notasComponente?.FirstOrDefault(nf => nf.PeriodoEscolar == null && nf.CodigoTurma == turma6Ano.Codigo)?.NotaConceito?.NotaConceito;

                            if (turma7Ano != null)
                                e.NotaConceitoSetimoAno = notasComponente?.FirstOrDefault(nf => nf.PeriodoEscolar == null && nf.CodigoTurma == turma7Ano.Codigo)?.NotaConceito?.NotaConceito;

                            if (turma8Ano != null)
                                e.NotaConceitoOitavoAno = notasComponente?.FirstOrDefault(nf => nf.PeriodoEscolar == null && nf.CodigoTurma == turma8Ano.Codigo)?.NotaConceito?.NotaConceito;

                            if (turma9Ano != null)
                                e.NotaConceitoNonoAno = notasComponente?.FirstOrDefault(nf => nf.PeriodoEscolar == null && nf.CodigoTurma == turma9Ano.Codigo)?.NotaConceito?.NotaConceito;
                        }
                        else
                        {
                            if (turma1Ano != null)
                                e.NotaConceitoPrimeiroAno = ObterSintese(frequenciasComponente.Where(fc => fc.TurmaId == turma1Ano.Codigo), mediasFrequencia, false, false);

                            if (turma2Ano != null)
                                e.NotaConceitoSegundoAno = ObterSintese(frequenciasComponente.Where(fc => fc.TurmaId == turma2Ano.Codigo), mediasFrequencia, false, false);

                            if (turma3Ano != null)
                                e.NotaConceitoTerceiroAno = ObterSintese(frequenciasComponente.Where(fc => fc.TurmaId == turma3Ano.Codigo), mediasFrequencia, false, false);

                            if (turma4Ano != null)
                                e.NotaConceitoQuartoAno = ObterSintese(frequenciasComponente.Where(fc => fc.TurmaId == turma4Ano.Codigo), mediasFrequencia, false, false);

                            if (turma5Ano != null)
                                e.NotaConceitoQuintoAno = ObterSintese(frequenciasComponente.Where(fc => fc.TurmaId == turma5Ano.Codigo), mediasFrequencia, false, false);

                            if (turma6Ano != null)
                                e.NotaConceitoSextoAno = ObterSintese(frequenciasComponente.Where(fc => fc.TurmaId == turma6Ano.Codigo), mediasFrequencia, false, false);

                            if (turma7Ano != null)
                                e.NotaConceitoSetimoAno = ObterSintese(frequenciasComponente.Where(fc => fc.TurmaId == turma7Ano.Codigo), mediasFrequencia, false, false);

                            if (turma8Ano != null)
                                e.NotaConceitoOitavoAno = ObterSintese(frequenciasComponente.Where(fc => fc.TurmaId == turma8Ano.Codigo), mediasFrequencia, false, false);

                            if (turma9Ano != null)
                                e.NotaConceitoNonoAno = ObterSintese(frequenciasComponente.Where(fc => fc.TurmaId == turma9Ano.Codigo), mediasFrequencia, false, false);
                        }

                        return e;

                    });

            //Projetos
            if (historicoEscolarDTO.ProjetosAtividadesComplementares != null && historicoEscolarDTO.ProjetosAtividadesComplementares.Any())
                historicoEscolarDTO.ProjetosAtividadesComplementares
                 .Select(p =>
                 {
                     var notasComponente = notas?.Where(n => n.CodigoComponenteCurricular == p.Codigo);
                     var frequenciasComponente = frequencia?.Where(f => f.DisciplinaId == p.Codigo);

                     if (p.Nota)
                     {
                         if (turma1Ano != null)
                             p.NotaConceitoPrimeiroAno = notasComponente?.FirstOrDefault(nf => nf.PeriodoEscolar == null && nf.CodigoTurma == turma1Ano.Codigo)?.NotaConceito?.NotaConceito;

                         if (turma2Ano != null)
                             p.NotaConceitoSegundoAno = notasComponente?.FirstOrDefault(nf => nf.PeriodoEscolar == null && nf.CodigoTurma == turma2Ano.Codigo)?.NotaConceito?.NotaConceito;

                         if (turma3Ano != null)
                             p.NotaConceitoTerceiroAno = notasComponente?.FirstOrDefault(nf => nf.PeriodoEscolar == null && nf.CodigoTurma == turma3Ano.Codigo)?.NotaConceito?.NotaConceito;

                         if (turma4Ano != null)
                             p.NotaConceitoQuartoAno = notasComponente?.FirstOrDefault(nf => nf.PeriodoEscolar == null && nf.CodigoTurma == turma4Ano.Codigo)?.NotaConceito?.NotaConceito;

                         if (turma5Ano != null)
                             p.NotaConceitoQuintoAno = notasComponente?.FirstOrDefault(nf => nf.PeriodoEscolar == null && nf.CodigoTurma == turma5Ano.Codigo)?.NotaConceito?.NotaConceito;

                         if (turma6Ano != null)
                             p.NotaConceitoSextoAno = notasComponente?.FirstOrDefault(nf => nf.PeriodoEscolar == null && nf.CodigoTurma == turma6Ano.Codigo)?.NotaConceito?.NotaConceito;

                         if (turma7Ano != null)
                             p.NotaConceitoSetimoAno = notasComponente?.FirstOrDefault(nf => nf.PeriodoEscolar == null && nf.CodigoTurma == turma7Ano.Codigo)?.NotaConceito?.NotaConceito;

                         if (turma8Ano != null)
                             p.NotaConceitoOitavoAno = notasComponente?.FirstOrDefault(nf => nf.PeriodoEscolar == null && nf.CodigoTurma == turma8Ano.Codigo)?.NotaConceito?.NotaConceito;

                         if (turma9Ano != null)
                             p.NotaConceitoNonoAno = notasComponente?.FirstOrDefault(nf => nf.PeriodoEscolar == null && nf.CodigoTurma == turma9Ano.Codigo)?.NotaConceito?.NotaConceito;
                     }
                     else
                     {
                         if (turma1Ano != null)
                             p.NotaConceitoPrimeiroAno = ObterSintese(frequenciasComponente.Where(fc => fc.TurmaId == turma1Ano.Codigo), mediasFrequencia, false, false);

                         if (turma2Ano != null)
                             p.NotaConceitoSegundoAno = ObterSintese(frequenciasComponente.Where(fc => fc.TurmaId == turma2Ano.Codigo), mediasFrequencia, false, false);

                         if (turma3Ano != null)
                             p.NotaConceitoTerceiroAno = ObterSintese(frequenciasComponente.Where(fc => fc.TurmaId == turma3Ano.Codigo), mediasFrequencia, false, false);

                         if (turma4Ano != null)
                             p.NotaConceitoQuartoAno = ObterSintese(frequenciasComponente.Where(fc => fc.TurmaId == turma4Ano.Codigo), mediasFrequencia, false, false);

                         if (turma5Ano != null)
                             p.NotaConceitoQuintoAno = ObterSintese(frequenciasComponente.Where(fc => fc.TurmaId == turma5Ano.Codigo), mediasFrequencia, false, false);

                         if (turma6Ano != null)
                             p.NotaConceitoSextoAno = ObterSintese(frequenciasComponente.Where(fc => fc.TurmaId == turma6Ano.Codigo), mediasFrequencia, false, false);

                         if (turma7Ano != null)
                             p.NotaConceitoSetimoAno = ObterSintese(frequenciasComponente.Where(fc => fc.TurmaId == turma7Ano.Codigo), mediasFrequencia, false, false);

                         if (turma8Ano != null)
                             p.NotaConceitoOitavoAno = ObterSintese(frequenciasComponente.Where(fc => fc.TurmaId == turma8Ano.Codigo), mediasFrequencia, false, false);

                         if (turma9Ano != null)
                             p.NotaConceitoNonoAno = ObterSintese(frequenciasComponente.Where(fc => fc.TurmaId == turma9Ano.Codigo), mediasFrequencia, false, false);
                     }

                     return p;

                 });

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
