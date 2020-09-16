using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using MediatR;
using Sentry;
using SME.SR.Infra;
using SME.SR.Infra.Utilitarios;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class GerarRelatorioAtaFinalExcelCommandHandler : IRequestHandler<GerarRelatorioAtaFinalExcelCommand, Unit>
    {
        private readonly IServicoFila servicoFila;

        public GerarRelatorioAtaFinalExcelCommandHandler(IServicoFila servicoFila)
        {
            this.servicoFila = servicoFila ?? throw new ArgumentNullException(nameof(servicoFila));
        }

        public async Task<Unit> Handle(GerarRelatorioAtaFinalExcelCommand request, CancellationToken cancellationToken)
        {
            try
            {
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add(request.NomeWorkSheet);

                    if (!request.ObjetoExportacao.Any())
                        throw new NegocioException("Não foi possível localizar o objeto de consulta.");

                    //worksheet.SheetView.SetView(XLSheetViewOptions.PageLayout);

                    worksheet.PageSetup.Margins.SetTop(1);
                    worksheet.ShowGridLines = false;

                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine($"&B&10 SGP - Sistema de Gestão Pedagógica");
                    sb.AppendLine("ATA FINAL DE RESULTADOS");

                    //ws.PageSetup.Header.Left .AddImage(@"D:\SGP\SME-ServidorRelatorios\src\SME.SR.Reports\Sgp\RelatorioConselhoClasse\logo.png", XLHFOccurrence.AllPages);
                    worksheet.PageSetup.Header.Right.AddText(sb.ToString());
                    worksheet.PageSetup.Footer.Center.AddText(XLHFPredefinedText.PageNumber, XLHFOccurrence.AllPages);

                    var objetoExportacao = request.ObjetoExportacao.FirstOrDefault();

                    MontarCabecalho(worksheet, objetoExportacao.Cabecalho, request.TabelaDados.Columns.Count);

                    worksheet.Cell(9, 1).InsertData(request.TabelaDados);

                    MergearTabela(worksheet, request.TabelaDados);

                    AdicionarEstilo(worksheet);

                    worksheet.ColumnsUsed().AdjustToContents();
                    worksheet.RowsUsed().AdjustToContents();

                    var caminhoBase = AppDomain.CurrentDomain.BaseDirectory;
                    var caminhoParaSalvar = Path.Combine(caminhoBase, $"relatorios", request.CodigoCorrelacao.ToString());

                    workbook.SaveAs($"{caminhoParaSalvar}.xlsx");
                }

                servicoFila.PublicaFila(new PublicaFilaDto(new MensagemRelatorioProntoDto(), RotasRabbit.FilaSgp, RotasRabbit.RotaRelatoriosProntosSgp, null, request.CodigoCorrelacao));

                return await Task.FromResult(Unit.Value);
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw ex;
            }
        }

        private void AdicionarEstilo(IXLWorksheet worksheet)
        {
            int ultimaColunaUsada = worksheet.LastColumnUsed().ColumnNumber();
            int ultimaLinhaUsada = worksheet.LastRowUsed().RowNumber();

            worksheet.Range(6, 1, 7, ultimaColunaUsada).Style.Border.TopBorder = XLBorderStyleValues.Thin;
            worksheet.Range(6, 1, 7, ultimaColunaUsada).Style.Border.TopBorderColor = XLColor.Black;

            worksheet.Range(6, 1, 7, ultimaColunaUsada).Style.Border.RightBorder = XLBorderStyleValues.Thin;
            worksheet.Range(6, 1, 7, ultimaColunaUsada).Style.Border.RightBorderColor = XLColor.Black;

            worksheet.Range(6, 1, 7, ultimaColunaUsada).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            worksheet.Range(6, 1, 7, ultimaColunaUsada).Style.Border.BottomBorderColor = XLColor.Black;

            worksheet.Range(6, 1, 7, ultimaColunaUsada).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            worksheet.Range(6, 1, 7, ultimaColunaUsada).Style.Border.LeftBorderColor = XLColor.Black;

            worksheet.Range(6, 1, 7, ultimaColunaUsada).Style.Font.Bold = true;

            worksheet.Range(9, 1, ultimaLinhaUsada, ultimaColunaUsada).Style.Border.TopBorder = XLBorderStyleValues.Thin;
            worksheet.Range(9, 1, ultimaLinhaUsada, ultimaColunaUsada).Style.Border.TopBorderColor = XLColor.Black;

            worksheet.Range(9, 1, ultimaLinhaUsada, ultimaColunaUsada).Style.Border.RightBorder = XLBorderStyleValues.Thin;
            worksheet.Range(9, 1, ultimaLinhaUsada, ultimaColunaUsada).Style.Border.RightBorderColor = XLColor.Black;

            worksheet.Range(9, 1, ultimaLinhaUsada, ultimaColunaUsada).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            worksheet.Range(9, 1, ultimaLinhaUsada, ultimaColunaUsada).Style.Border.BottomBorderColor = XLColor.Black;

            worksheet.Range(9, 1, ultimaLinhaUsada, ultimaColunaUsada).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            worksheet.Range(9, 1, ultimaLinhaUsada, ultimaColunaUsada).Style.Border.LeftBorderColor = XLColor.Black;

            worksheet.Range(9, 1, ultimaLinhaUsada, ultimaColunaUsada).Style.Font.Bold = true;

            worksheet.Rows(9, 10).Style.Fill.BackgroundColor = XLColor.LightGray;
        }

        private void MergearTabela(IXLWorksheet worksheet, DataTable tabelaDados)
        {
            worksheet.Range(4, 1, 5, 2).Merge();

            var listaTitulos = tabelaDados.Columns.Cast<DataColumn>()
                                  .Select(c => c.ColumnName).Where(t => t.StartsWith("Grupo"));

            var agrupamento = listaTitulos.GroupBy(g => g.Substring(0, g.IndexOf('_')));

            foreach (var grupo in agrupamento)
            {
                var itemInicial = grupo.FirstOrDefault();

                var indiceInicial = tabelaDados.Columns[itemInicial].Ordinal + 1;

                var contagemCelulas = grupo.Count() - 1;

                if (grupo.Key.Equals("Grupo99"))
                {
                    worksheet.Range(4, indiceInicial, 5, indiceInicial + contagemCelulas).Merge();
                }
                else
                {
                    worksheet.Range(4, indiceInicial, 4, indiceInicial + contagemCelulas).Merge();

                    var componentes = listaTitulos.Where(t => t.StartsWith(grupo.Key)).GroupBy(g => g.Substring(g.IndexOf("Componente"), g.LastIndexOf('_')));

                    foreach (var componente in componentes)
                    {
                        itemInicial = componente.FirstOrDefault();

                        indiceInicial = tabelaDados.Columns[itemInicial].Ordinal + 1;

                        contagemCelulas = componente.Count() - 1;

                        worksheet.Range(5, indiceInicial, 5, indiceInicial + contagemCelulas).Merge();
                    }
                }
            }
        }

        private void MontarCabecalho(IXLWorksheet worksheet, ConselhoClasseAtaFinalCabecalhoDto dadosCabecalho, int totalColunas)
        {
            var imagePath = @"c:\path\to\your\image.jpg";

            var image = worksheet.AddPicture(imagePath)
                .MoveTo((IXLCell)worksheet.Cell(1,1).Address)
                .Scale(0.5); // optional: resize picture

            int indiceFinal = SetarItemCabecalho(worksheet, $"DRE: {dadosCabecalho.Dre}", 0.4, 6, 1, totalColunas);
            indiceFinal = SetarItemCabecalho(worksheet, $"Unidade Escolar (UE): {dadosCabecalho.Ue}", 0.4, 6, indiceFinal, totalColunas);
            SetarItemCabecalho(worksheet, $"Turma: {dadosCabecalho.Turma}", 0.2, 6, indiceFinal, totalColunas);

            indiceFinal = SetarItemCabecalho(worksheet, $"Ciclo: {dadosCabecalho.Ciclo}", 0.6, 7, 1, totalColunas);
            indiceFinal = SetarItemCabecalho(worksheet, $"Ano Letivo: {dadosCabecalho.AnoLetivo}", 0.2, 7, indiceFinal, totalColunas);
            SetarItemCabecalho(worksheet, $"Data: {dadosCabecalho.Data}", 0.2, 7, indiceFinal, totalColunas);
        }

        private int SetarItemCabecalho(IXLWorksheet worksheet, string valor, double fator, int linha, int celulaInicial, int totalColunas)
        {
            int range = Convert.ToInt32(totalColunas * fator);
            worksheet.Cell(linha, celulaInicial).Value = valor;

            int final = (celulaInicial + range);
            worksheet.Range(linha, celulaInicial, linha, final - 1).Merge();

            return final;
        }
    }
}
