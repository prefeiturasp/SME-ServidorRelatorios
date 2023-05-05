using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra.Dtos;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    class ObterAusenciaPorAlunoTurmaBimestreQueryHandler : IRequestHandler<ObterAusenciaPorAlunoTurmaBimestreQuery, IEnumerable<AusenciaBimestreDto>>
    {
        private readonly IFrequenciaAlunoRepository frequenciaAlunoRepository;
        public ObterAusenciaPorAlunoTurmaBimestreQueryHandler(IFrequenciaAlunoRepository frequenciaAlunoRepository)
        {
            this.frequenciaAlunoRepository = frequenciaAlunoRepository ?? throw new ArgumentNullException(nameof(frequenciaAlunoRepository));
        }
        public Task<IEnumerable<AusenciaBimestreDto>> Handle(ObterAusenciaPorAlunoTurmaBimestreQuery request, CancellationToken cancellationToken)
        {
            return frequenciaAlunoRepository.ObterAusenciaPorAlunoTurmaBimestre(request.AlunosCodigo,request.TurmaCodigo,request.Bimestre, request.DisciplinasId);
        }
    }
}
