using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterComponenteCurricularGrupoAreaOrdenacaoQueryHandler : IRequestHandler<ObterComponenteCurricularGrupoAreaOrdenacaoQuery, IEnumerable<ComponenteCurricularGrupoAreaOrdenacaoDto>>
    {
        private readonly IComponenteCurricularGrupoAreaOrdenacaoRepository  ccGrupoAreaOrdenacaoRepository;

        public ObterComponenteCurricularGrupoAreaOrdenacaoQueryHandler(IComponenteCurricularGrupoAreaOrdenacaoRepository ccGrupoAreaOrdenacaoRepository)
        {
            this.ccGrupoAreaOrdenacaoRepository = ccGrupoAreaOrdenacaoRepository ?? throw new ArgumentNullException(nameof(ccGrupoAreaOrdenacaoRepository));
        }

        public async Task<IEnumerable<ComponenteCurricularGrupoAreaOrdenacaoDto>> Handle(ObterComponenteCurricularGrupoAreaOrdenacaoQuery request, CancellationToken cancellationToken)
        {
            var ordenacoes = await ccGrupoAreaOrdenacaoRepository.ObterOrdenacaoPorGruposAreas(request.GrupoMatrizIds, request.AreaDoConhecimentoIds);

            if (ordenacoes == null || !ordenacoes.Any())
                throw new NegocioException("Não foi possível obter as ordenações dos grupos e areas dos componentes informados");

            return ordenacoes;
        }
    }
}
