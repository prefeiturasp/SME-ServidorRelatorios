using MediatR;
using SME.SR.Application.Queries.CDEP.ObterRelatorioCDEPControleAcervoAutor;
using SME.SR.Infra;
using SME.SR.Infra.Dtos.Relatorios.CDEP;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application.Commands.CDEP.GerarRelatorioControleAcervoAutor
{
    public class GerarRelatorioControleAcervoAutorCommandHandler : GerarRelatorioControleLivrosBase, IRequestHandler<GerarRelatorioControleAcervoAutorCommand, MemoryStream>
    {
        private readonly IMediator mediator;

        public GerarRelatorioControleAcervoAutorCommandHandler(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public async Task<MemoryStream> Handle(GerarRelatorioControleAcervoAutorCommand request, CancellationToken cancellationToken)
        {
            var acervos = await mediator.Send(new ObterRelatorioCDEPControleAcervoAutorQuery()
            {
                Filtros = request.Filtros
            });

            if (!acervos.Any())
                throw new NegocioException("Não possui informações.");

            return await GerarAqruivoParaExcel(acervos, request.Filtros.Usuario, request.Filtros.UsuarioRF);
        }

        private async Task<MemoryStream> GerarAqruivoParaExcel(IEnumerable<ControleAcervoAutorDTO> acervos, string usuario, string rf)
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

                var autores = acervos.Select(a => a.Autor).Distinct().OrderBy(a => a).ToList();

                var cabecalhoHtml = ObterCabecalhoHtml("Relatório de Controle por Autor/Crédito", usuario, rf, autores?.Count > 1 ? string.Empty : autores.FirstOrDefault());
                await writer.WriteLineAsync(cabecalhoHtml);

                await writer.WriteLineAsync("<table border='1' cellspacing='0' cellpadding='5'>");
                await writer.WriteLineAsync("<tr>" +
                    "<th>Autor/Crédito</th>" +
                    "<th>Título de acervo</th>" +
                    "<th>Tombo/Código</th>" +
                    "<th>Título</th>" +
                    "</tr>");

                foreach (var acervo in acervos)
                {
                    await writer.WriteLineAsync("<tr>" +
                        $"<td>{acervo.Autor}</td>" +
                        $"<td>{ObterTipoAcervo(acervo.TipoAcervo)}</td>" +
                        $"<td class=\"numero\">{acervo.Tombo}</td>" +
                        $"<td>{acervo.Titulo}</td>" +
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
