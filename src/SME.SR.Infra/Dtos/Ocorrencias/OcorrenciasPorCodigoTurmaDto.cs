using SME.SR.Infra.Utilitarios;
using System;

namespace SME.SR.Infra
{
    public class OcorrenciasPorCodigoTurmaDto
    {
        public long OcorrenciaId { get; set; }
        public long TurmaId { get; set; }
        public string OcorrenciaTitulo { get; set; }
        public string OcorrenciaData { get; set; }
        public string OcorrenciaDescricao { get => UtilRegex.RemoverTagsHtml(UtilRegex.RemoverTagsHtmlMultiMidia(OcorrenciaDescricaoComTagsHtml)); }
        public string OcorrenciaTipo { get; set; }
        public long CodigoAluno { get; set; }

        public string OcorrenciaDescricaoComTagsHtml { get; set; }
    }
}
