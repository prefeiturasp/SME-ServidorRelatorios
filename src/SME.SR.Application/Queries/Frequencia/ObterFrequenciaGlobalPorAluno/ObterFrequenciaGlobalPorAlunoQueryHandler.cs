using MediatR;
using SME.SR.Data.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterFrequenciaGlobalPorAlunoQueryHandler : IRequestHandler<ObterFrequenciaGlobalPorAlunoQuery, double>
    {
        private IFrequenciaAlunoRepository _frequenciaRepository;

        public ObterFrequenciaGlobalPorAlunoQueryHandler(IFrequenciaAlunoRepository frequenciaRepository)
        {
            this._frequenciaRepository = frequenciaRepository;
        }

        public async Task<double> Handle(ObterFrequenciaGlobalPorAlunoQuery request, CancellationToken cancellationToken)
        {
            return await _frequenciaRepository.ObterFrequenciaGlobal(request.CodigoTurma, request.CodigoAluno);
        }
    }
}
