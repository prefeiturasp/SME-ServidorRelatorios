using MediatR;
using SME.SR.Data;
using SME.SR.Data.Extensions;
using SME.SR.Data.Interfaces;
using SME.SR.Infra.Dtos.ElasticSearch;
using SME.SR.Infra.Utilitarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class AdicionarComponentesCurricularesTerritorioSaberTurmaQueryHandler : IRequestHandler<AdicionarComponentesCurricularesTerritorioSaberTurmaQuery, List<ComponenteCurricular>>
    {
        private readonly IComponenteCurricularRepository componenteCurricularRepository;

        public AdicionarComponentesCurricularesTerritorioSaberTurmaQueryHandler(IComponenteCurricularRepository componenteCurricularRepository, IAreaDoConhecimentoRepository areaDoConhecimentoRepository)
        {
            this.componenteCurricularRepository = componenteCurricularRepository ?? throw new ArgumentNullException(nameof(componenteCurricularRepository));
        }

        public async Task<List<ComponenteCurricular>> Handle(AdicionarComponentesCurricularesTerritorioSaberTurmaQuery request, CancellationToken cancellationToken)
        {
            foreach (var turma in request.CodigosTurmas)
            {
                var codigosComponentesCurricularesTerritorio = request.ComponentesCurricularesTurma.Where(cc => cc.CodigoTurma == turma &&
                                                                                              cc.TerritorioSaber).Select(cc => cc.Codigo);
                if (!codigosComponentesCurricularesTerritorio.Any())
                    continue;

                var componentesTerritorioTurma = await componenteCurricularRepository.ObterComponentesTerritorioDosSaberes(turma, codigosComponentesCurricularesTerritorio);
                foreach (var informacoesComponenteTerritorioSaber in componentesTerritorioTurma)
                {
                    var dadosTurmaComponente = request.ComponentesCurricularesTurma.Where(cc => cc.Codigo == informacoesComponenteTerritorioSaber.CodigoComponenteCurricular &&
                                                                                            cc.CodigoTurma == turma).FirstOrDefault();
                    var tipoEscola = dadosTurmaComponente.TipoEscola;
                    var turnoTurma = dadosTurmaComponente.TurnoTurma;
                    var anoTurma = dadosTurmaComponente.AnoTurma;
                    var codigoAluno = dadosTurmaComponente.CodigoAluno;
                    var grupoMatrizId = dadosTurmaComponente.GrupoMatrizId;

                    request.ComponentesCurricularesTurma.RemoveAll(cc => cc.Codigo == informacoesComponenteTerritorioSaber.CodigoComponenteCurricular &&
                                                            cc.CodigoTurma == turma);

                    var agrupamentosTerritorioSaber = await ObterAgrupamentosTerritorioSaber(long.Parse(turma), informacoesComponenteTerritorioSaber.CodigoTerritorioSaber,
                                                                                            informacoesComponenteTerritorioSaber.CodigoExperienciaPedagogica,
                                                                                            informacoesComponenteTerritorioSaber.CodigoComponenteCurricular,
                                                                                            request.RfProfessor);

                    foreach (var agrupamentoTerritorioSaber in agrupamentosTerritorioSaber)
                    {
                        if (!request.ComponentesCurricularesTurma.Any(cc => cc.Codigo == agrupamentoTerritorioSaber.CodigoAgrupamento))
                        {
                            request.ComponentesCurricularesTurma.Add(new ComponenteCurricular()
                            {
                                Codigo = agrupamentoTerritorioSaber.CodigoAgrupamento,
                                CodigoComponenteCurricularTerritorioSaber = agrupamentoTerritorioSaber.ComponentesCurricularesAgrupados.First(),
                                Descricao = informacoesComponenteTerritorioSaber.ObterDescricaoComponenteCurricular(),
                                TipoEscola = tipoEscola,
                                TerritorioSaber = true,
                                Professor = agrupamentoTerritorioSaber.RfProfessor,
                                CodigoTurma = turma,
                                TurnoTurma = turnoTurma,
                                AnoTurma = anoTurma,
                                CodigoAluno = codigoAluno,
                                GrupoMatrizId = grupoMatrizId                                
                            });
                        }
                    }

                    if (!request.ComponentesCurricularesTurma.Any(cc => cc.Codigo == informacoesComponenteTerritorioSaber.CodigoComponenteCurricular))
                    {
                        var agrupamentoTerritorioSaberMaisRecente = agrupamentosTerritorioSaber.FirstOrDefault();
                        var atribuicaoNaoAgrupada = await ObterComponenteCurricularTerritorioAtribuicaoNaoAgrupadaQuery(
                                                                                        long.Parse(turma),
                                                                                        informacoesComponenteTerritorioSaber.CodigoComponenteCurricular,
                                                                                        request.RfProfessor);

                        var possuiAtribuicaoUnica = !(atribuicaoNaoAgrupada is null);
                        var possuiAtribuicaoAgrupamento = !(agrupamentoTerritorioSaberMaisRecente is null);
                        var ehFiltroGestor = string.IsNullOrEmpty(request.RfProfessor);

                        if (possuiAtribuicaoUnica ||
                            (!possuiAtribuicaoAgrupamento && ehFiltroGestor))
                            request.ComponentesCurricularesTurma.Add(new ComponenteCurricular()
                            {
                                Codigo = informacoesComponenteTerritorioSaber.CodigoComponenteCurricular,
                                CodigoComponenteCurricularTerritorioSaber = informacoesComponenteTerritorioSaber.CodigoComponenteCurricular,
                                Descricao = informacoesComponenteTerritorioSaber.ObterDescricaoComponenteCurricular(),
                                TipoEscola = tipoEscola,
                                TerritorioSaber = true,
                                Professor = atribuicaoNaoAgrupada?.RfProfessor,
                                CodigoTurma = turma,
                                TurnoTurma = turnoTurma,
                                AnoTurma = anoTurma,
                                CodigoAluno = codigoAluno,
                                GrupoMatrizId = grupoMatrizId
                            });
                    }
                }
            }
            return request.ComponentesCurricularesTurma;
        }

        private async Task<ComponenteCurricularTerritorioAtribuidoTurmaDTO> ObterComponenteCurricularTerritorioAtribuicaoNaoAgrupadaQuery(long codigoTurma, long codigoComponenteCurricular,
                                                                             string codigoRf = null,
                                                                             DateTime? dataBaseVigencia = null)
        {
            var componentesCurricularesAtribuidosTurma = await componenteCurricularRepository.ObterComponentesCurricularesTerritorioAtribuidos(codigoTurma, codigoRf);
            var componentesCurricularesAtribuidosTurmaAgrupados = componentesCurricularesAtribuidosTurma.GroupBy(aa => new { aa.TurmaCodigo, aa.CodigoTerritorioSaber, aa.CodigoExperienciaPedagogica, aa.RfProfessor, aa.DataAtribuicao })
                              .Where(aa => aa.Count() == 1)
                              .SelectMany(grp => grp.ToList())
                              .OrderByDescending(aa => aa.DataAtribuicao)
                              .ThenByDescending(aa => aa.DataDisponibilizacao is null)
                              .Where(cc => cc.CodigoComponenteCurricular == codigoComponenteCurricular
                                    && (dataBaseVigencia == null || cc.DataAtribuicao <= dataBaseVigencia));
            return componentesCurricularesAtribuidosTurmaAgrupados.FirstOrDefault();
        }

        private async Task<IEnumerable<AgrupamentoAtribuicaoTerritorioSaber>> ObterAgrupamentosTerritorioSaber(long codigoTurma, long codigoTerritorioSaber, long codigoExperienciaPegagogica, long codigoComponenteCurricular, string rfProf)
        {
            var agrupamentosTerritorio = (await componenteCurricularRepository.ObterAgrupamentosTerritorioSaber(codigoTurma, codigoTerritorioSaber, codigoExperienciaPegagogica, codigoComponenteCurricular))
                                                                                        .OrderByDescending(aa => aa.DtInicioAtribuicao)
                                                                                        .ThenByDescending(aa => aa.DtFimAtribuicao is null);

            var retorno = new List<AgrupamentoAtribuicaoTerritorioSaber>();
            foreach (var item in agrupamentosTerritorio.Where(aa => aa.RfProfessor.Equals(rfProf) || string.IsNullOrEmpty(rfProf)))
            {
                if (!retorno.Any(r => r.RfProfessor.Equals(item.RfProfessor) &&
                                      r.CodigosComponentesCurriculares.Equals(item.CodigosComponentesCurriculares)))
                {
                    retorno.Add(item);
                }
            }
            return retorno;
        }

    }
}
