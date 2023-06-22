using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterTurmasRelatorioBoletimQueryHandler : IRequestHandler<ObterTurmasRelatorioBoletimQuery, IEnumerable<Turma>>
    {
        private IMediator mediator;

        public ObterTurmasRelatorioBoletimQueryHandler(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public async Task<IEnumerable<Turma>> Handle(ObterTurmasRelatorioBoletimQuery request, CancellationToken cancellationToken)
        {
            var turmas = Enumerable.Empty<Turma>();

            if (!string.IsNullOrEmpty(request.CodigoTurma))
            {
                var turma = await ObterTurmaPorCodigo(request.CodigoTurma);

                if (turma != null)
                    turmas = turmas.Append(turma);
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

        private async Task<IEnumerable<FechamentoTurma>> ObterFechamentosPorCodigosTurma(string[] codigosTurma)
        {
            return await mediator.Send(new ObterFechamentosPorCodigosTurmaQuery()
            {
                CodigosTurma = codigosTurma
            });
        }

        private async Task<Turma> ObterTurmaPorCodigo(string codigoTurma)
        {
            return await mediator.Send(new ObterTurmaQuery()
            {
                CodigoTurma = codigoTurma
            });
        }

        private async Task<IEnumerable<Turma>> ObterTurmasPorFiltro(string ueCodigo, int anoLetivo, Modalidade modalidade, int semestre, Usuario usuario, bool consideraHistorico)
        {
            var turmaAbrangencia = await mediator.Send(new ObterTurmasPorAbrangenciaFiltroQuery()
            {
                CodigoUe = ueCodigo,
                AnoLetivo = anoLetivo,
                Modalidade = modalidade,
                Semestre = semestre,
                Login = usuario.Login,
                Perfil = usuario.PerfilAtual,
                ConsideraHistorico = consideraHistorico,
                PossuiFechamento = true,
                SomenteEscolarizadas = true
            });

            var codigoTumas = turmaAbrangencia.Select(turma => turma.Codigo).ToArray();

            return await mediator.Send(new ObterTurmasPorCodigoQuery(codigoTumas));
        }
    }
}
