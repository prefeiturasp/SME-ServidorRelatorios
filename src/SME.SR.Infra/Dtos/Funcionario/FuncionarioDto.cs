namespace SME.SR.Infra
{
    public class FuncionarioDto
    {
        public int CodigoRF { get; set; }

        public string NomeServidor { get; set; }

        public string Documento { get; set; }

        public string DataInicio { get; set; }

        public string DataFim { get; set; }

        public string Cargo { get; set; }

        public string DocumentoFormatado => Documento.TrimStart('0');
    }
}
