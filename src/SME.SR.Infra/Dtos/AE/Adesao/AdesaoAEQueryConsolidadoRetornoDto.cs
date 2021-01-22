namespace SME.SR.Infra.Dtos.AE.Adesao
{
    public class AdesaoAEQueryConsolidadoRetornoDto
    {
        public string DreCodigo { get; set; }
        public string UeCodigo { get; set; }
        public long TurmaCodigo { get; set; }
        public int PrimeiroAcessoIncompleto { get; set; }
        public int Validos { get; set; }
        public int CpfsInvalidos { get; set; }
        public int SemAppInstalado { get; set; }
        public string DreNome { get; set; }
    }
}
