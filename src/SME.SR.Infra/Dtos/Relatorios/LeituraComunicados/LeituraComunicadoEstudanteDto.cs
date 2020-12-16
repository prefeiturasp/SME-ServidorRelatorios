namespace SME.SR.Infra
{
    public class LeituraComunicadoEstudanteDto
    {
        public string NumeroChamada { get; set; }
        public long ComunicadId { get; set; }
        public string CodigoEstudante { get; set; }
        public string Estudante { get; set; }
        public string ResponsavelCPF { get; set; }
        public string Responsavel { get; set; }
        public string TipoResponsavel { get; set; }
        public string ContatoResponsavel { get; set; }
        public string Situacao { get; set; }
        public bool Instalado { get; set; }
    }
}
