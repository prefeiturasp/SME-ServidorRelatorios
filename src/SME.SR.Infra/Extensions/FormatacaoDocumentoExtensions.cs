using System;
using System.Linq;

namespace SME.SR.Infra.Extensions
{
    public static class FormatacaoDocumentoExtensions
    {
        public static string FormatarDocumento(this string documento)
        {
            if (string.IsNullOrWhiteSpace(documento))
                return string.Empty;

            var digitos = new string(documento.Where(char.IsDigit).ToArray());
            if (digitos.Length == 11)
            {
                return Convert.ToUInt64(digitos).ToString(@"000\.000\.000\-00");
            }

            if (digitos.Length == 7)
            {
                return Convert.ToUInt64(digitos).ToString(@"000\.000\.0");
            }

            return documento;
        }
    }
}
