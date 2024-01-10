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
                "46b0a450-0836-4130-8cb2-0d55ad0d2142.pdf", 
                "649a5efd-cd4e-44eb-938a-64a397335c8f.pdf",
                "2448ac8f-c242-4376-b12e-301f9b3be0cc.pdf",
                "Boleto TRT Danyllo.pdf",
                "Padroes_de_Projetos_-_Solucoes_Reutiliza.pdf"};

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
