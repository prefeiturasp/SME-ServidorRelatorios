using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterFechamentosPorCodigoTurmaQueryHandler : IRequestHandler<ObterFechamentosPorCodigoTurmaQuery, IEnumerable<FechamentoTurma>>
    {
        private IFechamentoTurmaRepository fechamentoTurmaRepository;

        public ObterFechamentosPorCodigoTurmaQueryHandler(IFechamentoTurmaRepository fechamentoTurmaRepository)
        {
            this.fechamentoTurmaRepository = fechamentoTurmaRepository;
        }

        public async Task<IEnumerable<FechamentoTurma>> Handle(ObterFechamentosPorCodigoTurmaQuery request, CancellationToken cancellationToken)
        {
            return await fechamentoTurmaRepository.ObterTurmaPeriodoFechamentoPorId(request.TurmaCodigo);
        }
    }
}
