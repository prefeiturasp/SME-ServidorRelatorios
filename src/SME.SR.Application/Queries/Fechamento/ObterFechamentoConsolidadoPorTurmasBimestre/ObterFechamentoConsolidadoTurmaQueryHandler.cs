using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterFechamentoConsolidadoTurmaQueryHandler : IRequestHandler<ObterFechamentoConsolidadoTurmaQuery, IEnumerable<FechamentoConsolidadoTurmaDto>>
    {
        private readonly IFechamentoConsolidadoRepository fechamentoConsolidadoRepository;
        public ObterFechamentoConsolidadoTurmaQueryHandler(IFechamentoConsolidadoRepository fechamentoConsolidadoRepository)
        {
            this.fechamentoConsolidadoRepository = fechamentoConsolidadoRepository ?? throw new System.ArgumentNullException(nameof(fechamentoConsolidadoRepository));
        }

        public async Task<IEnumerable<FechamentoConsolidadoTurmaDto>> Handle(ObterFechamentoConsolidadoTurmaQuery request, CancellationToken cancellationToken)
            => await fechamentoConsolidadoRepository.ObterFechamentoConsolidadoPorTurmasTodasUe(request.DreCodigo, 
                                                                                                request.ModalidadeCodigo, 
                                                                                                request.Bimestres, 
                                                                                                request.Situacao, 
                                                                                                request.AnoLetivo, 
                                                                                                request.Semestre,
                                                                                                request.ExibirHistorico);        
    }
}
