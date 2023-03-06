using Newtonsoft.Json;
using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class HistoricoEscolarDTO
    {
        [JsonProperty("nomeDre")]
        public string NomeDre { get; set; }
        [JsonProperty("cabecalho")]
        public CabecalhoDto Cabecalho { get; set; }
        [JsonProperty("informacoesAluno")]
        public InformacoesAlunoDto InformacoesAluno { get; set; }
        [JsonProperty("ciclos")]
        public List<CicloDto> Ciclos { get; set; }
       
        [JsonProperty("historicoEscolar")]
        public HistoricoEscolarNotasFrequenciaDto DadosHistorico { get; set; }

        [JsonProperty("legenda")]
        public LegendaDto Legenda { get; set; }
        public Modalidade Modalidade { get; set; }

        [JsonProperty("responsaveisUe")]
        public ResponsaveisUeDto ResponsaveisUe { get; set; }

        [JsonProperty("dadosData")]
        public DadosDataDto DadosData { get; set; }

        [JsonProperty("estudosRealizados")]
        public List<UeConclusaoDto> EstudosRealizados { get; set; }

        [JsonProperty("historicoEscolarTransferencia")]
        public TransferenciaDto DadosTransferencia{ get; set; }
        [JsonProperty("observacaoComplementar")]
        public string ObservacaoComplementar { get; set; }
    }
}
