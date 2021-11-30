namespace SME.SR.Infra
{
    public class RelatorioOcorrenciasDto
    {
        public string CriancaNome { get; set; }
        public string DataOcorrencia { get; set; }
        public string Turma { get; set; }
        public string TipoOcorrencia { get; set; }
        public string TituloOcorrencia { get; set; }
        public string DescricaoOcorrencia { get; set; }  
        public bool ImprimirDadosOcorrencia { get; set; }
        public bool EhUltimaPagina { get; set; }

    }
}
