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
        private const string FREQUENCIA_100 = "100";

        private readonly IMediator mediator;
        private ComponenteCurricularPorTurma componenteRegencia;

        public ObterRelatorioConselhoClasseAtaFinalPdfQueryHandler(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<List<ConselhoClasseAtaFinalPaginaDto>> Handle(ObterRelatorioConselhoClasseAtaFinalPdfQuery request, CancellationToken cancellationToken)
        {
            var mensagensErro = new StringBuilder();
            var relatoriosTurmas = new List<ConselhoClasseAtaFinalPaginaDto>();

            if (request.Filtro.Visualizacao == AtaFinalTipoVisualizacao.Turma)
            {
                foreach (var turmaCodigo in request.Filtro.TurmasCodigos)
                {
                    try
                    {
                        var turma = await ObterTurma(turmaCodigo);
                        var retorno = await ObterRelatorioTurma(turma, request.Filtro, request.Usuario, request.Filtro.Visualizacao);
                        if (retorno != null && retorno.Any())
                            relatoriosTurmas.AddRange(retorno);

                    }
                    catch (Exception e)
                    {
                        var turma = await ObterTurma(turmaCodigo);
                        mensagensErro.AppendLine($"<br/>Erro na carga de dados da turma {turma.NomeRelatorio}: {e}");
                    }
                }
            }

            else if (request.Filtro.Visualizacao == AtaFinalTipoVisualizacao.Estudantes)
            {
                foreach (var turmaCodigo in request.Filtro.TurmasCodigos)
                {
                    try
                    {
                        var turma = await ObterTurma(turmaCodigo);
                        if (turma.TipoTurma == TipoTurma.Regular)
                        {
                            var retorno = await ObterRelatorioEstudante(turma, request.Filtro, request.Usuario, request.Filtro.Visualizacao);
                            if (retorno != null && retorno.Any())
                                relatoriosTurmas.AddRange(retorno);
                        }

                    }
                    catch (Exception e)
                    {
                        var turma = await ObterTurma(turmaCodigo);
                        mensagensErro.AppendLine($"<br/>Erro na carga de dados da turma {turma.NomeRelatorio}: {e}");
                    }
                }
            }



            if (mensagensErro.Length > 0 && relatoriosTurmas.Count() == 0)
                throw new NegocioException(mensagensErro.ToString());

            return relatoriosTurmas;
        }

        private async Task<IEnumerable<ConselhoClasseAtaFinalPaginaDto>> ObterRelatorioTurma(Turma turma, FiltroConselhoClasseAtaFinalDto filtro, Usuario usuario, AtaFinalTipoVisualizacao? visualizacao)
        {
            var alunos = await ObterAlunos(turma.Codigo);
            var alunosCodigos = alunos.Select(x => x.CodigoAluno.ToString()).ToArray();
            List<int> tiposTurma = new List<int>() { (int)turma.TipoTurma };
            if (turma.TipoTurma == TipoTurma.Regular)
                tiposTurma.Add((int)TipoTurma.EdFisica);
            else
                tiposTurma.Add((int)TipoTurma.Regular);

            var notas = await ObterNotasAlunos(alunosCodigos, turma.AnoLetivo, turma.ModalidadeCodigo, turma.Semestre, tiposTurma.ToArray());
            if (notas == null || !notas.Any())
                return Enumerable.Empty<ConselhoClasseAtaFinalPaginaDto>();
            var tipoCalendarioId = await ObterIdTipoCalendario(turma.ModalidadeTipoCalendario, turma.AnoLetivo, turma.Semestre);
            var periodosEscolares = await ObterPeriodosEscolares(tipoCalendarioId);
            var cabecalho = await ObterCabecalho(turma.Codigo);
            var turmas = await ObterTurmasPorCodigo(notas.Select(n => n.Key).ToArray());
            var listaTurmas = ObterCodigosTurmaParaListagem(turma.TipoTurma, turma.Codigo, turmas);

            var componentesCurriculares = await ObterComponentesCurricularesTurmasRelatorio(listaTurmas.ToArray(), turma.Ue.Codigo, turma.ModalidadeCodigo, usuario);
            var frequenciaAlunos = await ObterFrequenciaComponente(turma.Codigo, tipoCalendarioId, periodosEscolares);
            var frequenciaAlunosGeral = await ObterFrequenciaGeral(turma.Codigo);


            List<ConselhoClasseParecerConclusivo> pareceresConclusivos = new List<ConselhoClasseParecerConclusivo>();

            if (turma.TipoTurma == TipoTurma.Itinerarios2AAno)
            {
                foreach (var checarTurma in turmas.Where(t => t.Codigo != turma.Codigo))
                {
                    if (checarTurma.TipoTurma == TipoTurma.Regular)
                        pareceresConclusivos.AddRange(await ObterPareceresConclusivos(checarTurma.Codigo));
                }
            }
            else
            {
                pareceresConclusivos.AddRange(await ObterPareceresConclusivos(turma.Codigo));
            }

            List<ComponenteCurricularPorTurma> componentesDaTurma = new List<ComponenteCurricularPorTurma>();
            foreach (var componente in componentesCurriculares)
            {
                componentesDaTurma.AddRange(componente.ToList());
            }

            List<NotaConceitoBimestreComponente> notasFinais = new List<NotaConceitoBimestreComponente>();
            foreach (var nota in notas)
            {
                notasFinais.AddRange(nota.Select(nf => new NotaConceitoBimestreComponente()
                {
                    AlunoCodigo = nf.CodigoAluno,
                    Nota = nf.NotaConceito.Nota,
                    Bimestre = nf.PeriodoEscolar.Bimestre,
                    ComponenteCurricularCodigo = Convert.ToInt64(nf.CodigoComponenteCurricular),
                    ConceitoId = nf.NotaConceito.ConceitoId,
                    Conceito = nf.NotaConceito.Conceito,
                    Sintese = nf.NotaConceito.Sintese
                }));
            }

            var dadosRelatorio = await MontarEstruturaRelatorio(turma.ModalidadeCodigo, cabecalho, alunos, componentesDaTurma, notasFinais, frequenciaAlunos, frequenciaAlunosGeral, pareceresConclusivos, periodosEscolares, turma.Codigo, null);
            return MontarEstruturaPaginada(dadosRelatorio);
        }

        private async Task<List<Turma>> ObterTurmasPorCodigo(string[] codigos)
        {
            List<Turma> turmas = new List<Turma>();

            foreach (var codigo in codigos)
                turmas.Add(await ObterTurma(codigo));


            return turmas;
        }

        private string[] ObterCodigosTurmaParaListagem(TipoTurma tipoTurma, string codigo, List<Turma> turmas)
        {
            if (tipoTurma == TipoTurma.Itinerarios2AAno)
                return new string[] { codigo };

            List<string> codigos = new List<string>();
            codigos.Add(codigo);

            foreach (var turma in turmas)
            {
                if (turma.TipoTurma != TipoTurma.Regular)
                    codigos.Add(turma.Codigo);
            }
            return codigos.ToArray();
        }

        private async Task<IEnumerable<ConselhoClasseAtaFinalPaginaDto>> ObterRelatorioEstudante(Turma turma, FiltroConselhoClasseAtaFinalDto filtro, Usuario usuario, AtaFinalTipoVisualizacao? visualizacao)
        {
            var alunos = await ObterAlunos(turma.Codigo);
            var alunosCodigos = alunos.Select(x => x.CodigoAluno.ToString()).ToArray();

            var notas = await ObterNotasAlunos(alunosCodigos, turma.AnoLetivo, turma.ModalidadeCodigo, turma.Semestre, new int[] { });
            if (notas == null || !notas.Any())
                return Enumerable.Empty<ConselhoClasseAtaFinalPaginaDto>();
            var tipoCalendarioId = await ObterIdTipoCalendario(turma.ModalidadeTipoCalendario, turma.AnoLetivo, turma.Semestre);
            var periodosEscolares = await ObterPeriodosEscolares(tipoCalendarioId);
            var cabecalho = await ObterCabecalho(turma.Codigo);

            var listaAlunos = await mediator.Send(new ObterDadosAlunosPorCodigosQuery(alunos.Select(x => x.CodigoAluno).ToArray()));
            listaAlunos = listaAlunos.Where(x => x.AnoLetivo == filtro.AnoLetivo);

            var listaTurmasAlunos = listaAlunos.GroupBy(x => x.CodigoTurma);
            List<string> listaTurmas = new List<string>();
            listaTurmas.Add(turma.Codigo);
            foreach (var lta in listaTurmasAlunos)
            {

                var turmaAluno = await ObterTurma(lta.Key.ToString());
                if (turmaAluno.TipoTurma != TipoTurma.Regular)
                    listaTurmas.Add(turmaAluno.Codigo);
            }

            listaTurmasAlunos = listaTurmasAlunos.Where(t => listaTurmas.Any(lt => lt == t.Key.ToString()));

            var componentesDaTurma = await ObterComponentesCurricularesTurmasRelatorio(listaTurmas.ToArray(), turma.Ue.Codigo, turma.ModalidadeCodigo, usuario);
            var frequenciaAlunos = await ObterFrequenciaComponente(turma.Codigo, tipoCalendarioId, periodosEscolares);
            var frequenciaAlunosGeral = await ObterFrequenciaGeral(turma.Codigo);
            var pareceresConclusivos = await ObterPareceresConclusivos(turma.Codigo);

            List<ComponenteCurricularPorTurma> componentesCurriculares = new List<ComponenteCurricularPorTurma>();
            foreach (var componente in componentesDaTurma)
            {
                componentesCurriculares.AddRange(componente.ToList());
            }

            List<NotaConceitoBimestreComponente> notasFinais = new List<NotaConceitoBimestreComponente>();
            foreach (var nota in notas)
            {
                notasFinais.AddRange(nota.Select(nf => new NotaConceitoBimestreComponente()
                {
                    AlunoCodigo = nf.CodigoAluno,
                    Nota = nf.NotaConceito.Nota,
                    Bimestre = nf.PeriodoEscolar.Bimestre,
                    ComponenteCurricularCodigo = Convert.ToInt64(nf.CodigoComponenteCurricular),
                    ConceitoId = nf.NotaConceito.ConceitoId,
                    Conceito = nf.NotaConceito.Conceito,
                    Sintese = nf.NotaConceito.Sintese
                })); ;
            }

            var dadosRelatorio = await MontarEstruturaRelatorio(turma.ModalidadeCodigo, cabecalho, alunos, componentesCurriculares,
                notasFinais, frequenciaAlunos, frequenciaAlunosGeral, pareceresConclusivos, periodosEscolares, turma.Codigo, listaTurmasAlunos);
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
                        Modalidade = dadosRelatorio.Modalidade,
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

        private async Task<ConselhoClasseAtaFinalDto> MontarEstruturaRelatorio(Modalidade modalidadeCodigo, ConselhoClasseAtaFinalCabecalhoDto cabecalho, IEnumerable<AlunoSituacaoAtaFinalDto> alunos, IEnumerable<ComponenteCurricularPorTurma> componentesCurriculares, IEnumerable<NotaConceitoBimestreComponente> notasFinais, IEnumerable<FrequenciaAluno> frequenciaAlunos, IEnumerable<FrequenciaAluno> frequenciaAlunosGeral,
            IEnumerable<ConselhoClasseParecerConclusivo> pareceresConclusivos, IEnumerable<PeriodoEscolar> periodosEscolares,
            string turmaCodigo, IEnumerable<IGrouping<int, AlunoHistoricoEscolar>> listaTurmasAlunos)
        {
            var relatorio = new ConselhoClasseAtaFinalDto();
            relatorio.Modalidade = modalidadeCodigo;

            relatorio.Cabecalho = cabecalho;
            var gruposMatrizes = componentesCurriculares.Where(c => c.GrupoMatriz != null).GroupBy(c => c.GrupoMatriz);

            MontarEstruturaGruposMatriz(relatorio, gruposMatrizes, periodosEscolares);
            await MontarEstruturaLinhas(relatorio, alunos, gruposMatrizes, notasFinais, frequenciaAlunos, frequenciaAlunosGeral, pareceresConclusivos, periodosEscolares, turmaCodigo, listaTurmasAlunos);
            return relatorio;
        }

        private async Task MontarEstruturaLinhas(ConselhoClasseAtaFinalDto relatorio, IEnumerable<AlunoSituacaoAtaFinalDto> alunos, IEnumerable<IGrouping<ComponenteCurricularGrupoMatriz, ComponenteCurricularPorTurma>> gruposMatrizes, IEnumerable<NotaConceitoBimestreComponente> notasFinais, IEnumerable<FrequenciaAluno> frequenciaAlunos, IEnumerable<FrequenciaAluno> frequenciaAlunosGeral, IEnumerable<ConselhoClasseParecerConclusivo> pareceresConclusivos, IEnumerable<PeriodoEscolar> periodosEscolares,
            string turmaCodigo, IEnumerable<IGrouping<int, AlunoHistoricoEscolar>> listaTurmasAlunos)
        {
            // Primmeiro alunos com numero de chamada
            foreach (var aluno in alunos.Where(a => int.Parse(a.NumeroAlunoChamada ?? "0") > 0).Select(a => new AlunoSituacaoAtaFinalDto(a)).OrderBy(a => a.NumeroAlunoChamada))
            {
                relatorio.Linhas.Add(await MontarLinhaAluno(aluno, gruposMatrizes, notasFinais, frequenciaAlunos, frequenciaAlunosGeral, pareceresConclusivos, periodosEscolares, turmaCodigo, listaTurmasAlunos));
            }

            // Depois alunos sem numero ordenados por nome
            foreach (var aluno in alunos.Where(a => int.Parse(a.NumeroAlunoChamada ?? "0") == 0).Select(a => new AlunoSituacaoAtaFinalDto(a)).OrderBy(a => a.NomeAluno))
            {
                relatorio.Linhas.Add(await MontarLinhaAluno(aluno, gruposMatrizes, notasFinais, frequenciaAlunos, frequenciaAlunosGeral, pareceresConclusivos, periodosEscolares, turmaCodigo, listaTurmasAlunos));
            }
        }

        private async Task<ConselhoClasseAtaFinalLinhaDto> MontarLinhaAluno(AlunoSituacaoAtaFinalDto aluno, IEnumerable<IGrouping<ComponenteCurricularGrupoMatriz, ComponenteCurricularPorTurma>> gruposMatrizes, IEnumerable<NotaConceitoBimestreComponente> notasFinais, IEnumerable<FrequenciaAluno> frequenciaAlunos, IEnumerable<FrequenciaAluno> frequenciaAlunosGeral, IEnumerable<ConselhoClasseParecerConclusivo> pareceresConclusivos, IEnumerable<PeriodoEscolar> periodosEscolares, string turmaCodigo, IEnumerable<IGrouping<int, AlunoHistoricoEscolar>> listaTurmasAlunos)
        {
            var linhaDto = new ConselhoClasseAtaFinalLinhaDto()
            {
                Id = long.Parse(aluno.NumeroAlunoChamada ?? "0"),
                Nome = aluno.NomeAluno,
                Situacao = aluno.SituacaoMatricula,
                Inativo = aluno.Inativo
            };
            bool possuiComponente = true;
            var turma = await mediator.Send(new ObterTurmaQuery(turmaCodigo));

            foreach (var grupoMatriz in gruposMatrizes)
            {
                foreach (var componente in grupoMatriz)
                {
                    var coluna = 0;

                    if (listaTurmasAlunos != null)
                        possuiComponente = listaTurmasAlunos.Any(lt => lt.Key.ToString() == componente.CodigoTurma && lt.ToList().Any(a => a.CodigoAluno == aluno.CodigoAluno));
                    // Monta Colunas notas dos bimestres
                    foreach (var bimestre in periodosEscolares.OrderBy(p => p.Bimestre).Select(a => a.Bimestre))
                    {


                        var notaConceito = notasFinais.FirstOrDefault(c => c.AlunoCodigo == aluno.CodigoAluno.ToString()
                                                && c.ComponenteCurricularCodigo == componente.CodDisciplina
                                                && c.Bimestre == bimestre);

                        linhaDto.AdicionaCelula(grupoMatriz.Key.Id,
                                                componente.CodDisciplina,
                                                possuiComponente ? (componente.LancaNota ?
                                                    notaConceito?.NotaConceito ?? "" :
                                                    notaConceito?.Sintese) : "-",
                                                ++coluna);
                    }
                    // Monta coluna Sintese Final - SF
                    var notaConceitofinal = notasFinais.FirstOrDefault(c => c.AlunoCodigo == aluno.CodigoAluno.ToString()
                                            && c.ComponenteCurricularCodigo == componente.CodDisciplina
                                            && !c.Bimestre.HasValue);

                    var frequenciaAluno = await ObterFrequenciaAluno(frequenciaAlunos, aluno.CodigoAluno.ToString(), componente, turmaCodigo);

                    var sintese = await ObterSinteseAluno(frequenciaAluno?.PercentualFrequencia ?? 100, componente);

                    linhaDto.AdicionaCelula(grupoMatriz.Key.Id,
                                            componente.CodDisciplina,
                                            possuiComponente ? (componente.LancaNota ?
                                                notaConceitofinal?.NotaConceito ?? "" :
                                                notaConceitofinal?.Sintese ?? sintese) : "-",
                                            ++coluna);

                    // Monta colunas frequencia F - CA - %


                    linhaDto.AdicionaCelula(grupoMatriz.Key.Id,
                                            componente.CodDisciplina,
                                           possuiComponente ? (frequenciaAluno?.TotalAusencias.ToString() ?? "0") : "-",
                                            ++coluna);
                    linhaDto.AdicionaCelula(grupoMatriz.Key.Id,
                                            componente.CodDisciplina,
                                             possuiComponente ? (frequenciaAluno?.TotalCompensacoes.ToString() ?? "0") : "-",
                                            ++coluna);
                    linhaDto.AdicionaCelula(grupoMatriz.Key.Id,
                                            componente.CodDisciplina,
                                         possuiComponente ? ((turma.AnoLetivo.Equals(2020) ? frequenciaAluno?.PercentualFrequenciaFinal.ToString() : frequenciaAluno?.PercentualFrequencia.ToString()) ?? FREQUENCIA_100) : "-",
                                            ++coluna);
                }
            }

            var frequenciaGlobalAluno = frequenciaAlunosGeral
                .FirstOrDefault(c => c.CodigoAluno == aluno.CodigoAluno.ToString());

            // Anual
            linhaDto.AdicionaCelula(99, 99, frequenciaGlobalAluno?.TotalAusencias.ToString() ?? "0", 1);
            linhaDto.AdicionaCelula(99, 99, frequenciaGlobalAluno?.TotalCompensacoes.ToString() ?? "0", 2);
            linhaDto.AdicionaCelula(99, 99, (turma.AnoLetivo.Equals(2020) ? frequenciaGlobalAluno?.PercentualFrequenciaFinal.ToString() : frequenciaGlobalAluno?.PercentualFrequencia.ToString()) ?? FREQUENCIA_100, 3);

            var parecerConclusivo = pareceresConclusivos.FirstOrDefault(c => c.AlunoCodigo == aluno.CodigoAluno.ToString());
            var textoParecer = parecerConclusivo?.ParecerConclusivo;
            if (textoParecer == null)
                textoParecer = aluno.SituacaoMatricula != null ? string.Concat(aluno.SituacaoMatricula, " em ", aluno.DataSituacaoAluno.ToString("dd/MM/yyyy")) : "Sem Parecer";
            linhaDto.AdicionaCelula(99, 99, textoParecer, 4);

            return linhaDto;
        }

        public async Task<string> ObterSinteseAluno(double? percentualFrequencia, ComponenteCurricularPorTurma componente)
        {
            return percentualFrequencia >= await ObterFrequenciaMediaPorComponenteCurricular(componente.Regencia, componente.LancaNota) ?
                         "Frequente" : "Não Frequente";
        }

        private async Task<double> ObterFrequenciaMediaPorComponenteCurricular(bool ehRegencia, bool lancaNota)
        {
            if (ehRegencia || !lancaNota)
                return double.Parse(await mediator.Send(new ObterParametroSistemaPorTipoQuery()
                {
                    TipoParametro = TipoParametroSistema.CompensacaoAusenciaPercentualRegenciaClasse
                }));
            else
                return double.Parse(await mediator.Send(new ObterParametroSistemaPorTipoQuery()
                {
                    TipoParametro = TipoParametroSistema.CompensacaoAusenciaPercentualFund2
                }));
        }

        private async Task<FrequenciaAluno> ObterFrequenciaAluno(IEnumerable<FrequenciaAluno> frequenciaAlunos, string alunoCodigo, ComponenteCurricularPorTurma componenteCurricular, string turmaCodigo)
        {
            var componenteFrequencia = componenteCurricular.Regencia ?
                await ObterComponenteRegenciaTurma(turmaCodigo) :
                componenteCurricular;

            return frequenciaAlunos.FirstOrDefault(c => c.CodigoAluno == alunoCodigo
                                                && c.DisciplinaId == componenteFrequencia.CodDisciplina.ToString());
        }

        private void MontarEstruturaGruposMatriz(ConselhoClasseAtaFinalDto relatorio, IEnumerable<IGrouping<ComponenteCurricularGrupoMatriz, ComponenteCurricularPorTurma>> gruposMatrizes, IEnumerable<PeriodoEscolar> periodosEscolares)
        {
            if (gruposMatrizes != null)
                foreach (var grupoMatriz in gruposMatrizes.OrderBy(gm => gm.Key.Nome))
                {
                    var grupoMatrizDto = new ConselhoClasseAtaFinalGrupoDto()
                    {
                        Id = grupoMatriz.Key.Id,
                        Nome = grupoMatriz.Key.Nome
                    };

                    foreach (var componenteCurricular in grupoMatriz.OrderBy(c => c.Disciplina))
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

        private async Task<IEnumerable<FrequenciaAluno>> ObterFrequenciaGeral(string turmaCodigo)
            => await mediator.Send(new ObterFrequenciasGeralAlunosNaTurmaQuery(turmaCodigo));

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

        private async Task<IEnumerable<IGrouping<string, NotasAlunoBimestre>>> ObterNotasAlunos(string[] alunosCodigo, int anoLetivo, Modalidade modalidade, int semestre, int[] tiposTurma)
        {
            return await mediator.Send(new ObterNotasRelatorioAtaFinalQuery(alunosCodigo, anoLetivo, (int)modalidade, semestre, tiposTurma));
        }

        private async Task<IEnumerable<IGrouping<string, ComponenteCurricularPorTurma>>> ObterComponentesCurricularesTurmasRelatorio(string[] turmaCodigo, string codigoUe, Modalidade modalidade, Usuario usuario)
        {
            return await mediator.Send(new ObterComponentesCurricularesTurmasRelatorioBoletimQuery()
            {
                CodigosTurma = turmaCodigo,
                CodigoUe = codigoUe,
                Modalidade = modalidade,
                Usuario = usuario
            });
        }
    }
}
