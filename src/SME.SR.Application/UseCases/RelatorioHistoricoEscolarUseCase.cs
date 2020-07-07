using MediatR;
using Newtonsoft.Json;
using SME.SR.Infra;
using System;
using System.Linq;
using System.Threading.Tasks;
using static SME.SR.Infra.Enumeradores;

namespace SME.SR.Application
{
    public class RelatorioHistoricoEscolarUseCase : IRelatorioHistoricoEscolarUseCase
    {
        private readonly IMediator mediator;

        public RelatorioHistoricoEscolarUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Executar(FiltroRelatorioDto request)
        {
            CabecalhoDto cabecalho = new CabecalhoDto();
            var relatorioQuery = request.ObterObjetoFiltro<ObterHistoricoEscolarQueryHandler>();
            var relatorioHistoricoEscolar = await mediator.Send(relatorioQuery);

            var queryEnderecosEAtos = request.ObterObjetoFiltro<ObterEnderecoEAtosDaUeQuery>();
            var enderecosEAtos = await mediator.Send(queryEnderecosEAtos);
            if (enderecosEAtos != null)
            {
                cabecalho.NomeUe = enderecosEAtos.FirstOrDefault().NomeUe;
                cabecalho.Endereco = enderecosEAtos.FirstOrDefault().Endereco;
                cabecalho.AtoCriacao = enderecosEAtos.FirstOrDefault(teste => teste.TipoOcorrencia == "1").Ato;
                cabecalho.AtoAutorizacao = enderecosEAtos.FirstOrDefault(teste => teste.TipoOcorrencia == "7").Ato;
            }

            var jsonString = "";
            if (relatorioHistoricoEscolar != null)
            {
                jsonString = JsonConvert.SerializeObject(relatorioHistoricoEscolar);
            }
            
            await mediator.Send(new GerarRelatorioAssincronoCommand("/sgp/RelatorioHistoricoEscolarFundamental/HistoricoEscolar", jsonString, FormatoEnum.Pdf, request.CodigoCorrelacao));
        }
    }
}
