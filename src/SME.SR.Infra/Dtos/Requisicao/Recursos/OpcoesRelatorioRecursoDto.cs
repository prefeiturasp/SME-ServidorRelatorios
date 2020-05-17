using Newtonsoft.Json;

namespace SME.SR.Infra.Dtos
{
    public class OpcoesRelatorioRecursoDto : DetalhesRecursoDto
    {
        [JsonProperty("reportUri")]
        public string CaminhoRelatorio { get; set; }

        [JsonProperty("reportParameters")]
        public ParametroRepositorioRecursoDto[] Parametros { get; set; }
    }
}
