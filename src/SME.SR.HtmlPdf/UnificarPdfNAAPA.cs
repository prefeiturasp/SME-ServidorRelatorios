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
        private readonly List<PdfUnificadoEncaminhamentoNAAPADto> pdfsPorEncaminhamento;


        public UnificarPdfNAAPA(
                    VariaveisAmbiente variaveisAmbiente, 
                    string nomePdfUnificado,
                    List<PdfUnificadoEncaminhamentoNAAPADto> pdfsPorEncaminhamento)
        {
            this.variaveisAmbiente = variaveisAmbiente;
            this.nomePdfUnificado = nomePdfUnificado;
            this.pdfsPorEncaminhamento = pdfsPorEncaminhamento;
        }

        private string ObterDiretorioPdfGerado(string nomePdf)
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"relatorios/{nomePdf}.pdf");
        }

        public bool Execute()
        {
            if (!pdfsPorEncaminhamento.Any() || string.IsNullOrEmpty(this.nomePdfUnificado))
                return false;

            var unificador = new UnificadorPdf(nomePdfUnificado, ObterDiretoriosPdfParaUnificacao());

            unificador.Processar();

            RemoveArquivosGerados();

            return true;
        }

        private List<string> ObterDiretoriosPdfParaUnificacao()
        {
            var diretorioPdfs = new List<string>();

            foreach (var arquivo in pdfsPorEncaminhamento)
            {
                diretorioPdfs.Add(ObterDiretorioPdfGerado(arquivo.pdfGerado));
                diretorioPdfs.AddRange(ObterCaminhoCompletoPdfs(arquivo.Anexos));
            }

            return diretorioPdfs;
        }

        private void RemoveArquivosGerados()
        {
            foreach (var arquivo in pdfsPorEncaminhamento)
                File.Delete(ObterDiretorioPdfGerado(arquivo.pdfGerado)); 
        }

        private List<string> ObterCaminhoCompletoPdfs(List<ArquivoDto> arquivosPdf)
        {
            var caminhos = new List<string>();

            foreach (var arquivo in arquivosPdf)
            {
                if (arquivo.Extensao == "pdf")
                {
                    var caminho = ObterCaminho(arquivo);

                    if (!string.IsNullOrEmpty(caminho))
                        caminhos.Add(caminho);
                }
            }

            return caminhos;    
        }
        
        private string ObterCaminho(ArquivoDto arquivo)
        {
            var diretorio = Path.Combine(variaveisAmbiente.PastaArquivosSGP ?? string.Empty, $@"Arquivos/{arquivo.Tipo}");

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
