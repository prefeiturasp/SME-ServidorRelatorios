using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterTurmaCodigosEAlunosPorAnoLetivoAlunoTipoTurmaQueryHandler: IRequestHandler<ObterTurmaCodigosEAlunosPorAnoLetivoAlunoTipoTurmaQuery, IEnumerable<CodigosTurmasAlunoPorAnoLetivoAlunoEdFisicaDto>>
    {
        private readonly ITurmaEolRepository turmaEolRepository;

        public ObterTurmaCodigosEAlunosPorAnoLetivoAlunoTipoTurmaQueryHandler(ITurmaEolRepository turmaEolRepository)
        {
            this.turmaEolRepository = turmaEolRepository ?? throw new System.ArgumentNullException(nameof(turmaEolRepository));
        }

        public Task<IEnumerable<CodigosTurmasAlunoPorAnoLetivoAlunoEdFisicaDto>> Handle(ObterTurmaCodigosEAlunosPorAnoLetivoAlunoTipoTurmaQuery request, CancellationToken cancellationToken)
        {
            return turmaEolRepository.BuscarCodigosTurmasAlunosPorAnoLetivoAluno(request.AnoLetivo,request.CodigoAlunos,request.TiposTurmas,request.ConsideraHistorico,request.DataReferencia,request.UeCodigo);
        }
    }
}
