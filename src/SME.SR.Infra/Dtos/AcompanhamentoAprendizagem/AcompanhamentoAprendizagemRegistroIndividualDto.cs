using SME.SR.Infra.Utilitarios;
using System;

namespace SME.SR.Infra
{
    public class AcompanhamentoAprendizagemRegistroIndividualDto
    {
        public long AlunoCodigo { get; set; }
        public long TurmaId { get; set; }
        public DateTime DataRegistro { get; set; }
        public string Registro { get; set; }

        public string DataRelatorio =>
           $"{DataRegistro:dd/MM/yyyy}";

        public string RegistroFormatado()
        {
            if (string.IsNullOrEmpty(Registro))
                return string.Empty;

            var registroFormatado = UtilRegex.RemoverTagsHtmlMidia(Registro);
            return UtilRegex.RemoverTagsHtml(registroFormatado);
        }
    }
}
