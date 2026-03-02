using SME.SR.HtmlPdf;
using System;
using System.IO;

namespace SME.SR.Application.Commands.Sondagem
{
    public abstract class GerarRelatorioSondagemPorTurmaBase
    {
        protected static Stream ObterLogo()
        {
            string base64Logo = SmeConstants.LogoSmeMono.Substring(SmeConstants.LogoSmeMono.IndexOf(',') + 1);
            return new MemoryStream(Convert.FromBase64String(base64Logo));
        }
    }
}
