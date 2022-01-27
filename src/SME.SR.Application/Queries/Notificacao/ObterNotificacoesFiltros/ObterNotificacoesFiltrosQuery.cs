using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterNotificacoesFiltrosQuery : IRequest<IEnumerable<NotificacaoRetornoDto>>
    {
        public ObterNotificacoesFiltrosQuery(long anoLetivo, string usuarioRf, long[] categorias, 
            long[] tipos, long[] situacoes, bool exibirDescricao, bool exibirExcluidas, string dreCodigo, string ueCodigo, string turmaCodigo)
        {
            AnoLetivo = anoLetivo;
            UsuarioRf = usuarioRf;
            Categorias = categorias;
            Tipos = tipos;
            Situacoes = situacoes;
            ExibirDescricao = exibirDescricao;
            ExibirExcluidas = exibirExcluidas;
            DreCodigo = dreCodigo;
            UeCodigo = ueCodigo;
            TurmaCodigo = turmaCodigo;
        }
        public long AnoLetivo { get; set; }
        public long[] Categorias { get; set; }
        public long[] Tipos { get; set; }
        public long[] Situacoes { get; set; }
        public bool ExibirDescricao { get; set; }
        public bool ExibirExcluidas { get; set; }
        public string UsuarioRf { get; set; }
        public string DreCodigo { get; set; }
        public string UeCodigo { get; set; }
        public string TurmaCodigo { get; set; }
    }
}
