using Refit;

namespace SME.SR.Infra.Dtos
{
    public class BuscaRepositorioRequisicaoDto
    {
        [AliasAs("q")]
        public string Query { get; set; }

        [AliasAs("folderUri")]
        public string CaminhoPasta { get; set; }

        [AliasAs("recursive")]
        public bool Recursivo { get; set; } = true;

        [AliasAs("type")]
        public string Tipo { get; set; }

        [AliasAs("accessType")]
        public string TipoAcesso { get; set; }

        [AliasAs("dependsOn")]
        public string DiretorioDependencia { get; set; }

        [AliasAs("showHiddenItems")]
        public bool ExibirItensOcultos { get; set; }

        [AliasAs("sortBy")]
        public string Ordenacao { get; set; }

        [AliasAs("limit")]
        public int? LimitePaginacao { get; set; }

        [AliasAs("offset")]
        public int? Offset { get; set; }

        [AliasAs("forceFullPage")]
        public bool? ForcarPaginaInteira { get; set; }

        [AliasAs("forceTotalCount")]
        public bool? ForcarContadorPaginas { get; set; }
    }
}
