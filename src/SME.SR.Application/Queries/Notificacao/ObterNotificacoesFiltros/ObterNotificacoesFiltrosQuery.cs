using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterNotificacoesFiltrosQuery : IRequest<IEnumerable<NotificacaoDto>>
    {
        public ObterNotificacoesFiltrosQuery(long ano, string usuarioRf, long[] categorias, 
            long[] tipos, long[] situacoes, bool exibirDescricao, bool exibirExcluidas, string dre, string ue, string turmaCodigo)
        {
            Ano = ano;
            UsuarioRf = usuarioRf;
            Categorias = categorias;
            Tipos = tipos;
            Situacoes = situacoes;
            ExibirDescricao = exibirDescricao;
            ExibirExcluidas = exibirExcluidas;
            DRE = dre;
            UE = ue;
            TurmaCodigo = turmaCodigo;
        }
        public long Ano { get; set; }
        public long[] Categorias { get; set; }
        public long[] Tipos { get; set; }
        public long[] Situacoes { get; set; }

        public bool ExibirDescricao { get; set; }
        public bool ExibirExcluidas { get; set; }
        public string UsuarioRf { get; set; }
        public string DRE { get; set; }
        public string UE { get; set; }
        public string TurmaCodigo { get; set; }
    }
}
