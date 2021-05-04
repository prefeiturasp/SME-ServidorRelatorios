using System;
using System.Drawing.Imaging;
using System.IO;

namespace SME.SR.Infra
{
    public class AcompanhamentoAprendizagemAlunoFotoDto
    {
        public string TipoArquivo { get; set; }
        public Guid Codigo { get; set; }
        public string NomeOriginal { get; set; }
        public string Extensao { get => NomeOriginal != String.Empty ? NomeOriginal.Split(".")[NomeOriginal.Split(".").Length - 1] : ""; }

        public string ArquivoBase64()
        {
            //TODO: Pegar a pasta do servidor onde ficarão as imagens
            //var diretorio = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Arquivos/Editor");
            var diretorio = @"C:\Development\AMcom\Projects\SME-NovoSGP\src\SME.SGP.Api\bin\Debug\netcoreapp2.2\Arquivos\FotoAluno";

            if (!Directory.Exists(diretorio))
                Directory.CreateDirectory(diretorio);

            var nomeArquivo = $"{Codigo}.{Extensao}";
            var caminhoArquivo = Path.Combine(diretorio, nomeArquivo);
            if (!File.Exists(caminhoArquivo))
                return "";

            var arquivo = File.ReadAllBytes(caminhoArquivo);
            return $"data:{TipoArquivo};base64,{Convert.ToBase64String(arquivo)}";
        }
    }
}
