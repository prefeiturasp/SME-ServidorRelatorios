using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application.Queries.ConselhoClasse.ObterConselhosClasseConsolidadoPorTurmasBimestre
{
    class ObterConselhoClasseConsolidadoTurmaQueryHandler : IRequestHandler<ObterConselhoClasseConsolidadoTurmaQuery, IEnumerable<ConselhoClasseConsolidadoTurmaDto>>
    {
        private readonly IConselhoClasseConsolidadoRepository conselhoClasseConsolidadoRepository;

        public ObterConselhoClasseConsolidadoTurmaQueryHandler(IConselhoClasseConsolidadoRepository conselhoClasseConsolidadoRepository)
        {
            this.conselhoClasseConsolidadoRepository = conselhoClasseConsolidadoRepository;
        }

        public async Task<IEnumerable<ConselhoClasseConsolidadoTurmaDto>> Handle(ObterConselhoClasseConsolidadoTurmaQuery request, CancellationToken cancellationToken)
            => await conselhoClasseConsolidadoRepository.ObterConselhosClasseConsolidadoPorTurmasTodasUesAsync(request.DreCodigo, 
                                                                                                               request.ModalidadeCodigo, 
                                                                                                               request.Bimestres, 
                                                                                                               request.SituacaoConselhoClasse, 
                                                                                                               request.AnoLetivo, 
                                                                                                               request.Semestre, 
                                                                                                               request.ExibirHistorico);        
    }
}
