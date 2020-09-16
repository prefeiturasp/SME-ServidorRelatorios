using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using MediatR;
using Sentry;
using SME.SR.HtmlPdf;
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

        private const int LINHA_CABECALHO_DRE = 6;
        private const int LINHA_CABECALHO_CICLO = 7;

        private const int LINHA_GRUPOS = 9;
        private const int LINHA_COMPONENTES = 10;

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

                    worksheet.ShowGridLines = false;

                    var objetoExportacao = request.ObjetoExportacao.FirstOrDefault();

                    MontarCabecalho(worksheet, objetoExportacao.Cabecalho, request.TabelaDados.Columns.Count);

                    worksheet.Cell(LINHA_GRUPOS, 1).InsertData(request.TabelaDados);

                    MergearTabela(worksheet, request.TabelaDados);

                    AdicionarEstilo(worksheet, request.TabelaDados);

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

        private void AdicionarEstilo(IXLWorksheet worksheet, DataTable tabelaDados)
        {
            int ultimaColunaUsada = worksheet.LastColumnUsed().ColumnNumber();
            int ultimaLinhaUsada = worksheet.LastRowUsed().RowNumber();

            AdicionarEstiloCabecalho(worksheet, ultimaColunaUsada);
            AdicionarEstiloCorpo(worksheet, tabelaDados, ultimaColunaUsada, ultimaLinhaUsada);
        }

        private void AdicionarEstiloCorpo(IXLWorksheet worksheet, DataTable tabelaDados, int ultimaColunaUsada, int ultimaLinhaUsada)
        {
            worksheet.Range(LINHA_GRUPOS, 1, ultimaLinhaUsada, ultimaColunaUsada).Style.Border.TopBorder = XLBorderStyleValues.Thin;
            worksheet.Range(LINHA_GRUPOS, 1, ultimaLinhaUsada, ultimaColunaUsada).Style.Border.TopBorderColor = XLColor.Black;

            worksheet.Range(LINHA_GRUPOS, 1, ultimaLinhaUsada, ultimaColunaUsada).Style.Border.RightBorder = XLBorderStyleValues.Thin;
            worksheet.Range(LINHA_GRUPOS, 1, ultimaLinhaUsada, ultimaColunaUsada).Style.Border.RightBorderColor = XLColor.Black;

            worksheet.Range(LINHA_GRUPOS, 1, ultimaLinhaUsada, ultimaColunaUsada).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            worksheet.Range(LINHA_GRUPOS, 1, ultimaLinhaUsada, ultimaColunaUsada).Style.Border.BottomBorderColor = XLColor.Black;

            worksheet.Range(LINHA_GRUPOS, 1, ultimaLinhaUsada, ultimaColunaUsada).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            worksheet.Range(LINHA_GRUPOS, 1, ultimaLinhaUsada, ultimaColunaUsada).Style.Border.LeftBorderColor = XLColor.Black;

            worksheet.Range(LINHA_GRUPOS, 1, ultimaLinhaUsada, ultimaColunaUsada).Style.Font.Bold = true;

            worksheet.Rows(LINHA_GRUPOS, LINHA_COMPONENTES).Style.Fill.BackgroundColor = XLColor.LightGray;

            var linhaInicialInativos = tabelaDados.AsEnumerable()
              .Where(r => r.Field<string>("NumeroChamada") == "0").FirstOrDefault();

            var indiceLinhaInativos = tabelaDados.Rows.IndexOf(linhaInicialInativos);

            worksheet.Rows(LINHA_GRUPOS + indiceLinhaInativos, ultimaLinhaUsada).Style.Fill.BackgroundColor = XLColor.LightGray;

        }

        private void AdicionarEstiloCabecalho(IXLWorksheet worksheet, int ultimaColunaUsada)
        {
            worksheet.Range(LINHA_CABECALHO_DRE, 1, LINHA_CABECALHO_CICLO, ultimaColunaUsada).Style.Border.TopBorder = XLBorderStyleValues.Thin;
            worksheet.Range(LINHA_CABECALHO_DRE, 1, LINHA_CABECALHO_CICLO, ultimaColunaUsada).Style.Border.TopBorderColor = XLColor.Black;

            worksheet.Range(LINHA_CABECALHO_DRE, 1, LINHA_CABECALHO_CICLO, ultimaColunaUsada).Style.Border.RightBorder = XLBorderStyleValues.Thin;
            worksheet.Range(LINHA_CABECALHO_DRE, 1, LINHA_CABECALHO_CICLO, ultimaColunaUsada).Style.Border.RightBorderColor = XLColor.Black;

            worksheet.Range(LINHA_CABECALHO_DRE, 1, LINHA_CABECALHO_CICLO, ultimaColunaUsada).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            worksheet.Range(LINHA_CABECALHO_DRE, 1, LINHA_CABECALHO_CICLO, ultimaColunaUsada).Style.Border.BottomBorderColor = XLColor.Black;

            worksheet.Range(LINHA_CABECALHO_DRE, 1, LINHA_CABECALHO_CICLO, ultimaColunaUsada).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            worksheet.Range(LINHA_CABECALHO_DRE, 1, LINHA_CABECALHO_CICLO, ultimaColunaUsada).Style.Border.LeftBorderColor = XLColor.Black;

            worksheet.Range(LINHA_CABECALHO_DRE, 1, LINHA_CABECALHO_CICLO, ultimaColunaUsada).Style.Font.Bold = true;
        }

        private void MergearTabela(IXLWorksheet worksheet, DataTable tabelaDados)
        {
            worksheet.Range(LINHA_GRUPOS, 1, LINHA_COMPONENTES, 2).Merge();

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
                    worksheet.Range(LINHA_GRUPOS, indiceInicial, LINHA_COMPONENTES, indiceInicial + contagemCelulas).Merge();
                }
                else
                {
                    worksheet.Range(LINHA_GRUPOS, indiceInicial, LINHA_GRUPOS, indiceInicial + contagemCelulas).Merge();

                    var componentes = listaTitulos.Where(t => t.StartsWith(grupo.Key)).GroupBy(g => g.Substring(g.IndexOf("Componente"), g.LastIndexOf('_')));

                    foreach (var componente in componentes)
                    {
                        itemInicial = componente.FirstOrDefault();

                        indiceInicial = tabelaDados.Columns[itemInicial].Ordinal + 1;

                        contagemCelulas = componente.Count() - 1;

                        worksheet.Range(LINHA_COMPONENTES, indiceInicial, LINHA_COMPONENTES, indiceInicial + contagemCelulas).Merge();
                    }
                }
            }
        }

        private void MontarCabecalho(IXLWorksheet worksheet, ConselhoClasseAtaFinalCabecalhoDto dadosCabecalho, int totalColunas)
        {
            worksheet.AddPicture(ObterLogo())
                .MoveTo(worksheet.Cell(1, 1))
                .Scale(0.22);

            worksheet.Row(2).Cell(totalColunas - 4).Value = "SGP - Sistema de Gestão Pedagógica";
            worksheet.Range(2, totalColunas - 4, 2, totalColunas).Merge().Style.Font.Bold = true;
            worksheet.Row(3).Cell(totalColunas - 4).Value = "ATA FINAL DE RESULTADOS";
            worksheet.Range(3, totalColunas - 4, 3, totalColunas).Merge();

            int indiceFinal = SetarItemCabecalho(worksheet, $"DRE: {dadosCabecalho.Dre}", 0.4, LINHA_CABECALHO_DRE, 1, totalColunas);
            indiceFinal = SetarItemCabecalho(worksheet, $"Unidade Escolar (UE): {dadosCabecalho.Ue}", 0.4, LINHA_CABECALHO_DRE, indiceFinal, totalColunas);
            SetarItemCabecalho(worksheet, $"Turma: {dadosCabecalho.Turma}", 0.2, LINHA_CABECALHO_DRE, indiceFinal, totalColunas);

            indiceFinal = SetarItemCabecalho(worksheet, $"Ciclo: {dadosCabecalho.Ciclo}", 0.6, LINHA_CABECALHO_CICLO, 1, totalColunas);
            indiceFinal = SetarItemCabecalho(worksheet, $"Ano Letivo: {dadosCabecalho.AnoLetivo}", 0.2, LINHA_CABECALHO_CICLO, indiceFinal, totalColunas);
            SetarItemCabecalho(worksheet, $"Data: {dadosCabecalho.Data}", 0.2, LINHA_CABECALHO_CICLO, indiceFinal, totalColunas);
        }

        public Stream ObterLogo()
        {
            string base64Logo = SmeConstants.LogoSmeMono.Substring(SmeConstants.LogoSmeMono.IndexOf(',') + 1);
            return new MemoryStream(Convert.FromBase64String(base64Logo));
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
