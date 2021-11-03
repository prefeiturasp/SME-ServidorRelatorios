using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterAcompanhamentoAprendizagemPorTurmaESemestreQuery : IRequest<IEnumerable<AcompanhamentoAprendizagemAlunoDto>>
    {
        public ObterAcompanhamentoAprendizagemPorTurmaESemestreQuery(long turmaId, string alunoCodigo, int semestre)
        {
            TurmaId = turmaId;
            AlunoCodigo = alunoCodigo;
            Semestre = semestre;
        }

        public long TurmaId { get; set; }
        public string AlunoCodigo { get; set; }
        public int Semestre { get; set; }
    }
}
