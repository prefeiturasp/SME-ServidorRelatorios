using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;
using System;
using System.Collections.Generic;
using System.IO;

namespace SME.SR.HtmlPdf
{
    public class UnificadorPdf
    {
        private string NomePdfUnificado { get; set; }
        private List<string> DiretorioCaminhoArquivosCompleto {  get; set; }
        private string CaminhoBase { get; set; }

        public UnificadorPdf(string nomePdfUnificado, List<string> diretorioCaminhoArquivosCompleto)
        {
            NomePdfUnificado = nomePdfUnificado;
            DiretorioCaminhoArquivosCompleto = diretorioCaminhoArquivosCompleto;

            CaminhoBase = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "relatorios");
        }

        public void Processar()
        {
            var destino = new PdfDocument();

            foreach (var diretorioArquivo in DiretorioCaminhoArquivosCompleto)
            {
                using (var origem = PdfReader.Open(diretorioArquivo, PdfDocumentOpenMode.Import))
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
