using MediatR;
using SME.SR.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application.Queries.BoletimEscolar
{
    public class ObterConselhoClassePorFechamentoTurmaIdQueryHandler : IRequestHandler<ObterConselhoClassePorFechamentoTurmaIdQuery, long>
    {
        private IConselhoClasseRepository _conselhoClasseRepository;

        public async Task<long> Handle(ObterConselhoClassePorFechamentoTurmaIdQuery request, CancellationToken cancellationToken)
        {
            return await _conselhoClasseRepository.ObterConselhoPorFechamentoTurmaId(request.FechamentoTurmaId);
        }
    }
}
