using MediatR;
using SME.SR.Data.Models.Conecta;

namespace SME.SR.Application
{
    public class GerarRelatorioLaudaDePublicacaoDiretaDocCommand : IRequest<string>
    {
        public GerarRelatorioLaudaDePublicacaoDiretaDocCommand(Proposta proposta)
        {
            Proposta = proposta;
        }

        public Proposta Proposta { get; set; }
    }
}
