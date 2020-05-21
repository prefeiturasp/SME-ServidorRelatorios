using Newtonsoft.Json;
using System.Collections.Generic;

namespace SME.SR.Infra.Dtos
{
    public class ListaValoresRecursoDto : DetalhesRecursoDto
    {
        [JsonProperty("items")]
        public IEnumerable<ItemListaValoresRecursoDto> Items { get; set; }
    }
}
