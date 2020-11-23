using MediatR;
using SME.SR.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterUltimaFrequenciaRegistradaProfessorQueryHandler : IRequestHandler<ObterUltimaFrequenciaRegistradaProfessorQuery, DateTime?>
    {
        private readonly IFrequenciaAlunoRepository frequenciaRepository;

        public ObterUltimaFrequenciaRegistradaProfessorQueryHandler(IFrequenciaAlunoRepository frequenciaRepository)
        {
            this.frequenciaRepository = frequenciaRepository ?? throw new ArgumentNullException(nameof(frequenciaRepository));
        }

        public async Task<DateTime?> Handle(ObterUltimaFrequenciaRegistradaProfessorQuery request, CancellationToken cancellationToken)
            => await frequenciaRepository.ObterUltimaFrequenciaRegistradaProfessor(request.ProfessorRf);
    }
}
