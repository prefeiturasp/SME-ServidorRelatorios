using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Data
{
    public class ComponenteRegenciaComNotaBimestre
    {
        [JsonProperty("Componente")]
        public string Componente { get; set; }

        [JsonProperty("EhEja")]
        public bool EhEja { get; set; }

        [JsonProperty("NotaConceito")]
        public string NotaConceito { get; set; }

        [JsonProperty("NotaPosConselho")]
        public string NotaPosConselho { get; set; }
    }
}
