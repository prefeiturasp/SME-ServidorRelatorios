using SME.SR.Infra.Utilitarios;
using System.Text.RegularExpressions;

namespace SME.SR.Infra
{
    public class AcompanhamentoAprendizagemAlunoDto
    {
        public string PercursoColetivoTurma { get; set; }
        public string Semestre { get; set; }
        public string AlunoCodigo { get; set; }
        public string Observacoes { get; set; }
        public string PercursoIndividual { get; set; }
    }
}
