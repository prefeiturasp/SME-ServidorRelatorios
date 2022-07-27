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
                        //var imagem = new Bitmap(FixImageOrientation(imagem));
                        var format = imagem.RawFormat;
                        var codecs = ImageCodecInfo
                            .GetImageDecoders();

                        var codec = codecs.First(x=> x.FormatID == format.Guid); 
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

        private Image FixImageOrientation(Image image)
        {
            const int exifOrientationId = 0x112;
            if (!image.PropertyIdList.Contains(exifOrientationId))
                return image;
            //Gets the specified property item from the image
            var property = image.GetPropertyItem(exifOrientationId);
            var orient = BitConverter.ToInt16(property.Value, 0);
            //Get the rotated or flipped image
            image = RotateImageSrc(orient, image);
            return image;
        }

        private Image RotateImageSrc(int orient, Image image)
        {
            switch (orient)
            {
                case 1:
                    image.RotateFlip(RotateFlipType.RotateNoneFlipNone);
                    return image;
                case 2:
                    image.RotateFlip(RotateFlipType.RotateNoneFlipX);
                    return image;
                case 3:
                    image.RotateFlip(RotateFlipType.Rotate180FlipNone);
                    return image;
                case 4:
                    image.RotateFlip(RotateFlipType.Rotate180FlipX);
                    return image;
                case 5:
                    image.RotateFlip(RotateFlipType.Rotate90FlipX);
                    return image;
                case 6:
                    image.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    return image;
                case 7:
                    image.RotateFlip(RotateFlipType.Rotate270FlipX);
                    return image;
                case 8:
                    image.RotateFlip(RotateFlipType.Rotate270FlipNone);
                    return image;
                default:
                    image.RotateFlip(RotateFlipType.RotateNoneFlipNone);
                    return image;
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
