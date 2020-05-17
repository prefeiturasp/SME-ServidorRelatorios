using Newtonsoft.Json;

namespace SME.SR.Infra.Dtos
{
    public class ConexaoSeguraMondrianRecursoDto : ConexaoMondrianRecursoDto
    {
        [JsonProperty("accessGrantSchemas")]
        public PadraoConcessaoAcessoRecursoDto[] Acesso { get; set; }
    }
}
