namespace SME.SR.Data
{
    public class Turma
    {
        public string CodigoTurma { get; set; }
        public int AnoLetivo { get; set; }
        public string Nome { get; set; }
        public int Semestre { get; set; }
        public Modalidade ModalidadeCodigo { get; set; }
        public ModalidadeTipoCalendario ModalidadeTipoCalendario
        {
            get => ModalidadeCodigo == Modalidade.EJA ?
                ModalidadeTipoCalendario.EJA :
                ModalidadeTipoCalendario.FundamentalMedio;
        }
    }
}
