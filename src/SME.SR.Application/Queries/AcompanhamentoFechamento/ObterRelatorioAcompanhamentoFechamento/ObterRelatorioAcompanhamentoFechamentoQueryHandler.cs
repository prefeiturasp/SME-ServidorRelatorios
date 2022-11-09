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
    public class ObterRelatorioAcompanhamentoFechamentoQueryHandler : IRequestHandler<ObterRelatorioAcompanhamentoFechamentoQuery, RelatorioAcompanhamentoFechamentoPorUeDto>
    {
        private readonly IMediator mediator;

        public ObterRelatorioAcompanhamentoFechamentoQueryHandler(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator)); ;
        }

        public async Task<RelatorioAcompanhamentoFechamentoPorUeDto> Handle(ObterRelatorioAcompanhamentoFechamentoQuery request, CancellationToken cancellationToken)
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

            var turmas = await ObterTurmasRelatorioPorSituacaoConsolidacao(request.TurmasCodigo?.ToArray(), request.UeCodigo, request.AnoLetivo, request.Modalidade, request.Semestre, request.Usuario, request.AnoLetivo < DateTime.Now.Year, request.SituacaoFechamento, request.SituacaoConselhoClasse, bimestres);
            string[] codigosTurma = turmas.Select(t => t.Codigo).ToArray();
            int[] semestres = turmas.Select(t => t.Semestre).ToArray();
            var consolidadoFechamento = await ObterFechamentosConsolidado(codigosTurma, semestres, bimestres);
            var consolidadoConselhosClasse = await ObterConselhosClasseConsolidado(codigosTurma);
            if ((consolidadoFechamento == null || !consolidadoFechamento.Any()) &&
                (consolidadoConselhosClasse == null || !consolidadoConselhosClasse.Any()))
                throw new NegocioException("Acompanhamento de Fechamentos das turmas do filtro não encontrado");

            var componentesCurricularesId = consolidadoFechamento?.Select(c => c.ComponenteCurricularCodigo)?.Distinct()?.ToArray();

            var componentesCurriculares = await ObterComponentesCurricularesPorCodigo(componentesCurricularesId, codigosTurma);

            componentesCurriculares = componentesCurriculares.ToList().OrderBy(c => c.Disciplina);
            var pendencias = Enumerable.Empty<PendenciaParaFechamentoConsolidadoDto>();

            if (request.ListarPendencias && componentesCurricularesId != null && componentesCurricularesId.Any())
            {
                pendencias = await ObterPendenciasFechamentosConsolidado(codigosTurma, bimestres, componentesCurricularesId);
            }
            return await mediator.Send(new MontarRelatorioAcompanhamentoFechamentoQuery(dre, ue, request.TurmasCodigo?.ToArray(), turmas, componentesCurriculares, bimestres, consolidadoFechamento, consolidadoConselhosClasse, request.ListarPendencias, pendencias, request.Usuario));

        }

        private async Task<IEnumerable<ComponenteCurricularPorTurma>> OrdenarComponentes(IEnumerable<ComponenteCurricularPorTurma> componentesCurriculares)
        {
            return await mediator.Send(new OrdenarComponentesPorGrupoMatrizAreaConhecimentoQuery(componentesCurriculares));
        }

        private async Task<IEnumerable<ComponenteCurricularPorTurma>> ObterComponentesCurricularesPorCodigo(long[] componentesCurricularesId, string[] codigosTurma)
        {
            return await mediator.Send(new ObterComponentesCurricularesEolPorIdsQuery(componentesCurricularesId, codigosTurma));
        }

        private async Task<IEnumerable<PendenciaParaFechamentoConsolidadoDto>> ObterPendenciasFechamentosConsolidado(string[] codigosTurma, int[] bimestres, long[] componentesCurricularesId)
        {
            return await mediator.Send(new ObterPendenciasParaFechamentoConsolidadoQuery(codigosTurma, bimestres, componentesCurricularesId));
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

        private async Task<IEnumerable<Turma>> ObterTurmasRelatorioPorSituacaoConsolidacao(string[] turmasCodigo, string ueCodigo, int anoLetivo, Modalidade modalidade, int semestre, Usuario usuario, bool consideraHistorico, SituacaoFechamento? situacaoFechamento, SituacaoConselhoClasse? situacaoConselhoClasse, int[] bimestres)
        {
            try
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
                    Bimestres = bimestres

                });
            }
            catch (NegocioException)
            {
                throw new NegocioException("As turmas selecionadas não possuem fechamento.");
            }
        }

        private async Task<IEnumerable<ConselhoClasseConsolidadoTurmaAlunoDto>> ObterConselhosClasseConsolidado(string[] turmasId)
        {
            return await mediator.Send(new ObterConselhosClasseConsolidadoPorTurmasQuery(turmasId));
        }

        private async Task<IEnumerable<FechamentoConsolidadoComponenteTurmaDto>> ObterFechamentosConsolidado(string[] turmasId, int[] semestres = null, int[] bimestres = null)
        {
            return await mediator.Send(new ObterFechamentoConsolidadoPorTurmasQuery(turmasId,semestres,bimestres));
        }
    }
}
