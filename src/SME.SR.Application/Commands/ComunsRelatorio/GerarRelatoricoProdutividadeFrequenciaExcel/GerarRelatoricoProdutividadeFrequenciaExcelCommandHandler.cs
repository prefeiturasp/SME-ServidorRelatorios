using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using MediatR;
using SME.SR.Data;
using SME.SR.HtmlPdf;
using SME.SR.Infra;
using SME.SR.Infra.Utilitarios;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class GerarRelatoricoProdutividadeFrequenciaExcelCommandHandler : AsyncRequestHandler<GerarRelatoricoProdutividadeFrequenciaExcelCommand>
    {
        private readonly IMediator mediator;
        private readonly IServicoFila servicoFila;
        private const int LINHA_NOME_SISTEMA = 2;
        private const int LINHA_NOME_RELATORIO = 3;
        private const int LINHA_CABECALHO_TITULO = 7;
        
        private const int LINHA_INICIO_REGISTROS = 8;

        private List<(string nmColuna, string titulo)> ColunasCabecalhoAnalitico = new List<(string nmColuna, string titulo)>()
        {
            ("DescricaoDre", "Nome DRE"),
            ("CodigoUe", "Código UE"),
            ("DescricaoUe", "Nome UE"),
            ("NomeProfessor", "Nome Professor"),
            ("RfProfessor", "RF Professor"),
            ("Bimestre", "Bimestre"),
            ("Modalidade", "Modalidade"),
            ("DescricaoTurma", "Turma"),
            ("NomeComponenteCurricular", "Componente curricular"),
            ("DataAula", "Data aula"),
            ("DataRegistroFrequencia", "Data registro de frequência"),
            ("DiferenciaDiasDataAulaRegistroFrequencia", "Diferença entre data da aula e registro"),
        };

        private List<(string nmColuna, string titulo)> ColunasCabecalhoMediaProf = new List<(string nmColuna, string titulo)>()
        {
            ("DescricaoDre", "Nome DRE"),
            ("CodigoUe", "Código UE"),
            ("DescricaoUe", "Nome UE"),
            ("NomeProfessor", "Nome Professor"),
            ("RfProfessor", "RF Professor"),
            ("Bimestre", "Bimestre"),
            ("MediaDiasDataAulaRegistroFrequencia", "Média de dias entre data da aula e registro"),
        };

        private List<(string nmColuna, string titulo)> ColunasCabecalhoMediaUe = new List<(string nmColuna, string titulo)>()
        {
            ("DescricaoDre", "Nome DRE"),
            ("CodigoUe", "Código UE"),
            ("DescricaoUe", "Nome UE"),
            ("Bimestre", "Bimestre"),
            ("MediaDiasDataAulaRegistroFrequencia", "Média de dias entre data da aula e registro"),
        };

        public GerarRelatoricoProdutividadeFrequenciaExcelCommandHandler(IMediator mediator, IServicoFila servicoFila)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this.servicoFila = servicoFila ?? throw new ArgumentNullException(nameof(servicoFila));
        }

        protected override async Task Handle(GerarRelatoricoProdutividadeFrequenciaExcelCommand request, CancellationToken cancellationToken)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add($"produtividade_frequencia");
                var colunasCabecalho = ObterColunasCabecalhoRelatorio(request.Filtro.TipoRelatorioProdutividade);
                MontarCabecalhoGeral(worksheet, colunasCabecalho);
                MontarCabecalhoTitulos(worksheet, colunasCabecalho);

                var linhaFinal = LINHA_INICIO_REGISTROS;
                foreach (var dto in request.Consolidacoes)
                {
                    var qdadeLinhasInseridas = MontarLinhaRegistros(worksheet, linhaFinal, dto, colunasCabecalho);
                    linhaFinal += qdadeLinhasInseridas;
                }

                AdicionarEstiloGeralTodasColunas(worksheet, LINHA_CABECALHO_TITULO, linhaFinal-1, colunasCabecalho);
                var caminhoBase = AppDomain.CurrentDomain.BaseDirectory;
                var caminhoParaSalvar = Path.Combine(caminhoBase, $"relatorios", $"{request.CodigoCorrelacao}");

                workbook.SaveAs($"{caminhoParaSalvar}.xlsx");
            }
            await servicoFila.PublicaFila(new PublicaFilaDto(new MensagemRelatorioProntoDto(), RotasRabbitSGP.RotaRelatoriosProntosSgp, ExchangeRabbit.Sgp, request.CodigoCorrelacao));
        }

        private List<(string nmColuna, string titulo)> ObterColunasCabecalhoRelatorio(TipoRelatorioProdutividadeFrequencia tipoRelatorio)
        {
            return tipoRelatorio switch
            {
                TipoRelatorioProdutividadeFrequencia.MédiaPorUE => ColunasCabecalhoMediaUe,
                TipoRelatorioProdutividadeFrequencia.MédiaPorProfessor => ColunasCabecalhoMediaProf,
                _ => ColunasCabecalhoAnalitico,
            };
        }

        private DataTable ObterDataCabecalhoTitulos(List<(string nmColuna, string titulo)> colunasCabecalho)
        {
            var data = new DataTable();
            colunasCabecalho.ForEach(c =>
                data.Columns.Add(c.nmColuna)
            );

            DataRow titulos = data.NewRow();
            colunasCabecalho.ForEach(c =>
                titulos[c.nmColuna] = c.titulo
            );
            data.Rows.Add(titulos);
            return data;
        }

        private DataTable ObterDataRegistros(ConsolidacaoProdutividadeFrequenciaDto dto, List<(string nmColuna, string titulo)> colunasCabecalho)
        {
            var data = new DataTable();
            colunasCabecalho.ForEach(c =>
                data.Columns.Add(c.nmColuna)
            );

            DataRow valores = data.NewRow();
            Type tipo = dto.GetType();
            foreach (var coluna in colunasCabecalho)
            {
                PropertyInfo prop = tipo.GetProperty(coluna.nmColuna);
                if (prop != null)
                    valores[coluna.nmColuna] = prop.PropertyType == typeof(DateTime) 
                                                ? ((DateTime)prop.GetValue(dto)).ToString("dd/MM/yyyy")
                                                : prop.GetValue(dto).ToString();
            }
            data.Rows.Add(valores);
            return data;
        }

        private void MontarCabecalhoTitulos(IXLWorksheet worksheet, List<(string nmColuna, string titulo)> colunasCabecalho)
        {
            worksheet.Cell(LINHA_CABECALHO_TITULO, 1).InsertData(ObterDataCabecalhoTitulos(colunasCabecalho));
            AdicionarFundoCinzaClaro(worksheet.Range(LINHA_CABECALHO_TITULO, 1, LINHA_CABECALHO_TITULO, colunasCabecalho.Count()));
            AdicionarAlinhamentoCentro(worksheet.Range(LINHA_CABECALHO_TITULO, 1, LINHA_CABECALHO_TITULO, colunasCabecalho.Count()));
        }

        private void AdicionarEstiloRegistros(IXLWorksheet worksheet, int linhaInicial, int linhaFinal, List<(string nmColuna, string titulo)> colunasCabecalho)
        {
            foreach (var coluna in colunasCabecalho)
            {
                var indice = colunasCabecalho.IndexOf((coluna.nmColuna, coluna.titulo)) + 1;
                worksheet.Range(linhaInicial, indice, linhaFinal, indice).Merge();
                AdicionarAlinhamentoCentro(worksheet.Range(linhaInicial, indice, linhaFinal, indice));
            }
        }

        private int MontarLinhaRegistros(IXLWorksheet worksheet, int linhaFinal, ConsolidacaoProdutividadeFrequenciaDto dto, List<(string nmColuna, string titulo)> colunasCabecalho)
        {
            worksheet.Cell(linhaFinal, 1).InsertData(ObterDataRegistros(dto, colunasCabecalho));
            var qdadeLinhasInseridas = 1;
            AdicionarEstiloRegistros(worksheet, linhaFinal, linhaFinal + qdadeLinhasInseridas - 1, colunasCabecalho);
            return qdadeLinhasInseridas;
        }


        private void MontarCabecalhoGeral(IXLWorksheet worksheet, List<(string nmColuna, string titulo)> colunasCabecalho)
        {
            worksheet.AddPicture(ObterLogo())
                .MoveTo(worksheet.Cell(2, 1))
                .Scale(0.15);

            worksheet.Cell(LINHA_NOME_SISTEMA, 4).Value = "SGP - Sistema de Gestão Pedagógica";
            var linhaNomeSistema = worksheet.Range(LINHA_NOME_SISTEMA, 4, LINHA_NOME_SISTEMA, colunasCabecalho.Count());
            linhaNomeSistema.Merge().Style.Font.Bold = true;
            AdicionarFonte(linhaNomeSistema);

            worksheet.Cell(LINHA_NOME_RELATORIO, 4).Value = "Relatório de Produtividade de Frequência";
            AdicionarFonte(worksheet.Range(LINHA_NOME_RELATORIO, 4, LINHA_NOME_RELATORIO, colunasCabecalho.Count()));  
        }

        private Stream ObterLogo()
        {
            string base64Logo = SmeConstants.LogoSmeMono.Substring(SmeConstants.LogoSmeMono.IndexOf(',') + 1);
            return new MemoryStream(Convert.FromBase64String(base64Logo));
        }

        private void AdicionarFonte(IXLRange range)
        {
            range.Style.Font.FontSize = 10;
            range.Style.Font.FontName = "Arial";
        }

        private void AdicionarAlinhamentoCentro(IXLRange range)
        {
           range.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
           range.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
        }
        private void AdicionarQuebraAutoamticaTexto(IXLRange range)
        {
            range.Style.Alignment.WrapText = true;
        }

        private void AdicionarBorda(IXLRange range)
        {
            range.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
            range.Style.Border.SetOutsideBorderColor(XLColor.Black);
        }

        private void AdicionarFundoCinzaClaro(IXLRange range)
        {
            range.Style.Fill.SetBackgroundColor(XLColor.LightGray);
        }

        private void AdicionarEstiloGeralTodasColunas(IXLWorksheet worksheet, int linhaInicial, int linhaFinal, List<(string nmColuna, string titulo)> colunasCabecalho)
        {
            worksheet.ShowGridLines = false;
            foreach (var coluna in colunasCabecalho)
            {
                var indice = colunasCabecalho.IndexOf((coluna.nmColuna, coluna.titulo)) + 1;
                for (int linha = linhaInicial; linha <= linhaFinal; linha++)
                {
                    var range = worksheet.Range(linha, indice, linha, indice);
                    AdicionarBorda(range);
                    AdicionarFonte(range);
                    AdicionarQuebraAutoamticaTexto(range);
                }
            }
            worksheet.Columns().AdjustToContents();
            worksheet.ColumnsUsed().AdjustToContents();
            worksheet.RowsUsed().AdjustToContents();
        }


    }
}
