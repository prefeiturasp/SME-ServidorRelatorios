using MediatR;
using SME.SR.Infra;

namespace SME.SR.Application
{
    public class ObterEnderecoUeEolPorCodigoQuery : IRequest<UeEolEnderecoDto>
    {
        public ObterEnderecoUeEolPorCodigoQuery(long ueCodigo)
        {
            this.ueCodigo = ueCodigo;
        }

        public long ueCodigo { get; set; }
    }
}
