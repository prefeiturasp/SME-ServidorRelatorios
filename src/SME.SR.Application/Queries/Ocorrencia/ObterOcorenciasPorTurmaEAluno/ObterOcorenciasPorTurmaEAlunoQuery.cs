using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterOcorenciasPorTurmaEAlunoQuery : IRequest<IEnumerable<AcompanhamentoAprendizagemOcorrenciaDto>>
    {
        public ObterOcorenciasPorTurmaEAlunoQuery(long turmaId, long? alunoCodigo)
        {
            TurmaId = turmaId;
            AlunoCodigo = alunoCodigo;
        }

        public long TurmaId { get; set; }
        public long? AlunoCodigo { get; set; }
    }
}
