using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterNotificacoesPorAnoEUsuarioRfQuery : IRequest<IEnumerable<NotificacaoDto>>
    {
        public ObterNotificacoesPorAnoEUsuarioRfQuery(long ano, string usuarioRf)
        {
            Ano = ano;
            UsuarioRf = usuarioRf;
        }
        public long Ano { get; set; }
        public string UsuarioRf { get; set; }
    }
}
