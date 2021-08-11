using MediatR;
using SME.SR.Data.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ExisteFrequenciaRegistradaPorTurmaComponenteCurricularEAnoQueryHandler : IRequestHandler<ExisteFrequenciaRegistradaPorTurmaComponenteCurricularEAnoQuery, bool>
    {
        private readonly IFrequenciaAlunoRepository frequenciaAlunoRepository;

        public ExisteFrequenciaRegistradaPorTurmaComponenteCurricularEAnoQueryHandler(IFrequenciaAlunoRepository frequenciaAlunoRepository)
        {
            this.frequenciaAlunoRepository = frequenciaAlunoRepository ?? throw new ArgumentNullException(nameof(frequenciaAlunoRepository));
        }

        public async Task<bool> Handle(ExisteFrequenciaRegistradaPorTurmaComponenteCurricularEAnoQuery request, CancellationToken cancellationToken)
            => await frequenciaAlunoRepository.ExisteFrequenciaRegistradaPorTurmaComponenteCurricularEAno(request.CodigoTurma,
                                                                                                          request.ComponenteCurricularId,
                                                                                                          request.AnoLetivo);
    }
}
