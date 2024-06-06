using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.IO;

namespace SME.SR.Infra.Word
{
    public abstract class AbstractGeradorArquivoDoc 
    {
        protected AbstractGeradorArquivoDoc()
        {
        }

        protected abstract void CriarContexto(Body corpo, MainDocumentPart mainPart);

        protected void GerarArquivoDoc(string nomedoc)
        {
            var nomeArquivo = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "relatorios", nomedoc);
            nomeArquivo = String.Format("{0}.doc", nomeArquivo);

            using (WordprocessingDocument wordDocument = WordprocessingDocument.Create(nomeArquivo, WordprocessingDocumentType.Document, true))
            {
                MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();
                mainPart.Document = new Document();
                Body docBody = mainPart.Document.AppendChild(new Body());

                CriarContexto(docBody, mainPart);

                mainPart.Document.Save();
            }
        }
    }
}
