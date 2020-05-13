using Refit;
using System.Collections.Generic;
using static SME.SR.Infra.Enumeradores.Enumeradores;

namespace SME.SR.Infra.Dtos.Requisicao
{
    public class ExecutarRelatorioSincronoDto
    {
        [AliasAs("format")]
        public FormatoEnum Formato { get; set; }
        [AliasAs("page")]
        public int? Pagina { get; set; }
        [AliasAs("ignorepagination")]
        public bool? IgnorarPaginacao { get; set; }
        [AliasAs("path")]
        public string Relatorio { get; set; }
        [AliasAs("name")]
        public string Nome { get; set; }
        [AliasAs("interactive")]
        public bool? Interativo { get; set; }
        [AliasAs("onePagePerSheet")]
        public bool? UmaPaginaPorPlanilha { get; set; }
        [AliasAs("baseUrl")]
        public string BaseUrl { get; set; }

        [Query]
        public Dictionary<string, string> ControlesDeEntrada { get; set; }
    }
}
