using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterFrequenciaAlunoDiariaQueryHandler : IRequestHandler<ObterFrequenciaAlunoDiariaQuery, IEnumerable<RelatorioFrequenciaIndividualDiariaAlunoDto>>
    {
        private readonly IFrequenciaAlunoRepository frequenciaAlunoRepository;

        public ObterFrequenciaAlunoDiariaQueryHandler(IFrequenciaAlunoRepository frequenciaAlunoRepository)
        {
            this.frequenciaAlunoRepository = frequenciaAlunoRepository ?? throw new ArgumentNullException(nameof(frequenciaAlunoRepository));
        }

        public async Task<IEnumerable<RelatorioFrequenciaIndividualDiariaAlunoDto>> Handle(ObterFrequenciaAlunoDiariaQuery request, CancellationToken cancellationToken)
        {
            return await frequenciaAlunoRepository.ObterFrequenciaAlunosDiario(request.CodigosAlunos, request.Bimestre, request.TurmaCodigo, request.ComponenteCurricularId);
        }
    }
}
