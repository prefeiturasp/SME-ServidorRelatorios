using ClosedXML.Excel;
using MediatR;
using SME.SR.HtmlPdf;
using SME.SR.Infra;
using SME.SR.Infra.Dtos.FrequenciaMensal;
using System;
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
        private const int LINHA_MES = 11;
        private const int LINHA_MES_FREQUENCIA = 12;
        private const int LINHA_TABELA = 13;
        private const int LINHA_SUBCABELHO = 14;

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

            using (var workbook = new XLWorkbook())
            {
                foreach (var dtoExcel in controlesFrequencias)
                {
                    var worksheet = workbook.Worksheets.Add(dtoExcel.CodigoCriancaEstudante);
                    var colunaFinal = 0;

                    MontarCabecalho(worksheet, dtoExcel, 20);

                    foreach (var dtoExcelMes in dtoExcel.FrequenciasMeses)
                    {
                        colunaFinal += 1;
                        AdicionarColunaMes(worksheet, dtoExcelMes, colunaFinal, dtoExcelMes.TabelaDeDado.Columns.Count);
                        worksheet.Cell(LINHA_TABELA, colunaFinal).InsertData(dtoExcelMes.TabelaDeDado);
                        AdicionarEstilo(worksheet, dtoExcelMes.TabelaDeDado.Rows.Count, colunaFinal, dtoExcelMes.TabelaDeDado.Columns.Count);
                        colunaFinal += dtoExcelMes.TabelaDeDado.Columns.Count;
                    }
                }

                var caminhoBase = AppDomain.CurrentDomain.BaseDirectory;
                var caminhoParaSalvar = Path.Combine(caminhoBase, $"relatorios", $"{request.CodigoCorrelacao}");

                workbook.SaveAs($"{caminhoParaSalvar}.xlsx");
            }

            await servicoFila.PublicaFila(new PublicaFilaDto(new MensagemRelatorioProntoDto(), RotasRabbitSGP.RotaRelatoriosProntosSgp, ExchangeRabbit.Sgp, request.CodigoCorrelacao));
        }

        private void AdicionarColunaMes(IXLWorksheet worksheet, FrequenciaPorMesExcelDto dto, int colunaFinal, int totalColuna)
        {
            worksheet.Cell(LINHA_MES, colunaFinal).Value = $"Mês: {dto.Mes}";
            var rangeMes = worksheet.Range(LINHA_MES, colunaFinal, LINHA_MES, colunaFinal + totalColuna);
            rangeMes.Merge().Style.Font.Bold = true;
            AdicinarFonte(rangeMes);
            worksheet.Cell(LINHA_MES_FREQUENCIA, colunaFinal).Value = $"Frequência global do mês: {dto.FrequenciaGlobal}";
            var rangeFreq = worksheet.Range(LINHA_MES_FREQUENCIA, colunaFinal, LINHA_MES_FREQUENCIA, colunaFinal + totalColuna);
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

        private void AdicionarEstilo(IXLWorksheet worksheet, int totalRegistro, int colunaFinal, int ultimaColunaMes)
        {
            int ultimaColunaUsada = worksheet.LastColumnUsed().ColumnNumber();
            int ultimaLinhaUsada = worksheet.LastRowUsed().RowNumber();

            AdicionarEstiloCabecalho(worksheet, ultimaColunaUsada);
            AdicionarEstiloCorpo(worksheet, ultimaLinhaUsada, totalRegistro, colunaFinal, ultimaColunaMes);

            worksheet.ShowGridLines = false;

            worksheet.ColumnsUsed().AdjustToContents();
            worksheet.RowsUsed().AdjustToContents();
        }

        private void AdicionarEstiloCabecalho(IXLWorksheet worksheet, int ultimaColunaUsada)
        {
            AdicionarEstiloCabecalhoLinha(worksheet, ultimaColunaUsada, LINHA_CABECALHO_DRE_UE);
            AdicionarEstiloCabecalhoLinha(worksheet, ultimaColunaUsada, LINHA_NOME_SISTEMA);
            AdicionarEstiloCabecalhoLinha(worksheet, ultimaColunaUsada, LINHA_NOME_RELATORIO);
            AdicionarEstiloCabecalhoLinha(worksheet, ultimaColunaUsada, LINHA_CABECALHO_ESTUDANTE);
            AdicionarEstiloCabecalhoLinha(worksheet, ultimaColunaUsada, LINHA_CABECALHO_TURMA);
            AdicionarEstiloCabecalhoLinha(worksheet, ultimaColunaUsada, LINHA_CABECALHO_USUARIO_DATA);
            AdicionarEstiloCabecalhoLinha(worksheet, ultimaColunaUsada, LINHA_MES);
        }

        private void AdicionarEstiloCorpo(IXLWorksheet worksheet, int ultimaLinhaUsada, int totalRegistro, int colunaFinal, int ultimaColunaMes)
        {
            var ultimaColuna = (colunaFinal + ultimaColunaMes) - 1;

            worksheet.Range(LINHA_TABELA, colunaFinal, ultimaLinhaUsada, ultimaColuna).Style.Font.SetFontName("Arial");
            worksheet.Range(LINHA_TABELA, colunaFinal, ultimaLinhaUsada, ultimaColuna).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

            worksheet.Range(LINHA_TABELA, colunaFinal, ultimaLinhaUsada, ultimaColuna).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
            worksheet.Range(LINHA_TABELA, colunaFinal, ultimaLinhaUsada, ultimaColuna).Style.Border.SetOutsideBorderColor(XLColor.Black);

            worksheet.Range(LINHA_TABELA, colunaFinal, ultimaLinhaUsada, ultimaColuna).Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);
            worksheet.Range(LINHA_TABELA, colunaFinal, ultimaLinhaUsada, ultimaColuna).Style.Border.SetInsideBorderColor(XLColor.Black);

            worksheet.Range(LINHA_TABELA, colunaFinal, LINHA_TABELA, ultimaColuna).Style.Font.SetFontSize(10);
            worksheet.Range(LINHA_TABELA, colunaFinal, LINHA_TABELA, ultimaColuna).Style.Font.Bold = true;
            worksheet.Range(LINHA_TABELA, colunaFinal, LINHA_TABELA, ultimaColuna).Style.Fill.BackgroundColor = XLColor.LightGray;
            worksheet.Range(LINHA_SUBCABELHO, colunaFinal, LINHA_SUBCABELHO, ultimaColuna).Style.Font.SetFontSize(10);
            worksheet.Range(LINHA_SUBCABELHO, colunaFinal, LINHA_SUBCABELHO, ultimaColuna).Style.Font.Bold = true;
            worksheet.Range(LINHA_SUBCABELHO, colunaFinal, LINHA_SUBCABELHO, ultimaColuna).Style.Fill.BackgroundColor = XLColor.LightGray;
            worksheet.Range(LINHA_TABELA, colunaFinal + 2, ultimaLinhaUsada, ultimaColuna).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            worksheet.Range(LINHA_TABELA, colunaFinal, LINHA_TABELA + 1, colunaFinal + 1).Merge();
            worksheet.Range(LINHA_TABELA, ultimaColuna - 1, LINHA_TABELA + 1, ultimaColuna - 1).Merge();
            worksheet.Range(LINHA_TABELA, ultimaColuna, LINHA_TABELA + 1, ultimaColuna).Merge();

            Merge(worksheet, colunaFinal, totalRegistro);
            Merge(worksheet, ultimaColuna, totalRegistro);
        }

        private void Merge(IXLWorksheet worksheet, int coluna, int totalRegistro)
        {
            var linhaInicio = 15;
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
