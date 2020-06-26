using MediatR;
using Newtonsoft.Json;
using SME.SR.Application.Commands.ComunsRelatorio.GerarRelatorioHtmlParaPdf;
using SME.SR.Data;
using SME.SR.Infra;
using SME.SR.Infra.Dtos.Relatorios.ConselhoClasse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static SME.SR.Infra.Enumeradores;

namespace SME.SR.Application
{
    public class RelatorioConselhoClasseAtaFinalUseCase : IRelatorioConselhoClasseAtaFinalUseCase
    {
        private readonly IMediator mediator;

        public RelatorioConselhoClasseAtaFinalUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Executar(FiltroRelatorioDto request, ConselhoClasseAtaFinalDto modelCompleto) // TODO modelCompleto deve ser removido antes da entrega.
        {
            try
            {
                var parametros = request.ObterObjetoFiltro<FiltroConselhoClasseAtaFinalDto>();

                var cabecalho = await mediator.Send(new ObterAtaFinalCabecalhoQuery(parametros.TurmaCodigo));
                var alunos = await ObterAlunos(parametros.TurmaCodigo);
                var componentesCurriculares = await ObterComponentesCurriculares(parametros.TurmaCodigo);
                var notasFinais = await ObterNotasFinaisPorTurmaQuery(parametros.TurmaCodigo);
                // TODO montar relatorio

                var maximoComponentesPorPagina = 7;
                var maximoComponentesPorPaginaFinal = 3;
                var quantidadeDeLinhasPorPagina = 20;

                List<ConselhoClasseAtaFinalPaginaDto> modelsPaginas = new List<ConselhoClasseAtaFinalPaginaDto>();

                List<ConselhoClasseAtaFinalComponenteCurricularDto> todasAsDisciplinas = modelCompleto.GruposMatriz.SelectMany(x => x.ComponentesCurriculares).ToList();

                int quantidadePaginasHorizontal = CalcularPaginasHorizontal(maximoComponentesPorPagina, maximoComponentesPorPaginaFinal, todasAsDisciplinas.Count());

                int quantidadePaginasVertical = (int)Math.Ceiling(modelCompleto.Linhas.Count / (decimal)quantidadeDeLinhasPorPagina);

                int contPagina = 1;

                for (int v = 0; v < quantidadePaginasVertical; v++)
                {
                    for (int h = 0; h < quantidadePaginasHorizontal; h++)
                    {
                        bool ehPaginaFinal = (h + 1) == quantidadePaginasHorizontal;

                        int quantidadeDisciplinasDestaPagina = ehPaginaFinal ? maximoComponentesPorPaginaFinal : maximoComponentesPorPagina;

                        ConselhoClasseAtaFinalPaginaDto modelPagina = new ConselhoClasseAtaFinalPaginaDto
                        {
                            Cabecalho = modelCompleto.Cabecalho,
                            NumeroPagina = contPagina++,
                            FinalHorizontal = ehPaginaFinal,
                            TotalPaginas = quantidadePaginasHorizontal * quantidadePaginasVertical,
                            GruposMatriz = new List<ConselhoClasseAtaFinalGrupoMatrizDto>(),
                            Linhas = new List<ConselhoClasseAtaFinalLinhaDto>()
                        };

                        if (todasAsDisciplinas.Any())
                        {
                            IEnumerable<ConselhoClasseAtaFinalComponenteCurricularDto> disciplinasDestaPagina = todasAsDisciplinas.Skip(h * maximoComponentesPorPagina).Take(quantidadeDisciplinasDestaPagina);

                            foreach (ConselhoClasseAtaFinalComponenteCurricularDto disciplina in disciplinasDestaPagina)
                            {
                                ConselhoClasseAtaFinalGrupoMatrizDto grupoMatrizAtual = VerificarGrupoMatrizNaPagina(modelCompleto, modelPagina, disciplina);
                                if (grupoMatrizAtual != null)
                                {
                                    grupoMatrizAtual.ComponentesCurriculares.Add(disciplina);
                                }
                            }
                        }

                        IEnumerable<int> gruposMatrizDestaPagina = modelPagina.GruposMatriz.Select(x => x.Id);
                        IEnumerable<int> idsDisciplinasDestaPagina = modelPagina.GruposMatriz.SelectMany(x => x.ComponentesCurriculares).Select(x => x.Id);

                        int quantidadeHorizontal = idsDisciplinasDestaPagina.Count();

                        List<ConselhoClasseAtaFinalLinhaDto> linhas = modelCompleto.Linhas.Skip((v) * quantidadeDeLinhasPorPagina).Take(quantidadeDeLinhasPorPagina).Select(x => new ConselhoClasseAtaFinalLinhaDto { Id = x.Id, Nome = x.Nome, Celulas = x.Celulas }).ToList();

                        foreach (ConselhoClasseAtaFinalLinhaDto linha in linhas)
                        {
                            List<ConselhoClasseAtaFinalCelulaDto> todasAsCelulas = linha.Celulas;

                            linha.Celulas = todasAsCelulas.Where(x => gruposMatrizDestaPagina.Contains(x.GrupoMatriz) && idsDisciplinasDestaPagina.Contains(x.ComponenteCurricular)).Select(x => new ConselhoClasseAtaFinalCelulaDto { GrupoMatriz = x.GrupoMatriz, ComponenteCurricular = x.ComponenteCurricular, Coluna = x.Coluna, Valor = x.Valor }).ToList();

                            if (ehPaginaFinal)
                            {
                                IEnumerable<ConselhoClasseAtaFinalCelulaDto> celulasFinais = todasAsCelulas.Where(x => x.GrupoMatriz == 99);

                                linha.Celulas.AddRange(celulasFinais);
                            }
                        }

                        modelPagina.Linhas.AddRange(linhas);

                        foreach (ConselhoClasseAtaFinalGrupoMatrizDto grupoMatriz in modelPagina.GruposMatriz)
                        {
                            grupoMatriz.QuantidadeColunas = modelPagina.Linhas.First().Celulas.Where(x => x.GrupoMatriz == grupoMatriz.Id).Count();
                        }

                        modelsPaginas.Add(modelPagina);
                    }
                }


                await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("relatorioAtasComColunaFinal.cshtml", modelsPaginas, request.CodigoCorrelacao));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task<IEnumerable<NotaConceitoBimestreComponente>> ObterNotasFinaisPorTurmaQuery(string turmaCodigo)
        {
            return await mediator.Send(new ObterNotasFinaisPorTurmaQuery(turmaCodigo));
        }

        private async Task<IEnumerable<AlunoSituacaoAtaFinalDto>> ObterAlunos(string turmaCodigo)
        {
            var alunos = await mediator.Send(new ObterAlunosSituacaoPorTurmaQuery(turmaCodigo));
            return alunos.Select(a => new AlunoSituacaoAtaFinalDto(a));
        }

        private async Task<IEnumerable<ConselhoClasseAtaFinalComponenteDto>> ObterComponentesCurriculares(string turmaCodigo)
        {
            var componentesCurriculares = await mediator.Send(new ObterComponentesCurricularesPorTurmaQuery(turmaCodigo));

            return MapearComponentes(componentesCurriculares);
        }

        private IEnumerable<ConselhoClasseAtaFinalComponenteDto> MapearComponentes(IEnumerable<ComponenteCurricularPorTurma> componentes)
        {
            foreach (var componente in componentes)
                yield return MapearComponente(componente);
        }

        private ConselhoClasseAtaFinalComponenteDto MapearComponente(ComponenteCurricularPorTurma componente)
            => componente == null ? null :
            new ConselhoClasseAtaFinalComponenteDto()
            {
                Codigo = componente.CodDisciplina,
                Nome = componente.Disciplina,
                GrupoMatriz = componente.GrupoMatriz?.Nome
            };



        private int CalcularPaginasHorizontal(int maximoComponentesPorPagina, int maximoComponentesPorPaginaFinal, int contagemTodasDisciplinas)
        {
            int contagemDisciplinas = contagemTodasDisciplinas;
            int quantidadePaginas = (int)(Math.Ceiling(contagemDisciplinas / (decimal)maximoComponentesPorPagina));

            while (contagemDisciplinas > maximoComponentesPorPagina)
            {
                contagemDisciplinas -= maximoComponentesPorPagina;
            }

            if (contagemDisciplinas > maximoComponentesPorPaginaFinal)
            {
                quantidadePaginas++;
            }

            return quantidadePaginas;
        }

        private ConselhoClasseAtaFinalGrupoMatrizDto VerificarGrupoMatrizNaPagina(ConselhoClasseAtaFinalDto modelCompleto, ConselhoClasseAtaFinalPaginaDto modelPagina, ConselhoClasseAtaFinalComponenteCurricularDto disciplina)
        {
            if (!modelPagina.GruposMatriz.Any(x => x.Id == disciplina.IdGrupoMatriz))
            {
                ConselhoClasseAtaFinalGrupoMatrizDto grupoMatriz = modelCompleto.GruposMatriz.FirstOrDefault(x => x.Id == disciplina.IdGrupoMatriz);

                ConselhoClasseAtaFinalGrupoMatrizDto novoGrupoMatriz = new ConselhoClasseAtaFinalGrupoMatrizDto
                {
                    ComponentesCurriculares = new List<ConselhoClasseAtaFinalComponenteCurricularDto>(),
                    Id = grupoMatriz.Id,
                    Nome = grupoMatriz.Nome
                };

                modelPagina.GruposMatriz.Add(novoGrupoMatriz);
            }

            return modelPagina.GruposMatriz.FirstOrDefault(x => x.Id == disciplina.IdGrupoMatriz);
        }
    }
}
