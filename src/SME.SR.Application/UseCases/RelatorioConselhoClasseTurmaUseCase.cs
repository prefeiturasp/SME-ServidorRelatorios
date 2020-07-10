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
                    jsonString = JsonConvert.SerializeObject(new { RelatorioConselhoDeClasse = listBimestre });
                }
                else
                {
                    List<RelatorioConselhoClasseFinal> listFinal = relatorioAlunos.Cast<RelatorioConselhoClasseFinal>().ToList();
                    jsonString = JsonConvert.SerializeObject(new { RelatorioConselhoDeClasse = listFinal });
                }

                var urlRelatorio = "";

                if (relatorioAlunos.FirstOrDefault().EhBimestreFinal)
                    urlRelatorio = "/sgp/RelatorioConselhoClasse/ConselhoClasseAbaFinal";
                else urlRelatorio = "/sgp/RelatorioConselhoClasse/ConselhoClasse";

                await mediator.Send(new GerarRelatorioAssincronoCommand(urlRelatorio, jsonString, TipoFormatoRelatorio.Pdf, request.CodigoCorrelacao));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
