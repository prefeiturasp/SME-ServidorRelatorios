using ClosedXML.Excel;

namespace SME.SR.Infra.Extensions.Codaf
{
    public static class ExcelLayoutExtensions
    {
        private const double FatorCmParaExcel = 4.85; // Ajustado para Arial 11
        private static readonly XLColor CorFundoPadrao = XLColor.FromHtml("#F2F2F2");

        public static void DefinirLarguraCm(this IXLColumn coluna, double cm)
        {
            coluna.Width = cm * FatorCmParaExcel;
        }

        public static void ConfigurarEstiloPadrao(this IXLWorkbook workbook)
        {
            workbook.Style.Font.FontName = "Arial";
            workbook.Style.Font.FontSize = 11;
        }

        public static void EstilizarLabel(this IXLRange range, bool negrito = true,
            XLAlignmentHorizontalValues alinhamento = XLAlignmentHorizontalValues.Right)
        {
            range.Style.Font.Bold = negrito;
            range.Style.Alignment.Horizontal = alinhamento;
            range.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        }

        public static void EstilizarValor(this IXLRange range, XLBorderStyleValues bordaDireita, bool centralizar = false)
        {
            range.Style.Font.Bold = true;
            range.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            range.Style.Alignment.Horizontal = centralizar ? XLAlignmentHorizontalValues.Center : XLAlignmentHorizontalValues.Left;
            range.Style.Border.RightBorder = bordaDireita;
            range.Style.Border.TopBorder = XLBorderStyleValues.Thin;
            range.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
        }

        public static void EstilizarValor(this IXLCell cell, XLBorderStyleValues bordaDireita, bool centralizar = false)
        {
            cell.Style.Font.Bold = true;
            cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            cell.Style.Alignment.Horizontal = centralizar ? XLAlignmentHorizontalValues.Center : XLAlignmentHorizontalValues.Left;
            cell.Style.Border.RightBorder = bordaDireita;
            cell.Style.Border.TopBorder = XLBorderStyleValues.Thin;
            cell.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
        }

        public static void ConfigurarLabelComFundo(this IXLRange range, string texto, bool negrito = false)
        {
            range.Value = texto.ToUpper();
            range.Style.Fill.BackgroundColor = CorFundoPadrao;
            range.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            range.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            range.Style.Font.Bold = negrito;
        }

        public static void ConfigurarLabelComFundo(this IXLCell cell, string texto, bool negrito = false)
        {
            cell.Value = texto.ToUpper();
            cell.Style.Fill.BackgroundColor = CorFundoPadrao;
            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            cell.Style.Font.Bold = negrito;
        }

        public static void ConfigurarCheckbox(this IXLCell cell, bool marcado)
        {
            cell.Value = marcado ? "( X )" : "(   )";
            cell.Style.Font.Bold = true;
            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        }
        public static void ConfigurarCelula(this IXLWorksheet sheet, int linha, string col, string valor, bool alinharDireita = false)
        {
            var cell = sheet.Cell(linha, col);
            cell.Value = valor;
            cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            cell.Style.Alignment.Horizontal = alinharDireita ? XLAlignmentHorizontalValues.Right : XLAlignmentHorizontalValues.Left;
        }

        public static IXLRange ObterRange(this IXLWorksheet sheet, string cols, int linha, bool merge = true)
        {
            var rangeString = ObterRangeString(cols, linha);
            var range = sheet.Range(rangeString);
            if (merge && cols.Contains(':')) range.Merge();
            return range;
        }

        public static string FormatarValorOuMascarar(this int valor)
        {
            return valor == 0 ? "***" : valor.ToString();
        }

        public static string FormatarValorOuMascarar(this string valor)
        {
            return string.IsNullOrWhiteSpace(valor) ? "***" : valor;
        }

        private static string ObterRangeString(string cols, int linha)
        {
            if (cols.Contains(':'))
            {
                var partes = cols.Split(':');
                return $"{partes[0]}{linha}:{partes[1]}{linha}";
            }
            return $"{cols}{linha}";
        }
    }
}
