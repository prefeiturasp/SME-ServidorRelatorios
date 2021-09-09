using MediatR;
using SME.SR.Infra;

namespace SME.SR.Application
{
    public class ObterAtaFinalCabecalhoQuery : IRequest<ConselhoClasseAtaFinalCabecalhoDto>
    {
        public ObterAtaFinalCabecalhoQuery(string turmaCodigo)
        {
            TurmaCodigo = turmaCodigo;
        }

        public string TurmaCodigo { get; set; }
    }
}
