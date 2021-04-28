using SME.SR.Infra.Utilitarios;
using System;

namespace SME.SR.Infra
{
    public class RegistroIndividualRetornoDto
    {
        public long TurmaId { get; set; }
        public long AlunoCodigo { get; set; }
        public string Registro { get; set; }
        public DateTime DataRegistro { get; set; }
        public string CriadoPor { get; set; }
        public string CriadoRf { get; set; }

        public string DataRelatorio =>
           $"{DataRegistro:dd/MM/yyyy}";

        public string RegistradoPor =>
            $"{CriadoPor} ({CriadoRf})".ToUpper();

        public string RegistroFormatado()
        {
            if (string.IsNullOrEmpty(Registro))
                return string.Empty;

            var registroFormatado = UtilRegex.RemoverTagsHtmlMidia(Registro);
            return UtilRegex.RemoverTagsHtml(registroFormatado);
        }
    }
}
