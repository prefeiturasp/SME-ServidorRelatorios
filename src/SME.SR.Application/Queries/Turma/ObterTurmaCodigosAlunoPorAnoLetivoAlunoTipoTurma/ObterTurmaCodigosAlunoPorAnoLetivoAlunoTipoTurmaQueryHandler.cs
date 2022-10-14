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
    public class ObterTurmaCodigosAlunoPorAnoLetivoAlunoTipoTurmaQueryHandler : IRequestHandler<ObterTurmaCodigosAlunoPorAnoLetivoAlunoTipoTurmaQuery, IEnumerable<int>>
    {
        private readonly ITurmaEolRepository turmaEolRepository;

        public ObterTurmaCodigosAlunoPorAnoLetivoAlunoTipoTurmaQueryHandler(ITurmaEolRepository turmaEolRepository)
        {
            this.turmaEolRepository = turmaEolRepository ?? throw new ArgumentNullException(nameof(turmaEolRepository));
        }
        public async Task<IEnumerable<int>> Handle(ObterTurmaCodigosAlunoPorAnoLetivoAlunoTipoTurmaQuery request, CancellationToken cancellationToken)
         => await turmaEolRepository.BuscarCodigosTurmasAlunoPorAnoLetivoAlunoAsync(request.AnoLetivo, request.CodigoAlunos,
                   request.TiposTurmas, request.ConsideraHistorico, request.DataReferencia);
    }
}
