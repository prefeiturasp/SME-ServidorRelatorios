using MediatR;
using SME.SR.Data.Models;

namespace SME.SR.Application.Queries.Dre.ObterDreUeNomePorUeCodigo
{
    public class ObterDreUeNomePorUeCodigoQuery : IRequest<DreUeNome>
    {
        public ObterDreUeNomePorUeCodigoQuery(string ueCodigo)
        {
            UeCodigo = ueCodigo;
        }

        public string UeCodigo { get; set; }
    }
}
