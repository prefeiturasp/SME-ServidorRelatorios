using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterDadosRelatorioAlteracaoNotasCommandHandler : IRequestHandler<ObterDadosRelatorioAlteracaoNotasCommand, IEnumerable<TurmaAlteracaoNotasDto>>
    {
        private readonly IMediator mediator;
        private readonly ITurmaRepository TurmaRepository;

        public ObterDadosRelatorioAlteracaoNotasCommandHandler(IMediator mediator, ITurmaRepository TurmaRepository)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this.TurmaRepository = TurmaRepository ?? throw new ArgumentNullException(nameof(TurmaRepository));
        }

        public  async Task<IEnumerable<TurmaAlteracaoNotasDto>> Handle(ObterDadosRelatorioAlteracaoNotasCommand request, CancellationToken cancellationToken)
        {
            var listaTurmaAlteracaoNotasDto = new List<TurmaAlteracaoNotasDto>();          

            return listaTurmaAlteracaoNotasDto;
        }

        private bool FiltrouTodasTurmas(string codigoUe)
            => codigoUe == "-99";
    }
}
