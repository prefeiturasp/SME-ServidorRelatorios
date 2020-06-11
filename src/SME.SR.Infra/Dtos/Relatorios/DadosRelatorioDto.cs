using System;

namespace SME.SR.Infra
{
    public class DadosRelatorioDto
    {
        public DadosRelatorioDto(Guid requisicaoId, Guid exportacaoId, Guid codigoCorrelacao, string jSessionId)
        {
            RequisicaoId = requisicaoId;
            ExportacaoId = exportacaoId;
            CodigoCorrelacao = codigoCorrelacao;
            JSessionId = jSessionId;
        }

        public Guid RequisicaoId { get; set; }
        public Guid ExportacaoId { get; set; }
        public Guid CodigoCorrelacao { get; set; }
        public string JSessionId { get; }
    }
}
