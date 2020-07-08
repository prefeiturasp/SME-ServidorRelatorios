using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterCabecalhoHistoricoEscolarQueryHandler : IRequestHandler<ObterCabecalhoHistoricoEscolarQuery, CabecalhoDto>
    {
        private IObterCabecalhoHistoricoEscolarRepository obterCabecalhoHistoricoEscolarRepository;

        public ObterCabecalhoHistoricoEscolarQueryHandler(IObterCabecalhoHistoricoEscolarRepository _obterCabecalhoHistoricoEscolarRepository)
        {
            obterCabecalhoHistoricoEscolarRepository = _obterCabecalhoHistoricoEscolarRepository;
        }

        public async Task<CabecalhoDto> Handle(ObterCabecalhoHistoricoEscolarQuery request, CancellationToken cancellationToken)
        {
            CabecalhoDto cabecalhoDto = new CabecalhoDto();

            cabecalhoDto = await obterCabecalhoHistoricoEscolarRepository.ObterCabecalhoHistoricoEscolar(request.AnoLetivo, request.DreCodigo, request.UeCodigo);

            return cabecalhoDto;
        }
    }
}
