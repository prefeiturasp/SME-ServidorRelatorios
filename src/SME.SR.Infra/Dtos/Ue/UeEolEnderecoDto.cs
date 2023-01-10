namespace SME.SR.Infra
{
    public class UeEolEnderecoDto
    {
        public long UeCodigo { get; set; }
        public string TipoLogradouro { get; set; }
        public string Logradouro { get; set; }
        public string ComplementoEndereco { get; set; }
        public string Bairro { get; set; }
        public string Numero { get; set; }
        public string Telefone { get; set; }
        public string Endereco
            => $"{TipoLogradouro.Trim()} {Logradouro.Trim()}, {Numero.Trim()}{ObterComplementoEndereco()} - {Bairro}.".ToUpper();
        public string TelefoneFormatado
           => !string.IsNullOrEmpty(Telefone) ? $"({Telefone.Substring(0,2)}) {Telefone.Substring(3,4)}-{Telefone.Substring(7, 4)}" : "";
   

        private string ObterComplementoEndereco()
        {
            if (string.IsNullOrEmpty(ComplementoEndereco))
                return string.Empty;
            return $", {ComplementoEndereco}";
        }


    }
}
