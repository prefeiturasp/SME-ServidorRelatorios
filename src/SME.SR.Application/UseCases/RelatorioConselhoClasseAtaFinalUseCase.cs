using MediatR;
using Newtonsoft.Json;
using SME.SR.Application.Commands.ComunsRelatorio.GerarRelatorioHtmlParaPdf;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class RelatorioConselhoClasseAtaFinalUseCase : IRelatorioConselhoClasseAtaFinalUseCase
    {
        private readonly IMediator mediator;

        public RelatorioConselhoClasseAtaFinalUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Executar(FiltroRelatorioDto request)
        {
            try
            {
                var parametros = request.ObterObjetoFiltro<FiltroConselhoClasseAtaFinalDto>();

                var turma = await ObterTurma(parametros.TurmaCodigo);
                var tipoCalendarioId = await ObterIdTipoCalendario(turma.ModalidadeTipoCalendario, turma.AnoLetivo, turma.Semestre);

                var cabecalho = await ObterCabecalho(parametros.TurmaCodigo);
                var alunos = await ObterAlunos(parametros.TurmaCodigo);
                var componentesCurriculares = await ObterComponentesCurriculares(parametros.TurmaCodigo);
                var notasFinais = await ObterNotasFinaisPorTurma(parametros.TurmaCodigo);
                var frequenciaAlunos = await ObterFrequenciaComponente(parametros.TurmaCodigo, tipoCalendarioId);
                var pareceresConclusivos = await ObterPareceresConclusivos(parametros.TurmaCodigo);

                var dadosRelatorio = MontarEstruturaRelatorio(cabecalho, alunos, componentesCurriculares, notasFinais, frequenciaAlunos, pareceresConclusivos);
                var resultadoPaginado = MontarEstruturaPaginada(dadosRelatorio);

                var jsonDados = JsonConvert.SerializeObject(resultadoPaginado);

                await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("relatorioAtasComColunaFinal.cshtml", resultadoPaginado, request.CodigoCorrelacao));

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private List<ConselhoClasseAtaFinalPaginaDto> MontarEstruturaPaginada(ConselhoClasseAtaFinalDto dadosRelatorio)
        {
            var maximoComponentesPorPagina = 7;
            var maximoComponentesPorPaginaFinal = 3;
            var quantidadeDeLinhasPorPagina = 20;

            List<ConselhoClasseAtaFinalPaginaDto> modelsPaginas = new List<ConselhoClasseAtaFinalPaginaDto>();

            List<ConselhoClasseAtaFinalComponenteDto> todasAsDisciplinas = dadosRelatorio.GruposMatriz.SelectMany(x => x.ComponentesCurriculares).ToList();

            int quantidadePaginasHorizontal = CalcularPaginasHorizontal(maximoComponentesPorPagina, maximoComponentesPorPaginaFinal, todasAsDisciplinas.Count());

            int quantidadePaginasVertical = (int)Math.Ceiling(dadosRelatorio.Linhas.Count / (decimal)quantidadeDeLinhasPorPagina);

            int contPagina = 1;

            for (int v = 0; v < quantidadePaginasVertical; v++)
            {
                for (int h = 0; h < quantidadePaginasHorizontal; h++)
                {
                    bool ehPaginaFinal = (h + 1) == quantidadePaginasHorizontal;

                    int quantidadeDisciplinasDestaPagina = ehPaginaFinal ? maximoComponentesPorPaginaFinal : maximoComponentesPorPagina;

                    ConselhoClasseAtaFinalPaginaDto modelPagina = new ConselhoClasseAtaFinalPaginaDto
                    {
                        Cabecalho = dadosRelatorio.Cabecalho,
                        NumeroPagina = contPagina++,
                        FinalHorizontal = ehPaginaFinal,
                        TotalPaginas = quantidadePaginasHorizontal * quantidadePaginasVertical
                    };

                    if (todasAsDisciplinas.Any())
                    {
                        IEnumerable<ConselhoClasseAtaFinalComponenteDto> disciplinasDestaPagina = todasAsDisciplinas.Skip(h * maximoComponentesPorPagina).Take(quantidadeDisciplinasDestaPagina);

                        foreach (ConselhoClasseAtaFinalComponenteDto disciplina in disciplinasDestaPagina)
                        {
                            var grupoMatrizAtual = VerificarGrupoMatrizNaPagina(dadosRelatorio, modelPagina, disciplina);
                            if (grupoMatrizAtual != null)
                            {
                                grupoMatrizAtual.ComponentesCurriculares.Add(disciplina);
                            }
                        }
                    }

                    var gruposMatrizDestaPagina = modelPagina.GruposMatriz.Select(x => x.Id);
                    var idsDisciplinasDestaPagina = modelPagina.GruposMatriz.SelectMany(x => x.ComponentesCurriculares).Select(x => x.Id);

                    int quantidadeHorizontal = idsDisciplinasDestaPagina.Count();

                    List<ConselhoClasseAtaFinalLinhaDto> linhas = dadosRelatorio.Linhas.Skip((v) * quantidadeDeLinhasPorPagina).Take(quantidadeDeLinhasPorPagina).Select(x => new ConselhoClasseAtaFinalLinhaDto { Id = x.Id, Nome = x.Nome, Celulas = x.Celulas }).ToList();

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

                    foreach (var grupoMatriz in modelPagina.GruposMatriz)
                    {
                        grupoMatriz.QuantidadeColunas = modelPagina.Linhas.First().Celulas.Where(x => x.GrupoMatriz == grupoMatriz.Id).Count();
                    }

                    modelsPaginas.Add(modelPagina);
                }
            }

            return modelsPaginas;
        }

        private ConselhoClasseAtaFinalDto MontarEstruturaRelatorio(ConselhoClasseAtaFinalCabecalhoDto cabecalho, IEnumerable<AlunoSituacaoAtaFinalDto> alunos, IEnumerable<ComponenteCurricularPorTurma> componentesCurriculares, IEnumerable<NotaConceitoBimestreComponente> notasFinais, IEnumerable<FrequenciaAluno> frequenciaAlunos, IEnumerable<ConselhoClasseParecerConclusivo> pareceresConclusivos)
        {
            var relatorio = new ConselhoClasseAtaFinalDto();

            relatorio.Cabecalho = cabecalho;
            var gruposMatrizes = componentesCurriculares.GroupBy(c => c.GrupoMatriz);

            MontarEstruturaGruposMatriz(ref relatorio, gruposMatrizes);
            MontarEstruturaLinhas(ref relatorio, alunos, gruposMatrizes, notasFinais, frequenciaAlunos, pareceresConclusivos);
            return relatorio;
        }

        private void MontarEstruturaLinhas(ref ConselhoClasseAtaFinalDto relatorio, IEnumerable<AlunoSituacaoAtaFinalDto> alunos, IEnumerable<IGrouping<ComponenteCurricularGrupoMatriz, ComponenteCurricularPorTurma>> gruposMatrizes, IEnumerable<NotaConceitoBimestreComponente> notasFinais, IEnumerable<FrequenciaAluno> frequenciaAlunos, IEnumerable<ConselhoClasseParecerConclusivo> pareceresConclusivos)
        {
            foreach (var aluno in alunos)
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
                        for(var bimestre = 1; bimestre <= 4; bimestre++)
                        {
                            var notaConceito = notasFinais.FirstOrDefault(c => c.AlunoCodigo == aluno.CodigoAluno.ToString()
                                                    && c.ComponenteCurricularCodigo == componente.CodDisciplina
                                                    && c.Bimestre == bimestre);

                            linhaDto.AdicionaCelula(grupoMatriz.Key.Id,
                                                    componente.CodDisciplina,
                                                    notaConceito?.NotaConceito ?? "",
                                                    ++coluna);
                        }
                        // Monta coluna Sintese Final - SF
                        var notaConceitofinal = notasFinais.FirstOrDefault(c => c.AlunoCodigo == aluno.CodigoAluno.ToString()
                                                && c.ComponenteCurricularCodigo == componente.CodDisciplina
                                                && !c.Bimestre.HasValue);

                        linhaDto.AdicionaCelula(grupoMatriz.Key.Id,
                                                componente.CodDisciplina,
                                                notaConceitofinal?.NotaConceito ?? "",
                                                ++coluna);


                        // Monta colunas frequencia F - CA - %
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
                                                frequenciaAluno?.PercentualFrequencia.ToString() ?? "100,00",
                                                ++coluna);
                    }
                }

                var frequenciaGlobalAluno = frequenciaAlunos
                                                .GroupBy(c => c.CodigoAluno)
                                                .Where(c => c.Key == aluno.CodigoAluno.ToString())
                                                .Select(f => new FrequenciaAluno()
                                                {
                                                    TotalAusencias = f.Sum(s => s.TotalAusencias),
                                                    TotalCompensacoes = f.Sum(s => s.TotalCompensacoes)
                                                }).FirstOrDefault();

                // Anual
                linhaDto.AdicionaCelula(0, 0, frequenciaGlobalAluno?.TotalAusencias.ToString() ?? "0", 1);
                linhaDto.AdicionaCelula(0, 0, frequenciaGlobalAluno?.TotalCompensacoes.ToString() ?? "0", 2);
                linhaDto.AdicionaCelula(0, 0, frequenciaGlobalAluno?.PercentualFrequencia.ToString() ?? "0", 3);

                var parecerConclusivo = pareceresConclusivos.FirstOrDefault(c => c.AlunoCodigo == aluno.CodigoAluno.ToString());
                linhaDto.AdicionaCelula(0, 0, parecerConclusivo?.ParecerConclusivo ?? "", 4);

                relatorio.Linhas.Add(linhaDto);
            }
        }

        private void MontarEstruturaGruposMatriz(ref ConselhoClasseAtaFinalDto relatorio, IEnumerable<IGrouping<ComponenteCurricularGrupoMatriz, ComponenteCurricularPorTurma>> gruposMatrizes)
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
                    grupoMatrizDto.AdicionarComponente(componenteCurricular.CodDisciplina, componenteCurricular.Disciplina, grupoMatrizDto.Id);
                }

                relatorio.GruposMatriz.Add(grupoMatrizDto);
            }
        }

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

        private ConselhoClasseAtaFinalGrupoDto VerificarGrupoMatrizNaPagina(ConselhoClasseAtaFinalDto modelCompleto, ConselhoClasseAtaFinalPaginaDto modelPagina, ConselhoClasseAtaFinalComponenteDto disciplina)
        {
            if (!modelPagina.GruposMatriz.Any(x => x.Id == disciplina.IdGrupoMatriz))
            {
                var grupoMatriz = modelCompleto.GruposMatriz.FirstOrDefault(x => x.Id == disciplina.IdGrupoMatriz);

                var novoGrupoMatriz = new ConselhoClasseAtaFinalGrupoDto
                {
                    ComponentesCurriculares = new List<ConselhoClasseAtaFinalComponenteDto>(),
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
