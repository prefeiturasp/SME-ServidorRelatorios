using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterRelatorioAcompanhamentoRegistrosPedagogicosQueryHandler : IRequestHandler<ObterRelatorioAcompanhamentoRegistrosPedagogicosQuery, RelatorioAcompanhamentoRegistrosPedagogicosDto>
    {
        private readonly IMediator mediator;

        public ObterRelatorioAcompanhamentoRegistrosPedagogicosQueryHandler(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<RelatorioAcompanhamentoRegistrosPedagogicosDto> Handle(ObterRelatorioAcompanhamentoRegistrosPedagogicosQuery request, CancellationToken cancellationToken)
        {
            Dre dre = null;
            Ue ue = null;

            if (!string.IsNullOrEmpty(request.DreCodigo))
                dre = await ObterDrePorCodigo(request.DreCodigo);

            if (!string.IsNullOrEmpty(request.UeCodigo))
                ue = await ObterUePorCodigo(request.UeCodigo);

            int[] bimestres = request.Bimestres?.ToArray();

            var turmas = await ObterTurmasPorCodigo(request.TurmasCodigo);

            var dadosRelatorio = await ObterDadosComponentesCurriculares(request.DreCodigo, request.UeCodigo, request.ComponentesCurriculares, request.AnoLetivo, 
                request.TurmasCodigo, request.ProfessorCodigo, request.ProfessorNome, request.Bimestres, request.Modalidade, request.Semestre);

            return await mediator.Send(new MontarRelatorioAcompanhamentoRegistrosPedagogicosQuery(dre, ue, turmas, dadosRelatorio, bimestres, request.UsuarioNome, request.UsuarioRF));
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

        private async Task<IEnumerable<Turma>> ObterTurmasPorCodigo(List<string> turmasCodigo)
        {
            return await mediator.Send(new ObterTurmasPorCodigoQuery(turmasCodigo?.ToArray()));
        }

        private async Task<List<RelatorioAcompanhamentoRegistrosPedagogicosBimestreDto>> ObterDadosComponentesCurriculares(string dreCodigo, string ueCodigo, long[] componentesCurriculares, int anoLetivo, List<string> turmasCodigo, string professorCodigo, string professorNome, List<int> bimestres, Modalidade modalidade, int semestre)
        {
            return await mediator.Send(new ObterDadosPedagogicosComponenteCurricularesQuery(dreCodigo, ueCodigo, componentesCurriculares, anoLetivo, turmasCodigo, professorCodigo, bimestres, modalidade, semestre));
        }
    }
}
