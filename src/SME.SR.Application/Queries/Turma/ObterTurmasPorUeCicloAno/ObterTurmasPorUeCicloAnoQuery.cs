using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterTurmasPorUeCicloAnoQuery : IRequest<IEnumerable<TurmaFiltradaUeCicloAnoDto>>
    {
        public ObterTurmasPorUeCicloAnoQuery(long tipoCicloId, string ano, long ueId)
        {
            TipoCicloId = tipoCicloId;
            Ano = ano;
            UeId = ueId;
        }

        public long TipoCicloId { get; set; }
        public string Ano { get; set; }
        public long UeId { get; set; }
    }
}
