using ClosedXML.Excel;
using ClosedXML.Excel.Drawings;
using SME.SR.Infra.Excel.Codaf.Gerador.Interfaces;
using SME.SR.Infra.Excel.CodafSuplementar.Gerador.Interfaces;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SME.SR.Infra.Excel.CodafSuplementar.Gerador
{
    public class BlocoTituloGeradorSuplementar : IBlocoTituloGerador, IBlocoTituloGeradorSuplementar
    {
        private readonly ConcurrentDictionary<string, string> _cacheImagensBase64 = new ConcurrentDictionary<string, string>();
        private readonly Assembly _assembly = Assembly.GetExecutingAssembly();
        public int Processar(IXLWorksheet sheet, int linhaInicial, object dados)
        {
            // Lógica de Imagem (refatorada na resposta anterior)
            RenderizarBrasao(sheet);

            // Títulos Fixos
            CriarLinhaTitulo(sheet, 3, "SECRETARIA MUNICIPAL DE EDUCAÇÃO - SME");
            CriarLinhaTitulo(sheet, 4, "CONTROLE DE DOCUMENTAÇÃO DAS AÇÕES FORMATIVAS - CODAF SUPLEMENTAR");
            CriarLinhaTitulo(sheet, 5, "RELATÓRIO DE CONCLUSÃO DE TURMA - MODELO 2026 - REDE DIRETA");

            return 6; // Próxima linha é a 6
        }
        private void RenderizarBrasao(IXLWorksheet sheet)
        {
            // Ajuste crucial de altura para o logo não ficar minúsculo
            sheet.Rows(2, 5).Height = 25;

            var rangeImagem = sheet.Range("A2:B5");
            rangeImagem.Merge();

            using var streamImagem = ObterBrasaoPrefeitura();
            var imagem = sheet.AddPicture(streamImagem);

            // Lógica de centralização corrigida
            const double FatorAlturaPx = 1.3333;
            const double FatorLarguraPx = 7.0;

            var alturaCanvas = sheet.Rows(2, 5).Sum(r => r.Height) * FatorAlturaPx;
            var larguraCanvas = (sheet.Columns("A:B").Sum(c => c.Width) * FatorLarguraPx) + 10;

            var ratioX = larguraCanvas / imagem.OriginalWidth;
            var ratioY = alturaCanvas / imagem.OriginalHeight;
            var escala = Math.Min(ratioX, ratioY);
            if (escala > 1) escala = 1;

            var novaLargura = (int)(imagem.OriginalWidth * escala);
            var novaAltura = (int)(imagem.OriginalHeight * escala);
            var offsetX = (int)((larguraCanvas - novaLargura) / 2);
            var offsetY = (int)((alturaCanvas - novaAltura) / 2);

            imagem.MoveTo(sheet.Cell("A2"), offsetX, offsetY);
            imagem.WithSize(novaLargura, novaAltura);
            imagem.Placement = XLPicturePlacement.MoveAndSize;
        }
        private Stream ObterBrasaoPrefeitura()
        {
            const string nomeArquivoImagem = "brasao_prefeitura_titulo_educacao.png";
            if (!_cacheImagensBase64.TryGetValue(nomeArquivoImagem, out var base64Imagem))
            {
                var resourcePath =
                    _assembly.GetManifestResourceNames().FirstOrDefault(r => r.EndsWith(nomeArquivoImagem, StringComparison.OrdinalIgnoreCase));

                using var stream = _assembly.GetManifestResourceStream(resourcePath) ?? 
                                   throw new InvalidOperationException($"Recurso incorporado '{resourcePath}' não encontrado.");
                using var memoryStream = new MemoryStream();
                stream.CopyTo(memoryStream);
                base64Imagem = Convert.ToBase64String(memoryStream.ToArray());
                _cacheImagensBase64[nomeArquivoImagem] = base64Imagem;
            }
            var imagemBytes = Convert.FromBase64String(base64Imagem);
            return new MemoryStream(imagemBytes);
        }
        private static void CriarLinhaTitulo(IXLWorksheet sheet, int linha, string texto)
        {
            var range = sheet.Range($"C{linha}:R{linha}");
            range.Merge();
            range.Value = texto;
            range.Style.Font.Bold = true;
            range.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            range.Style.Alignment.Vertical = XLAlignmentVerticalValues.Bottom;
        }
    }
}