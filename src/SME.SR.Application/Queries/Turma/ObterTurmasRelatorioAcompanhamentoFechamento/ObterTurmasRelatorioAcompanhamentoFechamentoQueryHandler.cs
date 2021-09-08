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
                turmas = await ObterTurmasPorCodigo(request.CodigosTurma.ToArray());
            }
            else
            {
                var turmasFiltro = await ObterTurmasPorFiltro(request.CodigoUe, request.AnoLetivo, request.Modalidade, request.Semestre, request.Usuario, request.ConsideraHistorico);

                if (turmasFiltro != null && turmasFiltro.Any())
                    turmas = turmas.Concat(turmasFiltro);
            }

            if (!turmas.Any())
                throw new NegocioException("Não foi possível localizar as turmas");
            else
                return turmas.OrderBy(a => a.Nome);
        }

        private async Task<IEnumerable<Turma>> ObterTurmasPorCodigo(string[] codigosTurma)
        {
            return await mediator.Send(new ObterTurmasPorCodigoQuery(codigosTurma));
        }

        private async Task<IEnumerable<Turma>> ObterTurmasPorFiltro(string ueCodigo, int anoLetivo, Modalidade modalidade, int semestre, Usuario usuario, bool consideraHistorico)
        {
            return await mediator.Send(new ObterTurmasPorAbrangenciaTiposFiltrosQuery()
            {
                CodigoUe = ueCodigo,
                AnoLetivo = anoLetivo,
                Modalidade = modalidade,
                Semestre = semestre,
                Login = usuario.Login,
                Perfil = usuario.PerfilAtual,
                ConsideraHistorico = consideraHistorico
            });
        }
    }
}
