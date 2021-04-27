using System;
using System.Drawing.Imaging;
using System.IO;

namespace SME.SR.Infra
{
    public class AcompanhamentoAprendizagemAlunoFotoDto
    {
        public string TipoArquivo { get; set; }
        public Guid Codigo { get; set; }

        

        public string Caminho()              
            => $"https://dev-novosgp.sme.prefeitura.sp.gov.br/arquivos/editor/{Codigo}.{FormatoArquivo.ToString().ToLower()}";        

        public ImageFormat FormatoArquivo
        {
            get
            {
                switch (TipoArquivo)
                {
                    case "image/jpeg":
                        return ImageFormat.Jpeg;
                    case "image/bmp":
                        return ImageFormat.Bmp;
                    case "image/emf":
                        return ImageFormat.Emf;
                    case "image/exif":
                        return ImageFormat.Exif;
                    case "image/gif":
                        return ImageFormat.Gif;
                    case "image/icon":
                        return ImageFormat.Icon;
                    case "image/png":
                        return ImageFormat.Png;
                    case "image/tiff":
                        return ImageFormat.Tiff;
                    case "image/wmf":
                        return ImageFormat.Wmf;
                    default:
                        throw new NegocioException("Formato da imagem não identificado");
                }
            }
        }

        public byte[] ArquivoBase64()
        {
            var caminho = @"C:\Workspace\SME-NovoSGP\src\SME.SGP.Api\bin\Debug\netcoreapp2.2\Arquivos\FotoAluno\";
            var nomeArquivo = $"{Codigo}.jpg";
            var caminhoArquivo = Path.Combine(caminho, nomeArquivo);
            var arquivo = File.ReadAllBytes(caminhoArquivo);
            return arquivo;
        }
    }
}
