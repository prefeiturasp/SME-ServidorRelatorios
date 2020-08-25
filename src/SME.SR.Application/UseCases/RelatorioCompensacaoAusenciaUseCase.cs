using MediatR;
using SME.SR.Infra;
using System.Linq;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class RelatorioCompensacaoAusenciaUseCase : IRelatorioCompensacaoAusenciaUseCase
    {
        private readonly IMediator mediator;

        public RelatorioCompensacaoAusenciaUseCase(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public async Task Executar(FiltroRelatorioDto request)
        {
            var filtros = request.ObterObjetoFiltro<FiltroRelatorioCompensacaoAusenciaDto>();

            var ue = mediator.Send(new ObterUePorCodigoQuery(filtros.UeCodigo));
            if (ue == null)
                throw new NegocioException("Não foi possível obter a UE.");

            var compensacoes = await mediator.Send(new ObterCompensacoesAusenciaPorUeModalidadeSemestreComponenteBimestreQuery(ue.Id, filtros.Modalidade, filtros.Semestre, filtros.TurmaCodigo, filtros.ComponentesCurriculares, filtros.Bimestre));

            if (!compensacoes.Any())
                throw new NegocioException("Não foi possível obter compensações de ausências com os filtros informados.");

            var alunosCodigos = compensacoes.Select(a => a.AlunoCodigo).Distinct();

            //var alunos = await mediator.Send(new ObterAlunos)

        }
    }
}
