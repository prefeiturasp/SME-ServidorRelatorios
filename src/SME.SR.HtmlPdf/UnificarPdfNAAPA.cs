using Microsoft.EntityFrameworkCore.Internal;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.IO;

namespace SME.SR.HtmlPdf
{
    public class UnificarPdfNAAPA
    {
        private readonly VariaveisAmbiente variaveisAmbiente;
        private readonly string nomePdfUnificado;
        private readonly string diretorioPdfGerado;
        private readonly List<ArquivoDto> anexos;

        public UnificarPdfNAAPA(
                    VariaveisAmbiente variaveisAmbiente,
                    string pdfGerado,
                    List<ArquivoDto> anexos,
                    string nomePdfUnificado)
        {
            this.variaveisAmbiente = variaveisAmbiente;
            this.nomePdfUnificado = nomePdfUnificado;
            this.anexos = anexos;
            this.diretorioPdfGerado = ObterDiretorioPdfGerado(pdfGerado);
        }

        private string ObterDiretorioPdfGerado(string nomePdf)
        {
            var diretorio = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "relatorios");

            return Path.Combine(diretorio, $"{nomePdf}.pdf");
        }

        public bool Execute()
        {
            if (NaoPodeUnificarPdf())
                return false;

            var unificador = new UnificadorPdf(nomePdfUnificado, ObterDiretoriosPdfParaUnificacao());

            unificador.Processar();

            RemoveArquivosGerados();

            return true;
        }

        private bool NaoPodeUnificarPdf()
        {
            return !(anexos.Any() 
                && !string.IsNullOrEmpty(nomePdfUnificado) 
                && !string.IsNullOrEmpty(diretorioPdfGerado) 
                && File.Exists(diretorioPdfGerado));
        }

        private List<string> ObterDiretoriosPdfParaUnificacao()
        {
            var diretorioPdfs = new List<string>() { diretorioPdfGerado };

            diretorioPdfs.AddRange(ObterCaminhoCompletoPdfs(anexos));

            return diretorioPdfs;
        }

        private void RemoveArquivosGerados()
        {
            File.Delete(diretorioPdfGerado); 
        }

        private List<string> ObterCaminhoCompletoPdfs(List<ArquivoDto> arquivosPdf)
        {
            var caminhos = new List<string>();

            foreach (var arquivo in arquivosPdf)
            {
                var caminho = ObterCaminho(arquivo);

                if (!string.IsNullOrEmpty(caminho))
                    caminhos.Add(caminho);
            }

            return caminhos;    
        }
        
        private string ObterCaminho(ArquivoDto arquivo)
        {
            var diretorio = Path.Combine(variaveisAmbiente.PastaArquivosSGP ?? string.Empty, "Arquivos");

            if (!Directory.Exists(diretorio))
                return string.Empty;

            var nomeArquivo = $"{arquivo.Codigo}.{arquivo.Extensao}";
            var caminhoArquivo = Path.Combine(diretorio, nomeArquivo);

            if (!File.Exists(caminhoArquivo))
                return string.Empty;

            return caminhoArquivo;
        }
    }
}
