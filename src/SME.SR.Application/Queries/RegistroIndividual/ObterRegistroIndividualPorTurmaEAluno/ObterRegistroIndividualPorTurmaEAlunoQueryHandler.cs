using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterRegistroIndividualPorTurmaEAlunoQueryHandler : IRequestHandler<ObterRegistroIndividualPorTurmaEAlunoQuery, IEnumerable<AcompanhamentoAprendizagemRegistroIndividualDto>>
    {
        private readonly IRegistroIndividualRepository registroIndividualRepository;

        public ObterRegistroIndividualPorTurmaEAlunoQueryHandler(IRegistroIndividualRepository registroIndividualRepository)
        {
            this.registroIndividualRepository = registroIndividualRepository ?? throw new ArgumentNullException(nameof(registroIndividualRepository));
        }

        public async Task<IEnumerable<AcompanhamentoAprendizagemRegistroIndividualDto>> Handle(ObterRegistroIndividualPorTurmaEAlunoQuery request, CancellationToken cancellationToken)
        {
            var registroIndividual = await registroIndividualRepository.ObterRegistroIndividualPorTurmaEAluno(request.TurmaId, request.AlunoCodigo);

            return registroIndividual;
        }
    }
}
