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

                var turma = await ObterTurma(parametros.TurmaCodigo);
                var tipoCalendarioId = await ObterIdTipoCalendario(turma.ModalidadeTipoCalendario, turma.AnoLetivo, turma.Semestre);
                var periodosEscolares = await ObterPeriodosEscolares(tipoCalendarioId);

                var cabecalho = await ObterCabecalho(parametros.TurmaCodigo);
                var alunos = await ObterAlunos(parametros.TurmaCodigo);
                var componentesCurriculares = await ObterComponentesCurriculares(parametros.TurmaCodigo);

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
                
                var notasFinais = await ObterNotasFinaisPorTurma(parametros.TurmaCodigo);
                var frequenciaAlunos = await ObterFrequenciaComponente(parametros.TurmaCodigo, tipoCalendarioId);

                var pareceresConclusivos = await ObterPareceresConclusivos(parametros.TurmaCodigo);

                var dadosRelatorio = await MontarEstruturaRelatorio(cabecalho, alunos, componentesCurriculares, notasFinais, frequenciaAlunos, pareceresConclusivos, periodosEscolares);
                var jsonRelatorio = JsonConvert.SerializeObject(dadosRelatorio);

                await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("relatorioAtasComColunaFinal.cshtml", modelsPaginas, request.CodigoCorrelacao));

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task<ConselhoClasseAtaFinalDto> MontarEstruturaRelatorio(ConselhoClasseAtaFinalCabecalhoDto cabecalho, IEnumerable<AlunoSituacaoAtaFinalDto> alunos, IEnumerable<ComponenteCurricularPorTurma> componentesCurriculares, IEnumerable<NotaConceitoBimestreComponente> notasFinais, IEnumerable<FrequenciaAluno> frequenciaAlunos, IEnumerable<ConselhoClasseParecerConclusivo> pareceresConclusivos, IEnumerable<PeriodoEscolar> periodosEscolares)
        {
            var relatorio = new ConselhoClasseAtaFinalDto();

            relatorio.Cabecalho = cabecalho;
            var gruposMatrizes = componentesCurriculares.GroupBy(c => c.GrupoMatriz);

            MontarEstruturaGruposMatriz(ref relatorio, gruposMatrizes, periodosEscolares);
            MontarEstruturaLinhas(ref relatorio, alunos, gruposMatrizes, periodosEscolares, notasFinais, frequenciaAlunos);
            return relatorio;
        }

        private void MontarEstruturaLinhas(ref ConselhoClasseAtaFinalDto relatorio, IEnumerable<AlunoSituacaoAtaFinalDto> alunos, IEnumerable<IGrouping<ComponenteCurricularGrupoMatriz, ComponenteCurricularPorTurma>> gruposMatrizes, IEnumerable<PeriodoEscolar> periodosEscolares, IEnumerable<NotaConceitoBimestreComponente> notasFinais, IEnumerable<FrequenciaAluno> frequenciaAlunos)
        {
            foreach(var aluno in alunos)
            {
                var linhaDto = new ConselhoClasseAtaFinalLinhaDto()
                {
                    Id = aluno.CodigoAluno,
                    Nome = aluno.NomeAluno
                };

                foreach (var grupoMatriz in gruposMatrizes)
                {
                    foreach(var componente in grupoMatriz)
                    {
                        var coluna = 0;
                        // Monta Colunas notas dos bimestres
                        foreach(var periodoEscolar in periodosEscolares)
                        {
                            var notaConceito = notasFinais.FirstOrDefault(c => c.AlunoCodigo == aluno.CodigoAluno.ToString()
                                                    && c.ComponenteCurricularCodigo == componente.CodDisciplina
                                                    && c.Bimestre == periodoEscolar.Bimestre);

                            linhaDto.AdicionaCelula(grupoMatriz.Key.Id,
                                                    componente.CodDisciplina,
                                                    notaConceito?.NotaConceito ?? "",
                                                    ++coluna);
                        }

                        // Monta colunas frequencia SF - F - CA - %
                        var frequenciaAluno = frequenciaAlunos.FirstOrDefault(c => c.CodigoAluno == aluno.CodigoAluno.ToString()
                                                                        && c.DisciplinaId == componente.CodDisciplina.ToString());
                        linhaDto.AdicionaCelula(grupoMatriz.Key.Id,
                                                componente.CodDisciplina,
                                                frequenciaAluno?.TotalAulas.ToString() ?? "0",
                                                ++coluna);
                        linhaDto.AdicionaCelula(grupoMatriz.Key.Id,
                                                componente.CodDisciplina,
                                                frequenciaAluno?.TotalAusencias.ToString() ?? "0",
                                                ++coluna);
                        linhaDto.AdicionaCelula(grupoMatriz.Key.Id,
                                                componente.CodDisciplina,
                                                frequenciaAluno?.TotalCompensacoes.ToString() ?? "0",
                                                ++coluna);
                        linhaDto.AdicionaCelula(grupoMatriz.Key.Id,
                                                componente.CodDisciplina,
                                                $"{frequenciaAluno?.PercentualFrequencia ?? 100} %",
                                                ++coluna);
                    }
                }

                relatorio.Linhas.Add(linhaDto);
            }
        }

        private void MontarEstruturaGruposMatriz(ref ConselhoClasseAtaFinalDto relatorio, IEnumerable<IGrouping<ComponenteCurricularGrupoMatriz, ComponenteCurricularPorTurma>> gruposMatrizes, IEnumerable<PeriodoEscolar> periodosEscolares)
        {
            foreach(var grupoMatriz in gruposMatrizes)
            {
                var grupoMatrizDto = new ConselhoClasseAtaFinalGrupoDto()
                {
                    Id = grupoMatriz.Key.Id,
                    Nome = grupoMatriz.Key.Nome
                };

                foreach (var componenteCurricular in grupoMatriz)
                {
                    grupoMatrizDto.AdicionarComponente(componenteCurricular.CodDisciplina, componenteCurricular.Disciplina, periodosEscolares.Select(s => s.Bimestre));
                }

                relatorio.GruposMatriz.Add(grupoMatrizDto);
            }
        }

        private async Task<IEnumerable<PeriodoEscolar>> ObterPeriodosEscolares(long tipoCalendarioId)
            => await mediator.Send(new ObterPeriodosEscolaresPorTipoCalendarioQuery(tipoCalendarioId));

        private async Task<long> ObterIdTipoCalendario(ModalidadeTipoCalendario modalidade, int anoLetivo, int semestre)
            => await mediator.Send(new ObterIdTipoCalendarioPorAnoLetivoEModalidadeQuery(anoLetivo, modalidade, semestre));

        private async Task<IEnumerable<ConselhoClasseParecerConclusivo>> ObterPareceresConclusivos(string turmaCodigo)
            => await mediator.Send(new ObterParecerConclusivoPorTurmaQuery(turmaCodigo));

        private async Task<ConselhoClasseAtaFinalCabecalhoDto> ObterCabecalho(string turmaCodigo)
            => await mediator.Send(new ObterAtaFinalCabecalhoQuery(turmaCodigo));

        private async Task<Turma> ObterTurma(string turmaCodigo)
            => await mediator.Send(new ObterTurmaQuery(turmaCodigo));

        private async Task<IEnumerable<FrequenciaAluno>> ObterFrequenciaComponente(string turmaCodigo, long tipoCalendarioId)
            => await mediator.Send(new ObterFrequenciaComponenteGlobalPorTurmaQuery(turmaCodigo, tipoCalendarioId));

        private async Task<IEnumerable<NotaConceitoBimestreComponente>> ObterNotasFinaisPorTurma(string turmaCodigo)
        {
            return await mediator.Send(new ObterNotasFinaisPorTurmaQuery(turmaCodigo));
        }

        private async Task<IEnumerable<AlunoSituacaoAtaFinalDto>> ObterAlunos(string turmaCodigo)
        {
            var alunos = await mediator.Send(new ObterAlunosSituacaoPorTurmaQuery(turmaCodigo)); 
            return alunos.Select(a => new AlunoSituacaoAtaFinalDto(a));
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
        
        private async Task<IEnumerable<ComponenteCurricularPorTurma>> ObterComponentesCurriculares(string turmaCodigo)
            => await mediator.Send(new ObterComponentesCurricularesPorTurmaQuery(turmaCodigo));
    }
}
