using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterRespostasEncaminhamentoNAAPAPorIdENomeComponenteSecaoQueryHandler : IRequestHandler<ObterRespostasEncaminhamentoNAAPAPorIdENomeComponenteSecaoQuery, IEnumerable<RespostaQuestaoDto>>
    {
        private readonly IEncaminhamentoNAAPARespostaRepository encaminhamentoNAAPARespostaRepository;

        public ObterRespostasEncaminhamentoNAAPAPorIdENomeComponenteSecaoQueryHandler(IEncaminhamentoNAAPARespostaRepository encaminhamentoNAAPARespostaRepository)
        {
            this.encaminhamentoNAAPARespostaRepository = encaminhamentoNAAPARespostaRepository ?? throw new ArgumentNullException(nameof(encaminhamentoNAAPARespostaRepository));
        }

        public Task<IEnumerable<RespostaQuestaoDto>> Handle(ObterRespostasEncaminhamentoNAAPAPorIdENomeComponenteSecaoQuery request, CancellationToken cancellationToken)
        {
            return encaminhamentoNAAPARespostaRepository.ObterRespostasPorEncaminhamentoIdAsync(request.EncaminhamentoNaapaId, request.NomeComponenteSecao);
        }
    }
}
