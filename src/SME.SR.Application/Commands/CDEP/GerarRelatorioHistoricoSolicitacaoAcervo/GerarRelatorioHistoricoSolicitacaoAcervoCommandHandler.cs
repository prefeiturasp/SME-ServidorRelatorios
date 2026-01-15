using ClosedXML.Excel;
using MediatR;
using SME.SR.Application.Queries.CDEP.ObterRelatorioCdepHistoricoSolicitacoes;
using SME.SR.Infra;
using SME.SR.Infra.Dtos.Relatorios.CDEP;
using SME.SR.Infra.Utilitarios;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application.Commands.CDEP.GerarRelatorioHistoricoSolicitacaoAcervo
{
    public class GerarRelatorioHistoricoSolicitacaoAcervoCommandHandler : GerarRelatorioControleLivrosBase, IRequestHandler<GerarRelatorioHistoricoSolicitacaoAcervoCommand, MemoryStream>
    {
        private readonly IMediator mediator;
        public GerarRelatorioHistoricoSolicitacaoAcervoCommandHandler(IMediator mediator)
        {
            this.mediator = mediator;
        }
        public async Task<MemoryStream> Handle(GerarRelatorioHistoricoSolicitacaoAcervoCommand request, CancellationToken cancellationToken)
        {
            var historicoSolicitacaoAcervo = await mediator.Send(new ObterRelatorioCdepHistoricoSolicitacaoAcervoQuery()
            {
                Filtros = request.Filtros
            });
            if (historicoSolicitacaoAcervo == null)
                throw new NegocioException("Não possui informações.");
            return GerarArquivoParaExcel(historicoSolicitacaoAcervo, request.Filtros);
        }

        private MemoryStream GerarArquivoParaExcel(IEnumerable<HistoricoSolicitacaoAcervoDto> dadosDoRelatorio, FiltroRelatorioHistoricoSolicitacaoAcervo filtros)
        {
            var stream = new MemoryStream();
            using var workbook = new XLWorkbook();
            var sheet = workbook.Worksheets.Add("Relatório");

            int row = 2;

            InserirCabecalhosPadrao(ref row, ref sheet, "Relatório Histórico de Solicitação de Acervo", filtros);

            // Cabeçalhos das colunas
            var headers = new List<string>
            {
                "Solicitante",
                "Tipo do Acervo",
                "Tombo/Código",
                "Título do Acervo",
                "Data da Solicitação",
                "Situação da Solicitação",
                "Data da Visita"
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
                sheet.Cell(row, 1).Value = $"{item.NomeSolicitante} ({item.LoginSolicitante})";
                sheet.Cell(row, 2).Value = item.TipoAcervo.Description();
                sheet.Cell(row, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                sheet.Cell(row, 3).Value = item.CodigoTombo;
                sheet.Cell(row, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                sheet.Cell(row, 4).Value = item.Titulo;
                sheet.Cell(row, 5).Value = item.DataSolicitacao.ToString("dd/MM/yyyy");
                sheet.Cell(row, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                sheet.Cell(row, 6).Value = item.SituacaoSolicitacao.Description();
                sheet.Cell(row, 7).Value = item.DataVisita?.ToString("dd/MM/yyyy") ?? "-";
                sheet.Cell(row, 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                row++;
            }

            sheet.Columns().AdjustToContents();
            workbook.SaveAs(stream);
            stream.Position = 0;
            return stream;
        }
    }
}
