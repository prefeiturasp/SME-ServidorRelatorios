using MediatR;
using SME.SR.Infra;
using SME.SR.Infra.Dtos;

namespace SME.SR.Application
{
    public class ObterRelatorioPedenciasQuery : IRequest<RelatorioPendenciasDto>
    {
        public ObterRelatorioPedenciasQuery(FiltroRelatorioPendenciasDto filtroRelatorioPendencias)
        {
            FiltroRelatorioPendencias = filtroRelatorioPendencias;            
        }

        public FiltroRelatorioPendenciasDto FiltroRelatorioPendencias { get; set; }        
    }
}
