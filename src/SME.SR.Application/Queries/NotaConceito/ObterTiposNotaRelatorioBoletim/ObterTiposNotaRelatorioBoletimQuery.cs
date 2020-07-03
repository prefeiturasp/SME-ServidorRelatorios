using SME.SR.Data;

namespace SME.SR.Application
{
    public class ObterTiposNotaRelatorioBoletimQuery
    {
        public long DreId { get; set; }
        public long UeId { get; set; }
        public int AnoLetivo { get; set; }
        public Turma[] Turma { get; set; }
    }
}
