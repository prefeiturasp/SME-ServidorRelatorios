using MediatR;
using SME.SR.Data.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterFrequenciaGlobalPorAlunoQueryHandler : IRequestHandler<ObterFrequenciaGlobalPorAlunoQuery, string>
    {
        private readonly IFrequenciaAlunoRepository frequenciaRepository;

        public ObterFrequenciaGlobalPorAlunoQueryHandler(IFrequenciaAlunoRepository frequenciaRepository)
        {
            this.frequenciaRepository = frequenciaRepository ?? throw new ArgumentNullException(nameof(frequenciaRepository));
        }

        public async Task<string> Handle(ObterFrequenciaGlobalPorAlunoQuery request, CancellationToken cancellationToken)
        {
            return await frequenciaRepository.ObterFrequenciaGlobal(request.CodigoTurma, request.CodigoAluno);
        }
    }
}
