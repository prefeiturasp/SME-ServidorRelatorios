using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SME.SR.Application.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using static SME.SR.Infra.Enumeradores;

namespace SME.SR.Application
{
    public class RelatorioConselhoClasseAlunoUseCase: IRelatorioConselhoClasseAlunoUseCase
    {
        private readonly IMediator mediator;

        public RelatorioConselhoClasseAlunoUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Executar(FiltroRelatorioDto request)
        {
            try
            {
                var relatorioQuery = request.ObterObjetoFiltro<ObterRelatorioConselhoClasseAlunoQuery>();
                var relatorio = await mediator.Send(relatorioQuery);

                var relatorioSerializado = JsonConvert.SerializeObject(relatorio, UtilJson.ObterConfigConverterNulosEmVazio());

                await mediator.Send(new GerarRelatorioAssincronoCommand("/sgp/RelatorioConselhoClasse/ConselhoClasse", relatorioSerializado, FormatoEnum.Pdf, request.CodigoCorrelacao));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

   

    
}
