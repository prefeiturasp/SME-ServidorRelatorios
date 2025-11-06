using ClosedXML.Excel;
using MediatR;
using SME.SR.Application.Queries.CDEP.ObterRelatorioCDEPTitulosMaisPesquisados;
using SME.SR.Infra;
using SME.SR.Infra.Dtos.Relatorios.CDEP;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application.Commands.CDEP.GerarRelatorioTitulosMaisPesquisados
{
    public class GerarRelatorioTitulosMaisPesquisadosCommandHandler : GerarRelatorioControleLivrosBase, IRequestHandler<GerarRelatorioTitulosMaisPesquisadosCommand, MemoryStream>
    {
        private readonly IMediator mediator;
        public GerarRelatorioTitulosMaisPesquisadosCommandHandler(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public async Task<MemoryStream> Handle(GerarRelatorioTitulosMaisPesquisadosCommand request, CancellationToken cancellationToken)
        {
            var titulosMaisPesquisados = await mediator.Send(new ObterRelatorioCDEPTitulosMaisPesquisadosQuery()
            {
                Filtros = request.Filtros
            });

            if(!titulosMaisPesquisados.Any())
                throw new NegocioException("Não possui informações.");

            return GerarArquivoParaExcel(titulosMaisPesquisados, request.Filtros);
        }

        private MemoryStream GerarArquivoParaExcel(IEnumerable<RelatorioTitulosMaisPesquisadosDto> dadosDoRelatorio, FiltroRelatorioTitulosMaisPesquisados filtros)
        {
            var stream = new MemoryStream();
            using var workbook = new XLWorkbook();
            var sheet = workbook.Worksheets.Add("Relatório");

            int row = 1;
            row++;

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
            sheet.Cell(row, 2).Value = "Relatório de Títulos Mais Pesquisados";
            sheet.Cell(row, 2).Style.Font.Bold = true;
            sheet.Cell(row, 2).Style.Font.FontSize = 12;
            sheet.Cell(row, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            sheet.Row(row).Height = 20;
            row += 2;

            // Informações do usuário
            sheet.Cell(row, 1).Value = "Usuário:";
            sheet.Cell(row, 2).Value = filtros.Usuario;
            sheet.Cell(row, 3).Value = $"RF: {filtros.UsuarioRF}";
            sheet.Cell(row, 4).Value = $"Período: {filtros.DataInicio:dd/MM/yyyy} a {filtros.DataFim:dd/MM/yyyy}";
            sheet.Cell(row, 6).Value = $"Data: {DateTime.Now.ToString("dd/MM/yyyy")}";
            row += 2;

            // Cabeçalho do relatório
            var headers = new[] { "Título da pesquisa", "Quantidade de Pesquisas" };

            for (int col = 0; col < headers.Length; col++)
            {
                sheet.Cell(row, col + 1).Value = headers[col];
                sheet.Cell(row, col + 1).Style.Font.Bold = true;
                sheet.Cell(row, col + 1).Style.Fill.BackgroundColor = XLColor.LightGray;
                sheet.Cell(row, col + 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                sheet.Cell(row, col + 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            }

            row++;

            // Dados do relatório
            foreach (var item in dadosDoRelatorio)
            {
                sheet.Cell(row, 1).Value = item.TermoNormalizado;
                sheet.Cell(row, 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                sheet.Cell(row, 2).Value = item.Quantidade;
                sheet.Cell(row, 2).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                row++;
            }

            sheet.Columns().AdjustToContents();
            workbook.SaveAs(stream);
            stream.Position = 0;
            return stream;
        }
    }
}
