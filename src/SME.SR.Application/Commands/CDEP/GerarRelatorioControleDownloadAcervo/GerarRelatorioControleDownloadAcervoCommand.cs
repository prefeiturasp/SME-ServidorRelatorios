using ClosedXML.Excel;
using MediatR;
using SME.SR.Application.Queries.CDEP.ObterRelatorioCDEPControleDownloadAcervo;
using SME.SR.Infra;
using SME.SR.Infra.Dtos.Relatorios.CDEP;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application.Commands.CDEP.GerarRelatorioControleDownloadAcervo
{
    public class GerarRelatorioControleDownloadAcervoCommand : IRequest<MemoryStream>
    {
        public GerarRelatorioControleDownloadAcervoCommand(FiltroRelatorioControleDownloadAcervo filtros)
        {
            Filtros = filtros;
        }
        public FiltroRelatorioControleDownloadAcervo Filtros { get; set; }
    }

    public class GerarRelatorioControleDownloadAcervoCommandHandler : GerarRelatorioControleLivrosBase, IRequestHandler<GerarRelatorioControleDownloadAcervoCommand, MemoryStream>
    {
        private readonly IMediator mediator;
        public GerarRelatorioControleDownloadAcervoCommandHandler(IMediator mediator)
        {
            this.mediator = mediator;
        }
        public async Task<MemoryStream> Handle(GerarRelatorioControleDownloadAcervoCommand request, CancellationToken cancellationToken)
        {
            var controleDownloadAcervo = await mediator.Send(new ObterRelatorioCDEPControleDownloadAcervoQuery()
            {
                Filtros = request.Filtros
            });
            if (controleDownloadAcervo == null)
                throw new NegocioException("Não possui informações.");
            return GerarArquivoParaExcel(controleDownloadAcervo, request.Filtros);
        }

        private MemoryStream GerarArquivoParaExcel(IEnumerable<ControleDownloadAcervoDTO> dadosDoRelatorio, FiltroRelatorioControleDownloadAcervo filtros)
        {
            var stream = new MemoryStream();
            using var workbook = new XLWorkbook();
            var sheet = workbook.Worksheets.Add("Relatório");

            int row = 2;

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
            sheet.Cell(row, 2).Value = "Relatório de Controle de Download de Acervo";
            sheet.Cell(row, 2).Style.Font.Bold = true;
            sheet.Cell(row, 2).Style.Font.FontSize = 12;
            sheet.Cell(row, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            sheet.Row(row).Height = 20;
            row += 2;

            // Informações do usuário
            sheet.Cell(row, 1).Value = "Usuário:";
            sheet.Cell(row, 2).Value = filtros.Usuario;
            sheet.Cell(row, 3).Value = $"RF: {filtros.UsuarioRF}";
            sheet.Cell(row, 6).Value = $"Data: {DateTime.Now.ToString("dd/MM/yyyy")}";
            row += 2;

            // Cabeçalhos das colunas
            var headers = new List<string>
            {
                "Tipo do Acervo",
                "Título do Acervo",
                "Tombo/Código",
                "Qtde de vezes baixado"
            };

            for (int i = 0; i < headers.Count; i++)
            {
                sheet.Cell(row, i + 1).Value = headers[i];
                sheet.Cell(row, i + 1).Style.Font.Bold = true;
                sheet.Cell(row, i + 1).Style.Fill.BackgroundColor = XLColor.LightGray;
                sheet.Cell(row, i + 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }

            row++;

            // Dados do relatório
            foreach (var item in dadosDoRelatorio)
            {
                sheet.Cell(row, 1).Value = ObterTipoAcervo(item.TipoAcervo);
                sheet.Cell(row, 2).Value = item.Titulo;
                sheet.Cell(row, 3).Value = item.CodigoTombo;
                sheet.Cell(row, 4).Value = item.QuantidadeVezBaixado;
                row++;
            }

            sheet.Columns().AdjustToContents();
            workbook.SaveAs(stream);
            stream.Position = 0;
            return stream;
        }
    }
}
