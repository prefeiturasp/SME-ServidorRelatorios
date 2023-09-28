using MediatR;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace SME.SR.Application
{
    public class ObterComponentesCurricularesPorAlunosQueryHandler : IRequestHandler<ObterComponentesCurricularesPorAlunosQuery, IEnumerable<IGrouping<string, ComponenteCurricularPorTurma>>>
    {
        private readonly IComponenteCurricularRepository componenteCurricularRepository;
        private readonly IMediator mediator;
        private readonly IAlunoRepository alunoRepository;

        public ObterComponentesCurricularesPorAlunosQueryHandler(IComponenteCurricularRepository componenteCurricularRepository, IMediator mediator, IAlunoRepository alunoRepository)
        {
            this.componenteCurricularRepository = componenteCurricularRepository ?? throw new ArgumentNullException(nameof(componenteCurricularRepository));
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this.alunoRepository = alunoRepository ?? throw new ArgumentNullException(nameof(alunoRepository));
        }

        public async Task<IEnumerable<IGrouping<string, ComponenteCurricularPorTurma>>> Handle(ObterComponentesCurricularesPorAlunosQuery request, CancellationToken cancellationToken)
        {
            var todosComponentes = await componenteCurricularRepository.ListarComponentes();
            var gruposMatriz = await componenteCurricularRepository.ListarGruposMatriz();
            var alunos = await alunoRepository.ObterPorCodigosTurma(request.CodigosTurmas.Select(ct => ct.ToString()));
            var codigoAlunos = alunos.Select(x => int.Parse(x.CodigoAluno.ToString())).ToArray();
            var turmasAlunos = await mediator.Send(new ObterTurmasPorAlunosQuery(codigoAlunos.Select(ca => (long)ca).ToArray(), request.AnoLetivo));

            var turmasCodigosFiltrado = turmasAlunos
                .Where(x => request.CodigosTurmas.Contains(int.Parse(x.TurmaCodigo)) ||
                            (!string.IsNullOrEmpty(x.RegularCodigo) && request.CodigosTurmas.Contains(int.Parse(x.RegularCodigo))))
                .Select(y => int.Parse(y.TurmaCodigo))
                .Distinct()
                .ToArray();

            if (!turmasCodigosFiltrado.Any())
                turmasCodigosFiltrado = request.CodigosTurmas;

            var componentesDasTurmas = await ObterComponentesPorAlunos(turmasCodigosFiltrado, codigoAlunos.Distinct().ToArray(), request.AnoLetivo, request.Semestre, request.ConsideraHistorico);
            var componentesId = componentesDasTurmas.Select(x => x.Codigo).Distinct().ToArray();
            var disciplinasDaTurma = await mediator.Send(new ObterComponentesCurricularesPorIdsQuery(componentesId));
            var areasConhecimento = await mediator.Send(new ObterAreasConhecimentoComponenteCurricularQuery(componentesId));
            var componentesCurricularesCompletos = await ObterComponentesCurriculares(request, todosComponentes, gruposMatriz, componentesDasTurmas);
            var componentesMapeados = MapearComponentes(todosComponentes, componentesDasTurmas, areasConhecimento, componentesCurricularesCompletos, disciplinasDaTurma, request.Modalidade == Modalidade.Fundamental, turmasAlunos);

            componentesMapeados.AddRange(AdicionarComponentesRegenciaClasse(todosComponentes, gruposMatriz, componentesDasTurmas, disciplinasDaTurma, areasConhecimento));

            var componentesRegencia = ObterCodigosComponentesRegenciaClasse(todosComponentes, componentesDasTurmas);

            if (componentesRegencia != null && componentesRegencia.Any())
            {
                var componentesRegenciaPorTurma = await ObterComponentesCurricularesRegenciaClasse(request, todosComponentes, gruposMatriz, componentesDasTurmas, componentesRegencia);

                if (componentesRegenciaPorTurma != null && componentesRegenciaPorTurma.Any())
                {
                    componentesMapeados = componentesMapeados.Select(c =>
                    {
                        if (c.Regencia && componentesRegenciaPorTurma.Any(r => r.Key == c.CodigoTurma))
                            c.ComponentesCurricularesRegencia = componentesRegenciaPorTurma.FirstOrDefault(r => r.Key == c.CodigoTurma).ToList();

                        return c;

                    }).ToList();
                }
            }

            if (componentesMapeados != null && componentesMapeados.Any())
                return componentesMapeados.GroupBy(cm => cm.CodigoAluno);

            throw new NegocioException("Não foi possível localizar os componentes curriculares da turma.");
        }

        private async Task<IEnumerable<IGrouping<string, ComponenteCurricularPorTurmaRegencia>>> ObterComponentesCurricularesRegenciaClasse(ObterComponentesCurricularesPorAlunosQuery request, IEnumerable<ComponenteCurricular> componentes, IEnumerable<ComponenteCurricularGrupoMatriz> gruposMatriz, IEnumerable<ComponenteCurricular> componentesDasTurmas, IEnumerable<long> componentesRegencia)
        {
            return await mediator.Send(new ObterComponentesCurricularesRegenciaPorCodigosTurmaQuery()
            {
                CodigosTurma = componentesDasTurmas.Select(r => r.CodigoTurma).Distinct().ToArray(),
                CdComponentesCurriculares = componentesRegencia.ToArray(),
                CodigoUe = request.CodigoUe,
                Modalidade = request.Modalidade,
                ComponentesCurriculares = componentes,
                GruposMatriz = gruposMatriz,
                Usuario = request.Usuario,
                ValidarAbrangenciaProfessor = false
            });
        }

        private IEnumerable<long> ObterCodigosComponentesRegenciaClasse(IEnumerable<ComponenteCurricular> componentes, IEnumerable<ComponenteCurricular> componentesDasTurmas)
        {
            return (from cpTurma in componentesDasTurmas
                    join reg in componentes on cpTurma.Codigo equals reg.Codigo
                    where reg.ComponentePlanejamentoRegencia
                    select reg.Codigo).Distinct();
        }

        private IEnumerable<ComponenteCurricularPorTurma> AdicionarComponentesRegenciaClasse(IEnumerable<ComponenteCurricular> componentes, IEnumerable<ComponenteCurricularGrupoMatriz> gruposMatriz, IEnumerable<ComponenteCurricular> componentesDasTurmas, IEnumerable<DisciplinaDto> disciplinasDaTurma, IEnumerable<AreaDoConhecimento> areasConhecimento)
        {
            return componentesDasTurmas?.Where(w => w.EhRegencia(componentes)).Select(c => new ComponenteCurricularPorTurma
            {
                CodigoAluno = c.CodigoAluno,
                CodigoTurma = c.CodigoTurma,
                CodDisciplina = c.Codigo,
                CodDisciplinaPai = c.CodigoComponentePai(componentes),
                BaseNacional = c.EhBaseNacional(componentes),
                Compartilhada = c.EhCompartilhada(componentes),
                Disciplina = disciplinasDaTurma.FirstOrDefault(d => d.Id == c.Codigo).Nome,
                GrupoMatriz = c.ObterGrupoMatrizSgp(disciplinasDaTurma, gruposMatriz),
                AreaDoConhecimento = c.ObterAreaDoConhecimento(areasConhecimento),
                LancaNota = c.PodeLancarNota(componentes),
                Frequencia = c.ControlaFrequencia(componentes),
                Regencia = c.EhRegencia(componentes),
                TerritorioSaber = c.TerritorioSaber,
                TipoEscola = c.TipoEscola,
            });
        }

        private async Task<IEnumerable<ComponenteCurricularPorTurmaRegencia>> ObterComponentesCurriculares(ObterComponentesCurricularesPorAlunosQuery request, IEnumerable<ComponenteCurricular> componentes, IEnumerable<ComponenteCurricularGrupoMatriz> gruposMatriz, IEnumerable<ComponenteCurricular> componentesDasTurmas)
        {
            return await mediator.Send(new ObterComponentesCurricularesPorCodigosTurmaLoginEPerfilQuery()
            {
                CodigosTurma = componentesDasTurmas.Select(r => r.CodigoTurma).Distinct().ToArray(),
                ComponentesCurriculares = componentes,
                GruposMatriz = gruposMatriz,
                Usuario = request.Usuario,
                ValidarAbrangenciaProfessor = false,
                EhEJA = request.Modalidade == Modalidade.EJA,
                NecessitaRetornoRfProfessor = true
            });
        }

        private List<ComponenteCurricularPorTurma> MapearComponentes(IEnumerable<ComponenteCurricular> componentes, IEnumerable<ComponenteCurricular> componentesDasTurmas, IEnumerable<AreaDoConhecimento> areasConhecimento, IEnumerable<ComponenteCurricularPorTurmaRegencia> componentesCurricularesCompletos, IEnumerable<DisciplinaDto> disciplinasDaTurma, bool ehFundamental, IEnumerable<AlunosTurmasCodigosDto> turmasAlunos)
        {
            return (from cpTurma in componentesDasTurmas
                    join cpCompleto in componentesCurricularesCompletos on (cpTurma.CodigoTurma, cpTurma.Codigo) equals (cpCompleto.CodigoTurma, cpCompleto.TerritorioSaber ? cpCompleto.CodigoComponenteCurricularTerritorioSaber : cpCompleto.CodDisciplina)
                    join disciplina in disciplinasDaTurma on (cpCompleto.TerritorioSaber ? cpCompleto.CodigoComponenteCurricularTerritorioSaber : cpCompleto.CodDisciplina) equals (disciplina.CodigoComponenteCurricular)
                    join tAluno in turmasAlunos on (cpTurma.CodigoTurma, cpTurma.CodigoAluno) equals (tAluno.TurmaCodigo, tAluno.AlunoCodigo.ToString())
                    select new ComponenteCurricularPorTurma()
                    {
                        CodigoAluno = cpTurma.CodigoAluno,
                        CodigoTurma = cpTurma.CodigoTurma,
                        CodDisciplina = cpCompleto.CodDisciplina,
                        CodDisciplinaPai = cpTurma.CodigoComponentePai(componentes),
                        CodigoComponenteCurricularTerritorioSaber = cpCompleto.CodigoComponenteCurricularTerritorioSaber,
                        BaseNacional = cpCompleto.BaseNacional,
                        Compartilhada = cpCompleto.Compartilhada,
                        Disciplina = cpCompleto.TerritorioSaber && ehFundamental ? cpCompleto.ObterDisciplina() : disciplina.ObterDisciplina(),
                        DescricaoCompletaTerritorio = cpCompleto.TerritorioSaber && ehFundamental ? cpCompleto.Disciplina : string.Empty,
                        GrupoMatriz = cpCompleto.GrupoMatriz,
                        AreaDoConhecimento = cpTurma.ObterAreaDoConhecimento(areasConhecimento),
                        LancaNota = cpCompleto.LancaNota,
                        Frequencia = cpCompleto.Frequencia,
                        Regencia = cpTurma.EhRegencia(componentes),
                        TerritorioSaber = cpCompleto.TerritorioSaber,
                        TipoEscola = cpTurma.TipoEscola,
                        OrdemTerritorioSaber = cpCompleto.OrdemComponenteTerritorioSaber,
                        Professor = cpCompleto.Professor
                    }).ToList();
        }

        private async Task<IEnumerable<ComponenteCurricular>> ObterComponentesPorAlunos(int[] codigosTurmas, int[] alunosCodigos, int anoLetivo, int semestre, bool consideraHistorico = false)
        {
            var componentes = new List<ComponenteCurricular>();
            int alunosPorPagina = 100;

            if (alunosCodigos.Length > alunosPorPagina)
            {
                int cont = 0;
                int i = 0;
                while (cont < alunosCodigos.Length)
                {
                    var alunosPagina = alunosCodigos.Skip(alunosPorPagina * i).Take(alunosPorPagina).ToList();
                    var componentesCurriculares = await componenteCurricularRepository.ObterComponentesPorAlunos(codigosTurmas, alunosPagina.ToArray(), anoLetivo, semestre, consideraHistorico);
                    componentes.AddRange(componentesCurriculares.ToList());
                    cont += alunosPagina.Count();
                    i++;
                }
                return componentes.AsEnumerable();
            }
            else
            {
                var componentesCurriculares = await componenteCurricularRepository.ObterComponentesPorAlunos(codigosTurmas, alunosCodigos, anoLetivo, semestre, consideraHistorico);
                return componentesCurriculares.AsEnumerable();
            }
        }


    }
}
