using Refit;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra.Dtos { 
    public class ExecutarRelatorioSincronoDto
    {
        [Query, AliasAs("page")]
        public int? Pagina { get; set; }

        [Query, AliasAs("ignorepagination")]
        public bool? IgnorarPaginacao { get; set; }

        [Query, AliasAs("interactive")]
        public bool? Interativo { get; set; }

        [Query, AliasAs("onePagePerSheet")]
        public bool? UmaPaginaPorPlanilha { get; set; }

        [Query, AliasAs("baseUrl")]
        public string BaseUrl { get; set; }

        [Query]
        public Dictionary<string, string> ControlesDeEntrada { get; set; }
    }
}
