using ClosedXML.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using MediatR;
using RazorEngine.Compilation.ImpromptuInterface.InvokeExt;
using SME.SR.HtmlPdf;
using SME.SR.Infra;
using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class GerarRelatorioAnaliticoDaSondagemExcelCommandHandler : AsyncRequestHandler<GerarRelatorioAnaliticoDaSondagemExcelCommand>
    {
        private readonly IMediator mediator;
        private readonly IServicoFila servicoFila;
        private const int LINHA_CABECALHO_DRE = 6;
        private const int LINHA_TABELA = 8;

        public GerarRelatorioAnaliticoDaSondagemExcelCommandHandler(IMediator mediator, IServicoFila servicoFila)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this.servicoFila = servicoFila ?? throw new ArgumentNullException(nameof(servicoFila));
        }

        protected override async Task Handle(GerarRelatorioAnaliticoDaSondagemExcelCommand request, CancellationToken cancellationToken)
        {
            var tabelaExcel = await mediator.Send(new ObterRelatorioAnaliticoSondagemExcelQuery(request.RelatorioAnalitico, request.TipoSondagem));

            if (!tabelaExcel.Any())
                throw new NegocioException("Não foi possui informações.");

            using (var workbook = new XLWorkbook())
            {
                foreach (var dtoExcel in tabelaExcel)
                {
                    var worksheet = workbook.Worksheets.Add(dtoExcel.DreSigla);

                    MontarCabecalho(worksheet, dtoExcel, dtoExcel.TabelaDeDado.Columns.Count);

                    worksheet.Cell(LINHA_TABELA, 1).InsertData(dtoExcel.TabelaDeDado);

                    AdicionarEstilo(worksheet);
                }

                var caminhoBase = AppDomain.CurrentDomain.BaseDirectory;
                var caminhoParaSalvar = Path.Combine(caminhoBase, $"relatorios", $"{request.CodigoCorrelacao}");

                workbook.SaveAs($"{caminhoParaSalvar}.xlsx");
            }

            await servicoFila.PublicaFila(new PublicaFilaDto(new MensagemRelatorioProntoDto(), RotasRabbitSGP.RotaRelatoriosProntosSgp, ExchangeRabbit.Sgp, request.CodigoCorrelacao));
        }

        private void MontarCabecalho(IXLWorksheet worksheet, RelatorioSondagemAnaliticoExcelDto dto, int totalColunas)
        {
            worksheet.AddPicture(ObterLogo())
                .MoveTo(worksheet.Cell(2, 1))
                .Scale(0.15);

            int indiceColunaTitulo = (int)(totalColunas * 0.7) + 1;

            worksheet.Row(2).Cell(indiceColunaTitulo).Value = "SGP - Sistema de Gestão Pedagógica";
            var rangeTitulo = worksheet.Range(2, indiceColunaTitulo, 2, totalColunas);
            rangeTitulo.Merge().Style.Font.Bold = true;
            AdicinarFonte(rangeTitulo);

            worksheet.Row(3).Cell(indiceColunaTitulo).Value = "Relatório analítico da Sondagem";
            AdicinarFonte(worksheet.Range(3, indiceColunaTitulo, 3, totalColunas));

            worksheet.Row(4).Cell(indiceColunaTitulo).Value = dto.DescricaoTipoSondagem;
            AdicinarFonte(worksheet.Range(4, indiceColunaTitulo, 4, totalColunas));

            worksheet.Cell(LINHA_CABECALHO_DRE, 1).Value = $"DRE: {dto.Dre}";
            AdicinarFonte(worksheet.Range(LINHA_CABECALHO_DRE, 1, LINHA_CABECALHO_DRE, totalColunas));
        }

        private void AdicinarFonte(IXLRange range)
        {
            range.Merge().Style.Font.FontSize = 10;
            range.Style.Font.FontName = "Arial";
        }

        private Stream ObterLogo()
        {
            string base64Logo = SmeConstants.LogoSmeMono.Substring(SmeConstants.LogoSmeMono.IndexOf(',') + 1);
            return new MemoryStream(Convert.FromBase64String(base64Logo));
        }

        private void AdicionarEstilo(IXLWorksheet worksheet)
        {
            int ultimaColunaUsada = worksheet.LastColumnUsed().ColumnNumber();
            int ultimaLinhaUsada = worksheet.LastRowUsed().RowNumber();

            AdicionarEstiloCabecalho(worksheet, ultimaColunaUsada);
            AdicionarEstiloCorpo(worksheet, ultimaColunaUsada, ultimaLinhaUsada);

            worksheet.ShowGridLines = false;

            worksheet.ColumnsUsed().AdjustToContents();
            worksheet.RowsUsed().AdjustToContents();
        }

        private void AdicionarEstiloCabecalho(IXLWorksheet worksheet, int ultimaColunaUsada)
        {
            var range = worksheet.Range(LINHA_CABECALHO_DRE, 1, LINHA_CABECALHO_DRE, ultimaColunaUsada);
            range.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
            range.Style.Border.SetOutsideBorderColor(XLColor.Black);

            range.Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);
            range.Style.Border.SetInsideBorderColor(XLColor.Black);

            range.Style.Font.SetFontSize(10);
            range.Style.Font.SetFontName("Arial");
        }

        private void AdicionarEstiloCorpo(IXLWorksheet worksheet, int ultimaColunaUsada, int ultimaLinhaUsada)
        {
            worksheet.Range(LINHA_TABELA, 1, ultimaLinhaUsada, ultimaColunaUsada).Style.Font.SetFontName("Arial");
            worksheet.Range(LINHA_TABELA, 1, ultimaLinhaUsada, ultimaColunaUsada).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

            worksheet.Range(LINHA_TABELA, 1, ultimaLinhaUsada, ultimaColunaUsada).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
            worksheet.Range(LINHA_TABELA, 1, ultimaLinhaUsada, ultimaColunaUsada).Style.Border.SetOutsideBorderColor(XLColor.Black);

            worksheet.Range(LINHA_TABELA, 1, ultimaLinhaUsada, ultimaColunaUsada).Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);
            worksheet.Range(LINHA_TABELA, 1, ultimaLinhaUsada, ultimaColunaUsada).Style.Border.SetInsideBorderColor(XLColor.Black);

            worksheet.Range(LINHA_TABELA, 1, LINHA_TABELA, ultimaColunaUsada).Style.Font.SetFontSize(10);
            worksheet.Range(LINHA_TABELA, 1, LINHA_TABELA, ultimaColunaUsada).Style.Font.Bold = true;
            worksheet.Range(LINHA_TABELA, 2, ultimaLinhaUsada, ultimaColunaUsada).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
        }
    }
}
