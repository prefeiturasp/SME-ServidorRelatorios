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

            var registroFormatado = UtilRegex.RemoverTagsHtmlMultiMidia(Registro);
            return UtilRegex.RemoverTagsHtml(registroFormatado);
        }
        
        public string DataRelatorioFormatada()
        {
            var elapsedTicks = DataRegistro.Date.Ticks - DataRegistro.Ticks;
            if(elapsedTicks == 0)
                return $"{DataRegistro:dd/MM/yyyy}";

            return $"{DataRegistro:dd/MM/yyyy HH:mm:ss}"; ;
        }
    }
}
