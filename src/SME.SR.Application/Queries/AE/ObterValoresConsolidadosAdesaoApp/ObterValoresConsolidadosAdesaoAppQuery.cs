using MediatR;
using SME.SR.Infra.Dtos.AE.Adesao;
using System.Collections.Generic;

namespace SME.SR.Application
{
    internal class ObterValoresConsolidadosAdesaoAppQuery : IRequest<IEnumerable<AdesaoAEQueryConsolidadoRetornoDto>>
    {
        public string DreCodigo { get; set; }

        public string UeCodigo { get; set; }

        public ObterValoresConsolidadosAdesaoAppQuery(string dreCodigo, string ueCodigo)
        {
            this.DreCodigo = dreCodigo;
            this.UeCodigo = ueCodigo;
        }
    }
}