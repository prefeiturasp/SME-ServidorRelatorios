using SME.SR.Infra.Utilitarios;
using System;

namespace SME.SR.Infra
{
    public class AcompanhamentoAprendizagemOcorrenciaDto
    {
        public long AlunoCodigo { get; set; }
        public long TurmaId { get; set; }
        public DateTime DataOcorrencia { get; set; }
        public TimeSpan? HoraOcorrencia { get; set; }
        public string TituloOcorrencia { get; set; }
        public string Descricao { get; set; }
        public string TipoOcorrencia { get; set; }

        public string DataRelatorio()
        {
            if (HoraOcorrencia == null)
                return $"{DataOcorrencia:dd/MM/yyyy}";

            return $"{DataOcorrencia:dd/MM/yyyy} {HoraOcorrencia}";
        }
        public string DescricaoFormatada()
        {
            if (string.IsNullOrEmpty(Descricao))
                return string.Empty;

            var registroFormatado = UtilRegex.RemoverTagsHtmlMultiMidia(Descricao);
            return UtilRegex.RemoverTagsHtml(registroFormatado);
        }
    }
}
