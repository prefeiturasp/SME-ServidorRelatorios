using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterAlunosPorTurmaAcompanhamentoApredizagemQuery : IRequest<IEnumerable<AlunoRetornoDto>>
    {
        public ObterAlunosPorTurmaAcompanhamentoApredizagemQuery(string turmaCodigo, long? alunoCodigo, int anoLetivo)
        {
            TurmaCodigo = turmaCodigo;
            AlunoCodigo = alunoCodigo;
            AnoLetivo = anoLetivo;
        }

        public string TurmaCodigo { get; set; }
        public long? AlunoCodigo { get; set; }
        public int AnoLetivo { get; set; }
    }
}
