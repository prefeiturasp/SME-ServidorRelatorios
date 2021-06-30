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

            if(!string.IsNullOrEmpty(request.DreCodigo))
                dre = await ObterDrePorCodigo(request.DreCodigo);

            if (!string.IsNullOrEmpty(request.UeCodigo))
                ue = await ObterUePorCodigo(request.UeCodigo);

            var turmas = await ObterTurmasRelatorio(request.TurmaCodigo, request.UeCodigo, request.AnoLetivo, request.Modalidade, request.Semestre, request.Usuario, request.AnoLetivo < DateTime.Now.Year);
            string[] codigosTurma = turmas.Select(t => t.Codigo).ToArray();
            int[] bimestres = request.Bimestres?.ToArray();

            var consolidadoFechamento = await ObterFechamentosConsolidado(codigosTurma, bimestres, request.SituacaoFechamento);
            var consolidadoConselhosClasse = await ObterConselhosClasseConsolidado(codigosTurma, bimestres, request.SituacaoConselhoClasse);

            var pendencias = Enumerable.Empty<PendenciaParaFechamentoConsolidadoDto>();

            if (request.ListarPendencias)
            {
                var componentesCurricularesId = consolidadoFechamento?.Select(c => c.ComponenteCurricularCodigo)?.Distinct()?.ToArray();

                if (componentesCurricularesId != null && componentesCurricularesId.Any())
                    pendencias = await ObterPendenciasFechamentosConsolidado(codigosTurma, bimestres, componentesCurricularesId);
            }

            return await mediator.Send(new MontarRelatorioAcompanhamentoFechamentoQuery(dre, ue, request.TurmaCodigo, turmas, bimestres, consolidadoFechamento, consolidadoConselhosClasse, pendencias, request.Usuario));
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

        private async Task<IEnumerable<Turma>> ObterTurmasRelatorio(string turmaCodigo, string ueCodigo, int anoLetivo, Modalidade modalidade, int semestre, Usuario usuario, bool consideraHistorico)
        {
            try
            {
                return await mediator.Send(new ObterTurmasRelatorioBoletimQuery()
                {
                    CodigoTurma = turmaCodigo,
                    CodigoUe = ueCodigo,
                    Modalidade = modalidade,
                    AnoLetivo = anoLetivo,
                    Semestre = semestre,
                    Usuario = usuario,
                    ConsideraHistorico = consideraHistorico
                });
            }
            catch (NegocioException)
            {
                throw new NegocioException("As turmas selecionadas não possuem fechamento.");
            }
        }

        private async Task<IEnumerable<ConselhoClasseConsolidadoTurmaAlunoDto>> ObterConselhosClasseConsolidado(string[] turmasId, int[] bimestres, SituacaoConselhoClasse? situacaoConselhoClasse)
        {
            return await mediator.Send(new ObterConselhosClasseConsolidadoPorTurmasBimestreQuery(turmasId, bimestres, (int)situacaoConselhoClasse));
        }

        private async Task<IEnumerable<FechamentoConsolidadoComponenteTurmaDto>> ObterFechamentosConsolidado(string[] turmasId, int[] bimestres, SituacaoFechamento? situacaoFechamento)
        {
            return await mediator.Send(new ObterFechamentoConsolidadoPorTurmasBimestreQuery(turmasId, bimestres, (int)situacaoFechamento));
        }
    }
}
