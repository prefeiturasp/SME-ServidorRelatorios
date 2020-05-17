using Newtonsoft.Json;

namespace SME.SR.Infra.Dtos
{
    public class ConexaoMondrianRecursoDto : DetalhesRecursoDto
    {
        [JsonProperty("dataSource")]
        public FonteDadosRecursoDto FonteDados{ get; set; }

        [JsonProperty("schema")]
        public PadraoRecursoDto Padrao { get; set; }
    }
}
