using ClosedXML.Excel;
using MediatR;
using SME.SR.HtmlPdf;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
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
        private const int LINHA_NOME_SISTEMA = 6;
        private const int LINHA_NOME_RELATORIO = 7;
        private const int LINHA_TIPO_SONDAGEM = 8;
        private const int LINHA_CABECALHO_DRE = 9;
        private const int LINHA_CABECALHO_ANO_PERIODO = 10;

        private const int LINHA_TABELA = 12;
        private const int LINHA_SUBTITULO = 13;
        private const int LINHA_TABTITULO = 14;
        private IEnumerable<RelatorioSondagemAnaliticoExcelDto> tabelaExcel;

        public GerarRelatorioAnaliticoDaSondagemExcelCommandHandler(IMediator mediator, IServicoFila servicoFila)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this.servicoFila = servicoFila ?? throw new ArgumentNullException(nameof(servicoFila));
        }

        protected override async Task Handle(GerarRelatorioAnaliticoDaSondagemExcelCommand request, CancellationToken cancellationToken)
        {
            tabelaExcel = await mediator.Send(new ObterRelatorioAnaliticoSondagemExcelQuery(request.RelatorioAnalitico, request.TipoSondagem));

            if (!tabelaExcel.Any())
                throw new NegocioException("Não foi possui informações.");

            using (var workbook = new XLWorkbook())
            {
                foreach (var dtoExcel in tabelaExcel)
                {
                    var worksheet = workbook.Worksheets.Add(dtoExcel.DreSigla);

                    MontarCabecalho(worksheet, dtoExcel, dtoExcel.TabelaDeDado.Columns.Count, request.TipoSondagem);

                    worksheet.Cell(LINHA_TABELA, 1).InsertData(dtoExcel.TabelaDeDado);

                    AdicionarEstilo(worksheet, request.TipoSondagem);
                }

                var caminhoBase = AppDomain.CurrentDomain.BaseDirectory;
                var caminhoParaSalvar = Path.Combine(caminhoBase, $"relatorios", $"{request.CodigoCorrelacao}");

                workbook.SaveAs($"{caminhoParaSalvar}.xlsx");
            }

            await servicoFila.PublicaFila(new PublicaFilaDto(new MensagemRelatorioProntoDto(), RotasRabbitSGP.RotaRelatoriosProntosSgp, ExchangeRabbit.Sgp, request.CodigoCorrelacao));
        }

        private void MontarCabecalho(IXLWorksheet worksheet, RelatorioSondagemAnaliticoExcelDto dto, int totalColunas, TipoSondagem tipoSondagem)
        {
            worksheet.AddPicture(ObterLogo())
                .MoveTo(worksheet.Cell(2, 1))
                .Scale(0.15);

            worksheet.Cell(LINHA_NOME_SISTEMA, 1).Value = "SGP - Sistema de Gestão Pedagógica";
            var linhaNomeSistema = worksheet.Range(LINHA_NOME_SISTEMA, 1, LINHA_NOME_SISTEMA, totalColunas);
            linhaNomeSistema.Merge().Style.Font.Bold = true;
            AdicinarFonte(linhaNomeSistema);

            worksheet.Cell(LINHA_NOME_RELATORIO, 1).Value = "Relatório analítico da Sondagem";
            AdicinarFonte(worksheet.Range(LINHA_NOME_RELATORIO, 1, LINHA_NOME_RELATORIO, totalColunas));

            worksheet.Cell(LINHA_TIPO_SONDAGEM, 1).Value = dto.DescricaoTipoSondagem;
            AdicinarFonte(worksheet.Range(LINHA_TIPO_SONDAGEM, 1, LINHA_TIPO_SONDAGEM, totalColunas));

            worksheet.Cell(LINHA_CABECALHO_DRE, 1).Value = $"DRE: {dto.Dre}";
            AdicinarFonte(worksheet.Range(LINHA_CABECALHO_DRE, 1, LINHA_CABECALHO_DRE, totalColunas));

            worksheet.Cell(LINHA_CABECALHO_ANO_PERIODO, 1).Value = $"ANO LETIVO: {dto.AnoLetivo}  PERÍODO: {dto.Periodo}º {ObterTituloSemestreBimestre(tipoSondagem, dto.AnoLetivo)}";
            AdicinarFonte(worksheet.Range(LINHA_CABECALHO_ANO_PERIODO, 1, LINHA_CABECALHO_ANO_PERIODO, totalColunas));
        }

        private string ObterTituloSemestreBimestre(TipoSondagem tipoSondagem, int anoLetivo)
        {
            var ehAnoLetivoAnterior2022 = anoLetivo < 2022;
            var ehAnoLetivo2022 = anoLetivo == 2022;
            var ehAnoLetivoApos2022 = anoLetivo > 2022;
            var ehTipoSondagemMatematica = tipoSondagem == TipoSondagem.MAT_IAD ||
                                           tipoSondagem == TipoSondagem.MAT_CampoMultiplicativo ||
                                           tipoSondagem == TipoSondagem.MAT_CampoAditivo ||
                                           tipoSondagem == TipoSondagem.MAT_Numeros;

            if (ehTipoSondagemMatematica) 
            {
                if (ehAnoLetivoAnterior2022)
                    return "SEMESTRE";
                else if (ehAnoLetivo2022)
                    return "BIMESTRE";
                else if (ehAnoLetivoApos2022 && tipoSondagem == TipoSondagem.MAT_IAD)
                    return "SEMESTRE";
                else
                    return "BIMESTRE";
            } else return "BIMESTRE";

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

        private void AdicionarEstilo(IXLWorksheet worksheet, TipoSondagem tipoSondagem)
        {
            int ultimaColunaUsada = worksheet.LastColumnUsed().ColumnNumber();
            int ultimaLinhaUsada = worksheet.LastRowUsed().RowNumber();

            AdicionarEstiloCabecalho(worksheet, ultimaColunaUsada);
            AdicionarEstiloCorpo(worksheet, ultimaColunaUsada, ultimaLinhaUsada, tipoSondagem);

            worksheet.ShowGridLines = false;

            worksheet.ColumnsUsed().AdjustToContents();
            worksheet.RowsUsed().AdjustToContents();
        }

        private void AdicionarEstiloCabecalho(IXLWorksheet worksheet, int ultimaColunaUsada)
        {
            var rangeDre = worksheet.Range(LINHA_CABECALHO_DRE, 1, LINHA_CABECALHO_DRE, ultimaColunaUsada);
            rangeDre.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
            rangeDre.Style.Border.SetOutsideBorderColor(XLColor.Black);

            rangeDre.Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);
            rangeDre.Style.Border.SetInsideBorderColor(XLColor.Black);

            rangeDre.Style.Font.SetFontSize(10);
            rangeDre.Style.Font.SetFontName("Arial");
            
            var rangeAnoPeriodo = worksheet.Range(LINHA_CABECALHO_ANO_PERIODO, 1, LINHA_CABECALHO_ANO_PERIODO, ultimaColunaUsada);
            rangeAnoPeriodo.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
            rangeAnoPeriodo.Style.Border.SetOutsideBorderColor(XLColor.Black);

            rangeAnoPeriodo.Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);
            rangeAnoPeriodo.Style.Border.SetInsideBorderColor(XLColor.Black);

            rangeAnoPeriodo.Style.Font.SetFontSize(10);
            rangeAnoPeriodo.Style.Font.SetFontName("Arial");

            var rangeNomeSistema = worksheet.Range(LINHA_NOME_SISTEMA, 1, LINHA_NOME_SISTEMA, ultimaColunaUsada);
            rangeNomeSistema.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
            rangeNomeSistema.Style.Border.SetOutsideBorderColor(XLColor.Black);

            rangeNomeSistema.Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);
            rangeNomeSistema.Style.Border.SetInsideBorderColor(XLColor.Black);

            rangeNomeSistema.Style.Font.SetFontSize(10);
            rangeNomeSistema.Style.Font.SetFontName("Arial");

            var rangeNomeRelatorio = worksheet.Range(LINHA_NOME_RELATORIO, 1, LINHA_NOME_RELATORIO, ultimaColunaUsada);
            rangeNomeRelatorio.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
            rangeNomeRelatorio.Style.Border.SetOutsideBorderColor(XLColor.Black);

            rangeNomeRelatorio.Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);
            rangeNomeRelatorio.Style.Border.SetInsideBorderColor(XLColor.Black);

            rangeNomeRelatorio.Style.Font.SetFontSize(10);
            rangeNomeRelatorio.Style.Font.SetFontName("Arial");


            var rangeTipoSondagem = worksheet.Range(LINHA_TIPO_SONDAGEM, 1, LINHA_TIPO_SONDAGEM, ultimaColunaUsada);
            rangeTipoSondagem.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
            rangeTipoSondagem.Style.Border.SetOutsideBorderColor(XLColor.Black);

            rangeTipoSondagem.Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);
            rangeTipoSondagem.Style.Border.SetInsideBorderColor(XLColor.Black);

            rangeTipoSondagem.Style.Font.SetFontSize(10);
            rangeTipoSondagem.Style.Font.SetFontName("Arial");
        }

        private void AdicionarEstiloCorpo(IXLWorksheet worksheet, int ultimaColunaUsada, int ultimaLinhaUsada, TipoSondagem tipoSondagem)
        {
            var dicionarioEstiloCustom = ObterDicionarioEstiloCorpoCustom();

            if (dicionarioEstiloCustom.ContainsKey(tipoSondagem))
                dicionarioEstiloCustom[tipoSondagem](worksheet, ultimaColunaUsada, ultimaLinhaUsada);
            else
                AdicionarEstiloCorpoPadrao(worksheet, ultimaColunaUsada, ultimaLinhaUsada);
        }

        private Dictionary<TipoSondagem, Action<IXLWorksheet, int, int>> ObterDicionarioEstiloCorpoCustom()
        {
            return new Dictionary<TipoSondagem, Action<IXLWorksheet, int, int>>()
            {
                { TipoSondagem.LP_CapacidadeLeitura, AdicionarEstiloCorpoCapacidadeDeLeitura },
                { TipoSondagem.MAT_CampoAditivo, AdicionarEstiloCorpoAditivoMultiplicativo },
                { TipoSondagem.MAT_CampoMultiplicativo, AdicionarEstiloCorpoAditivoMultiplicativo },
                { TipoSondagem.MAT_Numeros, AdicionarEstiloCorpoNumeroIAD },
                { TipoSondagem.MAT_IAD, AdicionarEstiloCorpoNumeroIAD }
            };
        }

        private void AdicionarEstiloCorpoPadrao(IXLWorksheet worksheet, int ultimaColunaUsada, int ultimaLinhaUsada)
        {
            AdicionarEstiloCorpo(worksheet, ultimaColunaUsada, ultimaLinhaUsada, LINHA_TABELA);
        }

        private void AdicionarEstiloCorpoCapacidadeDeLeitura(IXLWorksheet worksheet, int ultimaColunaUsada, int ultimaLinhaUsada)
        {
            AdicionarEstiloCorpo(worksheet, ultimaColunaUsada, ultimaLinhaUsada, LINHA_TABELA);
            AdicionarEstiloCorpo(worksheet, ultimaColunaUsada, ultimaLinhaUsada, LINHA_SUBTITULO);
            AdicionarEstiloCorpo(worksheet, ultimaColunaUsada, ultimaLinhaUsada, LINHA_TABTITULO);

            worksheet.Range(LINHA_TABELA, 1, LINHA_TABELA, 4).Merge();
            worksheet.Range(LINHA_TABELA, 1, LINHA_SUBTITULO, 4).Merge();

            AdicioneMerge(worksheet, 5, ultimaColunaUsada, LINHA_TABELA, 11);
            AdicioneMerge(worksheet, 1, ultimaColunaUsada, LINHA_SUBTITULO, 3);
        }

        private void AdicionarEstiloCorpoAditivoMultiplicativo(IXLWorksheet worksheet, int ultimaColunaUsada, int ultimaLinhaUsada)
        {
            AdicionarEstiloCorpo(worksheet, ultimaColunaUsada, ultimaLinhaUsada, LINHA_TABELA);
            AdicionarEstiloCorpo(worksheet, ultimaColunaUsada, ultimaLinhaUsada, LINHA_SUBTITULO);
            AdicionarEstiloCorpo(worksheet, ultimaColunaUsada, ultimaLinhaUsada, LINHA_TABTITULO);

            worksheet.Range(LINHA_TABELA, 1, LINHA_TABELA, 4).Merge();
            worksheet.Range(LINHA_TABELA, 1, LINHA_SUBTITULO, 4).Merge();

            AdicioneMerge(worksheet, 5, ultimaColunaUsada, LINHA_TABELA, 7);
            AdicioneMerge(worksheet, 5, ultimaColunaUsada, LINHA_SUBTITULO, 3);
        }

        private void AdicionarEstiloCorpoNumeroIAD(IXLWorksheet worksheet, int ultimaColunaUsada, int ultimaLinhaUsada)
        {
            AdicionarEstiloCorpo(worksheet, ultimaColunaUsada, ultimaLinhaUsada, LINHA_TABELA);
            AdicionarEstiloCorpo(worksheet, ultimaColunaUsada, ultimaLinhaUsada, LINHA_SUBTITULO);

            worksheet.Range(LINHA_TABELA, 1, LINHA_TABELA, 4).Merge();

            var dtoExcel = tabelaExcel.FirstOrDefault();

            if (dtoExcel.MergeColunas != null && dtoExcel.MergeColunas.Any())
            {
                foreach (var merge in dtoExcel.MergeColunas)
                {
                    worksheet.Range(LINHA_TABELA, merge.ColunaInicio, LINHA_TABELA, merge.ColunaFim).Merge();
                }
            }
        }

        private void AdicioneMerge(IXLWorksheet worksheet, int colunaInicio, int ultimaColunaUsada, int linhaMerge, int acrescimoColuna)
        {
            var colunafinal = 0;

            while (colunaInicio <= ultimaColunaUsada)
            {
                colunafinal = colunaInicio + acrescimoColuna;
                worksheet.Range(linhaMerge, colunaInicio, linhaMerge, colunafinal).Merge();
                colunaInicio = colunafinal + 1;
            }
        }

        private void AdicionarEstiloCorpo(IXLWorksheet worksheet, int ultimaColunaUsada, int ultimaLinhaUsada, int linha)
        {
            worksheet.Range(linha, 1, ultimaLinhaUsada, ultimaColunaUsada).Style.Font.SetFontName("Arial");
            worksheet.Range(linha, 1, ultimaLinhaUsada, ultimaColunaUsada).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

            worksheet.Range(linha, 1, ultimaLinhaUsada, ultimaColunaUsada).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
            worksheet.Range(linha, 1, ultimaLinhaUsada, ultimaColunaUsada).Style.Border.SetOutsideBorderColor(XLColor.Black);

            worksheet.Range(linha, 1, ultimaLinhaUsada, ultimaColunaUsada).Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);
            worksheet.Range(linha, 1, ultimaLinhaUsada, ultimaColunaUsada).Style.Border.SetInsideBorderColor(XLColor.Black);

            worksheet.Range(linha, 1, linha, ultimaColunaUsada).Style.Font.SetFontSize(10);
            worksheet.Range(linha, 1, linha, ultimaColunaUsada).Style.Font.Bold = true;
            worksheet.Range(linha, 2, ultimaLinhaUsada, ultimaColunaUsada).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
        }
    }
}
