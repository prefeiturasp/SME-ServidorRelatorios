using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra.Dtos
{
    public class ListaControlesEntradaDto
    {
        [JsonProperty("inputControl")]
        public IEnumerable<ControleEntradaDto> ControleEntrada { get; set; }
    }
}
