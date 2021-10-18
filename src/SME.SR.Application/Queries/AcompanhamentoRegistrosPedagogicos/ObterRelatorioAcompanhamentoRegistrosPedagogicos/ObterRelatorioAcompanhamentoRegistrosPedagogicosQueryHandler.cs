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

            var turmas = await ObterTurmasPorCodigo(request.Turmas);

            var dadosRelatorio = await ObterDadosComponentesCurriculares(request.ComponentesCurriculares, request.AnoLetivo, request.Turmas, request.ProfessorCodigo, request.ProfessorNome, request.Bimestres);

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

        private async Task<IEnumerable<Turma>> ObterTurmasPorCodigo(long[] turmasId)
        {
            return await mediator.Send(new ObterTurmasPorIdsQuery(turmasId));
        }

        private async Task<List<RelatorioAcompanhamentoRegistrosPedagogicosBimestreDto>> ObterDadosComponentesCurriculares(long[] componentesCurriculares, int anoLetivo, long[] turmasId, string professorNome, string professorCodigo, List<int> bimestres)
        {
            return await mediator.Send(new ObterDadosPedagogicosComponenteCurricularesQuery(componentesCurriculares, anoLetivo, turmasId, professorNome, professorCodigo, bimestres));
        }
    }
}
