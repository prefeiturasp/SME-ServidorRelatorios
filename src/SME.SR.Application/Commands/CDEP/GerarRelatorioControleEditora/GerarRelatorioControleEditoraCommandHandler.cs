using MediatR;
using SME.SR.Application.Commands.CDEP.GerarRelatorioControleEditora;
using SME.SR.Application.Queries.CDEP.ObterRelatorioCDEPControleEditora;
using SME.SR.Infra;
using SME.SR.Infra.Dtos.Relatorios.CDEP;
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

            return await GerarArquivoParaExcel(acervos, request.Filtros.Usuario, request.Filtros.UsuarioRF);
        }

        private async Task<MemoryStream> GerarArquivoParaExcel(IEnumerable<ControleEditoraDTO> acervos, string usuario, string rf)
        {
            var memoryStream = new MemoryStream();
            using (var writer = new StreamWriter(memoryStream, Encoding.UTF8, leaveOpen: true))
            {
                await writer.WriteLineAsync("<html><head>");
                await writer.WriteLineAsync("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">");
                await writer.WriteLineAsync("<style>");
                await writer.WriteLineAsync("th { font-weight: bold; }");
                await writer.WriteLineAsync(".numero { text-align: center; }");
                await writer.WriteLineAsync(".data { text-align: right; }");
                await writer.WriteLineAsync("</style>");
                await writer.WriteLineAsync("</head><body>");

                var cabecalhoHtml = ObterCabecalhoHtml("Relatório de Controle de Editora", usuario, rf);
                await writer.WriteLineAsync(cabecalhoHtml);

                await writer.WriteLineAsync("<table border='1' cellspacing='0' cellpadding='5' style='width: 100%';>");
                await writer.WriteLineAsync("<tr>" +
                    "<th>Editora</th>" +
                    "<th>Tombo/Código</th>" +
                    "<th>Título</th>" +
                    "<th>Situação do empréstimo</th>" +
                    "</tr>");

                var acervosGroup = acervos.GroupBy(x => new { x.Editora, x.Titulo, x.SituacaoEmprestimo }).ToList();

                foreach (var grupo in acervosGroup)
                {
                    var primeiro = grupo.First();
                    int numEmprestimos = grupo.Count();

                    await writer.WriteLineAsync("<tr>" +
                        $"<td>{primeiro.Editora}</td>" +
                        $"<td class=\"numero\">{primeiro.Tombo}</td>" +
                        $"<td>{primeiro.Titulo}</td>" +
                        $"<td>{ObterDescricaoSituacao(primeiro.SituacaoEmprestimo)}</td>" +
                        "</tr>");
                }

                await writer.WriteLineAsync("</table></body></html>");
                await writer.FlushAsync();
                memoryStream.Position = 0;
                return memoryStream;
            }
        }
    }
}
