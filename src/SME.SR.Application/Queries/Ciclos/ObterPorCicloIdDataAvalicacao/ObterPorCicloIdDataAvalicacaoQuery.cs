using MediatR;
using SME.SR.Data;
using System;

namespace SME.SR.Application
{
    public class ObterPorCicloIdDataAvalicacaoQuery : IRequest<NotaTipoValor>
    {
        public ObterPorCicloIdDataAvalicacaoQuery(long cicloId, DateTime dataAvalicao)
        {
            CicloId = cicloId;
            DataAvalicao = dataAvalicao;
        }

        public long CicloId { get; set; }
        public DateTime DataAvalicao { get; set; }
    }
}
