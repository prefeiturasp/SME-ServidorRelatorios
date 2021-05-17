using System;
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
            var diretorio = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Arquivos/Editor");

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
