using Newtonsoft.Json;

namespace SME.SR.Infra.Dtos
{
    public class ControleEntradaTipoInformacaoRecursoDto
    {
        [JsonProperty("dataTypeReference")]
        public CaminhoArquivoRecursoDto CaminhoArquivo { get; set; }
    }
}
