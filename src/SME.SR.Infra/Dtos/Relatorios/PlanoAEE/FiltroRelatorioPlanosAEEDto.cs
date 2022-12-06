namespace SME.SR.Infra
{
    public class FiltroRelatorioPlanosAeeDto
    {
        public int AnoLetivo { get; set; }
        public string DreCodigo { get; set; }
        public string UeCodigo { get; set; }
        public int Modalidade { get; set; }
        public int Semestre { get; set; }
        public string[] CodigosTurma { get; set; }
        public int Situacao { get; set; }
        public bool ExibirEncerrados { get; set; }
        public string[] CodigosResponsavel { get; set; }
        public string PAAIResponsavel { get; set; }
        public string UsuarioNome { get; set; }
        public string UsuarioRf { get; set; }
    }
}