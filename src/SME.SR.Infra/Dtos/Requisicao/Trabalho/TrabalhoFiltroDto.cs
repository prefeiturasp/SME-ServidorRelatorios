using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Refit;
using System;
using System.Collections.Generic;
using System.Text;
using static SME.SR.Infra.Enumeradores;

namespace SME.SR.Infra.Dtos
{
    public class TrabalhoFiltroDto
    {
        [AliasAs("label")]
        public string Texto { get; set; }

        [AliasAs("owner")]
        public string Proprietario { get; set; }

        [AliasAs("reportUnitURI")]
        public string CaminhoRelatorio { get; set; }

        [AliasAs("limit")]
        public int Limite { get; set; }

        [AliasAs("offset")]
        public int Deslocamento { get; set; }

        [AliasAs("isAscending")]
        public bool Ascendente { get; set; }

        [AliasAs("sortType")]        
        public OrdenacaoEnum OrdenarPor { get; set; }
    }
}
