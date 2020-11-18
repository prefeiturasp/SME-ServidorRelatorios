using MediatR;
using SME.SR.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterUltimoDiarioBordoProfessorQueryHandler : IRequestHandler<ObterUltimoDiarioBordoProfessorQuery, DateTime>
    {
        private readonly IDiarioBordoRepository diarioBordoRepository;

        public ObterUltimoDiarioBordoProfessorQueryHandler(IDiarioBordoRepository diarioBordoRepository)
        {
            this.diarioBordoRepository = diarioBordoRepository ?? throw new ArgumentNullException(nameof(diarioBordoRepository));
        }

        public async Task<DateTime> Handle(ObterUltimoDiarioBordoProfessorQuery request, CancellationToken cancellationToken)
            => await diarioBordoRepository.ObterUltimoDiarioBordoProfessor(request.ProfessorRf);
    }
}
