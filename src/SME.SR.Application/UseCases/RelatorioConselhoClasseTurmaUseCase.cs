using MediatR;
using SME.SR.Application.Interfaces;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

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
                request.RotaErro = RotasRabbit.RotaRelatoriosComErroConselhoDeClasse;
                var relatorioQuery = request.ObterObjetoFiltro<ObterRelatorioConselhoClasseTurmaQuery>();
                var relatorioAlunos = await mediator.Send(relatorioQuery);

                relatorioAlunos = relatorioAlunos.OrderBy(a => a.AlunoNome);

                string jsonString;

                if (relatorioAlunos.FirstOrDefault() is RelatorioConselhoClasseBimestre)
                {
                    List<RelatorioConselhoClasseBimestre> listBimestre = relatorioAlunos.Cast<RelatorioConselhoClasseBimestre>().ToList();
                    jsonString = JsonSerializer.Serialize(new { RelatorioConselhoDeClasse = listBimestre });
                }
                else
                {
                    List<RelatorioConselhoClasseFinal> listFinal = relatorioAlunos.Cast<RelatorioConselhoClasseFinal>().ToList();
                    jsonString = JsonSerializer.Serialize(new { RelatorioConselhoDeClasse = listFinal });
                }

                var urlRelatorio = "";

                if (relatorioAlunos.FirstOrDefault().EhBimestreFinal)
                    urlRelatorio = "/sgp/RelatorioConselhoClasse/ConselhoClasseAbaFinal";
                else urlRelatorio = "/sgp/RelatorioConselhoClasse/ConselhoClasse";

                await mediator.Send(new GerarRelatorioAssincronoCommand(urlRelatorio, jsonString, TipoFormatoRelatorio.Pdf, request.CodigoCorrelacao, RotasRabbit.RotaRelatoriosProcessandoConselhoDeClasse));
            }
            catch (Exception ex)
            {
                request.RotaErro = RotasRabbit.RotaRelatoriosComErroConselhoDeClasse;
                throw ex;
            }
        }
    }
}
