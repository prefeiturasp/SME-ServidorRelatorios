using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterRelatorioAcompanhamentoRegistrosPedagogicosInfantilQueryHandler : IRequestHandler<ObterRelatorioAcompanhamentoRegistrosPedagogicosInfantilQuery, RelatorioAcompanhamentoRegistrosPedagogicosInfantilDto>
    {
        private readonly IMediator mediator;

        public ObterRelatorioAcompanhamentoRegistrosPedagogicosInfantilQueryHandler(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator)); ;
        }

        public async Task<RelatorioAcompanhamentoRegistrosPedagogicosInfantilDto> Handle(ObterRelatorioAcompanhamentoRegistrosPedagogicosInfantilQuery request, CancellationToken cancellationToken)
        {
            Dre dre = null;
            Ue ue = null;

            if (!string.IsNullOrEmpty(request.DreCodigo))
                dre = await ObterDrePorCodigo(request.DreCodigo);

            if (!string.IsNullOrEmpty(request.UeCodigo))
                ue = await ObterUePorCodigo(request.UeCodigo);

            int[] bimestres = request.Bimestres?.ToArray();

            var turmas = await ObterTurmasPorCodigo(request.TurmasCodigo);

            var dadosTurmas = await ObterDadosPedagogicosInfantil(request.DreCodigo, request.UeCodigo, request.AnoLetivo, request.ProfessorCodigo,
                request.ProfessorNome, request.Bimestres, request.TurmasCodigo);

            return await mediator.Send(new MontarRelatorioAcompanhamentoRegistrosPedagogicosInfantilQuery(dre, ue, turmas, dadosTurmas, bimestres, request.UsuarioNome, request.UsuarioRF));
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

        private async Task<IEnumerable<Turma>> ObterTurmasPorCodigo(List<string> turmas)
        {
            return await mediator.Send(new ObterTurmasPorCodigoQuery(turmas?.ToArray()));
        }

        private async Task<List<RelatorioAcompanhamentoRegistrosPedagogicosBimestreInfantilDto>> ObterDadosPedagogicosInfantil(string dreCodigo, string ueCodigo, int anoLetivo, string professorCodigo, string professorNome, List<int> bimestres, List<string> turmasId = null)
        {
            return await mediator.Send(new ObterDadosPedagogicosTurmaQuery(dreCodigo, ueCodigo, anoLetivo, professorNome, professorCodigo, bimestres, turmasId));
        }
    }
}
