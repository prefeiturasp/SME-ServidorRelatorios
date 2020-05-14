using Newtonsoft.Json;

namespace SME.SR.Infra.Dtos
{
    public class ExportacoesDto
    {
        [JsonProperty("export")]
        public ExportacaoDto Exportacao { get; set; }
    }
}
