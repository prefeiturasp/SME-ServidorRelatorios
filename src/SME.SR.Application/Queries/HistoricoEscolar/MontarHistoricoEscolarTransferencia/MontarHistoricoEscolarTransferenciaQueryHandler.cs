using MediatR;
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
    public class MontarHistoricoEscolarTransferenciaQueryHandler : IRequestHandler<MontarHistoricoEscolarTransferenciaQuery, IEnumerable<TransferenciaDto>>
    {
        public MontarHistoricoEscolarTransferenciaQueryHandler()
        {

        }

        public async Task<IEnumerable<TransferenciaDto>> Handle(MontarHistoricoEscolarTransferenciaQuery request, CancellationToken cancellationToken)
        {
            var listaRetorno = new List<TransferenciaDto>();

            foreach (var aluno in request.AlunosTurmas)
            {
                var alunoTurmasPorModalidade = aluno.Turmas.GroupBy(t => t.ModalidadeCodigo);

                foreach (var agrupamentoTurmas in alunoTurmasPorModalidade)
                {
                    var componentesDaTurma = request.ComponentesCurricularesTurmas.Where(cc => agrupamentoTurmas.Select(c => c.Codigo).Contains(cc.Key)).SelectMany(ct => ct).DistinctBy(d => d.CodDisciplina);
                    var componentesPorGrupoMatriz = componentesDaTurma.Where(gm => gm.GrupoMatriz != null).GroupBy(cc => cc.GrupoMatriz);

                    //Obter grupo matriz
                    var baseNacionalComum = componentesPorGrupoMatriz.FirstOrDefault(cpm => cpm.Key.Id == 1)?.Select(b => b);
                    var diversificados = componentesPorGrupoMatriz.FirstOrDefault(cpm => cpm.Key.Id == 2)?.Select(d => d);
                    var enriquecimentos = componentesPorGrupoMatriz.FirstOrDefault(cpm => cpm.Key.Id == 3)?.Select(e => e);
                    var projetos = componentesPorGrupoMatriz.FirstOrDefault(cpm => cpm.Key.Id == 4)?.Select(p => p);
                    //

                    var notasAluno = request.Notas.Where(n => agrupamentoTurmas.Select(t => t.Codigo).Contains(n.Key)).SelectMany(a => a).Where(w => w.CodigoAluno == aluno.Aluno.Codigo && w.PeriodoEscolar != null);
                    var frequenciasAluno = request.Frequencias.Where(f => agrupamentoTurmas.Select(t => t.Codigo).Contains(f.Key)).SelectMany(a => a).Where(a => a.CodigoAluno == aluno.Aluno.Codigo);

                    var baseNacionalDto = ObterBaseNacionalComum(agrupamentoTurmas, notasAluno, frequenciasAluno, request.MediasFrequencia, baseNacionalComum, request.AreasConhecimento);
                    var diversificadosDto = ObterGruposDiversificado(agrupamentoTurmas, notasAluno, frequenciasAluno, request.MediasFrequencia, diversificados, request.AreasConhecimento);
                    var enriquecimentoDto = MontarComponentesNotasFrequencia(agrupamentoTurmas, enriquecimentos, notasAluno, frequenciasAluno, request.MediasFrequencia, request.AreasConhecimento)?.ToList();
                    var projetosDto = MontarComponentesNotasFrequencia(agrupamentoTurmas, projetos, notasAluno, frequenciasAluno, request.MediasFrequencia, request.AreasConhecimento)?.ToList();

                    var transferenciaDto = new TransferenciaDto()
                    {
                        CodigoAluno = aluno.Aluno.Codigo,
                        CodigoTurma = aluno.Turmas.FirstOrDefault()?.Codigo,
                        Data = aluno.Aluno.DataSituacaoFormatada,
                        Descricao = aluno.Turmas.FirstOrDefault()?.DescricaoRelatorioTransferencia,
                        Rodape = aluno.Turmas.FirstOrDefault()?.RodapeRelatorioTransferencia,
                        GruposComponentesCurriculares = diversificadosDto,
                        BaseNacionalComum = baseNacionalDto,
                        EnriquecimentoCurricular = enriquecimentoDto,
                        ProjetosAtividadesComplementares = projetosDto,
                    };

                    listaRetorno.Add(transferenciaDto);
                }
            }

            return await Task.FromResult(listaRetorno);
        }

        private List<GruposComponentesCurricularesTransferenciaDto> ObterGruposDiversificado(IEnumerable<Turma> turmas,
                                                            IEnumerable<NotasAlunoBimestre> notasAlunos,
                                                            IEnumerable<FrequenciaAluno> frequencias,
                                                            IEnumerable<MediaFrequencia> mediasFrequencia,
                                                            IEnumerable<ComponenteCurricularPorTurma> componentesCurricularesDaTurma,
                                                            IEnumerable<AreaDoConhecimento> areasDoConhecimentos)
        {
            var gruposComponentes = new List<GruposComponentesCurricularesTransferenciaDto>();

            if (componentesCurricularesDaTurma != null && componentesCurricularesDaTurma.Any())
            {
                var componentesPorGrupoMatriz = componentesCurricularesDaTurma.GroupBy(cc => cc.GrupoMatriz);
                var areasConhecimento = MapearAreasDoConhecimento(componentesCurricularesDaTurma, areasDoConhecimentos);

                gruposComponentes.AddRange(componentesPorGrupoMatriz.Select(gp => new GruposComponentesCurricularesTransferenciaDto
                {
                    Nome = gp.Key.Nome,
                    AreasDeConhecimento = areasConhecimento.Select(ac => new AreaDeConhecimentoTransferenciaDto()
                    {
                        Nome = ac.Key.Nome,
                        ComponentesCurriculares = MontarComponentesNotasFrequencia(turmas,
                                                ObterComponentesDasAreasDeConhecimento(componentesCurricularesDaTurma, ac),
                                                notasAlunos, frequencias, mediasFrequencia, ac)?.ToList()
                    }).ToList()
                }));
            }

            return gruposComponentes;
        }

        private BaseNacionalComumTransferenciaDto ObterBaseNacionalComum(IEnumerable<Turma> turmas,
                                                            IEnumerable<NotasAlunoBimestre> notasAlunos,
                                                            IEnumerable<FrequenciaAluno> frequencias,
                                                            IEnumerable<MediaFrequencia> mediasFrequencia,
                                                            IEnumerable<ComponenteCurricularPorTurma> componentesCurricularesDaTurma,
                                                            IEnumerable<AreaDoConhecimento> areasDoConhecimentos)
        {
            BaseNacionalComumTransferenciaDto baseNacional = null;

            if (componentesCurricularesDaTurma != null && componentesCurricularesDaTurma.Any())
            {
                var areasConhecimento = MapearAreasDoConhecimento(componentesCurricularesDaTurma, areasDoConhecimentos);

                baseNacional = new BaseNacionalComumTransferenciaDto()
                {
                    AreasDeConhecimento = areasConhecimento.Select(ac => new AreaDeConhecimentoTransferenciaDto()
                    {
                        Nome = ac.Key.Nome,
                        ComponentesCurriculares = MontarComponentesNotasFrequencia(turmas,
                                                ObterComponentesDasAreasDeConhecimento(componentesCurricularesDaTurma, ac),
                                                notasAlunos, frequencias, mediasFrequencia, ac)?.ToList()
                    }).ToList()
                };
            }

            return baseNacional;
        }

        private IEnumerable<ComponenteCurricularPorTurma> ObterComponentesDasAreasDeConhecimento(IEnumerable<ComponenteCurricularPorTurma> componentesCurricularesDaTurma,
                                                                                                IEnumerable<AreaDoConhecimento> areaDoConhecimento)
        {
            return componentesCurricularesDaTurma.Where(c => (!c.Regencia && areaDoConhecimento.Select(a => a.CodigoComponenteCurricular).Contains(c.CodDisciplina)) ||
                                                            (c.Regencia && areaDoConhecimento.Select(a => a.CodigoComponenteCurricular).Any(cr =>
                                                            c.ComponentesCurricularesRegencia.Select(cr => cr.CodDisciplina).Contains(cr))));
        }

        private IEnumerable<IGrouping<(string Nome, long Id), AreaDoConhecimento>> MapearAreasDoConhecimento(IEnumerable<ComponenteCurricularPorTurma> componentesCurricularesDaTurma,
                                                                                                             IEnumerable<AreaDoConhecimento> areasDoConhecimentos)
        {

            return areasDoConhecimentos.Where(a => ((componentesCurricularesDaTurma.Where(cc => !cc.Regencia).Select(cc => cc.CodDisciplina).Contains(a.CodigoComponenteCurricular)) ||
                                                                    (componentesCurricularesDaTurma.Any(cc => cc.Regencia) &&
                                                                     componentesCurricularesDaTurma.Where(cc => cc.Regencia).SelectMany(cc => cc.ComponentesCurricularesRegencia).
                                                                     Select(r => r.CodDisciplina).Contains(a.CodigoComponenteCurricular)))).GroupBy(g => (g.Nome, g.Id));
        }

        private IEnumerable<ComponenteCurricularHistoricoEscolarTransferenciaDto> MontarComponentesNotasFrequencia(IEnumerable<Turma> turmas, IEnumerable<ComponenteCurricularPorTurma> componentesCurricularesDaTurma, IEnumerable<NotasAlunoBimestre> notas, IEnumerable<FrequenciaAluno> frequencia, IEnumerable<MediaFrequencia> mediasFrequencia, IEnumerable<AreaDoConhecimento> areasDoConhecimentos)
        {
            List<ComponenteCurricularHistoricoEscolarTransferenciaDto> componentes = null;

            if (componentesCurricularesDaTurma != null && componentesCurricularesDaTurma.Any())
            {
                componentes = new List<ComponenteCurricularHistoricoEscolarTransferenciaDto>();

                foreach (var componenteCurricular in componentesCurricularesDaTurma)
                {
                    if (componenteCurricular.Regencia)
                    {
                        componentes.AddRange(MapearComponentesRegencia(componenteCurricular.CodDisciplina.ToString(), turmas, componenteCurricular.ComponentesCurricularesRegencia.Where(r => areasDoConhecimentos.Select(a => a.CodigoComponenteCurricular).Contains(r.CodDisciplina)), notas, frequencia.Where(f => f.DisciplinaId == componenteCurricular.CodDisciplina.ToString()), mediasFrequencia));
                    }
                    else
                    {
                        componentes.Add(new ComponenteCurricularHistoricoEscolarTransferenciaDto()
                        {
                            Codigo = componenteCurricular.CodDisciplina.ToString(),
                            Nome = componenteCurricular.Disciplina,
                            Frequencia = componenteCurricular.Frequencia,
                            Nota = componenteCurricular.LancaNota,
                            FrequenciaPrimeiroBimestre = ObterFrequenciaComponentePorTurma(turmas.FirstOrDefault(), componenteCurricular.CodDisciplina.ToString(), frequencia, 1),
                            FrequenciaSegundoBimestre = ObterFrequenciaComponentePorTurma(turmas.FirstOrDefault(), componenteCurricular.CodDisciplina.ToString(), frequencia, 2),
                            FrequenciaTerceiroBimestre = ObterFrequenciaComponentePorTurma(turmas.FirstOrDefault(), componenteCurricular.CodDisciplina.ToString(), frequencia, 3),
                            FrequenciaQuartoBimestre = ObterFrequenciaComponentePorTurma(turmas.FirstOrDefault(), componenteCurricular.CodDisciplina.ToString(), frequencia, 4),
                            NotaConceitoPrimeiroBimestre = ObterNotaComponentePorTurma(turmas.FirstOrDefault(), componenteCurricular.CodDisciplina.ToString(), false, componenteCurricular.LancaNota, frequencia, notas, mediasFrequencia, 1),
                            NotaConceitoSegundoBimestre = ObterNotaComponentePorTurma(turmas.FirstOrDefault(), componenteCurricular.CodDisciplina.ToString(), false, componenteCurricular.LancaNota, frequencia, notas, mediasFrequencia, 2),
                            NotaConceitoTerceiroBimestre = ObterNotaComponentePorTurma(turmas.FirstOrDefault(), componenteCurricular.CodDisciplina.ToString(), false, componenteCurricular.LancaNota, frequencia, notas, mediasFrequencia, 3),
                            NotaConceitoQuartoBimestre = ObterNotaComponentePorTurma(turmas.FirstOrDefault(), componenteCurricular.CodDisciplina.ToString(), false, componenteCurricular.LancaNota, frequencia, notas, mediasFrequencia, 4),
                        });
                    }
                }
            }

            return componentes;
        }

        private string ObterFrequenciaComponentePorTurma(Turma turma, string codigoComponente, IEnumerable<FrequenciaAluno> frequenciaAlunos, int bimestre)
        {
            if (turma != null)
                return frequenciaAlunos.FirstOrDefault(f => f.DisciplinaId == codigoComponente && f.TurmaId == turma.Codigo && f.Bimestre == bimestre)?.PercentualFrequencia.ToString() ?? "100";
            else
                return null;
        }

        private string ObterNotaComponentePorTurma(Turma turma, string codigoComponente, bool regencia, bool lancaNota, IEnumerable<FrequenciaAluno> frequenciaAlunos, IEnumerable<NotasAlunoBimestre> notasAluno, IEnumerable<MediaFrequencia> mediasFrequencias, int bimestre)
        {
            if (turma != null)
            {
                if (lancaNota)
                    return notasAluno.FirstOrDefault(n => n.CodigoComponenteCurricular == codigoComponente && n.CodigoTurma == turma.Codigo && n.PeriodoEscolar.Bimestre == bimestre)?.NotaConceito?.NotaConceito;
                else
                    return ObterSintese(frequenciaAlunos.Where(f => f.PeriodoEscolarId != null), mediasFrequencias, regencia, !lancaNota);
            }
            else
                return null;
        }

        private IEnumerable<ComponenteCurricularHistoricoEscolarTransferenciaDto> MapearComponentesRegencia(string codigoComponenteRegencia, IEnumerable<Turma> turmas, IEnumerable<ComponenteCurricularPorTurmaRegencia> componentesRegencia, IEnumerable<NotasAlunoBimestre> notas, IEnumerable<FrequenciaAluno> frequencia, IEnumerable<MediaFrequencia> mediasFrequencia)
        {
            var componentes = new List<ComponenteCurricularHistoricoEscolarTransferenciaDto>();

            var frequencia1Bimestre = ObterFrequenciaComponentePorTurma(turmas.FirstOrDefault(), codigoComponenteRegencia, frequencia, 1);
            var frequencia2Bimestre = ObterFrequenciaComponentePorTurma(turmas.FirstOrDefault(), codigoComponenteRegencia, frequencia, 2);
            var frequencia3Bimestre = ObterFrequenciaComponentePorTurma(turmas.FirstOrDefault(), codigoComponenteRegencia, frequencia, 3);
            var frequencia4Bimestre = ObterFrequenciaComponentePorTurma(turmas.FirstOrDefault(), codigoComponenteRegencia, frequencia, 4);

            foreach (var componenteCurricular in componentesRegencia)
            {
                componentes.Add(new ComponenteCurricularHistoricoEscolarTransferenciaDto()
                {
                    Codigo = componenteCurricular.CodDisciplina.ToString(),
                    Nome = componenteCurricular.Disciplina,
                    Frequencia = componenteCurricular.Frequencia,
                    Nota = componenteCurricular.LancaNota,
                    FrequenciaPrimeiroBimestre = frequencia1Bimestre,
                    FrequenciaSegundoBimestre = frequencia2Bimestre,
                    FrequenciaTerceiroBimestre = frequencia3Bimestre,
                    FrequenciaQuartoBimestre = frequencia4Bimestre,
                    NotaConceitoPrimeiroBimestre = ObterNotaComponentePorTurma(turmas.FirstOrDefault(), componenteCurricular.CodDisciplina.ToString(), false, componenteCurricular.LancaNota, frequencia, notas, mediasFrequencia, 1),
                    NotaConceitoSegundoBimestre = ObterNotaComponentePorTurma(turmas.FirstOrDefault(), componenteCurricular.CodDisciplina.ToString(), false, componenteCurricular.LancaNota, frequencia, notas, mediasFrequencia, 2),
                    NotaConceitoTerceiroBimestre = ObterNotaComponentePorTurma(turmas.FirstOrDefault(), componenteCurricular.CodDisciplina.ToString(), false, componenteCurricular.LancaNota, frequencia, notas, mediasFrequencia, 3),
                    NotaConceitoQuartoBimestre = ObterNotaComponentePorTurma(turmas.FirstOrDefault(), componenteCurricular.CodDisciplina.ToString(), false, componenteCurricular.LancaNota, frequencia, notas, mediasFrequencia, 4),
                });
            }

            return componentes;
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
