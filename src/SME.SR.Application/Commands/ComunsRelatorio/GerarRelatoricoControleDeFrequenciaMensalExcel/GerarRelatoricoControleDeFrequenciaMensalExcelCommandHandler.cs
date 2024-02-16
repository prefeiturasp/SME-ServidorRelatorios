using ClosedXML.Excel;
using MediatR;
using SME.SR.HtmlPdf;
using SME.SR.Infra;
using SME.SR.Infra.Dtos.FrequenciaMensal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class GerarRelatoricoControleDeFrequenciaMensalExcelCommandHandler : AsyncRequestHandler<GerarRelatoricoControleDeFrequenciaMensalExcelCommand>
    {
        private readonly IMediator mediator;
        private readonly IServicoFila servicoFila;
        private const int LINHA_NOME_SISTEMA = 2;
        private const int LINHA_NOME_RELATORIO = 3;
        private const int LINHA_CABECALHO_DRE_UE = 6;
        private const int LINHA_CABECALHO_ESTUDANTE = 7;
        private const int LINHA_CABECALHO_TURMA = 8;
        private const int LINHA_CABECALHO_USUARIO_DATA = 9;
        private const int LINHA_MES = 10;

        public GerarRelatoricoControleDeFrequenciaMensalExcelCommandHandler(IMediator mediator, IServicoFila servicoFila)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this.servicoFila = servicoFila ?? throw new ArgumentNullException(nameof(servicoFila));
        }

        protected override async Task Handle(GerarRelatoricoControleDeFrequenciaMensalExcelCommand request, CancellationToken cancellationToken)
        {
            var controlesFrequencias = await mediator.Send(new ObterControleFrequenciaMensalParaExcelQuery(request.ControlesFrequenciasMensais.ToList()));

            if (!controlesFrequencias.Any())
                throw new NegocioException("Não possui informações.");

            var workbook = new XLWorkbook();
            
                foreach (var dtoExcel in controlesFrequencias)
                {
                    var worksheet = workbook.Worksheets.Add(dtoExcel.CodigoCriancaEstudante);
                    var linhaFinal = LINHA_MES;

                    MontarCabecalho(worksheet, dtoExcel, 20);
                    AdicionarEstiloCabecalho(worksheet, dtoExcel.FrequenciasMeses.Max(data => data.TabelaDeDado.Columns.Count));

                    foreach (var dtoExcelMes in dtoExcel.FrequenciasMeses)
                    {
                        linhaFinal += 1;
                        AdicionarColunaMes(worksheet, dtoExcelMes, linhaFinal, dtoExcelMes.TabelaDeDado.Columns.Count);
                        linhaFinal += 2;
                        worksheet.Cell(linhaFinal, 1).InsertData(dtoExcelMes.TabelaDeDado);
                        AdicionarEstilo(worksheet, dtoExcelMes, linhaFinal);
                        linhaFinal += dtoExcelMes.TabelaDeDado.Rows.Count;
                    }
                }

                var caminhoBase = AppDomain.CurrentDomain.BaseDirectory;
                var caminhoParaSalvar = Path.Combine(caminhoBase, $"relatorios", $"{request.CodigoCorrelacao}");

                workbook.SaveAs($"{caminhoParaSalvar}.xlsx");

            workbook.Dispose();

            await servicoFila.PublicaFila(new PublicaFilaDto(new MensagemRelatorioProntoDto(), RotasRabbitSGP.RotaRelatoriosProntosSgp, ExchangeRabbit.Sgp, request.CodigoCorrelacao));
        }

        private void AdicionarColunaMes(IXLWorksheet worksheet, FrequenciaPorMesExcelDto dto, int linha, int totalColuna)
        {
            worksheet.Cell(linha, 1).Value = $"Mês: {dto.Mes}";
            var rangeMes = worksheet.Range(linha, 1, linha, 1 + totalColuna);
            rangeMes.Merge().Style.Font.Bold = true;
            AdicinarFonte(rangeMes);
            var linhaFrequencia = linha + 1;
            worksheet.Cell(linhaFrequencia, 1).Value = $"Frequência global do mês: {dto.FrequenciaGlobal}";
            var rangeFreq = worksheet.Range(linhaFrequencia, 1, linhaFrequencia, 1 + totalColuna);
            rangeFreq.Merge().Style.Font.Bold = true;
            AdicinarFonte(rangeFreq);
        }

        private void MontarCabecalho(IXLWorksheet worksheet, RelatorioControleFrequenciaMensalExcelDto dto, int totalColunas)
        {
            worksheet.AddPicture(ObterLogo())
                .MoveTo(worksheet.Cell(2, 1))
                .Scale(0.15);

            worksheet.Cell(LINHA_NOME_SISTEMA, 9).Value = "SGP - Sistema de Gestão Pedagógica";
            var linhaNomeSistema = worksheet.Range(LINHA_NOME_SISTEMA, 9, LINHA_NOME_SISTEMA, totalColunas);
            linhaNomeSistema.Merge().Style.Font.Bold = true;
            AdicinarFonte(linhaNomeSistema);

            worksheet.Cell(LINHA_NOME_RELATORIO, 9).Value = "Relatório de Controle de Frequência Mensal";
            AdicinarFonte(worksheet.Range(LINHA_NOME_RELATORIO, 9, LINHA_NOME_RELATORIO, totalColunas));

            worksheet.Cell(LINHA_CABECALHO_DRE_UE, 1).Value = $"DRE: {dto.Dre}    Unidade Escolar (UE): {dto.Ue}";
            AdicinarFonte(worksheet.Range(LINHA_CABECALHO_DRE_UE, 1, LINHA_CABECALHO_DRE_UE, totalColunas));

            worksheet.Cell(LINHA_CABECALHO_ESTUDANTE, 1).Value = $"Criança/Estudante: {dto.NomeCriancaEstudante} ({dto.CodigoCriancaEstudante})";
            AdicinarFonte(worksheet.Range(LINHA_CABECALHO_ESTUDANTE, 1, LINHA_CABECALHO_ESTUDANTE, totalColunas));

            worksheet.Cell(LINHA_CABECALHO_TURMA, 1).Value = $"Turma: {dto.Turma}";
            AdicinarFonte(worksheet.Range(LINHA_CABECALHO_TURMA, 1, LINHA_CABECALHO_TURMA, totalColunas));

            worksheet.Cell(LINHA_CABECALHO_USUARIO_DATA, 1).Value = $"Usuário: {dto.Usuario}    Data de impressão: {dto.DataImpressao}";
            AdicinarFonte(worksheet.Range(LINHA_CABECALHO_USUARIO_DATA, 1, LINHA_CABECALHO_USUARIO_DATA, totalColunas));
        }

        private Stream ObterLogo()
        {
            string base64Logo = SmeConstants.LogoSmeMono.Substring(SmeConstants.LogoSmeMono.IndexOf(',') + 1);
            return new MemoryStream(Convert.FromBase64String(base64Logo));
        }

        private void AdicinarFonte(IXLRange range)
        {
            range.Merge().Style.Font.FontSize = 10;
            range.Style.Font.FontName = "Arial";
        }

        private void AdicionarEstilo(IXLWorksheet worksheet, FrequenciaPorMesExcelDto dtoExcelMes, int linha)
        {
            int ultimaColunaUsada = worksheet.LastColumnUsed().ColumnNumber();
            int ultimaLinhaUsada = worksheet.LastRowUsed().RowNumber();

            AdicionarEstiloCabecalhoLinha(worksheet, ultimaColunaUsada, linha);
            AdicionarEstiloCorpo(worksheet, ultimaLinhaUsada, dtoExcelMes.TabelaDeDado.Rows.Count, linha, ultimaColunaUsada);
            AdicionarEstiloColunaDia(worksheet, dtoExcelMes.ColunasDiasNaoLetivosFinaisSemana, linha, ultimaLinhaUsada);

            worksheet.ShowGridLines = false;
            worksheet.Columns().AdjustToContents();
            worksheet.ColumnsUsed().AdjustToContents();
            worksheet.RowsUsed().AdjustToContents();
        }

        private void AdicionarEstiloColunaDia(IXLWorksheet worksheet, IEnumerable<int> colunasDiasSemAula, int linha, int ultimaLinha)
        {
            foreach(var indiceColuna in colunasDiasSemAula)
            {
                worksheet.Range(linha, indiceColuna, ultimaLinha, indiceColuna).Style.Fill.BackgroundColor = XLColor.LightGray;
            }
        }

        private void AdicionarEstiloCabecalho(IXLWorksheet worksheet, int ultimaColunaUsada)
        {
            AdicionarEstiloCabecalhoLinha(worksheet, ultimaColunaUsada, LINHA_CABECALHO_DRE_UE);
            AdicionarEstiloCabecalhoLinha(worksheet, ultimaColunaUsada, LINHA_NOME_SISTEMA);
            AdicionarEstiloCabecalhoLinha(worksheet, ultimaColunaUsada, LINHA_NOME_RELATORIO);
            AdicionarEstiloCabecalhoLinha(worksheet, ultimaColunaUsada, LINHA_CABECALHO_ESTUDANTE);
            AdicionarEstiloCabecalhoLinha(worksheet, ultimaColunaUsada, LINHA_CABECALHO_TURMA);
            AdicionarEstiloCabecalhoLinha(worksheet, ultimaColunaUsada, LINHA_CABECALHO_USUARIO_DATA);
        }

        private void AdicionarEstiloCorpo(IXLWorksheet worksheet, int ultimaLinhaUsada, int totalRegistro, int linha, int ultimaColunaMes)
        {
            var linhaTabela = linha;
            var linhaSubCabecalho = linhaTabela + 1;

            worksheet.Range(linhaTabela, 1, ultimaLinhaUsada, ultimaColunaMes).Style.Font.SetFontName("Arial");
            worksheet.Range(linhaTabela, 1, ultimaLinhaUsada, ultimaColunaMes).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

            worksheet.Range(linhaTabela, 1, ultimaLinhaUsada, ultimaColunaMes).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
            worksheet.Range(linhaTabela, 1, ultimaLinhaUsada, ultimaColunaMes).Style.Border.SetOutsideBorderColor(XLColor.Black);

            worksheet.Range(linhaTabela, 1, ultimaLinhaUsada, ultimaColunaMes).Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);
            worksheet.Range(linhaTabela, 1, ultimaLinhaUsada, ultimaColunaMes).Style.Border.SetInsideBorderColor(XLColor.Black);

            worksheet.Range(linhaTabela, 1, linhaTabela, ultimaColunaMes).Style.Font.SetFontSize(10);
            worksheet.Range(linhaTabela, 1, linhaTabela, ultimaColunaMes).Style.Font.Bold = true;
            worksheet.Range(linhaTabela, 1, linhaTabela, ultimaColunaMes).Style.Fill.BackgroundColor = XLColor.Gray;
            worksheet.Range(linhaSubCabecalho, 1, linhaSubCabecalho, ultimaColunaMes).Style.Font.SetFontSize(10);
            worksheet.Range(linhaSubCabecalho, 1, linhaSubCabecalho, ultimaColunaMes).Style.Font.Bold = true;
            worksheet.Range(linhaSubCabecalho, 1, linhaSubCabecalho, ultimaColunaMes).Style.Fill.BackgroundColor = XLColor.Gray;
            worksheet.Range(linhaTabela, 2, ultimaLinhaUsada, ultimaColunaMes).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            worksheet.Range(linhaTabela, 1, linhaTabela + 1, 1).Merge();
            worksheet.Range(linhaTabela, ultimaColunaMes - 1, linhaTabela + 1, ultimaColunaMes - 1).Merge();
            worksheet.Range(linhaTabela, ultimaColunaMes, linhaTabela + 1, ultimaColunaMes).Merge();

            Merge(worksheet, linhaTabela, 1, totalRegistro);
            Merge(worksheet, linhaTabela, ultimaColunaMes, totalRegistro);
        }

        private void Merge(IXLWorksheet worksheet, int linha, int coluna, int totalRegistro)
        {
            var linhaInicio = linha + 2;
            var linhaFinal = 0;

            totalRegistro = linhaInicio + (totalRegistro - 3);

            while (linhaInicio <= totalRegistro)
            {
                linhaFinal = linhaInicio + 2;
                worksheet.Range(linhaInicio, coluna, linhaFinal, coluna).Merge();
                linhaInicio = linhaFinal + 1;
            }
        }

        private void AdicionarEstiloCabecalhoLinha(IXLWorksheet worksheet, int ultimaColunaUsada, int linha)
        {
            var range = worksheet.Range(linha, 1, linha, ultimaColunaUsada);
            range.Style.Font.SetFontSize(10);
            range.Style.Font.SetFontName("Arial");
        }
    }
}
