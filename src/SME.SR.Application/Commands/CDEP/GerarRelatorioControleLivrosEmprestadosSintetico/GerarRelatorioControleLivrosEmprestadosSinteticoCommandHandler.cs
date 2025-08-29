using ClosedXML.Excel;
using ClosedXML.Excel.Drawings;
using MediatR;
using SME.SR.Application.Queries.CDEP.ObterRelatorioCDEPControleLivrosEmprestadoSintetico;
using SME.SR.Infra;
using SME.SR.Infra.Dtos.Relatorios.CDEP;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application.Commands.CDEP.GerarRelatorioControleLivrosEmprestadosSintetico
{
    public class GerarRelatorioControleLivrosEmprestadosSinteticoCommandHandler : GerarRelatorioControleLivrosBase, IRequestHandler<GerarRelatorioControleLivrosEmprestadosSinteticoCommand, MemoryStream>
    {
        private readonly IMediator mediator;

        public GerarRelatorioControleLivrosEmprestadosSinteticoCommandHandler(IMediator mediator)
        {
            this.mediator = mediator;
        }
        public async Task<MemoryStream> Handle(GerarRelatorioControleLivrosEmprestadosSinteticoCommand request, CancellationToken cancellationToken)
        {
            var livros = await mediator.Send(new ObterRelatorioCDEPControleLivrosEmprestadoQuery()
            {
                filtros = request.Filtros
            });

            if (!livros.Any())
                throw new NegocioException("Não possui informações.");

            var emprestimosAgrupados = livros
                           .GroupBy(e => e.Tombo)
                           .Select(g => new AcervoSolicitacaoSinteticoDto
                           {
                               Tombo = g.Key,
                               Titulo = g.First().Titulo,
                               QuantidadeEmprestimos = g.Count(),
                           })
                       .ToList();

            return GerarArquivoParaExcel(emprestimosAgrupados, request.Filtros.Usuario, request.Filtros.UsuarioRF);
        }

        public static MemoryStream GerarArquivoParaExcel(IEnumerable<AcervoSolicitacaoSinteticoDto> dadosDoRelatorio, string usuario, string rf)
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
            sheet.Cell(row, 2).Value = "Relatório de Controle de Livros Emprestados";
            sheet.Cell(row, 2).Style.Font.Bold = true;
            sheet.Cell(row, 2).Style.Font.FontSize = 12;
            sheet.Cell(row, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            sheet.Row(row).Height = 20;
            row += 2;

            // Informações do usuário
            sheet.Cell(row, 1).Value = "Usuário:";
            sheet.Cell(row, 2).Value = usuario;
            sheet.Cell(row, 3).Value = $"RF: {rf}";
            sheet.Cell(row, 5).Value = $"Data: {DateTime.Now.ToString("dd/MM/yyyy")}";
            row += 2;

            // Cabeçalho da tabela
            var headers = new[] {
                "Tombo", "Título", "Quantidade de empréstimos"
            };

            for (int col = 0; col < headers.Length; col++)
            {
                sheet.Cell(row, col + 1).Value = headers[col];
                sheet.Cell(row, col + 1).Style.Font.Bold = true;
                sheet.Cell(row, col + 1).Style.Fill.BackgroundColor = XLColor.LightGray;
                sheet.Cell(row, col + 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                sheet.Cell(row, col + 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            }

            row++;

            // Dados
            foreach (var item in dadosDoRelatorio)
            {
                sheet.Cell(row, 1).Value = item.Tombo;
                sheet.Cell(row, 2).Value = item.Titulo;
                sheet.Cell(row, 3).Value = item.QuantidadeEmprestimos;
                row++;
            }

            sheet.Columns().AdjustToContents();
            workbook.SaveAs(stream);
            stream.Position = 0;
            return stream;
        }

    }
}
