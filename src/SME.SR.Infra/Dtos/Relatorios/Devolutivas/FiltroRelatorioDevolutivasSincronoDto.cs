namespace SME.SR.Infra.Dtos
{
    public class FiltroRelatorioDevolutivasSincronoDto
    {
        public long DevolutivaId { get; set; }
        public string UsuarioNome { get; set; }
        public string UsuarioRF { get; set; }
        public long UeId { get; set; }
        public long TurmaId { get; set; }
    }
}
