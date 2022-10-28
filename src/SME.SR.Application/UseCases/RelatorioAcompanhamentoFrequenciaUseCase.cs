using MediatR;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class RelatorioAcompanhamentoFrequenciaUseCase : IRelatorioAcompanhamentoFrequenciaUseCase
    {
        public readonly IMediator mediator;
        public RelatorioAcompanhamentoFrequenciaUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        public async Task Executar(FiltroRelatorioDto request)
        {
            request.RotaErro = RotasRabbitSGP.RotaRelatoriosComErroAcompanhamentoFrequencia;

            // var filtroRelatorio = request.ObterObjetoFiltro<FiltroAcompanhamentoFrequenciaJustificativaDto>();
            var filtroRelatorio = new FiltroAcompanhamentoFrequenciaJustificativaDto()
            {
                AlunosCodigos = new List<string> { "-99" },
                Bimestre = "-99",
                ComponenteCurricularId = "7",
                DreCodigo = "108800",
                ImprimirFrequenciaDiaria = true,
                TurmaCodigo = "2497183",
                UeCodigo = "092819"
            };

            var retornoRelatorio = await mediator.Send(new ObterDadosRelatorioAcompanhamentoFrequenciaCommand(filtroRelatorio));

            await mediator.Send(new GerarRelatorioAcompanhamentoFrequenciaCommand(retornoRelatorio, request.CodigoCorrelacao));
        }
    }
}
