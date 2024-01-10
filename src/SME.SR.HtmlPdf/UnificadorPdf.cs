using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;
using System.IO;
using System;

namespace SME.SR.HtmlPdf
{
    public class UnificadorPdf
    {
        private string NomePdfUnificado { get; set; }
        private string[] NomeArquivos {  get; set; }
        private string CaminhoBase { get; set; }

        public UnificadorPdf(string nomePdfUnificado, string[] nomeArquivos)
        {
            NomePdfUnificado = nomePdfUnificado;
            NomeArquivos = new string[] {
                "Padroes_de_Projetos_-_Solucoes_Reutiliza.pdf",
                "Princípios, padrões e práticas ágeis Robert Martin.pdf",
                "Introdução à Arquitetura e Design de Software - Casa do Codigo.pdf"};

            CaminhoBase = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "relatorios");
        }

        public void Processar()
        {
            var destino = new PdfDocument();

            foreach(var arquivo in NomeArquivos)
            {
                using (var origem = PdfReader.Open(Path.Combine(CaminhoBase, arquivo), PdfDocumentOpenMode.Import))
                {
                    var totalPagina = origem.PageCount - 1;
                    for (var pagina = 0; pagina <= totalPagina; pagina++)
                    {
                        destino.AddPage(origem.Pages[pagina]);
                    }
                }
            }

            destino.Save(Path.Combine(CaminhoBase, NomePdfUnificado));
        }
    }
}
