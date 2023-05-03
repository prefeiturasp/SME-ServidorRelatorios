using MediatR;
using SME.SR.Application.Interfaces;
using SME.SR.Infra;
using System;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class RelatorioItineranciasUseCase : IRelatorioItineranciasUseCase
    {
        private readonly IMediator mediator;

        public RelatorioItineranciasUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Executar(FiltroRelatorioDto request)
        {
            var parametros = request.ObterObjetoFiltro<FiltroRelatorioItineranciasDto>();

            try
            {
                var relatorioDto = new RelatorioRegistroItineranciaDto();

                await ObterFiltrosRelatorio(relatorioDto, parametros);

                relatorioDto.Registros = await mediator.Send(new ObterListagemItineranciasQuery(parametros.Itinerancias));
                
                await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioRegistroItinerancia", relatorioDto, request.CodigoCorrelacao, diretorioComplementar: "itinerancia"));
            }
            catch
            {
                throw;
            }
        }

        private async Task ObterFiltrosRelatorio(RelatorioRegistroItineranciaDto relatorioDto, FiltroRelatorioItineranciasDto parametros)
        {
            relatorioDto.Usuario = parametros.UsuarioNome;
            relatorioDto.RF = parametros.UsuarioRF;
            relatorioDto.DataSolicitacao = DateTime.Now.ToString("dd/MM/yyyy");
        }
    }
}
