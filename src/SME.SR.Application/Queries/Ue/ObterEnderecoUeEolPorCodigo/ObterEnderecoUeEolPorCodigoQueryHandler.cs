using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterEnderecoUeEolPorCodigoQueryHandler : IRequestHandler<ObterEnderecoUeEolPorCodigoQuery, UeEolEnderecoDto>
    {
        private readonly IUeEolRepository ueEolRepository;

        public ObterEnderecoUeEolPorCodigoQueryHandler(IUeEolRepository ueEolRepository)
        {
            this.ueEolRepository = ueEolRepository ?? throw new ArgumentNullException(nameof(ueEolRepository));
        }

        public async Task<UeEolEnderecoDto> Handle(ObterEnderecoUeEolPorCodigoQuery request, CancellationToken cancellationToken)
        {
            var ueEndereco = await ueEolRepository.ObterEnderecoUePorCodigo(request.ueCodigo);
            return ueEndereco;
        }
    }
}
