using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application.Queries.RelatorioFaltasFrequencia
{
    public class ObterRelatorioFrequenciaPdfPorUeQueryHandler : IRequestHandler<ObterRelatorioFrequenciaPdfPorUeQuery, string>
    {
        private readonly IMediator mediator;

        public ObterRelatorioFrequenciaPdfPorUeQueryHandler(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<string> Handle(ObterRelatorioFrequenciaPdfPorUeQuery request, CancellationToken cancellationToken)
        {
            var ultimoAluno = string.Empty;
            
            foreach (var ue in request.Turmas)
                ultimoAluno = await mediator.Send(new ObterRelatorioFrequenciaPdfPorTurmaQuery(request.Filtro, request.PeriodosEscolares, request.Componentes, request.Alunos, request.DeveAdicionarFinal, request.MostrarSomenteFinal));

            return ultimoAluno;
        }
    }
}
