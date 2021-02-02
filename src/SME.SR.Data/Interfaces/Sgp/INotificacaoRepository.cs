using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public interface INotificacaoRepository
    {
        Task<IEnumerable<NotificacaoDto>> ObterPorAnoEUsuarioRf(long ano, string usuarioRf);
        Task<IEnumerable<NotificacaoDto>> ObterComFiltros(long ano, string usuarioRf, long[] categorias, 
            long[] tipos, long[] situacoes, bool exibirDescricao = false, bool exibirExcluidas = false, string dre = "-99", string ue = "-99");
    }
}
