using MediatR;
using SME.SR.Application.Queries.CDEP.ObterRelatorioCDEPControleLivrosEmprestadoSintetico;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application.Commands.CDEP.GerarRelatorioControleLivrosEmprestadosSintetico
{
    public class GerarRelatorioControleLivrosEmprestadosSinteticoCommandHandler : IRequestHandler<GerarRelatorioControleLivrosEmprestadosSinteticoCommand, string>
    {
        private readonly IMediator mediator;

        public GerarRelatorioControleLivrosEmprestadosSinteticoCommandHandler(IMediator mediator)
        {
            this.mediator = mediator;
        }
        public async Task<string> Handle(GerarRelatorioControleLivrosEmprestadosSinteticoCommand request, CancellationToken cancellationToken)
        {
            var livros = await mediator.Send(
               new ObterRelatorioCDEPControleLivrosEmprestadoSinteticoQuery()
               {
                   situacaoSolicitacaoItem = request.Filtros.SituacaoSolicitacaoItem
               });




            return string.Empty;
        }
    }
}
