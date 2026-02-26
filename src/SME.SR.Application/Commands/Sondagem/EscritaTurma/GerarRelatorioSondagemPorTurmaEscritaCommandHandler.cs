using ClosedXML.Excel;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using MediatR;
using Sentry;
using SME.SR.Application.Queries.ConsultaSondagemPorTurma;
using SME.SR.Application.Queries.Dre.ObterDreUeNomePorUeCodigo;
using SME.SR.Data.Models;
using SME.SR.Infra;
using SME.SR.Infra.Dtos.SondagemTurmaEscritaEF;
using SME.SR.Infra.Extensions;
using SME.SR.Infra.Utilitarios;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Index = DocumentFormat.OpenXml.Drawing.Charts.Index;
using NumberingFormat = DocumentFormat.OpenXml.Drawing.Charts.NumberingFormat;
using OrientationValues = DocumentFormat.OpenXml.Drawing.Charts.OrientationValues;
using Values = DocumentFormat.OpenXml.Drawing.Charts.Values;
using Xdr = DocumentFormat.OpenXml.Drawing.Spreadsheet;

namespace SME.SR.Application.Commands.Sondagem.EscritaTurma
{
    public class GerarRelatorioSondagemPorTurmaEscritaCommandHandler : GerarRelatorioSondagemPorTurmaBase, IRequestHandler<GerarRelatorioSondagemPorTurmaEscritaCommand, Unit>
    {
        private readonly IMediator mediator;
        private readonly IServicoFila servicoFila;

        public GerarRelatorioSondagemPorTurmaEscritaCommandHandler(IMediator mediator, IServicoFila servicoFila)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this.servicoFila = servicoFila ?? throw new ArgumentNullException(nameof(servicoFila));
        }

