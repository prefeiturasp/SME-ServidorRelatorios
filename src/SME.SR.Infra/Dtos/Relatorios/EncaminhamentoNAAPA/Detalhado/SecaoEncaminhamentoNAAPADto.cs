using System;
using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class SecaoEncaminhamentoNAAPADto
    {
        public long SecaoId { get; set; }
        public DateTime DataAtendimento { get; set; }
        public string TipoAtendimento { get; set; }
        public string CriadoPor { get; set; }
        public IEnumerable<QuestaoDto> Questoes { get; set; }
    }
}
