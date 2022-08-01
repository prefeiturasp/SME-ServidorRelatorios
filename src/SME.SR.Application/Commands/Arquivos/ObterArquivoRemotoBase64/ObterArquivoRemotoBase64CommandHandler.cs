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
                        var rawFormat = imagem.RawFormat;

                        FixImageOrientation(imagem);
                        var format = imagem.RawFormat;

                        var codecs = ImageCodecInfo
                            .GetImageDecoders();

                        string mimeType = codecs.FirstOrDefault(c=> c.FormatID == rawFormat.Guid).MimeType;
                        var imagemBase64 = RedimencionarImagem(imagem, request.EscalaHorizontal,request.EscalaVertical);

                        return $"data:{mimeType};base64,{imagemBase64}";
                    }
                }
                catch (Exception e)
                {
                    return "";
                }            
            }
        }

        static void FixImageOrientation(Image img)
        {
            const int ExifOrientationId = 0x112;
            // Read orientation tag
            if (!img.PropertyIdList.Contains(ExifOrientationId)) return;
            var prop = img.GetPropertyItem(ExifOrientationId);
            var orient = BitConverter.ToInt16(prop.Value, 0);
            // Force value to 1
            prop.Value = BitConverter.GetBytes((short)1);
            img.SetPropertyItem(prop);

            // Rotate/flip image according to <orient>
            switch (orient)
            {
                case 1:
                    img.RotateFlip(RotateFlipType.RotateNoneFlipNone);
                    break;
                case 2:
                    img.RotateFlip(RotateFlipType.RotateNoneFlipX);
                    break;
                case 3:
                    img.RotateFlip(RotateFlipType.Rotate180FlipNone);
                    break;
                case 4:
                    img.RotateFlip(RotateFlipType.Rotate180FlipX);
                    break;
                case 5:
                    img.RotateFlip(RotateFlipType.Rotate90FlipX);
                    break;
                case 6:
                    img.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    break;
                case 7:
                    img.RotateFlip(RotateFlipType.Rotate270FlipX);
                    break;
                case 8:
                    img.RotateFlip(RotateFlipType.Rotate270FlipNone);
                    break;
                default:
                    return;
            }
        }

        private string RedimencionarImagem(Bitmap imagem, float escalaHorizontal, float escalaVertical)
        {
            var escalaH = escalaHorizontal / imagem.Width;
            var escalaV = escalaVertical / imagem.Height;

            var escala = Math.Min(escalaV, escalaH);

            if (escala >= 1)
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
