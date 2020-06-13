using MediatR;
using SME.SR.Application.Interfaces;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Text.Json;
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

                string jsonString = relatorio is RelatorioConselhoClasseBimestre ?
                    JsonSerializer.Serialize((RelatorioConselhoClasseBimestre)relatorio) :
                    JsonSerializer.Serialize((RelatorioConselhoClasseFinal)relatorio);

                await mediator.Send(new GerarRelatorioAssincronoCommand("sme/sgp/RelatorioConselhoClasse/ConselhoClasse", jsonString, FormatoEnum.Pdf, request.CodigoCorrelacao));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
