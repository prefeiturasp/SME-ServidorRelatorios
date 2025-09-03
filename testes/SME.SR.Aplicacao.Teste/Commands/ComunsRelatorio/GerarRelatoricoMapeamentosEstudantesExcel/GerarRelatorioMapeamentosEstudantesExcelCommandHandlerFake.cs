using MediatR;
using SME.SR.Application;
using SME.SR.Infra;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Aplicacao.Teste.Commands.ComunsRelatorio.GerarRelatoricoMapeamentosEstudantesExcel
{
    public class GerarRelatorioMapeamentosEstudantesExcelCommandHandlerFake
    : GerarRelatorioMapeamentosEstudantesExcelCommandHandler
    {
        public GerarRelatorioMapeamentosEstudantesExcelCommandHandlerFake(
            IMediator mediator,
            IServicoFila servicoFila
        ) : base(mediator, servicoFila) { }

        public async Task ExecutarHandle(
            GerarRelatorioMapeamentosEstudantesExcelCommand comando,
            CancellationToken cancellationToken = default
        )
        {
            await Handle(comando, cancellationToken);
        }
    }
}
