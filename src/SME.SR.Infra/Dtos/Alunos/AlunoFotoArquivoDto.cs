namespace SME.SR.Infra
{
    public class AlunoFotoArquivoDto
    {
        public long CodigoAluno { get; set; }
        public ArquivoDto ArquivoDto { get; set; }

        public string FotoBase64 { get; private set; }

        public void DefinirFotoBase64(string base64) => FotoBase64 = base64;
    }
}
