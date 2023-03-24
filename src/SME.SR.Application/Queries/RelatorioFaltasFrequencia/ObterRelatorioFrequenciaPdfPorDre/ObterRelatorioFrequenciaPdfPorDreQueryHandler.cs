using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application.Queries.RelatorioFaltasFrequencia
{
    public class ObterRelatorioFrequenciaPdfPorDreQueryHandler : IRequestHandler<ObterRelatorioFrequenciaPdfPorDreQuery, string>
    {
        private readonly IMediator mediator;

        public ObterRelatorioFrequenciaPdfPorDreQueryHandler(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<string> Handle(ObterRelatorioFrequenciaPdfPorDreQuery request, CancellationToken cancellationToken)
        {
            var ultimoAluno = string.Empty;
            
            foreach (var ue in request.Ues)
                ultimoAluno = await mediator.Send(new ObterRelatorioFrequenciaPdfPorUeQuery(request.Filtro, request.PeriodosEscolares, request.Componentes, request.Alunos, request.Turmas, request.DeveAdicionarFinal, request.MostrarSomenteFinal));

            return ultimoAluno;
        }
    }
}
