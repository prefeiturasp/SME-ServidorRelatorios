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

            var turmas = await ObterTurmasRelatorioPorSituacaoConsolidacao(request.TurmasCodigo?.ToArray(), request.UeCodigo, request.AnoLetivo, request.Modalidade, request.Semestre, request.Usuario, request.AnoLetivo < DateTime.Now.Year, request.SituacaoFechamento, request.SituacaoConselhoClasse, bimestres, request.DreCodigo);
            if(turmas == null && turmas.Any())
                throw new NegocioException("As turmas selecionadas não possuem fechamento.");

            string[] codigosTurma = turmas.Select(t => t.Codigo).ToArray();
            var consolidadoFechamento = await ObterFechamentosConsolidadoTodasUe(codigosTurma);
            var consolidadoConselhosClasse = await ObterConselhosClasseConsolidadoTodasUe(codigosTurma);

            if ((consolidadoFechamento == null || !consolidadoFechamento.Any()) &&
               (consolidadoConselhosClasse == null || !consolidadoConselhosClasse.Any()))
                throw new NegocioException("Acompanhamento de Fechamentos das turmas do filtro não encontrado");

            return await mediator.Send(new MontasRelatorioAcompanhamentoFechamentoConselhoClasseConsolidadoQuery(dre, ue, turmas, bimestres, consolidadoFechamento, consolidadoConselhosClasse, request.TurmasCodigo?.ToArray(), request.Usuario));
        }

        private async Task<IEnumerable<FechamentoConsolidadoTurmaDto>> ObterFechamentosConsolidadoTodasUe(string[] turmasId)
        {
            return await mediator.Send(new ObterFechamentoConsolidadoTurmaQuery(turmasId));
        }
        private async Task<IEnumerable<ConselhoClasseConsolidadoTurmaDto>> ObterConselhosClasseConsolidadoTodasUe(string[] turmasId)
        {
            return await mediator.Send(new ObterConselhoClasseConsolidadoTurmaQuery(turmasId));
        }
        private async Task<Dre> ObterDrePorCodigo(string dreCodigo)
        {
            return await mediator.Send(new ObterDrePorCodigoQuery()
            {
                DreCodigo = dreCodigo
            });
        }

        private async Task<Ue> ObterUePorCodigo(string ueCodigo)
        {
            return await mediator.Send(new ObterUePorCodigoQuery(ueCodigo));
        }
        private async Task<IEnumerable<Turma>> ObterTurmasRelatorioPorSituacaoConsolidacao(string[] turmasCodigo, string ueCodigo, int anoLetivo, Modalidade modalidade, int semestre, Usuario usuario, bool consideraHistorico, SituacaoFechamento? situacaoFechamento, SituacaoConselhoClasse? situacaoConselhoClasse, int[] bimestres, string dreCodigo)
        {
            return await mediator.Send(new ObterTurmasRelatorioAcompanhamentoFechamentoQuery()
            {
                CodigosTurma = turmasCodigo,
                CodigoUe = ueCodigo,
                Modalidade = modalidade,
                AnoLetivo = anoLetivo,
                Semestre = semestre,
                Usuario = usuario,
                ConsideraHistorico = consideraHistorico,
                SituacaoConselhoClasse = situacaoConselhoClasse,
                SituacaoFechamento = situacaoFechamento,
                Bimestres = bimestres,
                CodigoDre = dreCodigo
            });
        }

    }
}
