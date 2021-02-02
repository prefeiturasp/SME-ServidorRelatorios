using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterHistoricoReinicioSenhaUsuarioPorDreQuery : IRequest<IEnumerable<HistoricoReinicioSenhaDto>>
    {
        public ObterHistoricoReinicioSenhaUsuarioPorDreQuery(string codigoDre)
        {
            CodigoDre = codigoDre;
        }

        public string CodigoDre { get; set; }
    }
}
