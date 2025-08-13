using MediatR;
using SME.SR.Application.Queries.CDEP.ObterRelatorioCDEPControleAcervo;
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
    public class GerarRelatorioControleAcervoCommandHandler : GerarRelatorioControleLivrosBase, IRequestHandler<GerarRelatorioControleAcervoCommand, MemoryStream>
    {
        private readonly IMediator mediator;

        public GerarRelatorioControleAcervoCommandHandler(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public async Task<MemoryStream> Handle(GerarRelatorioControleAcervoCommand request, CancellationToken cancellationToken)
        {
            var acervos = await mediator.Send(new ObterRelatorioCDEPControleAcervoQuery()
            {
                filtros = request.Filtros
            });

            if (!acervos.Any())
                throw new NegocioException("Não possui informações.");

            return await GerarAqruivoParaExcel(acervos, request.Filtros.Usuario, request.Filtros.UsuarioRF);
        }

        private async Task<MemoryStream> GerarAqruivoParaExcel(IEnumerable<ControleAcervoDTO> acervos, string usuario, string rf)
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

                var cabecalhoHtml = ObterCabecalhoHtml("Relatório de Controle do Tombo/Código", usuario, rf);
                await writer.WriteLineAsync(cabecalhoHtml);

                await writer.WriteLineAsync("<table border='1' cellspacing='0' cellpadding='5'>");
                await writer.WriteLineAsync("<tr>" +
                    "<th>Tipo do acervo</th>" +
                    "<th>Título do acervo</th>" +
                    "<th>Tombo/Código</th>" +
                    "<th>Situação do tombo/código</th>" +
                    "<th>Quantidade</th>" +
                    "</tr>");

                var acervosGroup = acervos.GroupBy(x => x.Tombo).ToList();

                foreach (var grupo in acervosGroup)
                {
                    var primeiro = grupo.First();
                    int numEmprestimos = grupo.Count();

                    // Primeira linha do agrupamento
                    await writer.WriteLineAsync("<tr>" +
                        $"<td>{ObterTipoAcervo(primeiro.TipoAcervo)}</td>" +
                        $"<td>{primeiro.Titulo}</td>" +
                        $"<td class=\"numero\">{primeiro.Tombo}</td>" +
                        $"<td>{ObterDescricaoSituacao(primeiro.SituacaoEmprestimo)}</td>" +
                        $"<td class=\"numero\">{numEmprestimos}</td>" +
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
