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
    public class GerarRelatorioControleLivrosEmprestadosSinteticoCommandHandler : GerarRelatorioControleLivrosEmprestadosBase, IRequestHandler<GerarRelatorioControleLivrosEmprestadosSinteticoCommand, string>
    {
        private readonly IMediator mediator;

        public GerarRelatorioControleLivrosEmprestadosSinteticoCommandHandler(IMediator mediator)
        {
            this.mediator = mediator;
        }
        public async Task<string> Handle(GerarRelatorioControleLivrosEmprestadosSinteticoCommand request, CancellationToken cancellationToken)
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

            var codigoCorrelacao = Guid.NewGuid();

            await GerarRelatorio(emprestimosAgrupados, codigoCorrelacao, request.Filtros.Usuario, request.Filtros.UsuarioRF);

            return codigoCorrelacao.ToString();
        }

        public async Task GerarRelatorio(IEnumerable<AcervoSolicitacaoSinteticoDto> dadosDoRelatorio, Guid codigoCorrelacao, string usuario, string rf)
        {
            var memoryStreamDoRelatorio = await GerarAqruivoParaExcel(dadosDoRelatorio, usuario, rf);

            var caminhoBase = AppDomain.CurrentDomain.BaseDirectory;
            var caminhoParaSalvar = Path.Combine(caminhoBase, $"relatorios", $"{codigoCorrelacao}");

            var caminhoDiretorio = Path.Combine(caminhoBase, "relatorios");
            if (!Directory.Exists(caminhoDiretorio))
            {
                Directory.CreateDirectory(caminhoDiretorio);
            }

            await SaveMemoryStreamToFile(memoryStreamDoRelatorio, $"{caminhoParaSalvar}.xls");

            memoryStreamDoRelatorio.Dispose();
        }

        private static async Task<MemoryStream> GerarAqruivoParaExcel(IEnumerable<AcervoSolicitacaoSinteticoDto> downloadProvasBoletimEscolarDtos, string usuario, string rf)
        {
            var memoryStream = new MemoryStream();
            using (var writer = new StreamWriter(memoryStream, Encoding.UTF8, leaveOpen: true))
            {
                await writer.WriteLineAsync("<html><head>");
                await writer.WriteLineAsync("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">");
                await writer.WriteLineAsync("<style>");
                await writer.WriteLineAsync("th { font-weight: bold; }");
                await writer.WriteLineAsync(".numero { text-align: center; }");
                await writer.WriteLineAsync("</style>");
                await writer.WriteLineAsync("</head><body>");

                var cabecalhoHtml = ObterCabecalhoHtml(usuario, rf);
                await writer.WriteLineAsync(cabecalhoHtml);

                await writer.WriteLineAsync("<table border='1'>");
                await writer.WriteLineAsync("<tr><th>Tombo</th><th>Título</th><th>Quantidade de empréstimos</th></tr>");

                foreach (var item in downloadProvasBoletimEscolarDtos)
                {
                    await writer.WriteLineAsync("<tr>" +
                        $"<td class=\"numero\">{item.Tombo}</td>" +
                        $"<td>{item.Titulo}</td>" +
                        $"<td class=\"numero\">{item.QuantidadeEmprestimos}</td>" +
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
