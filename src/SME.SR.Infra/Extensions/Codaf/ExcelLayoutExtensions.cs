using DocumentFormat.OpenXml.Spreadsheet;
using Nest;
using System;
using System.Collections.Generic;
using System.Text;
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

        public static void EstilizarValor(this IXLRange range, bool negrito = true, bool bordaDireita = false)
        {
            range.Style.Font.Bold = negrito;
            range.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            range.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            if (bordaDireita)
                range.Style.Border.RightBorder = XLBorderStyleValues.Thin;
        }

        public static void EstilizarValor(this IXLCell cell, bool negrito = true, bool bordaDireita = false)
        {
            cell.Style.Font.Bold = negrito;
            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            if (bordaDireita)
                cell.Style.Border.RightBorder = XLBorderStyleValues.Thin;
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
    }
}
