using Newtonsoft.Json;

namespace SME.SR.Infra.Dtos
{
    public class ControleEntradaListaValoresRecursoDto
    {
        [JsonProperty("listOfValuesReference")]
        public CaminhoArquivoRecursoDto CaminhoArquivo { get; set; }
    }
}
