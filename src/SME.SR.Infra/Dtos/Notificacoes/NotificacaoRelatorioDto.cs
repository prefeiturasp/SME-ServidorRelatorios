namespace SME.SR.Infra
{
    public class NotificacaoRelatorioDto
    {
        public long Codigo { get; set; }
        public string Titulo { get; set; }
        public string Categoria { get; set; }
        public string Tipo { get; set; }
        public string Situacao { get; set; }
        public string Descricao { get; set; }
        public string DataRecebimento { get; set; }
        public string DataLeitura { get; set; }
    }
}
