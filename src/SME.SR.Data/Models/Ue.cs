using SME.SR.Infra;

namespace SME.SR.Data
{
    public class Ue
    {
        public long Id { get; set; }
        public string Codigo { get; set; }
        public string Nome { get; set; }
        public TipoEscola TipoEscola { get; set; }
    }
}
