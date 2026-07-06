using ClosedXML.Excel;
using SME.SR.Infra.Excel.Codaf.Gerador.Interfaces;
using SME.SR.Infra.Excel.CodafSuplementar.Gerador.Interfaces;

namespace SME.SR.Infra.Excel.CodafSuplementar.Gerador
{
    public class BlocoAssinaturaGeradorSuplementar : IBlocoAssinaturaGerador, IBlocoAssinaturaGeradorSuplementar
    {
        public int Processar(IXLWorksheet sheet, int linhaInicial, object dados)
        {
            var linha = linhaInicial;
            linha++; // Espaçamento superior

            RenderizarBoxAssinatura(sheet, linha, 2, "Responsável da Área Promotora pela documentação"); // Coluna B (2)
            RenderizarBoxAssinatura(sheet, linha, 12, "Responsável da Área Promotora por conferir a documentação"); // Coluna L (12)

            // Altura do espaço da assinatura
            sheet.Row(linha + 3).Height = 70;

            return linha + 4; // Retorna linha após o bloco de assinatura
        }

        private static void RenderizarBoxAssinatura(IXLWorksheet sheet, int linha, int colInicial, string titulo)
        {
            int colFinal = colInicial + 8; // Ocupa 9 colunas

            // 1. Título do Box
            var rangeTitulo = sheet.Range(linha, colInicial, linha, colFinal);
            rangeTitulo.Merge();
            rangeTitulo.Value = titulo;
            rangeTitulo.Style.Font.Bold = true;
            rangeTitulo.Style.Font.FontSize = 9;
            rangeTitulo.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rangeTitulo.Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
            rangeTitulo.Style.Border.BottomBorder = XLBorderStyleValues.Thin;

            // 2. Label Nome/RF
            var rangeLabel = sheet.Range(linha + 1, colInicial, linha + 1, colFinal);
            rangeLabel.Merge();
            rangeLabel.Value = "NOME/RF DO RESPONSÁVEL:";
            rangeLabel.Style.Font.Bold = true;
            rangeLabel.Style.Font.FontSize = 9;
            rangeLabel.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            rangeLabel.Style.Border.BottomBorder = XLBorderStyleValues.Thin;

            // 3. Área de Assinatura
            var rangeAssinatura = sheet.Range(linha + 2, colInicial, linha + 3, colFinal);
            rangeAssinatura.Merge();
            rangeAssinatura.Value = "ASSINATURA / CARIMBO:";
            rangeAssinatura.Style.Font.FontSize = 9;
            rangeAssinatura.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            rangeAssinatura.Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
            rangeAssinatura.Style.Border.BottomBorder = XLBorderStyleValues.Thin;

            // Borda Externa do Box Completo
            var boxCompleto = sheet.Range(linha, colInicial, linha + 3, colFinal);
            boxCompleto.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        }
    }
}
