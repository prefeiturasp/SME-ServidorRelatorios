using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterRegistrosIndividuaisPorTurmaEAlunoQueryHandler : IRequestHandler<ObterRegistrosIndividuaisPorTurmaEAlunoQuery, IEnumerable<RegistroIndividualRetornoDto>>
    {
        private readonly IRegistroIndividualRepository registroIndividualRepository;

        public ObterRegistrosIndividuaisPorTurmaEAlunoQueryHandler(IRegistroIndividualRepository registroIndividualRepository)
        {
            this.registroIndividualRepository = registroIndividualRepository ?? throw new ArgumentNullException(nameof(registroIndividualRepository));
        }

        public async Task<IEnumerable<RegistroIndividualRetornoDto>> Handle(ObterRegistrosIndividuaisPorTurmaEAlunoQuery request, CancellationToken cancellationToken)
        {
            var registrosIndividuais = await registroIndividualRepository.ObterRegistrosIndividuaisPorTurmaEAluno(request.TurmaId, request.AlunoCodigo, request.DataInicio, request.DataFim);

            return registrosIndividuais;
        }
    }
}
