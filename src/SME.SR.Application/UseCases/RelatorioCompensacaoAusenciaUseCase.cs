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

            var ue = await mediator.Send(new ObterUePorCodigoQuery(filtros.UeCodigo));
            if (ue == null)
                throw new NegocioException("Não foi possível obter a UE.");

            var dre = await mediator.Send(new ObterDrePorCodigoQuery() { DreCodigo = filtros.DreCodigo });
            if (dre == null)
                throw new NegocioException("Não foi possível obter a DRE.");

            var turmaCodigo = "";
            if (filtros.TurmasCodigo != null && filtros.TurmasCodigo.Length > 0)
                turmaCodigo = filtros.TurmasCodigo[0];

            
            var compensacoes = await mediator.Send(new ObterCompensacoesAusenciaPorUeModalidadeSemestreComponenteBimestreQuery(ue.Id, filtros.Modalidade, filtros.Semestre, turmaCodigo, filtros.ComponentesCurriculares, filtros.Bimestre, filtros.AnoLetivo));

            if (!compensacoes.Any())
                throw new NegocioException("Não foi possível obter compensações de ausências com os filtros informados.");

            var alunosCodigos = compensacoes.Select(a => a.AlunoCodigo).Distinct().ToArray();

            var alunos = await mediator.Send(new ObterDadosAlunosPorCodigosQuery(alunosCodigos, filtros.AnoLetivo));

            var turmasCodigo = compensacoes.Select(a => a.TurmaCodigo).Distinct().ToArray();

            var bimestres = compensacoes.Select(a => a.Bimestre).Distinct().ToArray();

            var componetesCurricularesCodigo = compensacoes.Select(a => a.DisciplinaId).Distinct().ToArray();

            var componentesCurriculares = await mediator.Send(new ObterComponentesCurricularesEolPorIdsQuery() { ComponentesCurricularesIds = componetesCurricularesCodigo, TurmasId = turmasCodigo });

            if (componentesCurriculares == null || !componentesCurriculares.Any())
                throw new NegocioException("Não foi possível obter os componentes curriculares.");


            var frequencias = await mediator.Send(new ObterFrequenciaAlunoPorComponentesBimestresETurmasQuery(turmasCodigo, bimestres, componetesCurricularesCodigo));            

            var result = await mediator.Send(new RelatorioCompensacaoAusenciaObterResultadoFinalQuery(filtros, ue, dre, componentesCurriculares, compensacoes, alunos, frequencias));


            await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioCompensacaoAusencia", result, request.CodigoCorrelacao));

        }


    }
}
