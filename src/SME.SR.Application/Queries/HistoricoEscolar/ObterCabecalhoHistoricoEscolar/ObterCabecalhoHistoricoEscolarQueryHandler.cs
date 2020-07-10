using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterCabecalhoHistoricoEscolarQueryHandler : IRequestHandler<ObterCabecalhoHistoricoEscolarQuery, CabecalhoDto>
    {
        private readonly IObterCabecalhoHistoricoEscolarRepository ObterCabecalhoHistoricoEscolarRepository;

        public ObterCabecalhoHistoricoEscolarQueryHandler(IObterCabecalhoHistoricoEscolarRepository obterCabecalhoHistoricoEscolarRepository)
        {
            ObterCabecalhoHistoricoEscolarRepository = obterCabecalhoHistoricoEscolarRepository;
        }

        public async Task<CabecalhoDto> Handle(ObterCabecalhoHistoricoEscolarQuery request, CancellationToken cancellationToken)
        {
            CabecalhoDto cabecalhoDto = new CabecalhoDto();

            cabecalhoDto = await ObterCabecalhoHistoricoEscolarRepository.ObterCabecalhoHistoricoEscolar(request.AnoLetivo, request.DreCodigo, request.UeCodigo);

            return cabecalhoDto;
        }
    }
}
