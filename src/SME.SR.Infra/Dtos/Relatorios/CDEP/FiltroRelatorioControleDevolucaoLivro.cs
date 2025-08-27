namespace SME.SR.Infra.Dtos.Relatorios.CDEP
{
    public class FiltroRelatorioControleDevolucaoLivro
    {
        public string Solicitante { get; set; }
        public string Usuario { get; set; }
        public string UsuarioRF { get; set; }
        public long[] TiposAcervosPermitidos { get; set; }
        public bool SomenteEmAtraso { get; set; }
    }
}
