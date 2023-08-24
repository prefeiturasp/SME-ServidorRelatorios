using ClosedXML.Excel;
using MediatR;
using Sentry;
using SME.SR.HtmlPdf;
using SME.SR.Infra;
using SME.SR.Infra.Utilitarios;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class GerarRelatorioAtaFinalExcelCommandHandler : IRequestHandler<GerarRelatorioAtaFinalExcelCommand, Unit>
    {
        private readonly IMediator mediator;
        private readonly IServicoFila servicoFila;
        private const int LINHA_CABECALHO_DRE = 6;
        private const int LINHA_CABECALHO_CICLO = 7;

        private const int LINHA_GRUPOS = 9;
        private const int LINHA_COMPONENTES = 10;

        public GerarRelatorioAtaFinalExcelCommandHandler(IMediator mediator, IServicoFila servicoFila)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this.servicoFila = servicoFila ?? throw new ArgumentNullException(nameof(servicoFila));
        }

        public async Task<Unit> Handle(GerarRelatorioAtaFinalExcelCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (!request.ObjetoExportacao.Any())
                    throw new NegocioException("Não foi possível localizar o objeto de consulta.");

                var lstCodigosCorrelacao = new Dictionary<Guid, string>();

                var dadosAgrupadosTurma = request.ObjetoExportacao.GroupBy(g => g.Cabecalho);
                var modalidade = request.ObjetoExportacao.Select(o => o.Modalidade).FirstOrDefault();

                for (int i = 0; i < dadosAgrupadosTurma.Count(); i++)
                {
                    var codigoCorrelacao = Guid.NewGuid();
                    var mensagem = new MensagemInserirCodigoCorrelacaoDto(TipoRelatorio.ConselhoClasseAtaFinal, TipoFormatoRelatorio.Xlsx);
                    await mediator.Send(new InserirFilaRabbitCommand(new PublicaFilaDto(mensagem, RotasRabbitSGP.RotaRelatorioCorrelacaoInserir, ExchangeRabbit.Sgp, codigoCorrelacao, request.UsuarioRf)));

                    using (var workbook = new XLWorkbook())
                    {
                        await mediator.Send(new InserirFilaRabbitCommand(new PublicaFilaDto(mensagem, RotasRabbitSGP.RotaRelatorioCorrelacaoInserir, ExchangeRabbit.Sgp, codigoCorrelacao, request.UsuarioRf)));

                        var worksheet = workbook.Worksheets.Add(request.NomeWorkSheet);

                        var objetoExportacao = dadosAgrupadosTurma.ElementAt(i);

                        var tabelaDados = request.TabelasDados.ElementAt(i);

                        MontarCabecalho(worksheet, objetoExportacao.Key, tabelaDados.Columns.Count);

                        worksheet.Cell(LINHA_GRUPOS, 1).InsertData(tabelaDados);

                        MergearTabela(worksheet, tabelaDados);

                        AdicionarEstilo(worksheet, tabelaDados);
                        
                        worksheet.Column(worksheet.Columns().Count()).Delete();

                        var caminhoBase = AppDomain.CurrentDomain.BaseDirectory;
                        var caminhoParaSalvar = Path.Combine(caminhoBase, $"relatorios", $"{codigoCorrelacao}");

                        workbook.SaveAs($"{caminhoParaSalvar}.xlsx");

                        lstCodigosCorrelacao.Add(codigoCorrelacao, objetoExportacao.Key.Turma);
                    }
                }

                foreach (var codigoCorrelacao in lstCodigosCorrelacao)
                    await servicoFila.PublicaFila(new PublicaFilaDto(ObterNotificacao(modalidade, codigoCorrelacao.Value), RotasRabbitSGP.RotaRelatoriosProntosSgp, ExchangeRabbit.Sgp, codigoCorrelacao.Key));

                return await Task.FromResult(Unit.Value);
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw ex;
            }
        }

        private MensagemRelatorioProntoDto ObterNotificacao(Modalidade modalidade, string turma)
        {
            return new MensagemRelatorioProntoDto()
            {
                MensagemTitulo = $"Relatório Ata final de resultados - {turma}"
            };
        }

        private void AdicionarEstilo(IXLWorksheet worksheet, DataTable tabelaDados)
        {
            int ultimaColunaUsada = worksheet.LastColumnUsed().ColumnNumber();
            int ultimaLinhaUsada = worksheet.LastRowUsed().RowNumber();

            AdicionarEstiloCabecalho(worksheet, ultimaColunaUsada);
            AdicionarEstiloCorpo(worksheet, tabelaDados, ultimaColunaUsada, ultimaLinhaUsada);

            worksheet.ShowGridLines = false;

            worksheet.ColumnsUsed().AdjustToContents();
            worksheet.RowsUsed().AdjustToContents();
        }

        private void AdicionarEstiloCorpo(IXLWorksheet worksheet, DataTable tabelaDados, int ultimaColunaUsada, int ultimaLinhaUsada)
        {
            worksheet.Range(LINHA_GRUPOS, 1, ultimaLinhaUsada, ultimaColunaUsada).Style.Font.SetFontName("Arial");
            worksheet.Range(LINHA_GRUPOS, 1, ultimaLinhaUsada, ultimaColunaUsada).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

            worksheet.Range(LINHA_GRUPOS, 1, ultimaLinhaUsada, ultimaColunaUsada).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
            worksheet.Range(LINHA_GRUPOS, 1, ultimaLinhaUsada, ultimaColunaUsada).Style.Border.SetOutsideBorderColor(XLColor.Black);

            worksheet.Range(LINHA_GRUPOS, 1, ultimaLinhaUsada, ultimaColunaUsada).Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);
            worksheet.Range(LINHA_GRUPOS, 1, ultimaLinhaUsada, ultimaColunaUsada).Style.Border.SetInsideBorderColor(XLColor.Black);

            worksheet.Range(LINHA_GRUPOS, 1, LINHA_GRUPOS, ultimaColunaUsada).Style.Font.SetFontSize(10);

            worksheet.Range(LINHA_GRUPOS, 1, ultimaLinhaUsada, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            worksheet.Range(LINHA_GRUPOS, ultimaColunaUsada - 4, LINHA_GRUPOS, ultimaColunaUsada).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            worksheet.Range(LINHA_GRUPOS, ultimaColunaUsada - 4, LINHA_GRUPOS, ultimaColunaUsada).Style.Font.SetBold(true);

            worksheet.Range(LINHA_COMPONENTES, 1, LINHA_COMPONENTES, ultimaColunaUsada).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            worksheet.Range(LINHA_COMPONENTES, 1, LINHA_COMPONENTES, ultimaColunaUsada).Style.Font.SetFontSize(7);

            worksheet.Range(LINHA_COMPONENTES, 3, ultimaLinhaUsada, ultimaColunaUsada).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            worksheet.Range(LINHA_COMPONENTES, 3, LINHA_COMPONENTES + 1, ultimaColunaUsada).Style.Font.SetBold(true);

            worksheet.Range(LINHA_COMPONENTES + 1, 1, ultimaLinhaUsada, ultimaColunaUsada).Style.Font.SetFontSize(5);

            var linhasAlunosInativos = tabelaDados.AsEnumerable().Where(r => r.Field<string>("Inativo") == "True");
            foreach (DataRow linha in linhasAlunosInativos)
            {
                int indice = tabelaDados.Rows.IndexOf(linha);
                worksheet.Range(LINHA_GRUPOS + indice, 1, LINHA_GRUPOS + indice, ultimaColunaUsada).Style.Fill.SetBackgroundColor(XLColor.LightGray);
            }

            foreach (var celula in worksheet.Range(LINHA_COMPONENTES + 2, 1, ultimaLinhaUsada, ultimaColunaUsada).CellsUsed().Where(c => decimal.TryParse(c.Value.ToString().Replace(",", "."), out _)))
            {
                if (celula.Value.ToString().IndexOf(',') >= 0)
                    celula.SetValue(Convert.ToDecimal(celula.Value.ToString(), new CultureInfo("pt-BR")));

                celula.SetDataType(XLDataType.Number);
            }
        }

        private void AdicionarEstiloCabecalho(IXLWorksheet worksheet, int ultimaColunaUsada)
        {
            worksheet.Range(LINHA_CABECALHO_DRE, 1, LINHA_CABECALHO_CICLO, ultimaColunaUsada).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
            worksheet.Range(LINHA_CABECALHO_DRE, 1, LINHA_CABECALHO_CICLO, ultimaColunaUsada).Style.Border.SetOutsideBorderColor(XLColor.Black);

            worksheet.Range(LINHA_CABECALHO_DRE, 1, LINHA_CABECALHO_CICLO, ultimaColunaUsada).Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);
            worksheet.Range(LINHA_CABECALHO_DRE, 1, LINHA_CABECALHO_CICLO, ultimaColunaUsada).Style.Border.SetInsideBorderColor(XLColor.Black);

            worksheet.Range(LINHA_CABECALHO_DRE, 1, LINHA_CABECALHO_CICLO, ultimaColunaUsada).Style.Font.SetFontSize(10);
            worksheet.Range(LINHA_CABECALHO_DRE, 1, LINHA_CABECALHO_CICLO, ultimaColunaUsada).Style.Font.SetFontName("Arial");
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

                if (grupo.Key.Equals("Grupo99Normal"))
                {
                    worksheet.Range(LINHA_GRUPOS, indiceInicial, LINHA_COMPONENTES, indiceInicial + contagemCelulas).Merge();
                }
                else
                {
                    if (grupo.Key.Contains("Regencia"))
                        worksheet.Range(LINHA_GRUPOS, indiceInicial, LINHA_GRUPOS, indiceInicial + contagemCelulas).Merge();
                    
                    var componentesaAux = listaTitulos.Where(t => t.StartsWith(grupo.Key));      
                    var componentes = componentesaAux.GroupBy(g => g.Substring(g.IndexOf("Componente"), g.LastIndexOf('_') - g.IndexOf("Componente")));

                    foreach (var componente in componentes)
                    {
                        itemInicial = componente.FirstOrDefault();
                        indiceInicial = tabelaDados.Columns[itemInicial].Ordinal + 1;
                        contagemCelulas = componente.Count() - 1;
                        worksheet.Range(LINHA_COMPONENTES, indiceInicial, LINHA_COMPONENTES, indiceInicial + contagemCelulas).Merge();
                        if (!grupo.Key.Contains("Regencia"))
                            worksheet.Range(LINHA_GRUPOS, indiceInicial, LINHA_COMPONENTES, indiceInicial + contagemCelulas).Merge();

                    }
                }
            }
        }

        private void MontarCabecalho(IXLWorksheet worksheet, ConselhoClasseAtaFinalCabecalhoDto dadosCabecalho, int totalColunas)
        {
            worksheet.AddPicture(ObterLogo())
                .MoveTo(worksheet.Cell(2, 2))
                .Scale(0.15);

            int indiceColunaTitulo = (int)(totalColunas * 0.8) + 1;

            worksheet.Row(2).Cell(indiceColunaTitulo).Value = "SGP - Sistema de Gestão Pedagógica";
            worksheet.Range(2, indiceColunaTitulo, 2, totalColunas).Merge().Style.Font.Bold = true;
            worksheet.Range(2, indiceColunaTitulo, 2, totalColunas).Style.Font.FontSize = 10;
            worksheet.Range(2, indiceColunaTitulo, 2, totalColunas).Style.Font.FontName = "Arial";

            worksheet.Row(3).Cell(indiceColunaTitulo).Value = "ATA FINAL DE RESULTADOS";
            worksheet.Range(3, indiceColunaTitulo, 3, totalColunas).Merge().Style.Font.FontSize = 10;
            worksheet.Range(3, indiceColunaTitulo, 3, totalColunas).Style.Font.FontName = "Arial";

            int indiceFinal = SetarItemCabecalho(worksheet, $"DRE: {dadosCabecalho.Dre}", 0.4, LINHA_CABECALHO_DRE, 1, totalColunas);
            indiceFinal = SetarItemCabecalho(worksheet, $"Unidade Escolar (UE): {dadosCabecalho.Ue}", 0.4, LINHA_CABECALHO_DRE, indiceFinal, totalColunas);
            SetarItemCabecalho(worksheet, $"Turma: {dadosCabecalho.Turma}", 0.2, LINHA_CABECALHO_DRE, indiceFinal, totalColunas);

            indiceFinal = SetarItemCabecalho(worksheet, $"Ciclo: {dadosCabecalho.Ciclo}", 0.4, LINHA_CABECALHO_CICLO, 1, totalColunas);
            indiceFinal = SetarItemCabecalho(worksheet, $"Ano Letivo: {dadosCabecalho.AnoLetivo}", 0.4, LINHA_CABECALHO_CICLO, indiceFinal, totalColunas);
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
