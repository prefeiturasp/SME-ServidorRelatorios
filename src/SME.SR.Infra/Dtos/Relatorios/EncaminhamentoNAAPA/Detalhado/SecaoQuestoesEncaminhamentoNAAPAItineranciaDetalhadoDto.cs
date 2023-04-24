using System;

namespace SME.SR.Infra
{
    public class SecaoQuestoesEncaminhamentoNAAPAItineranciaDetalhadoDto : SecaoQuestoesEncaminhamentoNAAPADetalhadoDto
    {
        public SecaoQuestoesEncaminhamentoNAAPAItineranciaDetalhadoDto(string nomeComponenteSecao, string nomeSecao,
            long secaoId, DateTime dataAtendimento, string tipoAtendimento, string criadoPor) : base(nomeComponenteSecao, nomeSecao)
        {
            SecaoId = secaoId;
            DataAtendimento = dataAtendimento;
            TipoAtendimento = tipoAtendimento;
            CriadoPor = criadoPor;
        }

        public long SecaoId { get; set; }
        public DateTime DataAtendimento { get; set; }
        public string TipoAtendimento { get; set; }
        public string CriadoPor { get; set; }
    }
}
