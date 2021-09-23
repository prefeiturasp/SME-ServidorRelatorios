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
    public class ObterRelatorioAcompanhamentoFechamentoConselhoClasseConsolidadoQueryHandler : IRequestHandler<ObterRelatorioAcompanhamentoFechamentoConselhoClasseConsolidadoQuery, RelatorioAcompanhamentoFechamentoConsolidadoPorUeDto>
    {
        private readonly IMediator mediator;

        public ObterRelatorioAcompanhamentoFechamentoConselhoClasseConsolidadoQueryHandler(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator)); ;
        }
        public async Task<RelatorioAcompanhamentoFechamentoConsolidadoPorUeDto> Handle(ObterRelatorioAcompanhamentoFechamentoConselhoClasseConsolidadoQuery request, CancellationToken cancellationToken)
        {
            Dre dre = null;
            Ue ue = null;

            if (request.SituacaoConselhoClasse != null && (int)request.SituacaoConselhoClasse == -99)
                request.SituacaoConselhoClasse = null;

            if (request.SituacaoFechamento != null && (int)request.SituacaoFechamento == -99)
                request.SituacaoFechamento = null;

            if (!string.IsNullOrEmpty(request.DreCodigo))
                dre = await ObterDrePorCodigo(request.DreCodigo);

            if (!string.IsNullOrEmpty(request.UeCodigo))
                ue = await ObterUePorCodigo(request.UeCodigo);


            int[] bimestres = request.Bimestres?.ToArray();

            var consolidadoFechamento = await ObterFechamentosConsolidadoTodasUe(request.DreCodigo, (int)request.Modalidade, bimestres, request.SituacaoFechamento, request.AnoLetivo, request.Semestre);
            var consolidadoConselhosClasse = await ObterConselhosClasseConsolidadoTodasUe(request.DreCodigo, (int)request.Modalidade, bimestres, request.SituacaoConselhoClasse, request.AnoLetivo, request.Semestre);

            if ((consolidadoFechamento == null || !consolidadoFechamento.Any()) &&
               (consolidadoConselhosClasse == null || !consolidadoConselhosClasse.Any()))
                throw new NegocioException("Acompanhamento de Fechamentos das turmas do filtro não encontrado");

            return await mediator.Send(new MontasRelatorioAcompanhamentoFechamentoConselhoClasseConsolidadoQuery(dre, ue, bimestres, consolidadoFechamento, consolidadoConselhosClasse, request.TurmasCodigo?.ToArray(), request.Usuario));
        }

        private async Task<IEnumerable<FechamentoConsolidadoTurmaDto>> ObterFechamentosConsolidadoTodasUe(string dreCodigo, int modalidadeId, int[] bimestres, SituacaoFechamento? situacao, int anoLetivo, int semestre)
            => await mediator.Send(new ObterFechamentoConsolidadoTurmaQuery(dreCodigo, modalidadeId, anoLetivo, bimestres, situacao, semestre));


        private async Task<IEnumerable<ConselhoClasseConsolidadoTurmaDto>> ObterConselhosClasseConsolidadoTodasUe(string dreCodigo, int modalidadeId, int[] bimestres, SituacaoConselhoClasse? situacao, int anoLetivo, int semestre)
            => await mediator.Send(new ObterConselhoClasseConsolidadoTurmaQuery(dreCodigo, modalidadeId, bimestres, situacao, anoLetivo, semestre));

        private async Task<Dre> ObterDrePorCodigo(string dreCodigo)
            => await mediator.Send(new ObterDrePorCodigoQuery(dreCodigo));

        private async Task<Ue> ObterUePorCodigo(string ueCodigo)
            => await mediator.Send(new ObterUePorCodigoQuery(ueCodigo));

    }
}
