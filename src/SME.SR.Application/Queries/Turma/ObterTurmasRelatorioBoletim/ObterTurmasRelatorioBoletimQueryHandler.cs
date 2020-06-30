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

            if (string.IsNullOrEmpty(request.CodigoTurma))
            {
                var turma = await ObterTurmaPorCodigo(request.CodigoTurma);

                if (turma != null)
                    turmas.Append(turma);
            }
            else
            {
                var turmasFiltro = await ObterTurmasPorFiltro(request.CodigoUe, request.AnoLetivo, request.Modalidade, request.Semestre);

                if (turmasFiltro != null && turmas.Any())
                    turmas.Concat(turmasFiltro);
            }

            return turmas;
        }

        private async Task<Turma> ObterTurmaPorCodigo(string codigoTurma)
        {
            return await mediator.Send(new ObterTurmaQuery()
            {
                CodigoTurma = codigoTurma
            });
        }

        private async Task<IEnumerable<Turma>> ObterTurmasPorFiltro(string ueCodigo, int? anoLetivo, Modalidade? modalidade, int? semestre)
        {
            return await mediator.Send(new ObterTurmasPorFiltroQuery()
            {
                CodigoUe = ueCodigo,
                AnoLetivo = anoLetivo,
                Modalidade = modalidade,
                Semestre = semestre
            });
        }
    }
}
