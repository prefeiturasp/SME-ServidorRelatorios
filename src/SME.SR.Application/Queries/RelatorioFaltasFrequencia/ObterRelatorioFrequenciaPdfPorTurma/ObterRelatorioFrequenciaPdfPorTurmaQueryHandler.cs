using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application.Queries.RelatorioFaltasFrequencia
{
    public class ObterRelatorioFrequenciaPdfPorTurmaQueryHandler : IRequestHandler<ObterRelatorioFrequenciaPdfPorTurmaQuery, string>
    {
        private readonly IMediator mediator;

        public ObterRelatorioFrequenciaPdfPorTurmaQueryHandler(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<string> Handle(ObterRelatorioFrequenciaPdfPorTurmaQuery request, CancellationToken cancellationToken)
        {
            var ultimoAluno = string.Empty;
            
            foreach (var ue in request.Componentes)
                ultimoAluno = await mediator.Send(new ObterRelatorioFrequenciaPdfPorComponenteQuery(request.Filtro, request.PeriodosEscolares, request.Alunos, request.DeveAdicionarFinal, request.MostrarSomenteFinal));

            return ultimoAluno;
        }
    }
}
