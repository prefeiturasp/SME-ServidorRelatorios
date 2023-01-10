using SME.SR.Infra;

namespace SME.SR.Data.Models
{
    public class Questionario
    {
        public long Id { get; set; }
        public string Nome { get; set; }
        public TipoQuestionario Tipo { get; set; }
    }
}