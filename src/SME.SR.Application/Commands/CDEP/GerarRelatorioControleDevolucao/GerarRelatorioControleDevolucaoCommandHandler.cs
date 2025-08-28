using MediatR;
using SME.SR.Application.Queries.CDEP.ObterRelatorioCDEPControleLivrosDevolvidos;
using SME.SR.Infra;
using SME.SR.Infra.Dtos.Relatorios.CDEP;
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

            return await GerarAqruivoParaExcel(livros, request.Filtros.Usuario, request.Filtros.UsuarioRF);
        }

        private static async Task<MemoryStream> GerarAqruivoParaExcel(IEnumerable<AcervoDevolucaoDto> acervos, string usuario, string rf)
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

                var cabecalhoHtml = ObterCabecalhoHtml("Relatório de Controle Devolução de Livros", usuario, rf);
                await writer.WriteLineAsync(cabecalhoHtml);

                await writer.WriteLineAsync("<table border='1' cellspacing='0' cellpadding='5'>");
                await writer.WriteLineAsync("<tr>" +
                    "<th>Solicitante</th>" +
                    "<th>Tombo</th>" +
                    "<th>Título</th>" +
                    "<th>Telefone</th>" +
                    "<th>Email</th>" +
                    "<th>Data do empréstimo</th>" +
                    "<th>Data prevista de devolução</th>" +
                    "<th>Dias em atraso</th>" +
                    "</tr>");

                foreach (var acervo in acervos)
                {
                    await writer.WriteLineAsync("<tr>" +
                        $"<td>{acervo.Solicitante} ({acervo.Login})</td>" +
                        $"<td>{acervo.Tombo}</td>" +
                        $"<td>{acervo.Titulo}</td>" +
                        $"<td>{acervo.Telefone}</td>" +
                        $"<td>{acervo.Email}</td>" +
                        $"<td class=\"data\">{acervo.DataEmprestimo:dd/MM/yyyy}</td>" +
                        $"<td class=\"data\">{acervo.DataDevolucao:dd/MM/yyyy}</td>" +
                        $"<td class=\"numero\">{acervo.DiasAtraso}</td>" +
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
