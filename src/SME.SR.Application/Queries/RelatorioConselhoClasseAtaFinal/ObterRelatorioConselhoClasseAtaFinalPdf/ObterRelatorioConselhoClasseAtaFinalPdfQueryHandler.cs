using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using SME.SR.Infra.Utilitarios;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterRelatorioConselhoClasseAtaFinalPdfQueryHandler : IRequestHandler<ObterRelatorioConselhoClasseAtaFinalPdfQuery, List<ConselhoClasseAtaFinalPaginaDto>>
    {
        private const double FREQUENCIA_100 = 100;
        private const int PERCENTUAL_FREQUENCIA_PRECISAO = 2;
        private const string SEM_PARECER_CONCLUSIVO = "Sem parecer";
        private const int COLUNA_AUSENCIA = 1;
        private const int COLUNA_COMPENSACAO = 2;
        private const int COLUNA_PORCENTAGEM_FREQUENCIA = 3;
        private const int COLUNA_PARECER_CONCLUSIVO = 4;
        private const long CODIGO_FREQUENCIA = 999;
        private string frequencia100Formatada = FREQUENCIA_100.ToString($"N{PERCENTUAL_FREQUENCIA_PRECISAO}", CultureInfo.CurrentCulture);

        private readonly VariaveisAmbiente variaveisAmbiente;

        private readonly IMediator mediator;
        private ComponenteCurricularPorTurma componenteRegencia;

        public ObterRelatorioConselhoClasseAtaFinalPdfQueryHandler(IMediator mediator, VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<List<ConselhoClasseAtaFinalPaginaDto>> Handle(ObterRelatorioConselhoClasseAtaFinalPdfQuery request, CancellationToken cancellationToken)
        {
            var mensagensErro = new List<string>();
            var relatoriosTurmas = new List<ConselhoClasseAtaFinalPaginaDto>();
            var turmas = await mediator.Send(new ObterTurmasPorCodigoQuery(request.Filtro.TurmasCodigos.ToArray()));

            turmas.AsParallel().WithDegreeOfParallelism(variaveisAmbiente.ProcessamentoMaximoTurmas).ForAll(async turma =>
            {
                try
                {
                    if (request.Filtro.Visualizacao == AtaFinalTipoVisualizacao.Turma || !request.Filtro.Visualizacao.HasValue)
                    {
                        var retorno = ObterRelatorioTurma(turma, request.Filtro, request.Filtro.Visualizacao, request.Filtro.ImprimirComponentesQueNaoLancamNota).Result;
                        if (retorno != null && retorno.Any())
                            relatoriosTurmas.AddRange(retorno);
                    }
                    else if (request.Filtro.Visualizacao == AtaFinalTipoVisualizacao.Estudantes)
                        if (turma.TipoTurma == TipoTurma.Regular)
                        {
                            var retorno = ObterRelatorioEstudante(turma, request.Filtro, request.Filtro.Visualizacao, request.Filtro.ImprimirComponentesQueNaoLancamNota).Result;
                            if (retorno != null && retorno.Any())
                                relatoriosTurmas.AddRange(retorno);
                        }

                }
                catch (Exception e)
                {
                    mensagensErro.Add($"<br/>Erro na carga de dados da turma {turma.NomeRelatorio}: {e.InnerException.Message}");
                }
            });


            if (mensagensErro.Count() > 0 && relatoriosTurmas.Count() == 0)
            {
                StringBuilder erros = new StringBuilder();
                foreach (var erro in mensagensErro.OrderBy(a => a))
                    erros.AppendLine(erro);

                throw new NegocioException(erros.ToString());
            }

            return relatoriosTurmas.OrderBy(a => a.Cabecalho.Turma).ToList();
        }

        private async Task<IEnumerable<ConselhoClasseAtaFinalPaginaDto>> ObterRelatorioTurma(Turma turma, FiltroConselhoClasseAtaFinalDto filtro, AtaFinalTipoVisualizacao? visualizacao, bool imprimirComponentesQueNaoLancamNota)
        {
            string[] codigosTurmas;
            IEnumerable<Turma> codigosTurmasEdFisica;

            var tipoCalendarioId = await ObterIdTipoCalendario(turma.ModalidadeTipoCalendario, turma.AnoLetivo, turma.Semestre);
            var periodosEscolares = await ObterPeriodosEscolares(tipoCalendarioId);
            var alunos = await ObterAlunos(turma.Codigo, periodosEscolares);

            if (alunos == null || !alunos.Any())
                return Enumerable.Empty<ConselhoClasseAtaFinalPaginaDto>();

            var alunosCodigos = alunos.Select(x => x.CodigoAluno.ToString()).ToArray();
            var alunosCodigosLong = alunos.Select(x => x.CodigoAluno).ToArray();

            var turmasCodigos = new string[] { turma.Codigo };

            codigosTurmasEdFisica = await mediator.Send(new ObterTurmasComplementaresEdFisicaQuery(turmasCodigos, alunosCodigos, turma.AnoLetivo));


            var tiposTurma = new List<int>() { (int)turma.TipoTurma };

            if (turma.TipoTurma == TipoTurma.Regular)
                tiposTurma.Add((int)TipoTurma.EdFisica);
            else
                tiposTurma.Add((int)TipoTurma.Regular);

            if (codigosTurmasEdFisica != null && codigosTurmasEdFisica.Any())
                codigosTurmas = await RetornaTurmasComCodigoDeEdFisica(codigosTurmasEdFisica, turmasCodigos);
            else
                codigosTurmas = new string[] { turma.Codigo };

            var codigoTurmaRelacionada = await mediator.Send(new ObterTurmaCodigosAlunoPorAnoLetivoAlunoTipoTurmaQuery(turma.AnoLetivo, alunosCodigos,
                       tiposTurma, turma.AnoLetivo < DateTimeExtension.HorarioBrasilia().Year, DateTimeExtension.HorarioBrasilia()));

            if (turma.TipoTurma == TipoTurma.EdFisica)
                codigosTurmas = codigoTurmaRelacionada.Select(codigoTurma => codigoTurma.ToString()).ToArray();

            var notas = await ObterNotasAlunos(alunosCodigos, codigosTurmas, turma.AnoLetivo, turma.ModalidadeCodigo, filtro.Semestre, tiposTurma.ToArray());
            
            if (notas == null || !notas.Any())
                return Enumerable.Empty<ConselhoClasseAtaFinalPaginaDto>();

            var cabecalho = await ObterCabecalho(turma.Codigo);

            var turmas = await ObterTurmasPorCodigo(notas.Select(n => n.Key).ToArray());

            var informacoesAlunos = await mediator.Send(new ObterDadosAlunosPorCodigosQuery(alunos.Select(x => x.CodigoAluno).ToArray(), turma.AnoLetivo));
            informacoesAlunos = informacoesAlunos.Where(i => turmas.Any(x => x.Codigo == i.CodigoTurma.ToString()) && (i.CodigoSituacaoMatricula != SituacaoMatriculaAluno.RemanejadoSaida));
            notas = notas.Where(x => informacoesAlunos.Select(j => j.CodigoTurma.ToString()).ToList().Contains(x.Key));   
            var listaTurmas = ObterCodigosTurmaParaListagem(turma.TipoTurma, turma.Codigo, turmas);

            listaTurmas = await RetornaTurmasComCodigoDeEdFisica(codigosTurmasEdFisica, listaTurmas);

            var componentesCurriculares = await ObterComponentesCurricularesTurmasRelatorio(listaTurmas.ToArray(), turma.Ue.Codigo, turma.ModalidadeCodigo);
            var frequenciaAlunosGeral = await ObterFrequenciaGeralPorAlunos(turma.AnoLetivo, listaTurmas, tipoCalendarioId, alunosCodigos);

            var listaAlunos = await mediator.Send(new ObterDadosAlunosPorCodigosQuery(alunosCodigos.Select(long.Parse).ToArray(), filtro.AnoLetivo));
            listaAlunos = listaAlunos.Where(x => x.AnoLetivo == filtro.AnoLetivo);

            var listaTurmasAlunos = listaAlunos.GroupBy(x => x.CodigoTurma);

            listaTurmasAlunos = listaTurmasAlunos.Where(t => listaTurmas.Any(lt => lt == t.Key.ToString()));

            var pareceresConclusivos = new List<ConselhoClasseParecerConclusivo>();

            if (turma.TipoTurma == TipoTurma.Itinerarios2AAno || turma.TipoTurma == TipoTurma.EdFisica)
            {
                foreach (var checarTurma in turmas.Where(t => t.Codigo != turma.Codigo))
                {
                    if (checarTurma.TipoTurma == TipoTurma.Regular)
                        pareceresConclusivos.AddRange(await ObterPareceresConclusivos(checarTurma.Codigo));
                }
            }
            else
                pareceresConclusivos.AddRange(await ObterPareceresConclusivos(turma.Codigo));

            var componentesDaTurma = componentesCurriculares.SelectMany(cc => cc).ToList();
            var componentesCurricularesPorTurma = ObterComponentesCurricularesTurma(componentesDaTurma);
            var bimestres = periodosEscolares.Select(p => p.Bimestre).ToArray();
            var frequenciaAlunos = await ObterFrequenciaComponente(listaTurmas.ToArray(), componentesCurricularesPorTurma, bimestres, tipoCalendarioId, alunos.Select(a => (a.CodigoAluno.ToString(), a.DataMatricula, a.Inativo ? a.DataSituacaoAluno : (DateTime?)null)));
            var areasDoConhecimento = await ObterAreasConhecimento(componentesCurriculares);
            var ordenacaoGrupoArea = await ObterOrdenacaoAreasConhecimento(componentesCurriculares, areasDoConhecimento);

            var notasFinais = new List<NotaConceitoBimestreComponente>();
            foreach (var nota in notas)
            {
                notasFinais.AddRange(nota.Select(nf => new NotaConceitoBimestreComponente()
                {
                    AlunoCodigo = nf.CodigoAluno,
                    Nota = nf.NotaConceito.Nota,
                    Bimestre = nf.NotaConceito.Bimestre,
                    ComponenteCurricularCodigo = Convert.ToInt64(nf.CodigoComponenteCurricular),
                    ConceitoId = nf.NotaConceito.ConceitoId,
                    Conceito = nf.NotaConceito.Conceito,
                    NotaId = nf.NotaConceito.NotaId,
                    Sintese = nf.NotaConceito.Sintese,
                    ConselhoClasseAlunoId = nf.ConselhoClasseAlunoId,
                    Aprovado = nf.Aprovado
                }));
            }
            var alunosNotasConceito = await mediator.Send(new ObterNotaConceitoEducacaoFisicaNaEjaQuery(alunosCodigos, turma));
            var dadosRelatorio = await MontarEstruturaRelatorio(turma, cabecalho, alunos, componentesDaTurma, notasFinais, frequenciaAlunos, frequenciaAlunosGeral, pareceresConclusivos, periodosEscolares, listaTurmasAlunos, areasDoConhecimento, ordenacaoGrupoArea, alunosNotasConceito, imprimirComponentesQueNaoLancamNota, codigosTurmas);
            return MontarEstruturaPaginada(dadosRelatorio);
        }
        private async Task<string[]> RetornaTurmasComCodigoDeEdFisica(IEnumerable<Turma> turmasEdFisica, string[] listaTurmas)
        {
            if(!turmasEdFisica.Any())
                return await Task.FromResult(listaTurmas);

            var listaTurmasList = listaTurmas.ToList();

            foreach (var codigoTurmaEdFisica in turmasEdFisica)
            {
                listaTurmasList.Add(codigoTurmaEdFisica.Codigo);
            }

            if (listaTurmasList.Count() > 1)
                listaTurmas = listaTurmasList.ToArray();

            return await Task.FromResult(listaTurmas);
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

        private async Task<IEnumerable<ConselhoClasseAtaFinalPaginaDto>> ObterRelatorioEstudante(Turma turma, FiltroConselhoClasseAtaFinalDto filtro, AtaFinalTipoVisualizacao? visualizacao, bool imprimirComponentesQueNaoLancamNota)
        {
            var tipoCalendarioId = await ObterIdTipoCalendario(turma.ModalidadeTipoCalendario, turma.AnoLetivo, turma.Semestre);
            var periodosEscolares = await ObterPeriodosEscolares(tipoCalendarioId);
            var alunos = await ObterAlunos(turma.Codigo, periodosEscolares);
            var alunosCodigos = alunos.Select(x => x.CodigoAluno.ToString()).ToArray();
            var codigosDeTurmas = await ObterCodigoDeTurmas(turma, alunos);
            var notas = await ObterNotasAlunos(alunosCodigos, codigosDeTurmas.ToArray(), turma.AnoLetivo, turma.ModalidadeCodigo, turma.Semestre, new int[] { });
            if (notas == null || !notas.Any()) return default;

            var cabecalho = await ObterCabecalho(turma.Codigo);
            var listaAlunos = await mediator.Send(new ObterDadosAlunosPorCodigosQuery(alunos.Select(x => x.CodigoAluno).ToArray()));
            listaAlunos = listaAlunos.Where(x => x.AnoLetivo == filtro.AnoLetivo);

            var listaTurmasAlunos = listaAlunos.GroupBy(x => x.CodigoTurma);
            List<string> listaTurmas = new List<string>();
            listaTurmas.Add(turma.Codigo);

            var turmaDetalhes = await ObterTurmaDetalhes(turma.Codigo);

            var tipoTurma = turmaDetalhes.EtapaEnsino != (int)EtapaEnsino.Magisterio ? TipoTurma.Regular : TipoTurma.EdFisica;

            foreach (var lta in listaTurmasAlunos)
            {
                var turmaAluno = await ObterTurma(lta.Key.ToString());
                if (turmaAluno.TipoTurma != tipoTurma)
                    listaTurmas.Add(turmaAluno.Codigo);
            }

            listaTurmasAlunos = listaTurmasAlunos.Where(t => listaTurmas.Any(lt => lt == t.Key.ToString()));

            var componentesDaTurma = await ObterComponentesCurricularesTurmasRelatorio(listaTurmas.ToArray(), turma.Ue.Codigo, turma.ModalidadeCodigo);
            var frequenciaAlunosGeral = await ObterFrequenciaGeral(turma.AnoLetivo, tipoCalendarioId);
            var pareceresConclusivos = await ObterPareceresConclusivos(turma.Codigo);

            var componentesCurriculares = componentesDaTurma.SelectMany(cc => cc).ToList();
            var componentesCurricularesPorTurma = ObterComponentesCurricularesTurma(componentesCurriculares);

            var bimestres = periodosEscolares.Select(p => p.Bimestre).ToArray();
            var frequenciaAlunos = await ObterFrequenciaComponente(listaTurmas.ToArray(), componentesCurricularesPorTurma, bimestres, tipoCalendarioId, alunos.Select(a => (a.CodigoAluno.ToString(), a.DataMatricula, a.Inativo ? a.DataSituacaoAluno : (DateTime?)null)));

            var areasDoConhecimento = await ObterAreasConhecimento(componentesDaTurma);

            var ordenacaoGrupoArea = await ObterOrdenacaoAreasConhecimento(componentesDaTurma, areasDoConhecimento);

            List<NotaConceitoBimestreComponente> notasFinais = new List<NotaConceitoBimestreComponente>();
            foreach (var nota in notas)
            {
                notasFinais.AddRange(nota.Select(nf => new NotaConceitoBimestreComponente()
                {
                    AlunoCodigo = nf.CodigoAluno,
                    Nota = nf.NotaConceito.Nota,
                    Bimestre = nf.NotaConceito.Bimestre,
                    ComponenteCurricularCodigo = Convert.ToInt64(nf.CodigoComponenteCurricular),
                    ConceitoId = nf.NotaConceito.ConceitoId,
                    Conceito = nf.NotaConceito.Conceito,
                    Sintese = nf.NotaConceito.Sintese,
                    ConselhoClasseAlunoId = nf.ConselhoClasseAlunoId,
                    Aprovado = nf.Aprovado
                }));
            }
            var alunosNotasConceito = await mediator.Send(new ObterNotaConceitoEducacaoFisicaNaEjaQuery(alunosCodigos, turma));
            var dadosRelatorio = await MontarEstruturaRelatorio(turma, cabecalho, alunos, componentesCurriculares,
                notasFinais, frequenciaAlunos, frequenciaAlunosGeral, pareceresConclusivos, periodosEscolares, listaTurmasAlunos, areasDoConhecimento, ordenacaoGrupoArea, alunosNotasConceito, imprimirComponentesQueNaoLancamNota);
            return MontarEstruturaPaginada(dadosRelatorio);
        }

        private IEnumerable<(string, long)> ObterComponentesCurricularesTurma(List<ComponenteCurricularPorTurma> componentesCurriculares)
        {
            var componentes = new List<(string, long)>();
            componentes.AddRange(componentesCurriculares.Select(cc => (cc.CodigoTurma, cc.CodDisciplina)).Distinct());
            if (componentesCurriculares.Any(a => a.Regencia))
            {
                foreach (var componenteRegencia in componentesCurriculares.Where(a => a.Regencia))
                    componentes.AddRange(componenteRegencia.ComponentesCurricularesRegencia.Select(cc => (componenteRegencia.CodigoTurma, cc.CodDisciplina)).Distinct());
            }

            return componentes.Distinct().ToList();
        }

        private async Task<IEnumerable<AreaDoConhecimento>> ObterAreasConhecimento(IEnumerable<IGrouping<string, ComponenteCurricularPorTurma>> componentesCurriculares)
        {
            //TODO: MELHORAR CODIGO
            var listaCodigosComponentes = new List<long>();

            listaCodigosComponentes.AddRange(componentesCurriculares.SelectMany(a => a).Where(cc => cc.Regencia).SelectMany(a => a.ComponentesCurricularesRegencia).Select(cc => cc.CodDisciplina).ToList());
            listaCodigosComponentes.AddRange(componentesCurriculares.SelectMany(a => a).Where(cc => cc.TerritorioSaber).Select(a => a.CodigoComponenteCurricularTerritorioSaber).ToList());
            listaCodigosComponentes.AddRange(componentesCurriculares.SelectMany(a => a).Where(cc => !cc.Regencia).Select(a => a.CodDisciplina).ToList());

            return await mediator.Send(new ObterAreasConhecimentoComponenteCurricularQuery(listaCodigosComponentes.Distinct().ToArray()));
        }

        private async Task<IEnumerable<ComponenteCurricularGrupoAreaOrdenacaoDto>> ObterOrdenacaoAreasConhecimento(IEnumerable<IGrouping<string, ComponenteCurricularPorTurma>> componentesCurriculares, IEnumerable<AreaDoConhecimento> areasDoConhecimento)
        {
            var listaGrupoMatrizId = componentesCurriculares?.SelectMany(a => a)?.Select(a => a.GrupoMatriz.Id)?.Distinct().ToArray();
            var listaAreaConhecimentoId = areasDoConhecimento?.Select(a => a.Id).ToArray();

            return await mediator.Send(new ObterComponenteCurricularGrupoAreaOrdenacaoQuery(listaGrupoMatrizId, listaAreaConhecimentoId));
        }

        private int ObterTotalComponentesPorPagina(ConselhoClasseAtaFinalDto dadosRelatorio, int totalColunasPorPagina)
        {
            var colunas = 0;
            var totalComponentes = 0;

            foreach (var grupo in dadosRelatorio.GruposMatriz)
            {
                foreach (var componentes in grupo.ComponentesCurriculares)
                {
                    colunas += componentes.Colunas.Count;

                    if (colunas <= totalColunasPorPagina)
                        totalComponentes++;
                    else
                        break;
                }
            }

            return totalComponentes;
        }

        private List<ConselhoClasseAtaFinalPaginaDto> MontarEstruturaPaginada(ConselhoClasseAtaFinalDto dadosRelatorio)
        {
            var quantidadeDeLinhasPorPagina = 45;
            var totalColunas = dadosRelatorio.GruposMatriz.Sum(grupo => grupo.ComponentesCurriculares.Sum(componente => componente.Colunas.Count));
            var maximoColunasPorPagina = 52;
            var maximoComponentesPorPagina = ObterTotalComponentesPorPagina(dadosRelatorio, maximoColunasPorPagina);

            List<ConselhoClasseAtaFinalPaginaDto> modelsPaginas = new List<ConselhoClasseAtaFinalPaginaDto>();

            List<ConselhoClasseAtaFinalComponenteDto> todasAsDisciplinas = dadosRelatorio.GruposMatriz.SelectMany(x => x.ComponentesCurriculares).ToList();

            int quantidadePaginasHorizontal = CalcularPaginasHorizontal(maximoColunasPorPagina, totalColunas);

            int quantidadePaginasVertical = (int)Math.Ceiling(dadosRelatorio.Linhas.Count / (decimal)quantidadeDeLinhasPorPagina);

            int contPagina = 1;

            for (int v = 0; v < quantidadePaginasVertical; v++)
            {
                for (int h = 0; h < quantidadePaginasHorizontal; h++)
                {
                    bool ehPaginaFinal = (h + 1) == quantidadePaginasHorizontal;

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
                        IEnumerable<ConselhoClasseAtaFinalComponenteDto> disciplinasDestaPagina = todasAsDisciplinas.Skip(h * maximoComponentesPorPagina).Take(maximoComponentesPorPagina);

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

                        linha.Celulas = todasAsCelulas.Where(x => gruposMatrizDestaPagina.Contains(x.GrupoMatriz) && idsDisciplinasDestaPagina.Contains(x.ComponenteCurricular)).Select(x => new ConselhoClasseAtaFinalCelulaDto { GrupoMatriz = x.GrupoMatriz, ComponenteCurricular = x.ComponenteCurricular, Coluna = x.Coluna, Valor = x.Valor, Regencia = x.Regencia }).ToList();

                        if (ehPaginaFinal)
                        {
                            IEnumerable<ConselhoClasseAtaFinalCelulaDto> celulasFinais = todasAsCelulas.Where(x => x.GrupoMatriz == 99);

                            linha.Celulas.AddRange(celulasFinais);
                        }
                    }

                    modelPagina.Linhas.AddRange(linhas);

                    foreach (var grupoMatriz in modelPagina.GruposMatriz)
                        grupoMatriz.QuantidadeColunas = modelPagina.Linhas.First().Celulas.Where(x => x.GrupoMatriz == grupoMatriz.Id).Count();


                    modelPagina.GruposMatriz = modelPagina.GruposMatriz.OrderBy(grupo => !grupo.Regencia).ThenBy(grupo => grupo.Id).ToList();

                    modelsPaginas.Add(modelPagina);
                }
            }

            return modelsPaginas;
        }

        private async Task<ConselhoClasseAtaFinalDto> MontarEstruturaRelatorio(Turma turma, ConselhoClasseAtaFinalCabecalhoDto cabecalho, IEnumerable<AlunoSituacaoAtaFinalDto> alunos, IEnumerable<ComponenteCurricularPorTurma> componentesCurriculares, IEnumerable<NotaConceitoBimestreComponente> notasFinais, IEnumerable<FrequenciaAluno> frequenciaAlunos, IEnumerable<FrequenciaAluno> frequenciaAlunosGeral,
            IEnumerable<ConselhoClasseParecerConclusivo> pareceresConclusivos, IEnumerable<PeriodoEscolar> periodosEscolares,
            IEnumerable<IGrouping<int, AlunoHistoricoEscolar>> listaTurmasAlunos, IEnumerable<AreaDoConhecimento> areasDoConhecimento, IEnumerable<ComponenteCurricularGrupoAreaOrdenacaoDto> ordenacaoGrupoArea, IEnumerable<AlunoNotaTipoNotaDtoEducacaoFisicaDto> alunosNotasConceito, bool imprimirComponentesQueNaoLancamNota, string[] turmasRelacionadas = null)
        {
            var relatorio = new ConselhoClasseAtaFinalDto()
            {
                Modalidade = turma.ModalidadeCodigo,
                Cabecalho = cabecalho
            };

            componentesCurriculares = componentesCurriculares.OrderBy(a => a.Disciplina).ToList();
            var componentesMatriz = componentesCurriculares.Distinct().Where(c => c.GrupoMatriz != null);

            if (!imprimirComponentesQueNaoLancamNota)
                componentesMatriz = componentesMatriz.Where(componente => componente.LancaNota);

            var gruposMatrizes = componentesMatriz.GroupBy(c => c.GrupoMatriz).ToList();

            AdicionarFrequenciaRegencia(gruposMatrizes, ref areasDoConhecimento);
            MontarEstruturaGruposMatriz(relatorio, gruposMatrizes, periodosEscolares, areasDoConhecimento, ordenacaoGrupoArea);
            await MontarEstruturaLinhas(relatorio, alunos, gruposMatrizes, notasFinais, frequenciaAlunos, frequenciaAlunosGeral, pareceresConclusivos, periodosEscolares, turma, listaTurmasAlunos, alunosNotasConceito, turmasRelacionadas,componentesCurriculares.Count(cc => cc.Frequencia));
            return relatorio;
        }

        private void AdicionarFrequenciaRegencia(
                            IEnumerable<IGrouping<ComponenteCurricularGrupoMatriz, ComponenteCurricularPorTurma>> gruposMatrizes,
                            ref IEnumerable<AreaDoConhecimento> areasDoConhecimento)
        {

            foreach (var grupo in gruposMatrizes)
            {
                var componenteRegencia = grupo.FirstOrDefault(componente => componente.Regencia);

                if (componenteRegencia != null)
                {
                    var componentes = componenteRegencia.ComponentesCurricularesRegencia.ToList();

                    componentes.ForEach(componente => { componente.Frequencia = false; });

                    componentes.Add(new ComponenteCurricularPorTurmaRegencia()
                    {
                        LancaNota = false,
                        Frequencia = true,
                        CodDisciplina = CODIGO_FREQUENCIA,
                        Disciplina = "Frequência",
                        Regencia = true
                    });

                    componenteRegencia.ComponentesCurricularesRegencia = componentes;

                    var areas = areasDoConhecimento.ToList();

                    areas.Add(new AreaDoConhecimento() { CodigoComponenteCurricular = CODIGO_FREQUENCIA });

                    areasDoConhecimento = areas;
                    return;
                }
            }
        }

        private async Task MontarEstruturaLinhas(ConselhoClasseAtaFinalDto relatorio, IEnumerable<AlunoSituacaoAtaFinalDto> alunos, IEnumerable<IGrouping<ComponenteCurricularGrupoMatriz,
            ComponenteCurricularPorTurma>> gruposMatrizes, IEnumerable<NotaConceitoBimestreComponente> notasFinais, IEnumerable<FrequenciaAluno> frequenciaAlunos, IEnumerable<FrequenciaAluno> frequenciaAlunosGeral,
            IEnumerable<ConselhoClasseParecerConclusivo> pareceresConclusivos, IEnumerable<PeriodoEscolar> periodosEscolares,
            Turma turma, IEnumerable<IGrouping<int, AlunoHistoricoEscolar>> listaTurmasAlunos, IEnumerable<AlunoNotaTipoNotaDtoEducacaoFisicaDto> alunosNotasConceito, string[] turmasRelacionadas, int qtdeDisciplinasLancamFrequencia = 0)
        {
            var compensacaoAusenciaPercentualRegenciaClasse = double.Parse(await mediator.Send(new ObterParametroSistemaPorTipoQuery()
            {
                TipoParametro = TipoParametroSistema.CompensacaoAusenciaPercentualRegenciaClasse
            }));

            var compensacaoAusenciaPercentualFund2 = double.Parse(await mediator.Send(new ObterParametroSistemaPorTipoQuery()
            {
                TipoParametro = TipoParametroSistema.CompensacaoAusenciaPercentualFund2
            }));

            var alunosComNumeroChamada = await MontarLinhaAluno(alunos.Select(a => new AlunoSituacaoAtaFinalDto(a)).OrderBy(a => a.NomeAluno),
                gruposMatrizes, notasFinais, frequenciaAlunos, frequenciaAlunosGeral, pareceresConclusivos, periodosEscolares, turma, listaTurmasAlunos, alunosNotasConceito, turmasRelacionadas, qtdeDisciplinasLancamFrequencia,
                compensacaoAusenciaPercentualRegenciaClasse, compensacaoAusenciaPercentualFund2);

            relatorio.Linhas.AddRange(alunosComNumeroChamada);
        }

        private async Task<List<ConselhoClasseAtaFinalLinhaDto>> MontarLinhaAluno(IEnumerable<AlunoSituacaoAtaFinalDto> alunos,
                                                                                  IEnumerable<IGrouping<ComponenteCurricularGrupoMatriz,
                                                                                  ComponenteCurricularPorTurma>> gruposMatrizes,
                                                                                  IEnumerable<NotaConceitoBimestreComponente> notasFinais,
                                                                                  IEnumerable<FrequenciaAluno> frequenciaAlunos,
                                                                                  IEnumerable<FrequenciaAluno> frequenciaAlunosGeral,
                                                                                  IEnumerable<ConselhoClasseParecerConclusivo> pareceresConclusivos,
                                                                                  IEnumerable<PeriodoEscolar> periodosEscolares,
                                                                                  Turma turma,
                                                                                  IEnumerable<IGrouping<int, AlunoHistoricoEscolar>> listaTurmasAlunos,
                                                                                  IEnumerable<AlunoNotaTipoNotaDtoEducacaoFisicaDto> alunosNotasConceito,
                                                                                  string[] turmasRelacionadas,
                                                                                  int qtdeDisciplinasLancamFrequencia = 0,
                                                                                  double compensacaoAusenciaPercentualRegenciaClasse = 0,
                                                                                  double compensacaoAusenciaPercentualFund2 = 0)
        {
            List<ConselhoClasseAtaFinalLinhaDto> linhas = new List<ConselhoClasseAtaFinalLinhaDto>();
            var tipoNota = await mediator.Send(new ObterTipoNotaPorTurmaQuery(turma, turma.AnoLetivo));
            string[] turmas = turma.TipoTurma == TipoTurma.EdFisica && turmasRelacionadas != null && turmasRelacionadas.Any() ? turmasRelacionadas : new string[] { turma.Codigo };

            for (var i = 0; i < alunos.Count(); i++)
            {
                var aluno = alunos.ElementAt(i);
                var linhaDto = new ConselhoClasseAtaFinalLinhaDto()
                {
                    Id = long.Parse(aluno.NumeroAlunoChamada ?? "0"),
                    Nome = aluno.NomeAluno,
                    Situacao = aluno.SituacaoMatricula,
                    Inativo = aluno.Inativo
                };

                bool possuiComponente = true;
                bool existeFrequenciaRegistradaTurmaAno = false;
                bool possuiConselhoUltimoBimestreAtivo = false;
                bool possuiComponenteFrequencia = false;

                var conselhoClasseBimestres = new List<int>();

                foreach (var codigoTurmaRelacionada in turmas)
                {
                    var conselhoTurmaRelacionada = await mediator.Send(new AlunoConselhoClasseCadastradoBimestresQuery(aluno.CodigoAluno.ToString(), turma.AnoLetivo, turma.ModalidadeCodigo, turma.Semestre, codigoTurmaRelacionada));
                    
                    if(conselhoTurmaRelacionada != null && conselhoTurmaRelacionada.Any())
                    {
                        foreach(var conselho in conselhoTurmaRelacionada)
                            conselhoClasseBimestres.Add(conselho);
                    }
                }

                foreach (var grupoMatriz in gruposMatrizes)
                {
                    var componentes = ObterComponentesCurriculares(grupoMatriz.GroupBy(c => c.CodDisciplina).Select(x => x.FirstOrDefault()).ToList());
                    var componentesTurmas = ObterComponentesCurriculares(grupoMatriz.ToList());
                    foreach (var componente in componentes)
                    {
                        var coluna = 0;

                        if (EhComponenteFrequencia(componente))
                            continue;

                        if (listaTurmasAlunos != null)
                        {
                            var turmasDoAluno = listaTurmasAlunos.SelectMany(a => a.Where(b => b.CodigoAluno == aluno.CodigoAluno)).Select(a => a.CodigoTurma).Distinct().ToArray();
                            var componentesDoAluno = componentesTurmas.Where(a => a.CodigoTurma != null && turmasDoAluno.Contains(int.Parse(a.CodigoTurma)) && a.CodDisciplina == componente.CodDisciplina).ToList();
                            possuiComponente = componentesDoAluno.Any();
                        }

                        var ultimoBimestreAtivo = aluno.Inativo ?
                            periodosEscolares.OrderBy(o => o.Bimestre).FirstOrDefault(p => aluno.DataSituacaoAluno.Date <= p.PeriodoFim.Date)?.Bimestre : 4;

                        if (ultimoBimestreAtivo == null)
                            possuiComponente = false;

                        var codDisciplina = componente.Regencia ? componente.CodDisciplinaPai : componente.CodDisciplina;

                        var turmaPossuiFrequenciaRegistrada = await mediator.Send(new ExisteFrequenciaRegistradaPorTurmaComponenteCurricularEAnoQuery(turma.Codigo, codDisciplina.ToString(), turma.AnoLetivo));

                        if (turmaPossuiFrequenciaRegistrada && !existeFrequenciaRegistradaTurmaAno)
                            existeFrequenciaRegistradaTurmaAno = true;

                        var frequenciaAluno = ObterFrequenciaAluno(frequenciaAlunos, aluno.CodigoAluno.ToString(), componente, componentesTurmas);

                        if (componente.LancaNota)
                        {
                            var matriculadoDepois = !aluno.Inativo ? periodosEscolares.FirstOrDefault(p => aluno.DataMatricula > p.PeriodoFim)?.Bimestre : null;
                            var bimestres = periodosEscolares.OrderBy(p => p.Bimestre).Select(a => a.Bimestre).ToList();
                            foreach (var bimestre in bimestres)
                            {
                                var possuiConselho = notasFinais.Any(n => n.Bimestre == bimestre
                                && n.AlunoCodigo == aluno.CodigoAluno.ToString() && conselhoClasseBimestres.Any(a => a == bimestre));

                                if (matriculadoDepois != null && bimestre < matriculadoDepois)
                                {
                                    linhaDto.AdicionaCelula(grupoMatriz.Key.Id, componente.CodDisciplina, string.Empty, ++coluna, aluno.CodigoAluno.ToString(), bimestre, componente.Regencia);
                                    continue;
                                }

                                if (bimestre > ultimoBimestreAtivo)
                                {
                                    linhaDto.AdicionaCelula(grupoMatriz.Key.Id, componente.CodDisciplina, string.Empty, ++coluna, aluno.CodigoAluno.ToString(), bimestre, componente.Regencia);
                                    continue;
                                }

                                if (possuiConselho)
                                {
                                    var notaConceito = new NotaConceitoBimestreComponente();

                                    if (tipoNota == null)
                                        notaConceito = RetornaValorPadraoNotaAtaFinal(notasFinais, aluno, componente, bimestre);
                                    else
                                    {
                                        notaConceito = tipoNota.TipoNota == TipoNota.Nota
                                            ? notasFinais.OrderByDescending(n => n.ConselhoClasseAlunoId).ThenByDescending(n => n.NotaId).FirstOrDefault(c => c.AlunoCodigo == aluno.CodigoAluno.ToString()
                                                                 && c.ComponenteCurricularCodigo == componente.CodDisciplina
                                                                 && c.Bimestre == bimestre)
                                            : tipoNota.TipoNota == TipoNota.Conceito
                                                ? notasFinais.OrderByDescending(n => n.ConselhoClasseAlunoId).ThenByDescending(n => n.ConceitoId).FirstOrDefault(c => c.AlunoCodigo == aluno.CodigoAluno.ToString()
                                                                 && c.ComponenteCurricularCodigo == componente.CodDisciplina
                                                                 && c.Bimestre == bimestre)
                                                : RetornaValorPadraoNotaAtaFinal(notasFinais, aluno, componente, bimestre);
                                    }

                                    linhaDto.AdicionaCelula(grupoMatriz.Key.Id,
                                                            componente.CodDisciplina,
                                                            possuiComponente ? (componente.LancaNota ?
                                                                notaConceito?.NotaConceito ?? "" :
                                                                notaConceito?.Sintese) : string.Empty,
                                                            ++coluna,
                                                            aluno.CodigoAluno.ToString(),
                                                            bimestre,
                                                            componente.Regencia);

                                    if (ultimoBimestreAtivo > 0)
                                        possuiConselhoUltimoBimestreAtivo = bimestre == ultimoBimestreAtivo;
                                    continue;
                                }

                                linhaDto.AdicionaCelula(grupoMatriz.Key.Id, componente.CodDisciplina, possuiComponente ? "" : string.Empty, ++coluna, aluno.CodigoAluno.ToString(), bimestre, componente.Regencia);

                            }

                            var notaConceitofinal = notasFinais.OrderByDescending(n => n.Aprovado).ThenByDescending(n => n.NotaId).FirstOrDefault(c => c.AlunoCodigo == aluno.CodigoAluno.ToString()
                                                    && c.ComponenteCurricularCodigo == componente.CodDisciplina
                                                    && (!c.Bimestre.HasValue || c.Bimestre.Value == 0) &&
                                                    aluno.Ativo);

                            var possuiConselhoFinal = (notaConceitofinal != null) && conselhoClasseBimestres.Any(a => a == 0);
                            if (possuiConselhoFinal)
                            {
                                var sintese = ObterSinteseAluno(frequenciaAluno?.PercentualFrequencia ?? 100, componente, compensacaoAusenciaPercentualRegenciaClasse, compensacaoAusenciaPercentualFund2);

                                linhaDto.AdicionaCelula(grupoMatriz.Key.Id,
                                                    componente.CodDisciplina,
                                                    possuiComponente && (aluno.Ativo || possuiConselhoUltimoBimestreAtivo) ? (componente.LancaNota ?
                                                        notaConceitofinal?.NotaConceito ?? "" :
                                                        notaConceitofinal?.Sintese ?? sintese) : string.Empty,
                                                    ++coluna, aluno.CodigoAluno.ToString(), null, componente.Regencia);
                            }
                            else
                                linhaDto.AdicionaCelula(grupoMatriz.Key.Id,
                                                    componente.CodDisciplina,
                                                     possuiComponente ? "" : string.Empty,
                                                    ++coluna, aluno.CodigoAluno.ToString(), null, componente.Regencia);

                        }

                        if (ApresentarFrequencia(componente, possuiComponenteFrequencia))
                        {
                            if (componente.Regencia)
                                possuiComponenteFrequencia = true;

                            if (conselhoClasseBimestres.Any())
                            {
                                linhaDto.Celulas.AddRange(ObterCelulasFrequencias(
                                                                    grupoMatriz.Key.Id,
                                                                    componente.Regencia ? CODIGO_FREQUENCIA : componente.CodDisciplina,
                                                                    componente.Regencia ? 0 : coluna,
                                                                    possuiComponente,
                                                                    frequenciaAluno,
                                                                    turma,
                                                                    aluno,
                                                                    possuiConselhoUltimoBimestreAtivo,
                                                                    turmaPossuiFrequenciaRegistrada, componente.Regencia));
                            }
                            else linhaDto.Celulas.AddRange(ObterCelulasFrequencias(
                                                                    grupoMatriz.Key.Id,
                                                                    componente.Regencia ? CODIGO_FREQUENCIA : componente.CodDisciplina,
                                                                    componente.Regencia ? 0 : coluna,
                                                                    componente.Regencia));

                        }

                        continue;
                    }
                }

                if (conselhoClasseBimestres.Any())
                    TrataFrequenciaAnual(aluno, notasFinais, frequenciaAlunos, frequenciaAlunosGeral, pareceresConclusivos, linhaDto, turma, qtdeDisciplinasLancamFrequencia, existeFrequenciaRegistradaTurmaAno);
                else
                    TrataFrequenciaAnual(aluno, linhaDto);

                if (conselhoClasseBimestres.Any())
                    linhas.Add(linhaDto);
            }

            return linhas;
        }

        private bool EhComponenteFrequencia(ComponenteCurricularPorTurma componente)
        {
            return componente.CodDisciplina == CODIGO_FREQUENCIA;
        }

        private bool ApresentarFrequencia(ComponenteCurricularPorTurma componente, bool possuiFrequencia)
        {
            return (componente.Frequencia || (componente.Regencia && !possuiFrequencia));
        }

        private List<ConselhoClasseAtaFinalCelulaDto> ObterCelulasFrequencias(
                            long idGrupo,
                            long codDisciplina,
                            int coluna,
                            bool possuiComponente,
                            FrequenciaAluno frequenciaAluno,
                            Turma turma,
                            AlunoSituacaoAtaFinalDto aluno,
                            bool possuiConselhoUltimoBimestreAtivo,
                            bool turmaPossuiFrequenciaRegistrada,
                            bool regencia)
        {
            var celulasFrequencias = new List<ConselhoClasseAtaFinalCelulaDto>();

            celulasFrequencias.Add(new ConselhoClasseAtaFinalCelulaDto()
            {
                GrupoMatriz = idGrupo,
                ComponenteCurricular = codDisciplina,
                Coluna = ++coluna,
                Valor = possuiComponente && (aluno.Ativo || possuiConselhoUltimoBimestreAtivo) ? (frequenciaAluno?.TotalAusencias.ToString() ?? 0.ToString($"N{PERCENTUAL_FREQUENCIA_PRECISAO}", CultureInfo.CurrentCulture)) : string.Empty,
                Regencia = regencia
            });

            celulasFrequencias.Add(new ConselhoClasseAtaFinalCelulaDto()
            {
                GrupoMatriz = idGrupo,
                ComponenteCurricular = codDisciplina,
                Coluna = ++coluna,
                Valor = possuiComponente && (aluno.Ativo || possuiConselhoUltimoBimestreAtivo) ? (frequenciaAluno?.TotalCompensacoes.ToString() ?? 0.ToString($"N{PERCENTUAL_FREQUENCIA_PRECISAO}", CultureInfo.CurrentCulture)) : string.Empty,
                Regencia = regencia
            });

            celulasFrequencias.Add(new ConselhoClasseAtaFinalCelulaDto()
            {
                GrupoMatriz = idGrupo,
                ComponenteCurricular = codDisciplina,
                Coluna = ++coluna,
                Valor = possuiComponente ? ObterFrequencia(frequenciaAluno, turma, aluno, possuiConselhoUltimoBimestreAtivo, turmaPossuiFrequenciaRegistrada) : string.Empty,
                Regencia = regencia
            });

            return celulasFrequencias;
        }

        private List<ConselhoClasseAtaFinalCelulaDto> ObterCelulasFrequencias(
                            long idGrupo,
                            long codDisciplina,
                            int coluna,
                            bool regencia)
        {
            var celulasFrequencias = new List<ConselhoClasseAtaFinalCelulaDto>();
            var celulaFrequencia = new ConselhoClasseAtaFinalCelulaDto()
            {
                GrupoMatriz = idGrupo,
                ComponenteCurricular = codDisciplina,
                Coluna = ++coluna,
                Valor = string.Empty,
                Regencia = regencia
            };
            celulasFrequencias.Add(celulaFrequencia);
            ++coluna;
            celulasFrequencias.Add(celulaFrequencia);
            ++coluna;
            celulasFrequencias.Add(celulaFrequencia);
            return celulasFrequencias;
            return null;
        }

        private string ObterFrequencia(
                            FrequenciaAluno frequencia,
                            Turma turma,
                            AlunoSituacaoAtaFinalDto aluno,
                            bool possuiConselhoUltimoBimestreAtivo,
                            bool turmaPossuiFrequenciaRegistrada)
        {
            if (turma.AnoLetivo.Equals(2020))
            {
                return frequencia == null || frequencia.TotalAusencias == 0
                    ?
                    frequencia100Formatada
                    :
                    frequencia.PercentualFrequenciaFinal.ToString();
            }

            if (frequencia != null && frequencia.TotalAulas != 0 && (aluno.Ativo || possuiConselhoUltimoBimestreAtivo))
            {
                return frequencia.PercentualFrequenciaFormatado;
            }
            else if (frequencia == null && turmaPossuiFrequenciaRegistrada)
            {
                return string.Empty;
            }

            return string.Empty;
        }

        private NotaConceitoBimestreComponente RetornaValorPadraoNotaAtaFinal(IEnumerable<NotaConceitoBimestreComponente> notasFinais, AlunoSituacaoAtaFinalDto aluno,
                                                                                ComponenteCurricularPorTurma componente, int bimestre)
            => notasFinais.FirstOrDefault(c => c.AlunoCodigo == aluno.CodigoAluno.ToString()
                                                            && c.ComponenteCurricularCodigo == componente.CodDisciplina
                                                            && c.Bimestre == bimestre);

        private void TrataFrequenciaAnual(AlunoSituacaoAtaFinalDto aluno, IEnumerable<NotaConceitoBimestreComponente> notasFinais, IEnumerable<FrequenciaAluno> frequenciaAlunos,
            IEnumerable<FrequenciaAluno> frequenciaAlunosGeral, IEnumerable<ConselhoClasseParecerConclusivo> pareceresConclusivos, ConselhoClasseAtaFinalLinhaDto linhaDto,
            Turma turma, int qtdeDisciplinasLancamFrequencia = 0, bool turmaExisteFrequenciaRegistrada = false)
        {
            var frequenciaGlobalAluno = frequenciaAlunosGeral
                .FirstOrDefault(c => c.CodigoAluno == aluno.CodigoAluno.ToString());

            var frequenciasAluno = frequenciaAlunos
                .Where(c => c.CodigoAluno == aluno.CodigoAluno.ToString());

            var possuiConselhoFinalParaAnual = notasFinais
                .Any(n => n.AlunoCodigo == aluno.CodigoAluno.ToString() && n.ConselhoClasseAlunoId != 0 && (!n.Bimestre.HasValue || n.Bimestre.Value == 0));

            var percentualFrequencia2020 = Math.Round((((qtdeDisciplinasLancamFrequencia - frequenciasAluno.Count()) * 100)
                                                + (decimal)frequenciasAluno.Sum(f => f.PercentualFrequenciaFinal)) / qtdeDisciplinasLancamFrequencia, 2);

            string percentualFrequenciaFinal = frequenciaGlobalAluno != null ? frequenciaGlobalAluno.PercentualFrequenciaFormatado
                : ObterPercentualFrequenciaFinal(frequenciasAluno, turmaExisteFrequenciaRegistrada);

            var percentualFrequenciaAcumulado = (turma.AnoLetivo.Equals(2020) ? percentualFrequencia2020.ToString() : percentualFrequenciaFinal);

            linhaDto.AdicionaCelula(99, 99, frequenciasAluno?.Sum(f => f.TotalAusencias).ToString() ?? 0.ToString($"N{PERCENTUAL_FREQUENCIA_PRECISAO}", CultureInfo.CurrentCulture), COLUNA_AUSENCIA, aluno.CodigoAluno.ToString());
            linhaDto.AdicionaCelula(99, 99, frequenciasAluno?.Sum(f => f.TotalCompensacoes).ToString() ?? 0.ToString($"N{PERCENTUAL_FREQUENCIA_PRECISAO}", CultureInfo.CurrentCulture), COLUNA_COMPENSACAO, aluno.CodigoAluno.ToString());
            linhaDto.AdicionaCelula(99, 99, percentualFrequenciaAcumulado ?? string.Empty, COLUNA_PORCENTAGEM_FREQUENCIA,aluno.CodigoAluno.ToString());

            var parecerConclusivo = pareceresConclusivos.FirstOrDefault(c => c.AlunoCodigo == aluno.CodigoAluno.ToString() && c.Bimestre == null);
            var textoParecer = ObterSiglaParecerConclusivo(parecerConclusivo);
            if (textoParecer == null)
            {
                bool ativoOuConcluido = AlunoAtivo(aluno.CodigoSituacaoMatricula);
                textoParecer = !ativoOuConcluido ? string.Concat(VerificaSituacaoAlunoInativo(aluno.CodigoSituacaoMatricula), aluno.DataSituacaoAluno.ToString("dd/MM/yyyy")) : SEM_PARECER_CONCLUSIVO;
            }
            linhaDto.AdicionaCelula(99, 99, textoParecer, COLUNA_PARECER_CONCLUSIVO, aluno.CodigoAluno.ToString());
        }

        private string VerificaSituacaoAlunoInativo(SituacaoMatriculaAluno situacaoMatricula)
        {
            string statusAlunoInativoParecer = string.Empty;
            switch (situacaoMatricula)
            {
                case SituacaoMatriculaAluno.Desistente:
                    statusAlunoInativoParecer = "DES";
                    break;
                case SituacaoMatriculaAluno.Transferido:
                    statusAlunoInativoParecer = "TR";
                    break;
                case SituacaoMatriculaAluno.VinculoIndevido:
                    statusAlunoInativoParecer = "VI";
                    break;
                case SituacaoMatriculaAluno.Falecido:
                    statusAlunoInativoParecer = "FL";
                    break;
                case SituacaoMatriculaAluno.Deslocamento:
                    statusAlunoInativoParecer = "DESL";
                    break; 
                case SituacaoMatriculaAluno.Cessado:
                    statusAlunoInativoParecer = "CES";
                    break;
                case SituacaoMatriculaAluno.RemanejadoSaida:
                    statusAlunoInativoParecer = "REM";
                    break;
                case SituacaoMatriculaAluno.ReclassificadoSaida:
                    statusAlunoInativoParecer = "RC";
                    break;
                default:    
                    statusAlunoInativoParecer = "Inativo";
                    break;
            }

            return $@"{statusAlunoInativoParecer} em ";
        }

        private void TrataFrequenciaAnual(AlunoSituacaoAtaFinalDto aluno, ConselhoClasseAtaFinalLinhaDto linhaDto)
        {
            linhaDto.AdicionaCelula(99, 99, string.Empty, COLUNA_AUSENCIA, aluno.CodigoAluno.ToString());
            linhaDto.AdicionaCelula(99, 99, string.Empty, COLUNA_COMPENSACAO, aluno.CodigoAluno.ToString());
            linhaDto.AdicionaCelula(99, 99, string.Empty, COLUNA_PORCENTAGEM_FREQUENCIA, aluno.CodigoAluno.ToString());
            linhaDto.AdicionaCelula(99, 99, string.Empty, COLUNA_PARECER_CONCLUSIVO, aluno.CodigoAluno.ToString());
        }

        private string ObterSiglaParecerConclusivo(ConselhoClasseParecerConclusivo parecerConclusivo)
        {
            if (parecerConclusivo != null)
                return ParecerConclusivo.ObterSiglaParecer(parecerConclusivo.ParecerConclusivo) ?? parecerConclusivo.ParecerConclusivo;

            return null;
        }

        private string ObterPercentualFrequenciaFinal(IEnumerable<FrequenciaAluno> frequenciasAluno, bool existeFrequenciaRegistradaTurma)
        {
            var totalAulas = frequenciasAluno == null || frequenciasAluno?.Sum(f => f.TotalAulas) == 0 ? 0 : frequenciasAluno.Sum(f => f.TotalAulas);
            var totalFaltasNaoCompensadas = frequenciasAluno == null ? 0 : frequenciasAluno.Sum(f => f.NumeroFaltasNaoCompensadas);

            if (totalAulas == 0)
                return string.Empty;

            var frequenciaFinal = new FrequenciaAluno() 
            {
                TotalAulas = totalAulas,
                TotalAusencias = totalFaltasNaoCompensadas
            };

            var percentualFrequenciaFinal = frequenciaFinal.PercentualFrequenciaFormatado;
            return percentualFrequenciaFinal;
        }

        private bool AlunoAtivo(SituacaoMatriculaAluno situacaoMatricula)
        {
            SituacaoMatriculaAluno[] SituacoesAtiva = new[]
            {
                SituacaoMatriculaAluno.Ativo,
                SituacaoMatriculaAluno.Rematriculado,
                SituacaoMatriculaAluno.PendenteRematricula,
                SituacaoMatriculaAluno.SemContinuidade,
                SituacaoMatriculaAluno.Concluido
            };
            return SituacoesAtiva.Contains(situacaoMatricula);
        }

        private List<ComponenteCurricularPorTurma> ObterComponentesCurriculares(List<ComponenteCurricularPorTurma> componenteCurricularPorTurmas)
        {
            List<ComponenteCurricularPorTurma> componentes = new List<ComponenteCurricularPorTurma>();
            foreach (var componente in componenteCurricularPorTurmas)
            {
                if (componente.Regencia)
                    foreach (var componenteCurricularRegencia in componente.ComponentesCurricularesRegencia)
                    {
                        if (!componentes.Any(a => a.CodDisciplina == componenteCurricularRegencia.CodDisciplina))
                            componentes.Add(new ComponenteCurricularPorTurma()
                            {
                                CodDisciplina = componenteCurricularRegencia.CodDisciplina,
                                CodDisciplinaPai = componente.CodDisciplina,
                                LancaNota = componenteCurricularRegencia.LancaNota,
                                Disciplina = componenteCurricularRegencia.Disciplina,
                                GrupoMatriz = componente.GrupoMatriz,
                                CodigoTurma = componente.CodigoTurma,
                                Regencia = componenteCurricularRegencia.Regencia,
                                Frequencia = componenteCurricularRegencia.Frequencia
                            });
                    }
                else
                    componentes.Add(componente);
            }

            return componentes;
        }

        public string ObterSinteseAluno(double? percentualFrequencia, ComponenteCurricularPorTurma componente, double compensacaoAusenciaPercentualRegenciaClasse, double compensacaoAusenciaPercentualFund2)
        {
            var parametro = componente.Regencia || !componente.LancaNota ? compensacaoAusenciaPercentualRegenciaClasse : compensacaoAusenciaPercentualFund2;
            return percentualFrequencia >= parametro ? "F" : "NF";
        }

        private FrequenciaAluno ObterFrequenciaAluno(IEnumerable<FrequenciaAluno> frequenciaAlunos, string alunoCodigo, ComponenteCurricularPorTurma componenteCurricular, List<ComponenteCurricularPorTurma> componentesTurmas)
        {
            var componenteFrequencia = componenteCurricular.Regencia ? ObterComponenteRegenciaTurma(componentesTurmas) : componenteCurricular;

            var codComponenteCurricular = componenteCurricular.Regencia ? componenteFrequencia.CodDisciplinaPai : componenteFrequencia.CodDisciplina;

            return frequenciaAlunos.FirstOrDefault(c => c.CodigoAluno == alunoCodigo
                                                && c.DisciplinaId == codComponenteCurricular.ToString());

        }

        private void MontarEstruturaGruposMatriz(ConselhoClasseAtaFinalDto relatorio, IEnumerable<IGrouping<ComponenteCurricularGrupoMatriz, ComponenteCurricularPorTurma>> gruposMatrizes, IEnumerable<PeriodoEscolar> periodosEscolares, IEnumerable<AreaDoConhecimento> areasDoConhecimento, IEnumerable<ComponenteCurricularGrupoAreaOrdenacaoDto> ordenacaoGrupoArea)
        {
            var bimestres = periodosEscolares.OrderBy(p => p.Bimestre).Select(a => a.Bimestre);
            if (gruposMatrizes != null)
            {
                foreach (var grupoMatriz in gruposMatrizes.OrderBy(gm => gm.Key.Id))
                {
                    var grupoMatrizDto = new ConselhoClasseAtaFinalGrupoDto()
                    {
                        Id = grupoMatriz.Key.Id,
                        Nome = grupoMatriz.Key.Nome
                    };

                    var componentes = ObterComponentesCurriculares(grupoMatriz.GroupBy(c => c.CodDisciplina).Select(x => x.FirstOrDefault()).ToList());

                    foreach (var componenteCurricular in componentes)
                    {
                        if (!grupoMatrizDto.ComponentesCurriculares.Any(a => a.Id == componenteCurricular.CodDisciplina))
                            grupoMatrizDto.AdicionarComponente(componenteCurricular.CodDisciplina, componenteCurricular.CodigoComponenteCurricularTerritorioSaber, componenteCurricular.Disciplina, grupoMatrizDto.Id, bimestres, componenteCurricular.Regencia, componenteCurricular.LancaNota, componenteCurricular.Frequencia);
                    }

                    relatorio.GruposMatriz.Add(grupoMatrizDto);
                }

                foreach (var grupoMatriz in relatorio.GruposMatriz)
                {
                    var componentesDoGrupo = new List<ConselhoClasseAtaFinalComponenteDto>();
                    var areasConhecimento = MapearAreasDoConhecimento(grupoMatriz.ComponentesCurriculares, areasDoConhecimento, ordenacaoGrupoArea, grupoMatriz.Id);

                    foreach (var area in areasConhecimento)
                    {
                        componentesDoGrupo.AddRange(ObterComponentesDasAreasDeConhecimento(grupoMatriz.ComponentesCurriculares, area));
                    }

                    grupoMatriz.ComponentesCurriculares = componentesDoGrupo.OrderBy(componente => !componente.Regencia)
                                                                                                            .ThenBy(componente => !componente.LancaNota)
                                                                                                            .ThenBy(componente => componente.Id == CODIGO_FREQUENCIA)
                                                                                                            .ThenBy(componente => componente.Nome).ToList();

                }
            }
        }

        private IEnumerable<ConselhoClasseAtaFinalComponenteDto> ObterComponentesDasAreasDeConhecimento(IEnumerable<ConselhoClasseAtaFinalComponenteDto> componentesCurricularesDaTurma,
                                                                                               IEnumerable<AreaDoConhecimento> areaDoConhecimento)
        {
            return componentesCurricularesDaTurma.Where(c => areaDoConhecimento.Select(a => a.CodigoComponenteCurricular).Any(a => a == c.Id || a == c.IdComponenteCurricularTerritorio)).OrderBy(cc => cc.Nome);
        }

        private IEnumerable<IGrouping<(string Nome, int? Ordem, long Id), AreaDoConhecimento>> MapearAreasDoConhecimento(IEnumerable<ConselhoClasseAtaFinalComponenteDto> componentesCurricularesDaTurma,
                                                                                                           IEnumerable<AreaDoConhecimento> areasDoConhecimentos,
                                                                                                           IEnumerable<ComponenteCurricularGrupoAreaOrdenacaoDto> grupoAreaOrdenacao,
                                                                                                           long grupoMatrizId)
        {

            return areasDoConhecimentos.Where(a => ((componentesCurricularesDaTurma.Select(cc => new { cc.Id, cc.IdComponenteCurricularTerritorio })
                                                                    .Any(cc => cc.Id == a.CodigoComponenteCurricular || cc.IdComponenteCurricularTerritorio == a.CodigoComponenteCurricular))))
                                                                     .Select(a => { a.DefinirOrdem(grupoAreaOrdenacao, grupoMatrizId); return a; }).GroupBy(g => (g.Nome, g.Ordem, g.Id))
                                                                     .OrderByDescending(c => c.Key.Id > 0 && !string.IsNullOrEmpty(c.Key.Nome))
                                                                     .ThenByDescending(c => c.Key.Ordem.HasValue).ThenBy(c => c.Key.Ordem)
                                                                     .ThenBy(c => c.Key.Nome, StringComparer.OrdinalIgnoreCase);
        }

        private ComponenteCurricularPorTurma ObterComponenteRegenciaTurma(IEnumerable<ComponenteCurricularPorTurma> componentesTurma)
        {
            if (componenteRegencia == null)
            {
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

        private async Task<Turma> ObterTurmaDetalhes(string turmaCodigo)
        {
            var turmas = await mediator.Send(new ObterTurmasDetalhePorCodigoQuery(new long[] { Convert.ToInt64(turmaCodigo) }));
            return turmas.FirstOrDefault();
        }

        private async Task<Turma> ObterTurma(string turmaCodigo)
            => await mediator.Send(new ObterTurmaQuery(turmaCodigo));
        private async Task<IEnumerable<FrequenciaAluno>> ObterFrequenciaComponente(string[] turmasCodigo, IEnumerable<(string CodigoTurma, long ComponenteCurricularId)> componentesCurricularesPorTurma, int[] bimestres, long tipoCalendarioId, IEnumerable<(string codigoAluno, DateTime dataMatricula, DateTime? dataSituacao)> alunosDatasMatriculas)
            => await mediator.Send(new ObterFrequenciaComponenteGlobalPorTurmaQuery(turmasCodigo, componentesCurricularesPorTurma, bimestres, tipoCalendarioId, alunosDatasMatriculas));

        private async Task<IEnumerable<FrequenciaAluno>> ObterFrequenciaGeral(int anoTurma, long tipoCalendarioId)
            => await mediator.Send(new ObterFrequenciasGeralAlunosNaTurmaQuery(anoTurma, tipoCalendarioId));

        private async Task<IEnumerable<FrequenciaAluno>> ObterFrequenciaGeralPorAlunos(int anoletivo, string[] codigoTurma, long tipoCalendarioId, string[] alunosCodigos)
            => await mediator.Send(new ObterFrequenciasGeralPorAnoEAlunosQuery(anoletivo, codigoTurma, tipoCalendarioId, alunosCodigos));

        private async Task<IEnumerable<AlunoSituacaoAtaFinalDto>> ObterAlunos(string turmaCodigo, IEnumerable<PeriodoEscolar> periodos)
        {
            const int PRIMEIRO_BIMESTRE = 1;
            var alunos = await mediator.Send(new ObterAlunosSituacaoPorTurmaQuery(turmaCodigo));
            var alunoSituacao = alunos.Select(a => new AlunoSituacaoAtaFinalDto(a));
            var periodoEscolar = periodos.FirstOrDefault(periodo => periodo.Bimestre == PRIMEIRO_BIMESTRE);

            if (periodoEscolar != null)
                return alunoSituacao.Where(aluno => aluno.Ativo || (aluno.Inativo && aluno.DataSituacaoAluno >= periodoEscolar.PeriodoInicio));

            return alunoSituacao;
        }

        private int CalcularPaginasHorizontal(int maximoComponentesPorPagina, int contagemTodasDisciplinas)
        {
            return (int)(Math.Ceiling(contagemTodasDisciplinas / (decimal)maximoComponentesPorPagina));
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
        private async Task<IEnumerable<IGrouping<string, NotasAlunoBimestre>>> ObterNotasAlunos(string[] alunosCodigo, string[] codigosTurmas, int anoLetivo, Modalidade modalidade, int semestre, int[] tiposTurma)
        {
            return await mediator.Send(new ObterNotasRelatorioAtaFinalQuery(alunosCodigo, codigosTurmas, anoLetivo, (int)modalidade, semestre, tiposTurma));
        }

        private async Task<IEnumerable<IGrouping<string, ComponenteCurricularPorTurma>>> ObterComponentesCurricularesTurmasRelatorio(string[] turmaCodigo, string codigoUe, Modalidade modalidade)
            => await mediator.Send(new ObterComponentesCurricularesTurmasRelatorioAtaFinalResultadosQuery(turmaCodigo, codigoUe, modalidade));

        private async Task<string[]> ObterCodigoDeTurmas(Turma turma, IEnumerable<AlunoSituacaoAtaFinalDto> alunos)
        {
            if (turma.EhEja && turma.TipoTurma == TipoTurma.EdFisica)
            {
                var codigoAlunos = alunos.Select(x => x.CodigoAluno.ToString()).ToArray();

                return (await mediator.Send(new ObterTurmaCodigosAlunoPorAnoLetivoAlunoTipoTurmaQuery(turma.AnoLetivo, codigoAlunos,
                                    turma.ObterTiposRegularesDiferentes(), turma.AnoLetivo < DateTimeExtension.HorarioBrasilia().Year, DateTimeExtension.HorarioBrasilia()))).Select(x => x.ToString()).ToArray();
            }

            return new string[] { turma.Codigo };
        }
    }
}
