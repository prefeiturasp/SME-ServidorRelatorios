using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using SME.SR.Infra;

namespace SME.SR.Application.Queries.RelatorioFaltasFrequencia
{
    public class ObterRelatorioFrequenciaPdfPorDreQueryHandler : IRequestHandler<ObterRelatorioFrequenciaPdfPorDreQuery, RelatorioFrequenciaDreDto>
    {
        private readonly IMediator mediator;

        public ObterRelatorioFrequenciaPdfPorDreQueryHandler(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<RelatorioFrequenciaDreDto> Handle(ObterRelatorioFrequenciaPdfPorDreQuery request, CancellationToken cancellationToken)
        {
            var relatorioFrequenciaDreDto = new RelatorioFrequenciaDreDto();

            foreach (var ue in request.Ues)
            {
                relatorioFrequenciaDreDto.Ues.Add(await mediator.Send(new ObterRelatorioFrequenciaPdfPorUeQuery(request.Filtro,ue.TurmasAnos, request.PeriodosEscolares, request.Componentes, request.Alunos, request.Turmas,
                    request.DeveAdicionarFinal, request.MostrarSomenteFinal)));
            }

            return relatorioFrequenciaDreDto;
        }
    }
}
