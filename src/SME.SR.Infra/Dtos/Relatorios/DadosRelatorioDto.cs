using System;

namespace SME.SR.Infra
{
    public class DadosRelatorioDto
    {
        public DadosRelatorioDto(Guid requisicaoId, Guid exportacaoId, Guid correlacaoId, string jSessionId)
        {
            RequisicaoId = requisicaoId;
            ExportacaoId = exportacaoId;
            CorrelacaoId = correlacaoId;
            JSessionId = jSessionId;
        }

        public Guid RequisicaoId { get; set; }
        public Guid ExportacaoId { get; set; }
        public Guid CorrelacaoId { get; set; }
        public string JSessionId { get; }
    }
}
