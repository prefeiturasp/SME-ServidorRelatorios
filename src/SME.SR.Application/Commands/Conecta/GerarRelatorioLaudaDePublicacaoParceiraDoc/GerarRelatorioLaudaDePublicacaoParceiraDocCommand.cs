using MediatR;
using SME.SR.Data.Models.Conecta;

namespace SME.SR.Application
{
    public class GerarRelatorioLaudaDePublicacaoParceiraDocCommand : IRequest<string>
    {
        public GerarRelatorioLaudaDePublicacaoParceiraDocCommand(Proposta proposta)
        {
            Proposta = proposta;
        }

        public Proposta Proposta { get; set; }
    }
}
