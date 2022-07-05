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
            if (request.Url.StartsWith("data:") && request.Url.Contains(";base64,"))
                return request.Url;

            using (var client = new WebClient())
            {
                try
                {
                    var arquivo = client.DownloadData(new Uri(request.Url));

                    using (var memoryStream = new MemoryStream(arquivo))
                    {
                        var imagem = new Bitmap(memoryStream);
                        var format = imagem.RawFormat;
                        var codec = ImageCodecInfo
                            .GetImageDecoders()
                            .First(c => c.FormatID == format.Guid);
                        string mimeType = codec.MimeType;

                        var imagemBase64 = RedimencionarImagem(imagem,request.EscalaHorizontal,request.EscalaVertical);

                        return $"data:{mimeType};base64,{imagemBase64}";
                    }

                }
                catch (Exception e)
                {
                    return "";
                }            
            }
        }

        private string RedimencionarImagem(Bitmap imagem, float escalaHorizontal, float escalaVertical)
        {
            var escalaH = escalaHorizontal / imagem.Width;
            var escalaV = escalaVertical / imagem.Height;

            var escala = Math.Min(escalaV, escalaH);

            if (escala >= 1 || imagem.Width > escalaHorizontal)
                return ConverterImagem(imagem);

            var width = (int)(imagem.Width * escala);
            var height = (int)(imagem.Height * escala);

            var imagemRedimencionada = new Bitmap(imagem, new Size(width, height));
            return ConverterImagem(imagemRedimencionada);
        }

        private string ConverterImagem(Bitmap bitmap)
        {
            var converter = new ImageConverter();
            var arquivoConvertido = (byte[])converter.ConvertTo(bitmap, typeof(byte[]));
            return Convert.ToBase64String(arquivoConvertido);
        }
    }
}