        public async Task<Unit> Handle(GerarRelatorioSondagemPorTurmaEscritaCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var dreUeNome = await ObterNomeUeDre(request.UeCodigo);
                var turmaNome = await ObterTurma(request.TurmaId.ToString());

                var bimestreLabel = request.BimestreId is null ? string.Empty : $"Bimestre: {request.BimestreId.Value}";
                var usuarioSolicitacao = await mediator.Send(new ObterUsuarioPorCodigoRfQuery(request.UsuarioLogadoRF));
                var dto = (await mediator.Send(new ConsultaSondagemPorTurmaQuery(request.TurmaId, request.ProficienciaId, request.ComponenteCurricularId, (int)request.Modalidade, request.Ano, request.AnoLetivo, request.Semestre, request.UeCodigo, request.BimestreId)))
                            .MapToEscritaEfTurmaSondagemCabecalhoExcelDto(
                                    request.AnoLetivo,
                                    turmaNome,
                                    dreUeNome.UeNome,
                                    dreUeNome.DreNome,
                                    request.Modalidade.Name(),
                                    usuarioSolicitacao.NomeRelatorio
                                );

                switch (request.Modalidade)
                {
                    case Modalidade.EJA:
                    case Modalidade.Fundamental:
                        GerarExcelEF(dto, request, request.CodigoCorrelacao, request.Modalidade);
                        await servicoFila.PublicaFila(new PublicaFilaDto(new MensagemRelatorioProntoDto(string.Empty, "Relatório da Sondagem"), RotasRabbitSGP.RotaRelatoriosProntosSgp, RotasRabbitSR.RotaRelatoriosSolicitadosSondagemPorTurma, request.CodigoCorrelacao));
                        break;
                }
                return await Task.FromResult(Unit.Value);
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw ex;
            }
        }

        static XLColor ConverterCor(string cor)
        {
            if (string.IsNullOrEmpty(cor))
                return XLColor.White;

            var hex = cor.TrimStart('#');
            var r = Convert.ToInt32(hex.Substring(0, 2), 16);
            var g = Convert.ToInt32(hex.Substring(2, 2), 16);
            var b = Convert.ToInt32(hex.Substring(4, 2), 16);

            return XLColor.FromArgb(r, g, b);
        }

        private List<GraficoDto> ContarOcorrencias(
            List<EscritaEfTurmaSondagemCorpoExcelDto> corpo,
            Func<EscritaEfTurmaSondagemCorpoExcelDto, string> seletor)
        {
            return corpo
                .Where(x => !string.IsNullOrEmpty(seletor(x)))
                .GroupBy(seletor)
                .Select(g => new GraficoDto
                {
                    Descricao = g.Key,
                    Quantidade = g.Count(),
                    Cor = g.First().Cor
                })
                .ToList();
        }

        private void GerarExcelEF(EscritaEfTurmaSondagemCabecalhoExcelDto dto, GerarRelatorioSondagemPorTurmaEscritaCommand request, Guid codigoCorrelacao, Modalidade modalidade)
        {
            using var workbook = new XLWorkbook();
            var sheet = workbook.AddWorksheet("Sondagem");

            var graficoCompleto = ContarOcorrencias(dto.CorpoRelatorio, x => x.SondagemInicial)
                .Concat(ContarOcorrencias(dto.CorpoRelatorio, x => x.PrimeiroBimestre))
                .Concat(ContarOcorrencias(dto.CorpoRelatorio, x => x.TerceiroBimestre))
                .Concat(ContarOcorrencias(dto.CorpoRelatorio, x => x.QuartoBimestre))
                .GroupBy(x => x.Descricao)
                .Select(g => new GraficoDto
                {
                    Descricao = g.Key,
                    Quantidade = g.Sum(x => x.Quantidade),
                    Cor = g.First().Cor
                })
                .ToList();

            sheet.Column(1).Width = 6;
            sheet.Column(2).Width = 28;
            sheet.Column(3).Width = 12;
            sheet.Column(4).Width = 12;
            sheet.Column(5).Width = 10;
            sheet.Column(6).Width = 12;
            sheet.Column(7).Width = 10;
            sheet.Column(8).Width = 10;
            if (modalidade == Modalidade.Fundamental)
            {
                sheet.Column(9).Width = 10;
                sheet.Column(10).Width = 10;
            }

            int linha = 1;

            sheet.Row(1).Height = 20;
            sheet.Row(2).Height = 20;
            sheet.Row(3).Height = 20;

            var logo = sheet.AddPicture(ObterLogo())
                             .MoveTo(sheet.Cell(1, 1))
                             .WithSize(160, 60);

            linha = 4;
            EscreverCelula(sheet, linha, 1, $"Ano letivo: {request.AnoLetivo}   DRE: {dto.Dre}   Semestre: {dto.Semestre}", bold: false);
            sheet.Range(linha, 1, linha, 7).Merge();
            EscreverCelula(sheet, linha, 8, $"Turma: {dto.Turma}", bold: false);
            sheet.Range(linha, 8, linha, 10).Merge();
            AplicarBordaExterna(sheet.Range(linha, 1, linha, 10));
            linha++;

            EscreverCelula(sheet, linha, 1, $"Unidade Educacional: {dto.Ue}", bold: false);
            sheet.Range(linha, 1, linha, 10).Merge();
            AplicarBordaExterna(sheet.Range(linha, 1, linha, 10));
            linha++;

            EscreverCelula(sheet, linha, 1, $"Modalidade: {dto.Modalidade}", bold: false);
            sheet.Range(linha, 1, linha, 3).Merge();
            EscreverCelula(sheet, linha, 4, $"Proficiência: {dto.Proeficiencia}", bold: false);
            sheet.Range(linha, 4, linha, 6).Merge();
            EscreverCelula(sheet, linha, 7, $"Data de impressão: {dto.DataImpressao}", bold: false);
            sheet.Range(linha, 7, linha, 10).Merge();
            AplicarBordaExterna(sheet.Range(linha, 1, linha, 10));
            linha++;

            EscreverCelula(sheet, linha, 1, $"Usuário: {dto.NomeUsuarioSolicitacao}", bold: false);
            sheet.Range(linha, 1, linha, 10).Merge();
            AplicarBordaExterna(sheet.Range(linha, 1, linha, 10));
            linha++;

            linha++;

            var tituloCell = sheet.Cell(linha, 1);
            tituloCell.Value = "Relatório da Sondagem";
            tituloCell.Style.Font.Bold = true;
            tituloCell.Style.Font.FontSize = 14;
            tituloCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            sheet.Range(linha, 1, linha, 10).Merge();
            linha++;

            var subtituloCell = sheet.Cell(linha, 1);
            subtituloCell.Value = dto.Proeficiencia;
            subtituloCell.Style.Font.Bold = true;
            subtituloCell.Style.Font.FontSize = 12;
            subtituloCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            sheet.Range(linha, 1, linha, 10).Merge();
            linha++;

            linha++;

            if (modalidade == Modalidade.EJA)
            {
                var grupoEscrita = sheet.Range(linha, 6, linha, 8);
                grupoEscrita.Merge();
                grupoEscrita.Value = dto.Proeficiencia;
                grupoEscrita.Style.Font.Bold = true;
                grupoEscrita.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                grupoEscrita.Style.Fill.BackgroundColor = XLColor.LightGray;
                AplicarBordaExterna(grupoEscrita);
            }
            else
            {
                var grupoEscrita = sheet.Range(linha, 6, linha, 10);
                grupoEscrita.Merge();
                grupoEscrita.Value = dto.Proeficiencia;
                grupoEscrita.Style.Font.Bold = true;
                grupoEscrita.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                grupoEscrita.Style.Fill.BackgroundColor = XLColor.LightGray;
                AplicarBordaExterna(grupoEscrita);
            }
            linha++;

            var headers = modalidade == Modalidade.EJA ? new[]
            {
                (1, "Nº"),
                (2, "Nome"),
                (3, "Raça"),
                (4, "Gênero"),
                (5, "LP como 2ª língua?"),
                (6, "Sondagem inicial"),
                (7, "1º bim"),
                (8, "2º bim"),
            } : new[]
            {
                (1, "Nº"),
                (2, "Nome"),
                (3, "Raça"),
                (4, "Gênero"),
                (5, "LP como 2ª língua?"),
                (6, "Sondagem inicial"),
                (7, "1º bim"),
                (8, "2º bim"),
                (9, "3º bim"),
                (10, "4º bim"),
            };

            foreach (var (col, texto) in headers)
            {
                var cell = sheet.Cell(linha, col);
                cell.Value = texto;
                cell.Style.Font.Bold = true;
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                cell.Style.Alignment.WrapText = true;
                cell.Style.Fill.BackgroundColor = XLColor.LightGray;
                cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            }

            sheet.Row(linha).Height = 40;
            linha++;

            foreach (var item in dto.CorpoRelatorio)
            {
                sheet.Row(linha).Height = 45;
                var corFundo = ConverterCor(item.Cor);

                var cNum = sheet.Cell(linha, 1);
                cNum.Value = item.Numero;
                EstilarCelulaDados(cNum);

                var cNome = sheet.Cell(linha, 2);
                cNome.Value = item.Nome;
                EstilarCelulaDados(cNome);

                var cRaca = sheet.Cell(linha, 3);
                cRaca.Value = item.Raca;
                EstilarCelulaDados(cRaca);

                var cGenero = sheet.Cell(linha, 4);
                cGenero.Value = item.Genero;
                EstilarCelulaDados(cGenero);

                var cLp = sheet.Cell(linha, 5);
                cLp.Value = item.LpComoLinguaPrincipal == "Sim" ? "☑" : "☐";
                cLp.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                cLp.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                cLp.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                cLp.Style.Font.FontSize = 14;

                if (modalidade == Modalidade.EJA)
                {
                    preenchererCelulaSondagem(sheet.Cell(linha, 6), item.SondagemInicial, corFundo);
                    preenchererCelulaSondagem(sheet.Cell(linha, 7), item.PrimeiroBimestre, corFundo);
                    preenchererCelulaSondagem(sheet.Cell(linha, 8), item.SegundoBimestre, corFundo);
                }

                if (modalidade == Modalidade.Fundamental)
                {
                    preenchererCelulaSondagem(sheet.Cell(linha, 6), item.SondagemInicial, corFundo);
                    preenchererCelulaSondagem(sheet.Cell(linha, 7), item.PrimeiroBimestre, corFundo);
                    preenchererCelulaSondagem(sheet.Cell(linha, 8), item.SegundoBimestre, corFundo);
                    preenchererCelulaSondagem(sheet.Cell(linha, 9), item.TerceiroBimestre, corFundo);
                    preenchererCelulaSondagem(sheet.Cell(linha, 10), item.QuartoBimestre, corFundo);
                }

                linha++;
            }

            int linhaGrafico = linha + 6;

            var caminhoBase = AppDomain.CurrentDomain.BaseDirectory;
            var caminhoParaSalvar = Path.Combine(caminhoBase, $"relatorios", codigoCorrelacao.ToString());
            workbook.SaveAs($"{caminhoParaSalvar}.xlsx");

            InjetarGraficoOpenXml($"{caminhoParaSalvar}.xlsx", graficoCompleto, dto.Proeficiencia, linhaGrafico);
        }

        private void InjetarGraficoOpenXml(string caminhoArquivo, List<GraficoDto> dados, string tituloProficiencia, int linhaGrafico)
        {
            using var document = SpreadsheetDocument.Open(caminhoArquivo, true);
            var workbookPart = document.WorkbookPart;

            var sheetInfo = workbookPart.Workbook.Sheets.Elements<Sheet>().First();
            var worksheetPart = (WorksheetPart)workbookPart.GetPartById(sheetInfo.Id);

            DrawingsPart drawingsPart;
            if (worksheetPart.DrawingsPart != null)
                drawingsPart = worksheetPart.DrawingsPart;
            else
                drawingsPart = worksheetPart.AddNewPart<DrawingsPart>();

            var chartPart = drawingsPart.AddNewPart<ChartPart>();

            var chartSpace = new ChartSpace();
            chartSpace.AddNamespaceDeclaration("c", "http://schemas.openxmlformats.org/drawingml/2006/chart");
            chartSpace.AddNamespaceDeclaration("a", "http://schemas.openxmlformats.org/drawingml/2006/main");
            chartSpace.AddNamespaceDeclaration("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");

            var chart = new Chart();
            chart.Append(new AutoTitleDeleted() { Val = false });

            var chartTitle = new Title();
            var chartText = new ChartText();
            var richText = new RichText();
            richText.Append(new DocumentFormat.OpenXml.Drawing.BodyProperties());
            richText.Append(new DocumentFormat.OpenXml.Drawing.ListStyle());

            var titlePara1 = new DocumentFormat.OpenXml.Drawing.Paragraph();
            var titleRun1 = new DocumentFormat.OpenXml.Drawing.Run();
            titleRun1.Append(new DocumentFormat.OpenXml.Drawing.RunProperties() { Bold = true, FontSize = 1400 });
            titleRun1.Append(new DocumentFormat.OpenXml.Drawing.Text("Gráfico da Sondagem"));
            titlePara1.Append(titleRun1);
            richText.Append(titlePara1);

            var titlePara2 = new DocumentFormat.OpenXml.Drawing.Paragraph();
            var titleRun2 = new DocumentFormat.OpenXml.Drawing.Run();
            titleRun2.Append(new DocumentFormat.OpenXml.Drawing.RunProperties() { Bold = true, FontSize = 1100 });
            titleRun2.Append(new DocumentFormat.OpenXml.Drawing.Text(tituloProficiencia));
            titlePara2.Append(titleRun2);
            richText.Append(titlePara2);

            chartText.Append(richText);
            chartTitle.Append(chartText);
            chartTitle.Append(new Overlay() { Val = false });
            chart.Append(chartTitle);

            var plotArea = new PlotArea();

            var barChart = new BarChart();
            barChart.Append(new BarDirection() { Val = BarDirectionValues.Column });
            barChart.Append(new BarGrouping() { Val = BarGroupingValues.Clustered });
            barChart.Append(new VaryColors() { Val = false });

            var serie = new BarChartSeries();
            serie.Append(new Index() { Val = 0 });
            serie.Append(new Order() { Val = 0 });

            var serTitle = new SeriesText();
            serTitle.Append(new NumericValue(tituloProficiencia));
            serie.Append(serTitle);

            for (int i = 0; i < dados.Count; i++)
            {
                var hex = (dados[i].Cor ?? "CCCCCC").TrimStart('#').ToUpper();
                if (hex.Length != 6) hex = "CCCCCC";

                var dp = new DataPoint();
                dp.Append(new Index() { Val = (uint)i });
                dp.Append(new InvertIfNegative() { Val = false });
                var spPr = new ChartShapeProperties();
                var solidFill = new DocumentFormat.OpenXml.Drawing.SolidFill();
                solidFill.Append(new DocumentFormat.OpenXml.Drawing.RgbColorModelHex() { Val = hex });
                spPr.Append(solidFill);
                dp.Append(spPr);
                serie.Append(dp);
            }

            var catValues = new CategoryAxisData();
            var strRef = new StringReference();
            var strCache = new StringCache();
            strCache.Append(new PointCount() { Val = (uint)dados.Count });
            for (int i = 0; i < dados.Count; i++)
                strCache.Append(new StringPoint() { Index = (uint)i, NumericValue = new NumericValue(dados[i].Descricao) });
            strRef.Append(strCache);
            catValues.Append(strRef);
            serie.Append(catValues);

            var values = new Values();
            var numRef = new NumberReference();
            var numCache = new NumberingCache();
            numCache.Append(new FormatCode("General"));
            numCache.Append(new PointCount() { Val = (uint)dados.Count });
            for (int i = 0; i < dados.Count; i++)
                numCache.Append(new NumericPoint() { Index = (uint)i, NumericValue = new NumericValue(dados[i].Quantidade.ToString()) });
            numRef.Append(numCache);
            values.Append(numRef);
            serie.Append(values);

            var dLbls = new DataLabels();
            dLbls.Append(new ShowLegendKey() { Val = false });
            dLbls.Append(new ShowValue() { Val = true });
            dLbls.Append(new ShowCategoryName() { Val = false });
            dLbls.Append(new ShowSeriesName() { Val = false });
            dLbls.Append(new ShowPercent() { Val = false });
            dLbls.Append(new ShowBubbleSize() { Val = false });
            serie.Append(dLbls);

            barChart.Append(serie);
            barChart.Append(new AxisId() { Val = 48650112U });
            barChart.Append(new AxisId() { Val = 48672768U });
            plotArea.Append(barChart);

            var catAxis = new CategoryAxis();
            catAxis.Append(new AxisId() { Val = 48650112U });
            catAxis.Append(new Scaling(new Orientation() { Val = OrientationValues.MinMax }));
            catAxis.Append(new Delete() { Val = false });
            catAxis.Append(new AxisPosition() { Val = AxisPositionValues.Bottom });
            catAxis.Append(new NumberingFormat() { FormatCode = "General", SourceLinked = true });
            catAxis.Append(new MajorTickMark() { Val = TickMarkValues.None });
            catAxis.Append(new MinorTickMark() { Val = TickMarkValues.None });

            var catAxisTitle = new Title();
            var catAxisChartText = new ChartText();
            var catAxisRichText = new RichText();
            catAxisRichText.Append(new DocumentFormat.OpenXml.Drawing.BodyProperties());
            catAxisRichText.Append(new DocumentFormat.OpenXml.Drawing.ListStyle());
            var catAxisPara = new DocumentFormat.OpenXml.Drawing.Paragraph();
            var catAxisRun = new DocumentFormat.OpenXml.Drawing.Run();
            catAxisRun.Append(new DocumentFormat.OpenXml.Drawing.RunProperties() { Bold = true, FontSize = 1000 });
            catAxisRun.Append(new DocumentFormat.OpenXml.Drawing.Text("Opções de respostas"));
            catAxisPara.Append(catAxisRun);
            catAxisRichText.Append(catAxisPara);
            catAxisChartText.Append(catAxisRichText);
            catAxisTitle.Append(catAxisChartText);
            catAxisTitle.Append(new Overlay() { Val = false });
            catAxis.Append(catAxisTitle);

            catAxis.Append(new CrossingAxis() { Val = 48672768U });
            plotArea.Append(catAxis);

            var valAxis = new ValueAxis();
            valAxis.Append(new AxisId() { Val = 48672768U });
            valAxis.Append(new Scaling(new Orientation() { Val = OrientationValues.MinMax }));
            valAxis.Append(new Delete() { Val = false });
            valAxis.Append(new AxisPosition() { Val = AxisPositionValues.Left });
            valAxis.Append(new NumberingFormat() { FormatCode = "General", SourceLinked = true });
            valAxis.Append(new MajorTickMark() { Val = TickMarkValues.None });
            valAxis.Append(new MinorTickMark() { Val = TickMarkValues.None });

            var valAxisTitle = new Title();
            var valAxisChartText = new ChartText();
            var valAxisRichText = new RichText();
            var valAxisBodyProps = new DocumentFormat.OpenXml.Drawing.BodyProperties();
            valAxisRichText.Append(valAxisBodyProps);
            valAxisRichText.Append(new DocumentFormat.OpenXml.Drawing.ListStyle());
            var valAxisPara = new DocumentFormat.OpenXml.Drawing.Paragraph();
            var valAxisRun = new DocumentFormat.OpenXml.Drawing.Run();
            valAxisRun.Append(new DocumentFormat.OpenXml.Drawing.RunProperties() { Bold = true, FontSize = 1000 });
            valAxisRun.Append(new DocumentFormat.OpenXml.Drawing.Text("Quantidade de estudantes"));
            valAxisPara.Append(valAxisRun);
            valAxisRichText.Append(valAxisPara);
            valAxisChartText.Append(valAxisRichText);
            valAxisTitle.Append(valAxisChartText);
            valAxisTitle.Append(new Overlay() { Val = false });
            valAxis.Append(valAxisTitle);

            valAxis.Append(new CrossingAxis() { Val = 48650112U });
            plotArea.Append(valAxis);

            chart.Append(plotArea);
            chart.Append(new PlotVisibleOnly() { Val = true });
            chartSpace.Append(chart);

            chartPart.ChartSpace = chartSpace;
            chartPart.ChartSpace.Save();

            var wsDr = drawingsPart.WorksheetDrawing ?? new Xdr.WorksheetDrawing();

            if (wsDr.LookupNamespace("xdr") == null)
                wsDr.AddNamespaceDeclaration("xdr", "http://schemas.openxmlformats.org/drawingml/2006/spreadsheetDrawing");
            if (wsDr.LookupNamespace("a") == null)
                wsDr.AddNamespaceDeclaration("a", "http://schemas.openxmlformats.org/drawingml/2006/main");
            if (wsDr.LookupNamespace("r") == null)
                wsDr.AddNamespaceDeclaration("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");

            var chartRelId = drawingsPart.GetIdOfPart(chartPart);

            var twoCellAnchor = new Xdr.TwoCellAnchor();
            twoCellAnchor.Append(new Xdr.FromMarker(
                new Xdr.ColumnId("0"),
                new Xdr.ColumnOffset("0"),
                new Xdr.RowId((linhaGrafico - 1).ToString()),
                new Xdr.RowOffset("0")
            ));
            twoCellAnchor.Append(new Xdr.ToMarker(
                new Xdr.ColumnId("9"),
                new Xdr.ColumnOffset("0"),
                new Xdr.RowId((linhaGrafico + 19).ToString()),
                new Xdr.RowOffset("0")
            ));

            var graphicFrame = new Xdr.GraphicFrame() { Macro = "" };
            graphicFrame.Append(new Xdr.NonVisualGraphicFrameProperties(
                new Xdr.NonVisualDrawingProperties() { Id = (uint)(drawingsPart.Parts.Count() + 10), Name = "GraficoSondagem" },
                new Xdr.NonVisualGraphicFrameDrawingProperties()
            ));
            graphicFrame.Append(new Xdr.Transform(
                new DocumentFormat.OpenXml.Drawing.Offset() { X = 0L, Y = 0L },
                new DocumentFormat.OpenXml.Drawing.Extents() { Cx = 0L, Cy = 0L }
            ));

            var graphic = new DocumentFormat.OpenXml.Drawing.Graphic();
            var graphicData = new DocumentFormat.OpenXml.Drawing.GraphicData()
            {
                Uri = "http://schemas.openxmlformats.org/drawingml/2006/chart"
            };
            graphicData.Append(new ChartReference() { Id = chartRelId });
            graphic.Append(graphicData);
            graphicFrame.Append(graphic);

            twoCellAnchor.Append(graphicFrame);
            twoCellAnchor.Append(new Xdr.ClientData());
            wsDr.Append(twoCellAnchor);

            drawingsPart.WorksheetDrawing = wsDr;
            drawingsPart.WorksheetDrawing.Save();

            if (!worksheetPart.Worksheet.Elements<Drawing>().Any())
            {
                var drawingRelId = worksheetPart.GetIdOfPart(drawingsPart);
                worksheetPart.Worksheet.Append(new Drawing() { Id = drawingRelId });
            }

            worksheetPart.Worksheet.Save();
        }


        private async Task<string> ObterTurma(string codigoTurma)
        {
            if (codigoTurma == "-99")
                return "Todos";

            var turma = await mediator.Send(new ObterTurmaQuery(codigoTurma));
            return turma.NomeRelatorio;
        }

        private async Task<DreUeNome> ObterNomeUeDre(string ueCodigo)
        {
            return await mediator.Send(new ObterDreUeNomePorUeCodigoQuery(ueCodigo));
        }

        static void EscreverCelula(IXLWorksheet ws, int row, int col, string valor, bool bold = false)
        {
            var cell = ws.Cell(row, col);
            cell.Value = valor;
            cell.Style.Font.Bold = bold;
            cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            cell.Style.Alignment.WrapText = true;
        }

        static void AplicarBordaExterna(IXLRange range)
        {
            range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        }

        static void EstilarCelulaDados(IXLCell cell)
        {
            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            cell.Style.Alignment.WrapText = true;
            cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        }

        static void preenchererCelulaSondagem(IXLCell cell, string valor, XLColor corFundo)
        {
            cell.Value = valor;
            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            cell.Style.Alignment.WrapText = true;
            cell.Style.Font.Bold = true;
            cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            if (valor == "Sem preencher" || valor == "Vazio" || valor == "")
                cell.Style.Fill.BackgroundColor = XLColor.White;
            else
                cell.Style.Fill.BackgroundColor = corFundo;

            if (valor == "Sem preencher" || valor == "Vazio" || valor == "")
                cell.Style.Font.FontColor = XLColor.Gray;
            else
                cell.Style.Font.FontColor = XLColor.White;
        }
    }
}

