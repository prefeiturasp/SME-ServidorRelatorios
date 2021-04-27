using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterAlunosPorTurmaAcompanhamentoApredizagemQuery : IRequest<IEnumerable<AlunoRetornoDto>>
    {
        public ObterAlunosPorTurmaAcompanhamentoApredizagemQuery(string turmaCodigo, long? alunoCodigo)
        {
            TurmaCodigo = turmaCodigo;
            AlunoCodigo = alunoCodigo;
        }

        public string TurmaCodigo { get; set; }
        public long? AlunoCodigo { get; set; }
    }
}
