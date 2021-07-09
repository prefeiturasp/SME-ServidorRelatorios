using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterTurmasRelatorioAcompanhamentoFechamentoQueryHandler : IRequestHandler<ObterTurmasRelatorioAcompanhamentoFechamentoQuery, IEnumerable<Turma>>
    {
        private readonly IMediator mediator;

        public ObterTurmasRelatorioAcompanhamentoFechamentoQueryHandler(IMediator mediator)
        {
            this.mediator = mediator ?? throw new System.ArgumentNullException(nameof(mediator));
        }

        public async Task<IEnumerable<Turma>> Handle(ObterTurmasRelatorioAcompanhamentoFechamentoQuery request, CancellationToken cancellationToken)
        {
            var turmas = Enumerable.Empty<Turma>();

            if (request.CodigosTurma != null && request.CodigosTurma.Any())
            {
                turmas = await ObterTurmasPorCodigoESituacaoConsolidacao(request.CodigosTurma.ToArray(), request.SituacaoFechamento, request.SituacaoConselhoClasse, request.Bimestres);
            }
            else
            {
                var turmasFiltro = await ObterTurmasPorFiltroSituacaoConsolidado(request.CodigoUe, request.AnoLetivo, request.Modalidade, request.Semestre, request.Usuario, request.ConsideraHistorico, request.SituacaoConselhoClasse, request.SituacaoFechamento, request.Bimestres);

                if (turmasFiltro != null && turmasFiltro.Any())
                    turmas = turmas.Concat(turmasFiltro);
            }

            if (!turmas.Any())
                throw new NegocioException("Não foi possível localizar as turmas");
            else
                return turmas.OrderBy(a => a.Nome);
        }

        private async Task<IEnumerable<Turma>> ObterTurmasPorCodigoESituacaoConsolidacao(string[] codigosTurma, SituacaoFechamento? situacaoFechamento, SituacaoConselhoClasse? situacaoConselhoClasse, int[] bimestres)
        {
            return await mediator.Send(new ObterTurmasSituacaoConsolidacaoQuery(codigosTurma, situacaoFechamento, situacaoConselhoClasse, bimestres));
        }

        private async Task<IEnumerable<Turma>> ObterTurmasPorFiltroSituacaoConsolidado(string ueCodigo, int anoLetivo, Modalidade modalidade, int semestre, Usuario usuario, bool consideraHistorico, SituacaoConselhoClasse? situacaoConselhoClasse, SituacaoFechamento? situacaoFechamento , int[] bimestres)
        {
            return await mediator.Send(new ObterTurmasPorAbrangenciaTiposFiltrosQuery()
            {
                CodigoUe = ueCodigo,
                AnoLetivo = anoLetivo,
                Modalidade = modalidade,
                Semestre = semestre,
                Login = usuario.Login,
                Perfil = usuario.PerfilAtual,
                ConsideraHistorico = consideraHistorico,
                SituacaoFechamento = situacaoFechamento,
                SituacaoConselhoClasse = situacaoConselhoClasse,
                Bimestres = bimestres

            });
        }
    }
}
