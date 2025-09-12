using SME.SR.HtmlPdf;
using SME.SR.Infra.CDEP;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;


namespace SME.SR.Application.Commands.CDEP
{
    public abstract class GerarRelatorioControleLivrosBase
    {
        protected static Stream ObterLogo()
        {
            string base64Logo = SmeConstants.LogoSmeMono.Substring(SmeConstants.LogoSmeMono.IndexOf(',') + 1);
            return new MemoryStream(Convert.FromBase64String(base64Logo));
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
