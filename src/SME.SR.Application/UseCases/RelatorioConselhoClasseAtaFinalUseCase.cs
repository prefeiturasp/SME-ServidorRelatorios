using MediatR;
using SME.SR.Infra;
using System;
using System.Linq;
using System.Threading.Tasks;
using static SME.SR.Infra.Enumeradores;

namespace SME.SR.Application
{
    public class RelatorioConselhoClasseAtaFinalUseCase : IRelatorioConselhoClasseAtaFinalUseCase
    {
        private readonly IMediator mediator;

        public RelatorioConselhoClasseAtaFinalUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Executar(FiltroRelatorioDto request)
        {
            try
            {
                var parametros = request.ObterObjetoFiltro<FiltroConselhoClasseAtaFinalDto>();

                var cabecalho = await mediator.Send(new ObterAtaFinalCabecalhoQuery(parametros.TurmaCodigo));
                var alunos = await mediator.Send(new ObterAlunosSituacaoPorTurmaQuery(parametros.TurmaCodigo));
                var alunosAta = alunos.Select(a => new AlunoSituacaoAtaFinalDto(a));

                await mediator.Send(new GerarRelatorioAssincronoCommand("/sgp/RelatorioConselhoAta/ConselhoAta", null, FormatoEnum.Pdf, request.CodigoCorrelacao));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
