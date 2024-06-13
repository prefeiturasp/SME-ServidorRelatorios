using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using SME.SR.Infra.Word;
using System;
using System.Collections.Generic;

namespace SME.SR.Infra.Extensions
{
    public static class WordExtensions
    {

        public static void AdicionarParagrafo(this Body corpo, Paragraph paragrafo)
        {
            corpo.AppendChild(paragrafo);
        }

        public static void InserirLink(this Paragraph paragrafo, string url, string texto, MainDocumentPart mainPart)
        {
            if (Uri.IsWellFormedUriString(url, UriKind.Absolute))
                paragrafo.Append(ObterHyperLink(url, texto, mainPart));
            else
                InserirTexto(paragrafo, url);
        }

        public static void InserirTexto(this Paragraph paragrafo, string texto, bool novaLinha = false)
        {
            Run run = new Run();
            Text text = new Text(texto) { Space = SpaceProcessingModeValues.Preserve };
            run.Append(text);
            if (novaLinha) 
                run.Append(new Break());
            paragrafo.Append(run);
        }

        public static void InserirTexto(this Paragraph paragrafo, string texto, PropriedadesWord propriedade)
        {
            var funcoesPropriedade = new List<Func<PropriedadesWord, RunProperties>>()
            {
                ObterFonts, ObterSize, ObterCor, ObterNegrito, ObterItalico
            };

            Run run = new Run();

            foreach (var funcao in funcoesPropriedade)
            {
                var runPropriedade = funcao(propriedade);

                if (runPropriedade != null)
                    run.PrependChild(runPropriedade);
            }

            Text text = new Text(texto) { Space = SpaceProcessingModeValues.Preserve };
            run.Append(text);
            paragrafo.Append(run);
        }

        public static void InserirTextoCor(this Paragraph paragrafo, string texto, string cor)
        {
            Run run = new Run();
            RunProperties propriedade = new RunProperties();
            propriedade.Color = new Color() { Val = cor };
            run.Append(propriedade);
            Text text = new Text(texto) { Space = SpaceProcessingModeValues.Preserve };
            run.Append(text);
            paragrafo.Append(run);
        }

        public static void InserirTextoNegrito(this Paragraph paragrafo, string texto, bool novaLinha = false)
        {
            Run run = new Run();
            RunProperties propriedade = new RunProperties();
            propriedade.Bold = new Bold();
            run.Append(propriedade);
            Text text = new Text(texto) { Space = SpaceProcessingModeValues.Preserve };
            run.Append(text);
            if (novaLinha)
                run.Append(new Break());
            paragrafo.Append(run);
        }

        private static Hyperlink ObterHyperLink(string url, string text, MainDocumentPart mainPart)
        {
            HyperlinkRelationship hr = mainPart.AddHyperlinkRelationship(new Uri(url), true);
            string hrContactId = hr.Id;
            return
                new Hyperlink(
                    new ProofError() { Type = ProofingErrorValues.GrammarStart },
                    new Run(
                        new RunProperties(
                            new RunStyle() { Val = "Hyperlink" },
                            new Color { ThemeColor = ThemeColorValues.Hyperlink }),
                        new Text(text) { Space = SpaceProcessingModeValues.Preserve }
                    ))
                { History = OnOffValue.FromBoolean(true), Id = hrContactId };
        }

        private static RunProperties ObterFonts(PropriedadesWord propriedade)
        {
            if (string.IsNullOrEmpty(propriedade.Fonte))
                return null;

            var runPropriedade = new RunProperties();

            runPropriedade.Append(new RunFonts { Ascii = propriedade.Fonte });

            return runPropriedade;
        }

        private static RunProperties ObterSize(PropriedadesWord propriedade)
        {
            if (string.IsNullOrEmpty(propriedade.TamanhoFonte))
                return null;

            var runPropriedade = new RunProperties();

            runPropriedade.Append(new FontSize { Val = new StringValue(propriedade.TamanhoFonte) });

            return runPropriedade;
        }

        private static RunProperties ObterCor(PropriedadesWord propriedade)
        {
            if (string.IsNullOrEmpty(propriedade.Cor))
                return null;

            var runPropriedade = new RunProperties();
            runPropriedade.Color = new Color() { Val = propriedade.Cor };

            return runPropriedade;
        }

        private static RunProperties ObterNegrito(PropriedadesWord propriedade)
        {
            if (!propriedade.Negrito)
                return null;

            var runPropriedade = new RunProperties();
            runPropriedade.Bold = new Bold();

            return runPropriedade;
        }

        private static RunProperties ObterItalico(PropriedadesWord propriedade)
        {
            if (!propriedade.Italico)
                return null;

            var runPropriedade = new RunProperties();
            runPropriedade.Italic = new Italic();

            return runPropriedade;
        }
    }
}
