using SME.SR.Infra.CDEP;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;

namespace SME.SR.Application.Commands.CDEP
{
    public abstract class GerarRelatorioControleLivrosBase
    {
        protected static string ObterCabecalhoHtml(string título, string usuario, string rf, string autor = null)
        {
            var colunaAutor = string.Empty;
            if (!string.IsNullOrWhiteSpace(autor))
                colunaAutor = $"<td><strong>AUTOR:</strong> {autor}</td>";

            return $@"
                <div style='display: flex; justify-content: space-between; align-items: center; padding: 10px;'>
                 <div>
                   <div>
                      <img 
                           style='height: 64px; float: left;' 
                           src='{SME.SR.HtmlPdf.SmeConstants.LogoSmeMonoNovo}' 
                           alt='Logo SGP' 
                            />
                        </div>
                   </div>
                    <div style='text-align: center;'>
                        <p style='font-size: 14px; font-weight: bold; margin-bottom: 5px;'>CDEP - CENTRO DE DOCUMENTAÇÃO DA EDUCAÇÃO PAULISTANA</p>
                        <h3 style='margin-top: 0;'>{título}</h3>
                    </div>
                </div>
                <table border='1' cellpadding='5' cellspacing='0' style='width: 100%; margin-bottom: 20px; border-collapse: collapse;'>
                    <tr>
                        <td><strong>{usuario}</td>
                        <td><strong>RF: {rf}</strong></td>
                        {colunaAutor}
                        <td><strong>DATA:</strong> {DateTime.Now.ToString("dd-MM-yyyy")}</td>
                    </tr>
                </table>
            ";
        }

        protected static async Task SaveMemoryStreamToFile(MemoryStream memoryStream, string filePath)
        {
            memoryStream.Position = 0;

            using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                await memoryStream.CopyToAsync(fileStream);
            }
        }

        protected static string ObterDescricaoSituacao(SituacaoEmprestimo situacao)
        {
            var fieldInfo = situacao.GetType().GetField(situacao.ToString());
            var descriptionAttribute = (DisplayAttribute)Attribute.GetCustomAttribute(fieldInfo, typeof(DisplayAttribute));
            return descriptionAttribute?.Description ?? situacao.ToString();
        }

        protected static string ObterTipoAcervo(TipoAcervo tipoAcervo)
        {
            var fieldInfo = tipoAcervo.GetType().GetField(tipoAcervo.ToString());
            var descriptionAttribute = (DisplayAttribute)Attribute.GetCustomAttribute(fieldInfo, typeof(DisplayAttribute));
            return descriptionAttribute?.Description ?? tipoAcervo.ToString();
        }
    }
}
