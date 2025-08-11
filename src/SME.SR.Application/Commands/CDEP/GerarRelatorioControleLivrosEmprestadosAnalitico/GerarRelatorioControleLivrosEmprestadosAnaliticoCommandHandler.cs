using MediatR;
using SME.SR.Application.Queries.CDEP.ObterRelatorioCDEPControleLivrosEmprestadoSintetico;
using SME.SR.Infra;
using SME.SR.Infra.CDEP;
using SME.SR.Infra.Dtos.Relatorios.CDEP;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application.Commands.CDEP.GerarRelatorioControleLivrosEmprestadosAnalitico
{
    public class GerarRelatorioControleLivrosEmprestadosAnaliticoCommandHandler : GerarRelatorioControleLivrosEmprestadosBase, IRequestHandler<GerarRelatorioControleLivrosEmprestadosAnaliticoCommand, string>
    {
        private readonly IMediator mediator;

        public GerarRelatorioControleLivrosEmprestadosAnaliticoCommandHandler(IMediator mediator)
        {
            this.mediator = mediator;
        }
        public async Task<string> Handle(GerarRelatorioControleLivrosEmprestadosAnaliticoCommand request, CancellationToken cancellationToken)
        {
            var livros = await mediator.Send(new ObterRelatorioCDEPControleLivrosEmprestadoQuery()
            {
                filtros = request.Filtros
            });

            if (!livros.Any())
                throw new NegocioException("Não possui informações.");

            var codigoCorrelacao = Guid.NewGuid();

            return await GerarRelatorio(livros, codigoCorrelacao, request.Filtros.Usuario, request.Filtros.UsuarioRF);
        }


        public async Task<string> GerarRelatorio(IEnumerable<AcervoSolicitacaoDto> dadosDoRelatorio, Guid codigoCorrelacao, string usuario, string rf)
        {
            var memoryStreamDoRelatorio = await GerarAqruivoParaExcel(dadosDoRelatorio, usuario, rf);

            var caminhoBase = AppDomain.CurrentDomain.BaseDirectory;
            var caminhoParaSalvar = Path.Combine(caminhoBase, $"relatorios", $"{codigoCorrelacao}");

            var caminhoDiretorio = Path.Combine(caminhoBase, "relatorios");
            if (!Directory.Exists(caminhoDiretorio))
            {
                Directory.CreateDirectory(caminhoDiretorio);
            }

            var caminhoArquivo = $"{caminhoParaSalvar}.xls";

            await SaveMemoryStreamToFile(memoryStreamDoRelatorio, caminhoArquivo);

            memoryStreamDoRelatorio.Dispose();

            return caminhoArquivo;
        }

        private static async Task<MemoryStream> GerarAqruivoParaExcel(IEnumerable<AcervoSolicitacaoDto> acervos, string usuario, string rf)
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

                var cabecalhoHtml = ObterCabecalhoHtml(usuario, rf);
                await writer.WriteLineAsync(cabecalhoHtml);

                await writer.WriteLineAsync("<table border='1' cellspacing='0' cellpadding='5'>");
                await writer.WriteLineAsync("<tr>" +
                    "<th>Tombo</th>" +
                    "<th>Título</th>" +
                    "<th>Situação</th>" +
                    "<th>Quantidade de empréstimos</th>" +
                    "<th>Leitor</th>" +
                    "<th>Data de retirada</th>" +
                    "<th>Data de devolução</th>" +
                    "</tr>");

                var acervosGroup = acervos.GroupBy(x => x.Tombo).ToList();

                foreach (var grupo in acervosGroup)
                {
                    var primeiro = grupo.First();
                    int numEmprestimos = grupo.Count();

                    // Primeira linha do agrupamento
                    await writer.WriteLineAsync("<tr>" +
                        $"<td class=\"numero\">{primeiro.Tombo}</td>" +
                        $"<td>{primeiro.Titulo}</td>" +
                        $"<td>{ObterDescricaoSituacao(primeiro.SituacaoEmprestimo)}</td>" +
                        $"<td class=\"numero\">{numEmprestimos}</td>" +
                        "<td></td>" + // Leitor vazio
                        "<td class=\"data\"></td>" + // Data retirada vazia
                        "<td class=\"data\"></td>" + // Data devolução vazia
                        "</tr>");

                    // Linhas dos empréstimos
                    foreach (var emprestimo in grupo)
                    {
                        await writer.WriteLineAsync("<tr>" +
                            "<td></td>" + // Tombo vazio
                            "<td></td>" + // Título vazio
                            "<td></td>" + // Situação vazia
                            "<td></td>" + // Quantidade vazia
                            $"<td>{emprestimo.Solicitante}</td>" +
                            $"<td class=\"data\">{emprestimo.DataEmprestimo:dd/MM/yyyy}</td>" +
                            $"<td class=\"data\">{emprestimo.DataDevolucao:dd/MM/yyyy}</td>" +
                            "</tr>");
                    }
                }

                await writer.WriteLineAsync("</table></body></html>");
                await writer.FlushAsync();
                memoryStream.Position = 0;
                return memoryStream;
            }
        }

        private static string ObterDescricaoSituacao(SituacaoEmprestimo situacao)
        {
            var fieldInfo = situacao.GetType().GetField(situacao.ToString());
            var descriptionAttribute = (DisplayAttribute)Attribute.GetCustomAttribute(fieldInfo, typeof(DisplayAttribute));
            return descriptionAttribute?.Description ?? situacao.ToString();
        }

    }
}
