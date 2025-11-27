using ClosedXML.Excel;
using SME.SR.HtmlPdf;
using SME.SR.Infra.CDEP;
using SME.SR.Infra.Dtos.Relatorios.CDEP;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;


namespace SME.SR.Application.Commands.CDEP
{
    public abstract class GerarRelatorioControleLivrosBase
    {
        protected static Stream ObterLogo()
        {
            string base64Logo = SmeConstants.LogoSmeMono.Substring(SmeConstants.LogoSmeMono.IndexOf(',') + 1);
            return new MemoryStream(Convert.FromBase64String(base64Logo));
        }

        protected static async Task SaveMemoryStreamToFile(MemoryStream memoryStream, string filePath)
        {
            memoryStream.Position = 0;

            using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                await memoryStream.CopyToAsync(fileStream);
            }
        }

        protected static string ObterDescricaoSituacao(SituacaoEmprestimo situacao)
        {
            var fieldInfo = situacao.GetType().GetField(situacao.ToString());
            var descriptionAttribute = (DisplayAttribute)Attribute.GetCustomAttribute(fieldInfo, typeof(DisplayAttribute));
            return descriptionAttribute?.Description ?? situacao.ToString();
        }

        protected static string ObterTipoAcervo(TipoAcervo tipoAcervo)
        {
            var fieldInfo = tipoAcervo.GetType().GetField(tipoAcervo.ToString());
            var descriptionAttribute = (DisplayAttribute)Attribute.GetCustomAttribute(fieldInfo, typeof(DisplayAttribute));
            return descriptionAttribute?.Description ?? tipoAcervo.ToString();
        }

        protected static void InserirCabecalhosPadrao(ref int row, ref IXLWorksheet sheet, string tituloRelatorio, FiltroRelatorioCdepBase filtroRelatorioCdepBase)
        {
            var image = sheet.AddPicture(ObterLogo())
                .MoveTo(sheet.Cell(row, 1))
                .WithSize(100, 60);
            // Título institucional
            sheet.Range(row, 2, row, 5).Merge();
            sheet.Cell(row, 2).Value = "CDEP - CENTRO DE DOCUMENTAÇÃO DA EDUCAÇÃO PAULISTANA";
            sheet.Cell(row, 2).Style.Font.Bold = true;
            sheet.Cell(row, 2).Style.Font.FontSize = 14;
            sheet.Cell(row, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            sheet.Row(row).Height = 25;
            row++;
            // Título do relatório
            sheet.Range(row, 2, row, 5).Merge();
            sheet.Cell(row, 2).Value = tituloRelatorio;
            sheet.Cell(row, 2).Style.Font.Bold = true;
            sheet.Cell(row, 2).Style.Font.FontSize = 12;
            sheet.Cell(row, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            sheet.Row(row).Height = 20;
            row += 2;
            // Informações do usuário
            sheet.Cell(row, 1).Value = "Usuário:";
            sheet.Cell(row, 2).Value = filtroRelatorioCdepBase.Usuario;
            sheet.Cell(row, 3).Value = $"RF: {filtroRelatorioCdepBase.UsuarioRF}";
            sheet.Cell(row, 6).Value = $"Data: {DateTime.Now:dd/MM/yyyy}";
            row += 2;
        }
    }
}
