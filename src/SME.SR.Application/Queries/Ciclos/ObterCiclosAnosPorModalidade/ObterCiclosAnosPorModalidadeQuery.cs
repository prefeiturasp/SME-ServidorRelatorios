using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterCiclosAnosPorModalidadeQuery : IRequest<IEnumerable<TipoCiclo>>
    {
        public ObterCiclosAnosPorModalidadeQuery(string[] anos, Modalidade modalidade)
        {
            Anos = anos;
            Modalidade = modalidade;
        }
        public string[] Anos { get; set; }
        public Modalidade Modalidade { get; set; }
    }
}
