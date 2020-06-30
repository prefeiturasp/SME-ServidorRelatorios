using static SME.SR.Infra.Enumeradores;

namespace SME.SR.Infra
{
    public class FiltroConselhoClasseAtaFinalDto
    {
        public int AnoLetivo { get; set; }
        public string DreId { get; set; }
        public string UeId { get; set; }
        public Modalidade ModalidadeId { get; set; }
        public int? Semestre { get; set; }
        public string TurmaId { get; set; }
        public FormatoEnum Formato { get; set; }
    }
}
