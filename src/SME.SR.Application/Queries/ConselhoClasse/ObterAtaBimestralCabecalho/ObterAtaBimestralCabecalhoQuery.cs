using MediatR;
using SME.SR.Infra;

namespace SME.SR.Application
{
    public class ObterAtaBimestralCabecalhoQuery : IRequest<ConselhoClasseAtaBimestralCabecalhoDto>
    {
        public ObterAtaBimestralCabecalhoQuery(string turmaCodigo)
        {
            TurmaCodigo = turmaCodigo;
        }

        public string TurmaCodigo { get; set; }
    }
}
