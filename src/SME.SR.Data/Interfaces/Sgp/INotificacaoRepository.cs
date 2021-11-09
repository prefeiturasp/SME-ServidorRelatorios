using SME.SR.Infra;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public interface INotificacaoRepository
    {
        Task<IEnumerable<NotificacaoDto>> ObterPorAnoEUsuarioRf(long ano, string usuarioRf);
        Task<IEnumerable<NotificacaoRetornoDto>> ObterComFiltros(long ano, string usuarioRf, long[] categorias, long[] tipos, long[] situacoes,
            string turmaCodigo, string dreCodigo, string ueCodigo, bool exibirDescricao = false, bool exibirExcluidas = false)
    }
}
