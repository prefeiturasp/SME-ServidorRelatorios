using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterTurmasPorAbrangenciaTiposFiltrosQueryHandler : IRequestHandler<ObterTurmasPorAbrangenciaTiposFiltrosQuery, IEnumerable<Turma>>
    {
        private readonly ITurmaRepository turmaSgpRepository;

        public ObterTurmasPorAbrangenciaTiposFiltrosQueryHandler(ITurmaRepository turmaSgpRepository)
        {
            this.turmaSgpRepository = turmaSgpRepository ?? throw new ArgumentNullException(nameof(turmaSgpRepository));
        }

        public async Task<IEnumerable<Turma>> Handle(ObterTurmasPorAbrangenciaTiposFiltrosQuery request, CancellationToken cancellationToken)
        {
            return await turmaSgpRepository
                .ObterPorAbrangenciaTiposFiltros(request.CodigoUe,
                                            request.Login,
                                            request.Perfil,
                                            request.Modalidade,
                                            request.Tipos,
                                            request.SituacaoFechamento,
                                            request.SituacaoConselhoClasse,
                                            request.Bimestres,
                                            request.Semestre,
                                            request.ConsideraHistorico,
                                            request.AnoLetivo,
                                            request.PossuiFechamento,
                                            request.SomenteEscolarizadas,
                                            request.CodigoDre);
        }
    }
}
