using MediatR;
using SME.SR.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterAlunosComFrequenciaPorTurmaBimestreQueryHandler : IRequestHandler<ObterAlunosComFrequenciaPorTurmaBimestreQuery, IEnumerable<string>>
    {
        private readonly IFrequenciaAlunoRepository frequenciaAlunoRepository;

        public ObterAlunosComFrequenciaPorTurmaBimestreQueryHandler(IFrequenciaAlunoRepository frequenciaAlunoRepository)
        {
            this.frequenciaAlunoRepository = frequenciaAlunoRepository ?? throw new ArgumentNullException(nameof(frequenciaAlunoRepository));
        }

        public async Task<IEnumerable<string>> Handle(ObterAlunosComFrequenciaPorTurmaBimestreQuery request, CancellationToken cancellationToken)
            => await frequenciaAlunoRepository.ObterAlunosComRegistroFrequenciaPorTurmaBimestre(request.TurmaCodigo, request.Bimestre);
    }
}
