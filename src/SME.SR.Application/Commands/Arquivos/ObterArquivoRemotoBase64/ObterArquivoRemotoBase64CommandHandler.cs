using MediatR;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterArquivoRemotoBase64CommandHandler : IRequestHandler<ObterArquivoRemotoBase64Command, string>
    {


        public async Task<string> Handle(ObterArquivoRemotoBase64Command request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.Url.StartsWith("data:") && request.Url.Contains(";base64,"))
                    return request.Url;

                using (var client = new WebClient())
                {
                    var arquivo = client.DownloadData(new Uri(request.Url));

                    using (var memoryStream = new MemoryStream(arquivo))
                    {
                        var imagem = new Bitmap(memoryStream);
                        var format = imagem.RawFormat;
                        var codec = ImageCodecInfo.GetImageDecoders()
                            .First(c => c.FormatID == format.Guid);
                        string mimeType = codec.MimeType;

                        return $"data:{mimeType};base64,{Convert.ToBase64String(arquivo)}";
                    }
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
