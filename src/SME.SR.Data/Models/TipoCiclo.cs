using SME.SR.Infra;

namespace SME.SR.Data
{
    public class TipoCiclo
    {
        public long Id { get; set; }
        public string Descricao { get; set; }
        public Modalidade Modalidade { get; set; }
        public string Ano { get; set; }
    }
}
