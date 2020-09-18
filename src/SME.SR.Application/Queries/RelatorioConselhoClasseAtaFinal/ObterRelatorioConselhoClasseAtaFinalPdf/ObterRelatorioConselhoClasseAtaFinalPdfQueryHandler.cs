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
    public class ObterRelatorioConselhoClasseAtaFinalPdfQueryHandler : IRequestHandler<ObterRelatorioConselhoClasseAtaFinalPdfQuery, List<ConselhoClasseAtaFinalPaginaDto>>
    {
        private IMediator mediator;
        private ComponenteCurricularPorTurma componenteRegencia;

        public ObterRelatorioConselhoClasseAtaFinalPdfQueryHandler(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<List<ConselhoClasseAtaFinalPaginaDto>> Handle(ObterRelatorioConselhoClasseAtaFinalPdfQuery request, CancellationToken cancellationToken)
        {
            var mensagensErro = new StringBuilder();
            var relatoriosTurmas = new List<ConselhoClasseAtaFinalPaginaDto>();
            foreach (var turmaCodigo in request.FiltroConselhoClasseAtaFinal.TurmasCodigos)
            {
                try
                {
                    relatoriosTurmas.AddRange(await ObterRelatorioTurma(turmaCodigo, request.UsuarioLogadoRF, request.PerfilUsuario));
                }
                catch (Exception e)
                {
                    mensagensErro.AppendLine($"Erro na carga de cados da turma [{turmaCodigo}]: {e.Message}");
                }
            }

            if (mensagensErro.Length > 0)
                throw new NegocioException(mensagensErro.ToString());

            return relatoriosTurmas;
        }

        private async Task<IEnumerable<ConselhoClasseAtaFinalPaginaDto>> ObterRelatorioTurma(string turmaCodigo, string usuarioLogadoRF, string perfilUsuario)
        {
            var notasFinais = await ObterNotasFinaisPorTurma(turmaCodigo);
            if (notasFinais == null || !notasFinais.Any())
                throw new NegocioException($"Turma não possui conselho de classe");

            var turma = await ObterTurma(turmaCodigo);
            var tipoCalendarioId = await ObterIdTipoCalendario(turma.ModalidadeTipoCalendario, turma.AnoLetivo, turma.Semestre);

            var cabecalho = await ObterCabecalho(turmaCodigo);
            var alunos = await ObterAlunos(turmaCodigo);
            var componentesCurriculares = await ObterComponentesCurriculares(turmaCodigo, usuarioLogadoRF, perfilUsuario);
            var periodosEscolares = await ObterPeriodosEscolares(tipoCalendarioId);
            var frequenciaAlunos = await ObterFrequenciaComponente(turmaCodigo, tipoCalendarioId, periodosEscolares);
            var pareceresConclusivos = await ObterPareceresConclusivos(turmaCodigo);

            var dadosRelatorio = MontarEstruturaRelatorio(turma.ModalidadeCodigo, cabecalho, alunos, componentesCurriculares, notasFinais, frequenciaAlunos, pareceresConclusivos, periodosEscolares, turma.Codigo);
            return MontarEstruturaPaginada(dadosRelatorio);
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
                        EhEJA = dadosRelatorio.EhEJA,
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

                    List<ConselhoClasseAtaFinalLinhaDto> linhas = dadosRelatorio.Linhas.Skip((v) * quantidadeDeLinhasPorPagina).Take(quantidadeDeLinhasPorPagina)
                        .Select(x => new ConselhoClasseAtaFinalLinhaDto
                        {
                            Id = x.Id,
                            Nome = x.Nome,
                            Inativo = x.Inativo,
                            Situacao = x.Situacao,
                            Celulas = x.Celulas
                        }).ToList();

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

        private ConselhoClasseAtaFinalDto MontarEstruturaRelatorio(Modalidade modalidadeCodigo, ConselhoClasseAtaFinalCabecalhoDto cabecalho, IEnumerable<AlunoSituacaoAtaFinalDto> alunos, IEnumerable<ComponenteCurricularPorTurma> componentesCurriculares, IEnumerable<NotaConceitoBimestreComponente> notasFinais, IEnumerable<FrequenciaAluno> frequenciaAlunos, IEnumerable<ConselhoClasseParecerConclusivo> pareceresConclusivos, IEnumerable<PeriodoEscolar> periodosEscolares, string turmaCodigo)
        {
            var relatorio = new ConselhoClasseAtaFinalDto();
            relatorio.EhEJA = modalidadeCodigo == Modalidade.EJA;

            relatorio.Cabecalho = cabecalho;
            var gruposMatrizes = componentesCurriculares.Where(c => c.GrupoMatriz != null).GroupBy(c => c.GrupoMatriz);

            MontarEstruturaGruposMatriz(ref relatorio, gruposMatrizes, periodosEscolares);
            MontarEstruturaLinhas(ref relatorio, alunos, gruposMatrizes, notasFinais, frequenciaAlunos, pareceresConclusivos, periodosEscolares, turmaCodigo);
            return relatorio;
        }

        private void MontarEstruturaLinhas(ref ConselhoClasseAtaFinalDto relatorio, IEnumerable<AlunoSituacaoAtaFinalDto> alunos, IEnumerable<IGrouping<ComponenteCurricularGrupoMatriz, ComponenteCurricularPorTurma>> gruposMatrizes, IEnumerable<NotaConceitoBimestreComponente> notasFinais, IEnumerable<FrequenciaAluno> frequenciaAlunos, IEnumerable<ConselhoClasseParecerConclusivo> pareceresConclusivos, IEnumerable<PeriodoEscolar> periodosEscolares, string turmaCodigo)
        {
            // Primmeiro alunos com numero de chamada
            foreach (var aluno in alunos.Where(a => int.Parse(a.NumeroAlunoChamada ?? "0") > 0).Select(a => new AlunoSituacaoAtaFinalDto(a)).OrderBy(a => a.NumeroAlunoChamada))
            {
                relatorio.Linhas.Add(MontarLinhaAluno(aluno, gruposMatrizes, notasFinais, frequenciaAlunos, pareceresConclusivos, periodosEscolares, turmaCodigo));
            }

            // Depois alunos sem numero ordenados por nome
            foreach (var aluno in alunos.Where(a => int.Parse(a.NumeroAlunoChamada ?? "0") == 0).Select(a => new AlunoSituacaoAtaFinalDto(a)).OrderBy(a => a.NomeAluno))
            {
                relatorio.Linhas.Add(MontarLinhaAluno(aluno, gruposMatrizes, notasFinais, frequenciaAlunos, pareceresConclusivos, periodosEscolares, turmaCodigo));
            }
        }

        private ConselhoClasseAtaFinalLinhaDto MontarLinhaAluno(AlunoSituacaoAtaFinalDto aluno, IEnumerable<IGrouping<ComponenteCurricularGrupoMatriz, ComponenteCurricularPorTurma>> gruposMatrizes, IEnumerable<NotaConceitoBimestreComponente> notasFinais, IEnumerable<FrequenciaAluno> frequenciaAlunos, IEnumerable<ConselhoClasseParecerConclusivo> pareceresConclusivos, IEnumerable<PeriodoEscolar> periodosEscolares, string turmaCodigo)
        {
            var linhaDto = new ConselhoClasseAtaFinalLinhaDto()
            {
                Id = long.Parse(aluno.NumeroAlunoChamada ?? "0"),
                Nome = aluno.NomeAluno,
                Situacao = aluno.SituacaoMatricula,
                Inativo = aluno.Inativo
            };

            foreach (var grupoMatriz in gruposMatrizes)
            {
                foreach (var componente in grupoMatriz)
                {
                    var coluna = 0;
                    // Monta Colunas notas dos bimestres
                    foreach (var bimestre in periodosEscolares.OrderBy(p => p.Bimestre).Select(a => a.Bimestre))
                    {
                        var notaConceito = notasFinais.FirstOrDefault(c => c.AlunoCodigo == aluno.CodigoAluno.ToString()
                                                && c.ComponenteCurricularCodigo == componente.CodDisciplina
                                                && c.Bimestre == bimestre);

                        linhaDto.AdicionaCelula(grupoMatriz.Key.Id,
                                                componente.CodDisciplina,
                                                componente.LancaNota ?
                                                    notaConceito?.NotaConceito ?? "" :
                                                    notaConceito?.Sintese,
                                                ++coluna);
                    }
                    // Monta coluna Sintese Final - SF
                    var notaConceitofinal = notasFinais.FirstOrDefault(c => c.AlunoCodigo == aluno.CodigoAluno.ToString()
                                            && c.ComponenteCurricularCodigo == componente.CodDisciplina
                                            && !c.Bimestre.HasValue);

                    linhaDto.AdicionaCelula(grupoMatriz.Key.Id,
                                            componente.CodDisciplina,
                                            componente.LancaNota ?
                                                notaConceitofinal?.NotaConceito ?? "" :
                                                notaConceitofinal?.Sintese,
                                            ++coluna);

                    // Monta colunas frequencia F - CA - %
                    var frequenciaAluno = ObterFrequenciaAluno(frequenciaAlunos, aluno.CodigoAluno.ToString(), componente, turmaCodigo).Result;

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
            linhaDto.AdicionaCelula(99, 99, frequenciaGlobalAluno?.TotalAusencias.ToString() ?? "0", 1);
            linhaDto.AdicionaCelula(99, 99, frequenciaGlobalAluno?.TotalCompensacoes.ToString() ?? "0", 2);
            linhaDto.AdicionaCelula(99, 99, frequenciaGlobalAluno?.PercentualFrequencia.ToString() ?? "0", 3);

            var parecerConclusivo = pareceresConclusivos.FirstOrDefault(c => c.AlunoCodigo == aluno.CodigoAluno.ToString());
            linhaDto.AdicionaCelula(99, 99, parecerConclusivo?.ParecerConclusivo ?? "", 4);

            return linhaDto;
        }

        private async Task<FrequenciaAluno> ObterFrequenciaAluno(IEnumerable<FrequenciaAluno> frequenciaAlunos, string alunoCodigo, ComponenteCurricularPorTurma componenteCurricular, string turmaCodigo)
        {
            var componenteFrequencia = componenteCurricular.Regencia ?
                await ObterComponenteRegenciaTurma(turmaCodigo) :
                componenteCurricular;

            return frequenciaAlunos.FirstOrDefault(c => c.CodigoAluno == alunoCodigo
                                                && c.DisciplinaId == componenteFrequencia.CodDisciplina.ToString());
        }

        private void MontarEstruturaGruposMatriz(ref ConselhoClasseAtaFinalDto relatorio, IEnumerable<IGrouping<ComponenteCurricularGrupoMatriz, ComponenteCurricularPorTurma>> gruposMatrizes, IEnumerable<PeriodoEscolar> periodosEscolares)
        {
            if (gruposMatrizes != null)
                foreach (var grupoMatriz in gruposMatrizes)
                {
                    var grupoMatrizDto = new ConselhoClasseAtaFinalGrupoDto()
                    {
                        Id = grupoMatriz.Key.Id,
                        Nome = grupoMatriz.Key.Nome
                    };

                    foreach (var componenteCurricular in grupoMatriz)
                    {
                        grupoMatrizDto.AdicionarComponente(componenteCurricular.CodDisciplina, componenteCurricular.Disciplina, grupoMatrizDto.Id, periodosEscolares.OrderBy(p => p.Bimestre).Select(a => a.Bimestre));
                    }

                    relatorio.GruposMatriz.Add(grupoMatrizDto);
                }
        }

        private async Task<ComponenteCurricularPorTurma> ObterComponenteRegenciaTurma(string turmaCodigo)
        {
            if (componenteRegencia == null)
            {
                var componentesTurma = await mediator.Send(new ObterComponentesCurricularesPorTurmaQuery(turmaCodigo));
                componenteRegencia = componentesTurma.FirstOrDefault(c => c.Regencia);
            }

            return componenteRegencia;
        }

        private async Task<long> ObterIdTipoCalendario(ModalidadeTipoCalendario modalidade, int anoLetivo, int semestre)
            => await mediator.Send(new ObterIdTipoCalendarioPorAnoLetivoEModalidadeQuery(anoLetivo, modalidade, semestre));

        private async Task<IEnumerable<ConselhoClasseParecerConclusivo>> ObterPareceresConclusivos(string turmaCodigo)
            => await mediator.Send(new ObterParecerConclusivoPorTurmaQuery(turmaCodigo));

        private async Task<IEnumerable<PeriodoEscolar>> ObterPeriodosEscolares(long tipoCalendarioId)
            => await mediator.Send(new ObterPeriodosEscolaresPorTipoCalendarioQuery(tipoCalendarioId));

        private async Task<ConselhoClasseAtaFinalCabecalhoDto> ObterCabecalho(string turmaCodigo)
            => await mediator.Send(new ObterAtaFinalCabecalhoQuery(turmaCodigo));

        private async Task<Turma> ObterTurma(string turmaCodigo)
            => await mediator.Send(new ObterTurmaQuery(turmaCodigo));

        private async Task<IEnumerable<FrequenciaAluno>> ObterFrequenciaComponente(string turmaCodigo, long tipoCalendarioId, IEnumerable<PeriodoEscolar> periodosEscolares)
            => await mediator.Send(new ObterFrequenciaComponenteGlobalPorTurmaQuery(turmaCodigo, tipoCalendarioId, periodosEscolares.Select(a => a.Bimestre)));

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

        private async Task<IEnumerable<ComponenteCurricularPorTurma>> ObterComponentesCurriculares(string turmaCodigo, string usuarioLogadoRF, string perfilUsuario)
        {
            return await mediator.Send(new ObterComponentesCurricularesPorCodigoTurmaLoginEPerfilQuery(turmaCodigo, new Usuario() { Login = usuarioLogadoRF, PerfilAtual = new Guid(perfilUsuario) }));
        }
    }
}
