using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterEnderecoEAtosDaUeQueryHandler : IRequestHandler<ObterEnderecoEAtosDaUeQuery, CabecalhoDto>
    {
        private IObterEnderecoeAtosDaUeRepository obterEnderecoeAtosDaUeRepository;

        public ObterEnderecoEAtosDaUeQueryHandler(IObterEnderecoeAtosDaUeRepository _obterEnderecoeAtosDaUeRepository)
        {
            obterEnderecoeAtosDaUeRepository = _obterEnderecoeAtosDaUeRepository;
        }

        public async Task<CabecalhoDto> Handle(ObterEnderecoEAtosDaUeQuery request, CancellationToken cancellationToken)
        {
            return await obterEnderecoeAtosDaUeRepository.ObterEnderecoEAtos(request.UeCodigo);
        }
    }
}
