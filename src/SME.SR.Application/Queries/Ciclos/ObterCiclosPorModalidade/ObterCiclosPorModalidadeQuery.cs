using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterCiclosPorModalidadeQuery : IRequest<IEnumerable<TipoCiclo>>
    {
        public ObterCiclosPorModalidadeQuery(Modalidade modalidade)
        {
            Modalidade = modalidade;
        }
        public Modalidade Modalidade { get; set; }
    }
}
