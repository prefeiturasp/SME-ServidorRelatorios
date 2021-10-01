using MediatR;
using SME.SR.Data.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ExisteFrequenciaRegistradaPorTurmaComponenteCurricularQueryHandler : IRequestHandler<ExisteFrequenciaRegistradaPorTurmaComponenteCurricularQuery, bool>
    {
        private readonly IFrequenciaAlunoRepository frequenciaAlunoRepository;

        public ExisteFrequenciaRegistradaPorTurmaComponenteCurricularQueryHandler(IFrequenciaAlunoRepository frequenciaAlunoRepository)
        {
            this.frequenciaAlunoRepository = frequenciaAlunoRepository ?? throw new ArgumentNullException(nameof(frequenciaAlunoRepository));
        }

        public async Task<bool> Handle(ExisteFrequenciaRegistradaPorTurmaComponenteCurricularQuery request, CancellationToken cancellationToken)
            => await frequenciaAlunoRepository.ExisteFrequenciaRegistradaPorTurmaComponenteCurricular(request.CodigoTurma,
                                                                                                      request.ComponenteCurricularId,
                                                                                                      request.PeriodoEscolarId,request.Bimestres);
    }
}
