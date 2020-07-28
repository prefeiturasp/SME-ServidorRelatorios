using MediatR;
using SME.SR.Infra;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterRelatorioParecerConclusivoQueryHandler : IRequestHandler<ObterRelatorioParecerConclusivoQuery, RelatorioParecerConclusivoDto>
    {
        public ObterRelatorioParecerConclusivoQueryHandler()
        {

        }
        public async Task<RelatorioParecerConclusivoDto> Handle(ObterRelatorioParecerConclusivoQuery request, CancellationToken cancellationToken)
        {
            var retorno = new RelatorioParecerConclusivoDto();



            return await Task.FromResult(retorno);
        }
    }
}
