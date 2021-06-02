using MediatR;
using SME.SR.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class OrdenarComponentesPorGrupoMatrizAreaConhecimentoQueryHandler : IRequestHandler<OrdenarComponentesPorGrupoMatrizAreaConhecimentoQuery, IEnumerable<ComponenteCurricularPorTurma>>
    {
        private readonly IComponenteCurricularGrupoAreaOrdenacaoRepository ccGrupoAreaOrdenacaoRepository;

        public OrdenarComponentesPorGrupoMatrizAreaConhecimentoQueryHandler(IComponenteCurricularGrupoAreaOrdenacaoRepository ccGrupoAreaOrdenacaoRepository)
        {
            this.ccGrupoAreaOrdenacaoRepository = ccGrupoAreaOrdenacaoRepository ?? throw new System.ArgumentNullException(nameof(ccGrupoAreaOrdenacaoRepository));
        }

        public async Task<IEnumerable<ComponenteCurricularPorTurma>> Handle(OrdenarComponentesPorGrupoMatrizAreaConhecimentoQuery request, CancellationToken cancellationToken)
        {
            var gruposIds = request.ComponentesCurriculares.Where(cc => cc.GrupoMatriz != null)?.Select(cc => cc.GrupoMatriz.Id).Distinct().ToArray();
            var areasIds = request.ComponentesCurriculares.Where(cc => cc.AreaDoConhecimento != null)?.Select(cc => cc.AreaDoConhecimento.Id).Distinct().ToArray();

            var ordenacao = await ccGrupoAreaOrdenacaoRepository.ObterOrdenacaoPorGruposAreas(gruposIds, areasIds);

            var retorno = request.ComponentesCurriculares.Select(cc =>
                                                      {
                                                          if (cc.AreaDoConhecimento != null)
                                                              cc.AreaDoConhecimento.DefinirOrdem(ordenacao, cc.GrupoMatriz.Id);

                                                          return cc;
                                                      })
                                                      .OrderBy(c => c.GrupoMatriz.Id)
                                                      .ThenByDescending(c => c.AreaDoConhecimento != null)
                                                      .ThenBy(c => c.AreaDoConhecimento.Ordem)
                                                      .ThenBy(c => c.Disciplina, StringComparer.OrdinalIgnoreCase);

            return retorno;
        }
    }
}
