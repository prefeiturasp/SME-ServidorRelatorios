namespace SME.SR.Infra
{
    public class FiltroRelatorioSondagemPortuguesCapacidadeLeituraDto
    {
        public int Bimestre { get; set; }

        public int AnoLetivo { get; set; }

        public int TurmaCodigo { get; set; }

        public long DreCodigo { get; set; }

        public string UeCodigo { get; set; }

        public string Ano { get; set; }

        public string UsuarioRf { get; set; }
        public int Semestre { get; set; }
    }
}
