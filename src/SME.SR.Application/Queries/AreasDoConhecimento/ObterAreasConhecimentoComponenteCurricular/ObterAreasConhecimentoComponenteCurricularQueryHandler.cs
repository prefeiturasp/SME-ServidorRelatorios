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
    public class ObterAreasConhecimentoComponenteCurricularQueryHandler : IRequestHandler<ObterAreasConhecimentoComponenteCurricularQuery, IEnumerable<AreaDoConhecimento>>
    {
        private readonly IAreaDoConhecimentoRepository areaDoConhecimentoRepository;

        public ObterAreasConhecimentoComponenteCurricularQueryHandler(IAreaDoConhecimentoRepository areaDoConhecimentoRepository)
        {
            this.areaDoConhecimentoRepository = areaDoConhecimentoRepository ?? throw new ArgumentNullException(nameof(areaDoConhecimentoRepository));
        }

        public async Task<IEnumerable<AreaDoConhecimento>> Handle(ObterAreasConhecimentoComponenteCurricularQuery request, CancellationToken cancellationToken)
        {
           var areasDoConhecimento = await areaDoConhecimentoRepository.ObterAreasDoConhecimentoPorComponentesCurriculares(request.CodigosComponenteCurricular);

            if (areasDoConhecimento == null || !areasDoConhecimento.Any())
                throw new NegocioException($"Não foi possível obter as áreas de conhecimento dos componentes informados - COMPONENTES = {request.CodigosComponenteCurricular}");

            return areasDoConhecimento;
        }
    }
}
