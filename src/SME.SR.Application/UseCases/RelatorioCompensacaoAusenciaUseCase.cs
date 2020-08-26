using MediatR;
using SME.SR.Infra;
using SME.SR.Infra.Utilitarios;
using System;
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

            var compensacoes = await mediator.Send(new ObterCompensacoesAusenciaPorUeModalidadeSemestreComponenteBimestreQuery(ue.Id, filtros.Modalidade, filtros.Semestre, filtros.TurmaCodigo, filtros.ComponentesCurriculares, filtros.Bimestre));

            if (!compensacoes.Any())
                throw new NegocioException("Não foi possível obter compensações de ausências com os filtros informados.");

            var alunosCodigos = compensacoes.Select(a => a.AlunoCodigo).Distinct().ToArray();

            var alunos = await mediator.Send(new ObterDadosAlunosPorCodigosQuery(alunosCodigos));

            var turmasCodigo = compensacoes.Select(a => a.TurmaCodigo).Distinct().ToArray();

            var bimestres = compensacoes.Select(a => a.Bimestre).Distinct().ToArray();

            var componetesCurricularesCodigo = compensacoes.Select(a => a.DisciplinaId).Distinct().ToArray();

            var componentesCurriculares = await mediator.Send(new ObterComponentesCurricularesPorIdsQuery() { ComponentesCurricularesIds = componetesCurricularesCodigo });

            if (componentesCurriculares == null || !componentesCurriculares.Any())
                throw new NegocioException("Não foi possível obter os componentes curriculares.");


            var frequencias = await mediator.Send(new ObterFrequenciaAlunoPorComponentesBimestresETurmasQuery(turmasCodigo, bimestres, componetesCurricularesCodigo));

            if (frequencias == null || !frequencias.Any())
                throw new NegocioException("Não foi possível obter as frequências dos alunos.");

            var result = new RelatorioCompensacaoAusenciaDto();

            MontaCabecalho(filtros, ue, dre, compensacoes, componentesCurriculares, result);

        }

        private static void MontaCabecalho(FiltroRelatorioCompensacaoAusenciaDto filtros, Data.Ue ue, Data.Dre dre, System.Collections.Generic.IEnumerable<RelatorioCompensacaoAusenciaRetornoConsulta> compensacoes, System.Collections.Generic.IEnumerable<Data.ComponenteCurricularPorTurma> componentesCurriculares, RelatorioCompensacaoAusenciaDto result)
        {
            result.Bimestre = filtros.Bimestre.HasValue ? filtros.Bimestre.ToString() : "Todos";


            if (filtros.ComponentesCurriculares != null && filtros.ComponentesCurriculares.Any())
            {
                if (filtros.ComponentesCurriculares.Count() == 1)
                {
                    var componenteCabecalho = componentesCurriculares.FirstOrDefault(a => a.CodDisciplina == filtros.ComponentesCurriculares.FirstOrDefault())?.Disciplina;
                }
                else result.ComponenteCurricular = "";

            }
            else result.ComponenteCurricular = "Todos";

            result.Data = DateTime.Today.ToString("dd/MM/yyyy");
            result.Modalidade = filtros.Modalidade.Name();
            result.RF = filtros.UsuarioRf;

            if (string.IsNullOrEmpty(filtros.TurmaCodigo))
                result.TurmaNome = "Todas";
            else result.TurmaNome = $"{filtros.Modalidade.ShortName()} - {compensacoes.FirstOrDefault(a => a.TurmaCodigo == filtros.TurmaCodigo)?.TurmaNome}";

            result.UeNome = ue.Nome.ToString();
            result.DreNome = dre.Abreviacao;
            result.Usuario = filtros.UsuarioNome;
        }
    }
}
