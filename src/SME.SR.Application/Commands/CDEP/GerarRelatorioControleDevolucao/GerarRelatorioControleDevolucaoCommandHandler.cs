using ClosedXML.Excel;
using ClosedXML.Excel.Drawings;
using MediatR;
using SME.SR.Application.Queries.CDEP.ObterRelatorioCDEPControleLivrosDevolvidos;
using SME.SR.Infra;
using SME.SR.Infra.Dtos.Relatorios.CDEP;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application.Commands.CDEP.GerarRelatorioControleDevolucao
{
    public class GerarRelatorioControleDevolucaoCommandHandler : GerarRelatorioControleLivrosBase, IRequestHandler<GerarRelatorioControleDevolucaoCommand, MemoryStream>
    {
        private readonly IMediator mediator;

        public GerarRelatorioControleDevolucaoCommandHandler(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public async Task<MemoryStream> Handle(GerarRelatorioControleDevolucaoCommand request, CancellationToken cancellationToken)
        {
            var livros = await mediator.Send(new ObterRelatorioCDEPControleLivrosDevolvidosQuery()
            {
                Filtros = request.Filtros
            });

            if (!livros.Any())
                throw new NegocioException("Não possui informações.");

            return GerarRelatorioLivros(livros, request.Filtros.Usuario, request.Filtros.UsuarioRF);
        }

        public static MemoryStream GerarRelatorioLivros(IEnumerable<AcervoDevolucaoDto> acervos, string usuario, string rf, string autor = null)
        {
            var stream = new MemoryStream();
            using var workbook = new XLWorkbook();
            var sheet = workbook.Worksheets.Add("Relatório");

            int row = 1;
            row++;

            // Logo
            var image = sheet.AddPicture(ObterLogo())
                .MoveTo(sheet.Cell(row, 1))
                .WithSize(100, 60);

            // Título institucional
            sheet.Range(row, 2, row, 6).Merge();
            sheet.Cell(row, 2).Value = "CDEP - CENTRO DE DOCUMENTAÇÃO DA EDUCAÇÃO PAULISTANA";
            sheet.Cell(row, 2).Style.Font.Bold = true;
            sheet.Cell(row, 2).Style.Font.FontSize = 14;
            sheet.Cell(row, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            sheet.Row(row).Height = 25;
            row++;

            // Título do relatório
            sheet.Range(row, 2, row, 6).Merge();
            sheet.Cell(row, 2).Value = "Relatório de Controle Devolução de Livros";
            sheet.Cell(row, 2).Style.Font.Bold = true;
            sheet.Cell(row, 2).Style.Font.FontSize = 12;
            sheet.Cell(row, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            sheet.Row(row).Height = 20;
            row += 2;

            // Informações do usuário
            sheet.Cell(row, 1).Value = "Usuário:";
            sheet.Cell(row, 2).Value = usuario;
            sheet.Cell(row, 3).Value = $"RF: {rf}";

            if (!string.IsNullOrWhiteSpace(autor))
            {
                sheet.Cell(row, 4).Value = "Autor:";
                sheet.Cell(row, 5).Value = autor;
            }

            sheet.Cell(row, 6).Value = $"Data: {DateTime.Now.ToString("dd/MM/yyyy")}";
            row += 2;

            // Cabeçalho da tabela
            var headers = new[] {
                                    "Solicitante", "Tombo", "Título", "Telefone", "Email",
                                    "Data do empréstimo", "Data de devolução", "Dias em atraso"
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
            foreach (var acervo in acervos)
            {
                sheet.Cell(row, 1).Value = $"{acervo.Solicitante} ({acervo.Login})";
                sheet.Cell(row, 2).Value = acervo.Tombo;
                sheet.Cell(row, 3).Value = acervo.Titulo;
                sheet.Cell(row, 4).Value = acervo.Telefone;
                sheet.Cell(row, 5).Value = acervo.Email;
                sheet.Cell(row, 6).Value = acervo.DataEmprestimo.ToString("dd/MM/yyyy");
                sheet.Cell(row, 7).Value = acervo.DataDevolucao.ToString("dd/MM/yyyy");
                sheet.Cell(row, 8).Value = acervo.DiasAtraso;
                row++;
            }

            sheet.Columns().AdjustToContents();
            workbook.SaveAs(stream);
            stream.Position = 0;
            return stream;
        }
    }
}
