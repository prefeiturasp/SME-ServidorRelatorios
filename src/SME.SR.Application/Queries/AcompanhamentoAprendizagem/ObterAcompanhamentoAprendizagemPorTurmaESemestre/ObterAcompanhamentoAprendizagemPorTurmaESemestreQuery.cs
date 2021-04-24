using MediatR;
using SME.SR.Infra;

namespace SME.SR.Application
{
    public class ObterAcompanhamentoAprendizagemPorTurmaESemestreQuery : IRequest<AcompanhamentoAprendizagemAlunoRetornoDto>
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
