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

namespace SME.SR.Application.Commands.CDEP.GerarRelatorioControleLivrosEmprestadosAnalitico
{
    public class GerarRelatorioControleLivrosEmprestadosAnaliticoCommandHandler : IRequestHandler<GerarRelatorioControleLivrosEmprestadosAnaliticoCommand, string>
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

            await GerarRelatorio(livros, request.Filtros.CodigoCorrelacao, request.Filtros.Usuario);

            return request.Filtros.CodigoCorrelacao.ToString();
        }


        public async Task GerarRelatorio(IEnumerable<AcervoSolicitacaoDto> dadosDoRelatorio, Guid codigoCorrelacao, string usuario)
        {
            var memoryStreamDoRelatorio = await GerarAqruivoParaExcel(dadosDoRelatorio, usuario);

            var caminhoBase = AppDomain.CurrentDomain.BaseDirectory;
            var caminhoParaSalvar = Path.Combine(caminhoBase, $"relatorios", $"{codigoCorrelacao}");

            await SaveMemoryStreamToFile(memoryStreamDoRelatorio, $"{caminhoParaSalvar}.xls");

            memoryStreamDoRelatorio.Dispose();
        }

        private static async Task SaveMemoryStreamToFile(MemoryStream memoryStream, string filePath)
        {
            memoryStream.Position = 0;

            using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                await memoryStream.CopyToAsync(fileStream);
            }
        }

        private static async Task<MemoryStream> GerarAqruivoParaExcel(IEnumerable<AcervoSolicitacaoDto> downloadProvasBoletimEscolarDtos, string usuario)
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

                var cabecalhoHtml = ObterCabecalhoHtml(usuario);
                await writer.WriteLineAsync(cabecalhoHtml);

                await writer.WriteLineAsync("<table border='1'>");
                await writer.WriteLineAsync("<tr><th>Tombo</th><th>Título</th><th>Situação</th><th>Quantidade de empréstimos</th><th>Leitor</th><th>Data de retirada</th><th>Data de devolução</th></tr>");

                foreach (var item in downloadProvasBoletimEscolarDtos)
                {
                    await writer.WriteLineAsync("<tr>" +
                        $"<td class=\"numero\">{item.Tombo}</td>" +
                        $"<td>{item.Titulo}</td>" +
                        $"<td>{item.SituacaoEmprestimo}</td>" +
                        $"<td class=\"numero\">{item.QuantidadeEmprestimos}</td>" +
                        $"<td>{item.Solicitante}</td>" +
                        $"<td>{item.DataEmprestimo}</td>" +
                        $"<td>{item.DataDevolucao}</td>" +
                        "</tr>");
                }

                await writer.WriteLineAsync("</table></body></html>");
                await writer.FlushAsync();
                memoryStream.Position = 0;
                return memoryStream;
            }
        }

        private static string ObterCabecalhoHtml(string usuario)
        {
            return $@"
                        <div style='display: flex; justify-content: space-between; align-items: center; padding: 10px;'>
                            <div style='text-align: center;'>
                                <p style='font-size: 14px; font-weight: bold; margin-bottom: 5px;'>SGP - SISTEMA DE GESTÃO PEDAGÓGICA</p>
                                <h3 style='margin-top: 0;'>Relatório de Controle de Livros Emprestados</h3>
                            </div>
                        </div>
                        <table border='1' cellpadding='5' cellspacing='0' style='width: 100%; margin-bottom: 20px; border-collapse: collapse;'>
                            <tr>
                                <td><strong>{usuario}</td>
                                <td><strong>RF:</strong></td>
                                <td><strong>DATA:</strong> {DateTime.Now.ToString("dd-MM-yyyy")}</td>
                            </tr>
                        </table>
                    ";
        }
    }
}
