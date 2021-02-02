using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    class ObterQuantidadeDeAulaGradeQueryHandler : IRequestHandler<ObterQuantidadeDeAulaGradeQuery, int>
    {
        private readonly IAulaRepository aulaRepository;

        public ObterQuantidadeDeAulaGradeQueryHandler(IAulaRepository aulaRepository)
        {
            this.aulaRepository = aulaRepository ?? throw new ArgumentNullException(nameof(aulaRepository));
        }

        public async Task<int> Handle(ObterQuantidadeDeAulaGradeQuery request, CancellationToken cancellationToken)
            => await aulaRepository.ObterQuantidadeAulaGrade(request.TurmaId,
                                                    request.ComponenteCurricularId);
    }
}
