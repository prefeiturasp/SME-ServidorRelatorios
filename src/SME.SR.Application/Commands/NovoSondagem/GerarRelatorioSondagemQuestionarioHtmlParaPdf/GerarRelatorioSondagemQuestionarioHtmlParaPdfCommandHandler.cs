using DinkToPdf;
using DinkToPdf.Contracts;
using MediatR;
using SME.SR.HtmlPdf;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application.Commands.NovoSondagem.GerarRelatorioSondagemQuestionarioHtmlParaPdf
{
    public class GerarRelatorioSondagemQuestionarioHtmlParaPdfCommandHandler
        : IRequestHandler<GerarRelatorioSondagemQuestionarioHtmlParaPdfCommand, bool>
    {
        private readonly IConverter converter;
        private readonly IServicoFila servicoFila;
        private readonly IHtmlHelper htmlHelper;

        public GerarRelatorioSondagemQuestionarioHtmlParaPdfCommandHandler(
            IConverter converter,
            IServicoFila servicoFila,
            IHtmlHelper htmlHelper)
        {
            this.converter = converter;
            this.servicoFila = servicoFila ?? throw new ArgumentNullException(nameof(servicoFila));
            this.htmlHelper = htmlHelper ?? throw new ArgumentNullException(nameof(htmlHelper));
        }

        public async Task<bool> Handle(
            GerarRelatorioSondagemQuestionarioHtmlParaPdfCommand request,
            CancellationToken cancellationToken)
        {
            var paginasEmHtml = new List<string>();

            foreach (var modelPagina in request.Paginas)
            {
                var html = await htmlHelper.RenderRazorViewToString(request.NomeTemplate, modelPagina);
                html = html.Replace("logoPrefSPHorizontal.png", SmeConstants.Logo_PrefSP_Horizontal);
                html = html.Replace("logoPAP.png", SmeConstants.Logo_PAP);
                html = html.Replace("logoAEE.png", SmeConstants.Logo_AEE);
                html = html.Replace("logoAcessibilidade.png", SmeConstants.Logo_Acessibilidade);
                paginasEmHtml.Add(html);
            }

            if (!paginasEmHtml.Any())
                return true;

            var directory = AppDomain.CurrentDomain.BaseDirectory;
            var nomeDiretorio = Path.Combine(directory, "relatorios");

            if (!Directory.Exists(nomeDiretorio))
                Directory.CreateDirectory(nomeDiretorio);

            var nomeArquivo = Path.Combine(nomeDiretorio, $"{request.CodigoCorrelacao}.pdf");

            var pagina = request.Paginas.First();

            // ----------------------------------------------------------------
            // CABEÇALHO HTML — repetido em TODAS as páginas via HeaderSettings
            // ----------------------------------------------------------------
            var headerHtml = $@"<!DOCTYPE html>
                                    <html>
                                    <head>
                                      <meta charset=""utf-8""/>
                                      <style>
                                        * {{ box-sizing: border-box; margin: 0; padding: 0; }}
                                        body {{
                                          font-family: 'Roboto', 'DejaVu Sans', Arial, sans-serif;
                                          font-size: 10px;
                                          color: #42474A;
                                          padding: 10px 16px 4px 16px;
                                          background: #fff;
                                          /* FIX: espaçamento correto entre letras e palavras */
                                          letter-spacing: 0.02em;
                                          word-spacing: 0.05em;
                                        }}
                                        .header-logo {{
                                          margin-bottom: 6px;
                                        }}
                                        .header-logo img {{
                                          height: 36px;
                                        }}
                                        .meta-table {{
                                          width: 100%;
                                          border-collapse: collapse;
                                          border: 1px solid #D9D9D9;
                                        }}
                                        .meta-table td {{
                                          border: 1px solid #D9D9D9;
                                          padding: 4px 8px;
                                          font-size: 10px;
                                          font-weight: 400;
                                          line-height: 12px;
                                          vertical-align: middle;
                                          color: #42474A;
                                        }}
                                        .meta-table td strong {{
                                          font-weight: 700;
                                          margin-right: 2px;
                                        }}
                                      </style>
                                    </head>
                                    <body>
                                      <div class=""header-logo"">
                                        <img src=""{SmeConstants.Logo_PrefSP_Horizontal}"" alt=""Prefeitura de São Paulo"" />
                                      </div>
                                      <table class=""meta-table"">
                                        <tr>
                                          <td><strong>Ano letivo:</strong> {pagina.AnoLetivo}</td>
                                          <td><strong>DRE:</strong> {System.Web.HttpUtility.HtmlEncode(pagina.Dre)}</td>
                                          <td><strong>Semestre:</strong> {System.Web.HttpUtility.HtmlEncode(pagina.Semestre)}</td>
                                          <td><strong>Turma:</strong> {System.Web.HttpUtility.HtmlEncode(pagina.Turma)}</td>
                                        </tr>
                                        <tr>
                                          <td colspan=""4""><strong>Unidade Educacional:</strong> {System.Web.HttpUtility.HtmlEncode(pagina.UnidadeEducacional)}</td>
                                        </tr>
                                        <tr>
                                          <td colspan=""2""><strong>Modalidade:</strong> {System.Web.HttpUtility.HtmlEncode(pagina.Modalidade)}</td>
                                          <td><strong>Proficiência:</strong> {System.Web.HttpUtility.HtmlEncode(pagina.Proficiencia)}</td>
                                          <td><strong>Data de impressão:</strong> {pagina.DataImpressao:dd/MM/yyyy}</td>
                                        </tr>
                                        <tr>
                                          <td colspan=""4""><strong>Usuário:</strong> {System.Web.HttpUtility.HtmlEncode(pagina.Usuario)}</td>
                                        </tr>
                                      </table>
                                    </body>
                                    </html>";

            // ----------------------------------------------------------------
            // RODAPÉ HTML — contador de páginas no canto direito (ex: 1/4)
            // ----------------------------------------------------------------
            var footerHtml = @"<!DOCTYPE html>
                                    <html>
                                    <head>
                                      <meta charset=""utf-8""/>
                                      <style>
                                        body {
                                          font-family: 'Roboto', 'DejaVu Sans', Arial, sans-serif;
                                          font-size: 10px;
                                          color: #BFBFC2;
                                          margin: 0;
                                          padding: 2px 16px 0 16px;
                                        }
                                        .footer-content { text-align: right; }
                                      </style>
                                    </head>
                                    <body>
                                      <div class=""footer-content"">
                                        <span class=""pageNumber""></span>/<span class=""totalPages""></span>
                                      </div>
                                      <script>
                                        (function() {
                                          var vars = {};
                                          window.location.href.replace(/[?&]+([^=&]+)=([^&]*)/gi, function(m, key, value) {
                                            vars[key] = decodeURIComponent(value);
                                          });
                                          document.querySelector('.pageNumber').textContent = vars['page'] || '';
                                          document.querySelector('.totalPages').textContent = vars['topage'] || '';
                                        })();
                                      </script>
                                    </body>
                                    </html>";

            var headerPath = Path.Combine(nomeDiretorio, $"{request.CodigoCorrelacao}_header.html");
            var footerPath = Path.Combine(nomeDiretorio, $"{request.CodigoCorrelacao}_footer.html");

            await File.WriteAllTextAsync(headerPath, headerHtml);
            await File.WriteAllTextAsync(footerPath, footerHtml);

            var headerUri = $"file:///{headerPath.Replace("\\", "/")}";
            var footerUri = $"file:///{footerPath.Replace("\\", "/")}?page=[page]&topage=[topage]";

            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings =
                {
                    ColorMode   = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    PaperSize   = PaperKind.A4,

                    DPI = 300,
                    Margins     = new MarginSettings()
                    {
                        Top    = 50,
                        Bottom = 10,
                        Left   = 4,
                        Right  = 4
                    },
                    Out = nomeArquivo
                }
            };

            foreach (var paginaHtml in paginasEmHtml)
            {
                doc.Objects.Add(new ObjectSettings()
                {
                    HtmlContent = paginaHtml,
                    WebSettings =
                    {
                        DefaultEncoding = "utf-8",
                        EnableIntelligentShrinking = false
                    },
                    HeaderSettings = new HeaderSettings
                    {
                        HtmUrl = headerUri,
                        Spacing = 8
                    },
                    FooterSettings = new FooterSettings
                    {
                        HtmUrl = footerUri,
                        Spacing = 2
                    }
                });
            }

            converter.Convert(doc);

            if (File.Exists(headerPath)) File.Delete(headerPath);
            if (File.Exists(footerPath)) File.Delete(footerPath);

            await servicoFila.PublicaFila(new PublicaFilaDto(
                new MensagemRelatorioProntoDto(request.MensagemUsuario, string.Empty),
                RotasRabbitSGP.RotaRelatoriosProntosSgp,
                ExchangeRabbit.Sgp,
                request.CodigoCorrelacao));

            return true;
        }
    }
}