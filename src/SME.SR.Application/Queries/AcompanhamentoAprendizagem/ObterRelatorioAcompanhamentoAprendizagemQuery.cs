using MediatR;
using SME.SR.Infra;

namespace SME.SR.Application
{
    public class ObterRelatorioAcompanhamentoAprendizagemQuery : IRequest<RelatorioAcompanhamentoAprendizagemDto>
    {
        public ObterRelatorioAcompanhamentoAprendizagemQuery(string turmaCodigo, long? alunoCodigo)
        {
            TurmaCodigo = turmaCodigo;
            AlunoCodigo = alunoCodigo;
        }

        public string TurmaCodigo { get; set; }
        public long? AlunoCodigo { get; set; }
    }
}
