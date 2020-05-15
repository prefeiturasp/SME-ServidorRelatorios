using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra.Dtos
{
    public class ListaEstadosControleEntradaDto
    {
        [JsonProperty("inputControlState")]
        public IList<EstadoDto> EstadosControleEntrada { get; set; }
    }
}
