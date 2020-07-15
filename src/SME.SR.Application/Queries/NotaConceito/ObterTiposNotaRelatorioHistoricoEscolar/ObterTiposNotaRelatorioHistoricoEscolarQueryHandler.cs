using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterTiposNotaRelatorioHistoricoEscolarQueryHandler : IRequestHandler<ObterTiposNotaRelatorioHistoricoEscolarQuery, IEnumerable<TipoNotaCicloAno>>
    {
        private readonly INotaTipoRepository notaTipoRepository;

        public ObterTiposNotaRelatorioHistoricoEscolarQueryHandler(INotaTipoRepository notaTipoRepository)
        {
            this.notaTipoRepository = notaTipoRepository ?? throw new ArgumentNullException(nameof(notaTipoRepository));
        }
        public async Task<IEnumerable<TipoNotaCicloAno>> Handle(ObterTiposNotaRelatorioHistoricoEscolarQuery request, CancellationToken cancellationToken)
        {
            var notasTipo = await notaTipoRepository.Listar();

            if (notasTipo == null || !notasTipo.Any())
                throw new NegocioException("Não foi possível os tipos de nota");

            return notasTipo;
        }
    }
}
