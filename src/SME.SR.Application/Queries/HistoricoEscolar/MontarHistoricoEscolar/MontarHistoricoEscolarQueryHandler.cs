using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class MontarHistoricoEscolarQueryHandler : IRequestHandler<MontarHistoricoEscolarQuery, HistoricoEscolarDTO>
    {
        public MontarHistoricoEscolarQueryHandler()
        {
            
        }

        public async Task<HistoricoEscolarDTO> Handle(MontarHistoricoEscolarQuery request, CancellationToken cancellationToken)
        {

            var retorno = new HistoricoEscolarDTO();

            foreach (var turmaCodigo in request.TurmasCodigo)
            {






            }

            return retorno;

        }
    }
}
