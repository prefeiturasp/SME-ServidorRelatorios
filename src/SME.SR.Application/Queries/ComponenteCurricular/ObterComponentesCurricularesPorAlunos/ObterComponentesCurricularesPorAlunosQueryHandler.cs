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
            var todosComponentes = await componenteCurricularRepository.ListarInformacoesPedagogicasComponentesCurriculares();
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
            var componentesCurricularesCompletos = await ObterComponentesCurriculares(turmasCodigosFiltrado.Select(cod => cod.ToString()).ToArray(), todosComponentes, request.Modalidade == Modalidade.EJA);

            var componentesId = componentesCurricularesCompletos.Select(x => x.Codigo).Distinct();
            componentesId = componentesId.Concat(componentesCurricularesCompletos.Where(x => x.CodigoComponenteCurricularTerritorioSaber != 0).Select(x => x.CodigoComponenteCurricularTerritorioSaber).Distinct());
            var areasConhecimento = await mediator.Send(new ObterAreasConhecimentoComponenteCurricularQuery(componentesId.Distinct().ToArray()));

            var componentesMapeados = MapearComponentes(todosComponentes, gruposMatriz, areasConhecimento, componentesCurricularesCompletos, turmasAlunos);

            componentesMapeados.AddRange(AdicionarComponentesRegenciaClasse(todosComponentes, gruposMatriz, areasConhecimento, componentesDasTurmas));

            var componentesRegencia = ObterCodigosComponentesRegenciaClasse(todosComponentes, componentesDasTurmas);
            if (componentesRegencia != null && componentesRegencia.Any())
            {
                var componentesRegenciaPorTurma = await ObterComponentesCurricularesRegenciaClasse(turmasCodigosFiltrado.Select(cod => cod.ToString()).ToArray(), request, todosComponentes, gruposMatriz, 
                                                                                                   componentesRegencia);

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

        private async Task<IEnumerable<IGrouping<string, ComponenteCurricularPorTurmaRegencia>>> ObterComponentesCurricularesRegenciaClasse(string[] codigosTurmas, ObterComponentesCurricularesPorAlunosQuery request, IEnumerable<InformacaoPedagogicaComponenteCurricularSGPDTO> informacoesComponentesCurriculares, 
                                                                                                                                                IEnumerable<ComponenteCurricularGrupoMatriz> gruposMatriz, IEnumerable<long> componentesRegencia)
        {
            return await mediator.Send(new ObterComponentesCurricularesRegenciaPorCodigosTurmaQuery()
            {
                CodigosTurma = codigosTurmas,
                CdComponentesCurriculares = componentesRegencia.ToArray(),
                CodigoUe = request.CodigoUe,
                Modalidade = request.Modalidade,
                ComponentesCurriculares = informacoesComponentesCurriculares,
                GruposMatriz = gruposMatriz,
                Usuario = request.Usuario
            });
        }

        private IEnumerable<ComponenteCurricularPorTurma> AdicionarComponentesRegenciaClasse(IEnumerable<InformacaoPedagogicaComponenteCurricularSGPDTO> informacoesComponentesCurriculares, 
                                                                                             IEnumerable<ComponenteCurricularGrupoMatriz> gruposMatriz, IEnumerable<AreaDoConhecimento> areasConhecimento,
                                                                                             IEnumerable<ComponenteCurricular> componentesDasTurmas)
        {
            return componentesDasTurmas?.Where(w => w.EhRegencia(informacoesComponentesCurriculares)).Select(c => new ComponenteCurricularPorTurma
            {
                CodigoAluno = c.CodigoAluno,
                CodigoTurma = c.CodigoTurma,
                CodDisciplina = c.Codigo,
                CodDisciplinaPai = c.CodigoComponentePai(informacoesComponentesCurriculares),
                BaseNacional = c.EhBaseNacional(informacoesComponentesCurriculares),
                Compartilhada = c.EhCompartilhada(informacoesComponentesCurriculares),
                Disciplina = informacoesComponentesCurriculares.FirstOrDefault(d => d.Codigo == c.Codigo).Descricao,
                GrupoMatriz = c.ObterGrupoMatriz(gruposMatriz),
                AreaDoConhecimento = c.ObterAreaDoConhecimento(areasConhecimento),
                LancaNota = c.PodeLancarNota(informacoesComponentesCurriculares),
                Frequencia = c.ControlaFrequencia(informacoesComponentesCurriculares),
                Regencia = c.EhRegencia(informacoesComponentesCurriculares),
                TerritorioSaber = c.TerritorioSaber,
                TipoEscola = c.TipoEscola,
            });
        }

        private IEnumerable<long> ObterCodigosComponentesRegenciaClasse(IEnumerable<InformacaoPedagogicaComponenteCurricularSGPDTO> todosComponentesCurriculares, IEnumerable<ComponenteCurricular> componentesDasTurmas)
        {
            return (from cpTurma in componentesDasTurmas
                    join reg in todosComponentesCurriculares on cpTurma.Codigo equals reg.Codigo
                    where reg.EhRegencia
                    select reg.Codigo).Distinct();
        }

        private async Task<IEnumerable<ComponenteCurricular>> ObterComponentesCurriculares(string[] codigosTurmas, IEnumerable<InformacaoPedagogicaComponenteCurricularSGPDTO> componentes, bool ehEJA = false)
        {
            return await mediator.Send(new ObterComponentesCurricularesPorCodigosTurmaQuery()
            {
                CodigosTurma = codigosTurmas,
                ComponentesCurriculares = componentes,
                EhEJA = ehEJA
            });
        }

        private List<ComponenteCurricularPorTurma> MapearComponentes(IEnumerable<InformacaoPedagogicaComponenteCurricularSGPDTO> todosComponentesCurriculares,
                                                                     IEnumerable<ComponenteCurricularGrupoMatriz> gruposMatriz, IEnumerable<AreaDoConhecimento> areasConhecimento, 
                                                                     IEnumerable<ComponenteCurricular> componentesCurricularesCompletos, 
                                                                     IEnumerable<AlunosTurmasCodigosDto> turmasAlunos)
        {
            return (from tAluno in turmasAlunos
                    join cpCompleto in componentesCurricularesCompletos on tAluno.TurmaCodigo equals cpCompleto.CodigoTurma
                    select new ComponenteCurricularPorTurma()
                    {
                        CodigoAluno = tAluno.AlunoCodigo.ToString(),
                        CodigoTurma = tAluno.TurmaCodigo,
                        CodDisciplina = cpCompleto.Codigo,
                        CodDisciplinaPai = cpCompleto.CodigoComponentePai(todosComponentesCurriculares),
                        CodigoComponenteCurricularTerritorioSaber = cpCompleto.CodigoComponenteCurricularTerritorioSaber,
                        BaseNacional = cpCompleto.BaseNacional,
                        Compartilhada = cpCompleto.Compartilhada,
                        Disciplina = cpCompleto.Descricao,
                        DescricaoCompletaTerritorio = cpCompleto.TerritorioSaber ? cpCompleto.Descricao : string.Empty,
                        GrupoMatriz = cpCompleto.ObterGrupoMatriz(gruposMatriz),
                        AreaDoConhecimento = cpCompleto.ObterAreaDoConhecimento(areasConhecimento),
                        LancaNota = cpCompleto.LancaNota,
                        Frequencia = cpCompleto.Frequencia,
                        Regencia = cpCompleto.EhRegencia(todosComponentesCurriculares),
                        TerritorioSaber = cpCompleto.TerritorioSaber,
                        TipoEscola = cpCompleto.TipoEscola,
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
