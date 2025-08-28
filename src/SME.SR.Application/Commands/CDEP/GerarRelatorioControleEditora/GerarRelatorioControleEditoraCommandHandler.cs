using ClosedXML.Excel;
using ClosedXML.Excel.Drawings;
using MediatR;
using SME.SR.Application.Commands.CDEP.GerarRelatorioControleEditora;
using SME.SR.Application.Queries.CDEP.ObterRelatorioCDEPControleEditora;
using SME.SR.Infra;
using SME.SR.Infra.Dtos.Relatorios.CDEP;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application.Commands.CDEP.GerarRelatorioControleAcervo
{
    public class GerarRelatorioControleEditoraCommandHandler : GerarRelatorioControleLivrosBase, IRequestHandler<GerarRelatorioControleEditoraCommand, MemoryStream>
    {
        private readonly IMediator mediator;

        public GerarRelatorioControleEditoraCommandHandler(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public async Task<MemoryStream> Handle(GerarRelatorioControleEditoraCommand request, CancellationToken cancellationToken)
        {
            var acervos = await mediator.Send(new ObterRelatorioCDEPControleEditoraQuery()
            {
                filtros = request.Filtros
            });

            if (!acervos.Any())
                throw new NegocioException("Não possui informações.");

            return GerarArquivoParaExcel(acervos, request.Filtros.Usuario, request.Filtros.UsuarioRF);
        }

        private static MemoryStream GerarArquivoParaExcel(IEnumerable<ControleEditoraDTO> acervos, string usuario, string rf)
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
            sheet.Range(row, 2, row, 6).Merge();
            sheet.Cell(row, 2).Value = "CDEP - CENTRO DE DOCUMENTAÇÃO DA EDUCAÇÃO PAULISTANA";
            sheet.Cell(row, 2).Style.Font.Bold = true;
            sheet.Cell(row, 2).Style.Font.FontSize = 14;
            sheet.Cell(row, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            sheet.Row(row).Height = 25;
            row++;

            // Título do relatório
            sheet.Range(row, 2, row, 6).Merge();
            sheet.Cell(row, 2).Value = "Relatório de Controle de Editora";
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
                            "Editora", "Tombo/Código", "Título", "Situação do empréstimo"
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

            // Agrupamento e dados
            var acervosGroup = acervos.GroupBy(x => new { x.Editora, x.Titulo, x.SituacaoEmprestimo }).ToList();

            foreach (var grupo in acervosGroup)
            {
                var primeiro = grupo.First();

                sheet.Cell(row, 1).Value = primeiro.Editora;
                sheet.Cell(row, 2).Value = primeiro.Tombo;
                sheet.Cell(row, 3).Value = primeiro.Titulo;
                sheet.Cell(row, 4).Value = primeiro.SituacaoEmprestimo != null && primeiro.SituacaoEmprestimo != 0
                    ? ObterDescricaoSituacao(primeiro.SituacaoEmprestimo)
                    : string.Empty;

                row++;
            }

            sheet.Columns().AdjustToContents();
            workbook.SaveAs(stream);
            stream.Position = 0;
            return stream;
        }

    }
}
