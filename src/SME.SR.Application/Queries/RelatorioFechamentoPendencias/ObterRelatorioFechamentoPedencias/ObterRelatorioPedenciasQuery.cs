using MediatR;
using SME.SR.Infra;
using SME.SR.Infra.Dtos;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterRelatorioPedenciasQuery : IRequest<RelatorioPendenciasDto>
    {
        public FiltroRelatorioPendenciasDto filtroRelatorioPendenciasFechamentoDto { get; set; }
        public string UsuarioRf { get; internal set; }
    }
}
