using MediatR;
using Newtonsoft.Json;
using SME.SR.Application.Interfaces;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static SME.SR.Infra.Enumeradores;

namespace SME.SR.Application
{
    public class RelatorioConselhoClasseTurmaUseCase : IRelatorioConselhoClasseTurmaUseCase
    {
        private readonly IMediator mediator;

        public RelatorioConselhoClasseTurmaUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Executar(FiltroRelatorioDto request)
        {
            try
            {
                var relatorioQuery = request.ObterObjetoFiltro<ObterRelatorioConselhoClasseTurmaQuery>();
                var relatorioAlunos = await mediator.Send(relatorioQuery);

                string jsonString;

                if (relatorioAlunos.FirstOrDefault() is RelatorioConselhoClasseBimestre)
                {
                    List<RelatorioConselhoClasseBimestre> listBimestre = relatorioAlunos.Cast<RelatorioConselhoClasseBimestre>().ToList();
                    jsonString = JsonConvert.SerializeObject(new { relatorioConselhoDeClasse = listBimestre }, UtilJson.ObterConfigConverterNulosEmVazio());
                }
                else
                {
                    List<RelatorioConselhoClasseFinal> listFinal = relatorioAlunos.Cast<RelatorioConselhoClasseFinal>().ToList();
                    jsonString = JsonConvert.SerializeObject(new { relatorioConselhoDeClasse = listFinal }, UtilJson.ObterConfigConverterNulosEmVazio());
                }

                await mediator.Send(new GerarRelatorioAssincronoCommand("/sgp/RelatorioConselhoClasse/ConselhoClasse", jsonString, FormatoEnum.Pdf, request.CodigoCorrelacao));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
