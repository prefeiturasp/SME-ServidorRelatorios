using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterRelatorioConselhoClasseAtaBimestralPdfQueryHandler : IRequestHandler<ObterRelatorioConselhoClasseAtaBimestralPdfQuery, List<ConselhoClasseAtaBimestralPaginaDto>>
    {        
        private const double FREQUENCIA_100 = 100;
        private const int PERCENTUAL_FREQUENCIA_PRECISAO = 2;
        private readonly VariaveisAmbiente variaveisAmbiente;
        private readonly IMediator mediator;
        private ComponenteCurricularPorTurma componenteRegencia;

        private string frequencia100Formatada = FREQUENCIA_100.ToString($"N{PERCENTUAL_FREQUENCIA_PRECISAO}", CultureInfo.CurrentCulture);

        public ObterRelatorioConselhoClasseAtaBimestralPdfQueryHandler(IMediator mediator, VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<List<ConselhoClasseAtaBimestralPaginaDto>> Handle(ObterRelatorioConselhoClasseAtaBimestralPdfQuery request, CancellationToken cancellationToken)
        {
            var mensagensErro = new List<string>();
            var relatoriosTurmas = new List<ConselhoClasseAtaBimestralPaginaDto>();
            var turmas = await mediator.Send(new ObterTurmasPorCodigoQuery(request.Filtro.TurmasCodigo.ToArray()));

            turmas.AsParallel().WithDegreeOfParallelism(variaveisAmbiente.ProcessamentoMaximoTurmas).ForAll(turma =>
            {
                try
                {
                    var retorno = ObterRelatorioTurma(turma, request.Filtro).Result;
                    if (retorno != null && retorno.Any())
                        relatoriosTurmas.AddRange(retorno);
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

        private async Task<IEnumerable<ConselhoClasseAtaBimestralPaginaDto>> ObterRelatorioTurma(Turma turma, FiltroConselhoClasseAtaBimestralDto filtro)
        {
            var alunos = await ObterAlunos(turma.Codigo);
            var alunosCodigos = alunos.Select(x => x.CodigoAluno.ToString()).ToArray();
            var tiposTurma = new List<int>() { (int)turma.TipoTurma };

            if (turma.TipoTurma == TipoTurma.Regular)
                tiposTurma.Add((int)TipoTurma.EdFisica);
            else
                tiposTurma.Add((int)TipoTurma.Regular);

            var notas = await ObterNotasAlunos(alunosCodigos, turma.AnoLetivo, turma.ModalidadeCodigo, turma.Semestre, tiposTurma.ToArray(), filtro.Bimestre);

            notas = notas.Where(n => n.Key.Equals(turma.Codigo));

            if (notas == null || !notas.Any())
                return Enumerable.Empty<ConselhoClasseAtaBimestralPaginaDto>();
            var tipoCalendarioId = await ObterIdTipoCalendario(turma.ModalidadeTipoCalendario, turma.AnoLetivo, turma.Semestre);
            var periodosEscolares = await ObterPeriodosEscolares(tipoCalendarioId);
            var cabecalho = await ObterCabecalho(turma.Codigo);

            var alunosComRegistroFrequencia = await ObterAlunosComRegistroFrequencia(turma.Codigo, filtro.Bimestre);

            cabecalho.Bimestre = filtro.Bimestre.ToString();
            cabecalho.Usuario = filtro.UsuarioNome;
            cabecalho.RF = filtro.UsuarioRF;

            var turmas = await ObterTurmasPorCodigo(notas.Select(n => n.Key).ToArray());
            var listaTurmas = ObterCodigosTurmaParaListagem(turma.TipoTurma, turma.Codigo, turmas);
            var bimestreFiltro = new int[] { filtro.Bimestre };
            var componentesCurriculares = await ObterComponentesCurricularesTurmasRelatorio(listaTurmas.ToArray(), turma.Ue.Codigo, turma.ModalidadeCodigo, bimestreFiltro);
            var frequenciaAlunosGeral = await ObterFrequenciaGeralPorAlunos(turma.AnoLetivo, turma.Codigo, tipoCalendarioId, alunosCodigos);

            var listaAlunos = await mediator.Send(new ObterDadosAlunosPorCodigosQuery(alunosCodigos.Select(long.Parse).ToArray(), filtro.AnoLetivo));
            listaAlunos = listaAlunos.Where(x => x.AnoLetivo == filtro.AnoLetivo);

            var listaTurmasAlunos = listaAlunos.GroupBy(x => x.CodigoTurma);

            listaTurmasAlunos = listaTurmasAlunos.Where(t => listaTurmas.Any(lt => lt == t.Key.ToString()));

            var pareceresConclusivos = new List<ConselhoClasseParecerConclusivo>();

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

            var componentesDaTurma = componentesCurriculares.SelectMany(cc => cc).ToList();
            var componentesCurricularesPorTurma = ObterComponentesCurricularesTurma(componentesDaTurma);

            var bimestres = periodosEscolares.Select(p => p.Bimestre).ToArray();

            var frequenciaBimestre = await mediator.Send(new ObterFrequenciaAlunoPorComponentesBimestresETurmasQuery(listaTurmas.ToArray(), new int[] { filtro.Bimestre }, componentesCurricularesPorTurma.Select(a => a.Item2)));

            var areasDoConhecimento = await ObterAreasConhecimento(componentesCurriculares);

            var ordenacaoGrupoArea = await ObterOrdenacaoAreasConhecimento(componentesCurriculares, areasDoConhecimento);

            var notasFinais = new List<NotaConceitoBimestreComponente>();

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
                    Sintese = nf.NotaConceito.Sintese,
                    ConselhoClasseAlunoId = nf.ConselhoClasseAlunoId,
                    NotaId = nf.NotaConceito.NotaId
                }));
            }

            var conselhosDeClasseAlunosId = notasFinais.Select(a => a.ConselhoClasseAlunoId).Distinct().ToArray();

            var anotacoesPedagogicaDosAlunos = await mediator.Send(new ObterAnotacoesPedagogicasPorConselhoClasseAlunoIdsQuery(conselhosDeClasseAlunosId));

            var dadosRelatorio = await MontarEstruturaRelatorio(turma,
                                                                cabecalho,
                                                                alunos,
                                                                componentesDaTurma,
                                                                notasFinais,
                                                                frequenciaBimestre,
                                                                frequenciaAlunosGeral,
                                                                pareceresConclusivos,
                                                                periodosEscolares,
                                                                listaTurmasAlunos,
                                                                areasDoConhecimento,
                                                                ordenacaoGrupoArea,
                                                                filtro.Bimestre,
                                                                anotacoesPedagogicaDosAlunos,
                                                                alunosComRegistroFrequencia);

            return MontarEstruturaPaginada(dadosRelatorio);
        }

        private async Task<IEnumerable<string>> ObterAlunosComRegistroFrequencia(string codigoTurma, int bimestre)
            => await mediator.Send(new ObterAlunosComFrequenciaPorTurmaBimestreQuery(codigoTurma, bimestre));

        private async Task<List<Turma>> ObterTurmasPorCodigo(string[] codigos)
        {
            var turmas = new List<Turma>();

            foreach (var codigo in codigos)
                turmas.Add(await ObterTurma(codigo));

            return turmas;
        }

        private string[] ObterCodigosTurmaParaListagem(TipoTurma tipoTurma, string codigo, List<Turma> turmas)
        {
            if (tipoTurma == TipoTurma.Itinerarios2AAno)
                return new string[] { codigo };

            var codigos = new List<string>();
            codigos.Add(codigo);

            foreach (var turma in turmas)
            {
                if (turma.TipoTurma != TipoTurma.Regular)
                    codigos.Add(turma.Codigo);
            }
            return codigos.ToArray();
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
            listaCodigosComponentes.AddRange(componentesCurriculares.SelectMany(a => a).Where(cc => !cc.Regencia).Select(a => a.CodDisciplina).ToList());

            return await mediator.Send(new ObterAreasConhecimentoComponenteCurricularQuery(listaCodigosComponentes.Distinct().ToArray()));
        }

        private async Task<IEnumerable<ComponenteCurricularGrupoAreaOrdenacaoDto>> ObterOrdenacaoAreasConhecimento(IEnumerable<IGrouping<string, ComponenteCurricularPorTurma>> componentesCurriculares, IEnumerable<AreaDoConhecimento> areasDoConhecimento)
        {
            var listaGrupoMatrizId = componentesCurriculares?.SelectMany(a => a)?.Select(a => a.GrupoMatriz.Id)?.Distinct().ToArray();
            var listaAreaConhecimentoId = areasDoConhecimento?.Select(a => a.Id).ToArray();

            return await mediator.Send(new ObterComponenteCurricularGrupoAreaOrdenacaoQuery(listaGrupoMatrizId, listaAreaConhecimentoId));
        }

        private List<ConselhoClasseAtaBimestralPaginaDto> MontarEstruturaPaginada(ConselhoClasseAtaBimestralDto dadosRelatorio)
        {
            var maximoComponentesPorPagina = 10;
            var maximoComponentesPorPaginaFinal = 3;
            var quantidadeDeLinhasPorPagina = 20;

            var modelsPaginas = new List<ConselhoClasseAtaBimestralPaginaDto>();

            var todasAsDisciplinas = dadosRelatorio.GruposMatriz.SelectMany(x => x.ComponentesCurriculares).ToList();

            int quantidadePaginasHorizontal = CalcularPaginasHorizontal(maximoComponentesPorPagina, maximoComponentesPorPaginaFinal, todasAsDisciplinas.Count());

            int quantidadePaginasVertical = (int)Math.Ceiling(dadosRelatorio.Linhas.Count / (decimal)quantidadeDeLinhasPorPagina);

            int contPagina = 1;

            for (int v = 0; v < quantidadePaginasVertical; v++)
            {
                for (int h = 0; h < quantidadePaginasHorizontal; h++)
                {
                    bool ehPaginaFinal = (h + 1) == quantidadePaginasHorizontal;
                    if (ehPaginaFinal)
                        continue;

                    int quantidadeDisciplinasDestaPagina = ehPaginaFinal ? maximoComponentesPorPaginaFinal : maximoComponentesPorPagina;

                    ConselhoClasseAtaBimestralPaginaDto modelPagina = new ConselhoClasseAtaBimestralPaginaDto
                    {
                        Modalidade = dadosRelatorio.Modalidade,
                        Cabecalho = dadosRelatorio.Cabecalho,
                        NumeroPagina = contPagina++,
                        FinalHorizontal = ehPaginaFinal,
                        TotalPaginas = (quantidadePaginasHorizontal * quantidadePaginasVertical) - quantidadePaginasVertical
                    };

                    if (todasAsDisciplinas.Any())
                    {
                        IEnumerable<ConselhoClasseAtaBimestralComponenteDto> disciplinasDestaPagina = todasAsDisciplinas.Skip(h * maximoComponentesPorPagina).Take(quantidadeDisciplinasDestaPagina);

                        foreach (ConselhoClasseAtaBimestralComponenteDto disciplina in disciplinasDestaPagina)
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

                    List<ConselhoClasseAtaBimestralLinhaDto> linhas = dadosRelatorio.Linhas.Skip((v) * quantidadeDeLinhasPorPagina).Take(quantidadeDeLinhasPorPagina)
                        .Select(x => new ConselhoClasseAtaBimestralLinhaDto
                        {
                            Id = x.Id,
                            Nome = x.Nome,
                            Inativo = x.Inativo,
                            Situacao = x.Situacao,
                            Celulas = x.Celulas,
                            ConselhoClasse = x.ConselhoClasse
                        }).ToList();

                    foreach (ConselhoClasseAtaBimestralLinhaDto linha in linhas)
                    {
                        List<ConselhoClasseAtaBimestralCelulaDto> todasAsCelulas = linha.Celulas;

                        linha.Celulas = todasAsCelulas.Where(x => gruposMatrizDestaPagina.Contains(x.GrupoMatriz) && idsDisciplinasDestaPagina.Contains(x.ComponenteCurricular)).Select(x => new ConselhoClasseAtaBimestralCelulaDto { GrupoMatriz = x.GrupoMatriz, ComponenteCurricular = x.ComponenteCurricular, Coluna = x.Coluna, Valor = x.Valor }).ToList();

                        if (ehPaginaFinal)
                        {
                            IEnumerable<ConselhoClasseAtaBimestralCelulaDto> celulasFinais = todasAsCelulas.Where(x => x.GrupoMatriz == 99);

                            linha.Celulas.AddRange(celulasFinais);
                        }
                    }

                    modelPagina.Linhas.AddRange(linhas);

                    foreach (var grupoMatriz in modelPagina.GruposMatriz)                    
                        grupoMatriz.QuantidadeColunas = modelPagina.Linhas.First().Celulas.Where(x => x.GrupoMatriz == grupoMatriz.Id).Count();                    

                    modelsPaginas.Add(modelPagina);
                }
            }

            return modelsPaginas;
        }

        private async Task<ConselhoClasseAtaBimestralDto> MontarEstruturaRelatorio(Turma turma, ConselhoClasseAtaBimestralCabecalhoDto cabecalho, IEnumerable<AlunoSituacaoAtaFinalDto> alunos,
            IEnumerable<ComponenteCurricularPorTurma> componentesCurriculares, IEnumerable<NotaConceitoBimestreComponente> notasFinais, IEnumerable<FrequenciaAluno> frequenciaAlunos,
            IEnumerable<FrequenciaAluno> frequenciaAlunosGeral, IEnumerable<ConselhoClasseParecerConclusivo> pareceresConclusivos, IEnumerable<PeriodoEscolar> periodosEscolares,
            IEnumerable<IGrouping<int, AlunoHistoricoEscolar>> listaTurmasAlunos, IEnumerable<AreaDoConhecimento> areasDoConhecimento, IEnumerable<ComponenteCurricularGrupoAreaOrdenacaoDto> ordenacaoGrupoArea,
            int bimestre, IEnumerable<AnotacoesPedagogicasAlunoIdsQueryDto> anotacoes, IEnumerable<string> alunosComRegistroFrequencia)
        {
            var relatorio = new ConselhoClasseAtaBimestralDto()
            {
                Modalidade = turma.ModalidadeCodigo,
                Cabecalho = cabecalho
            };

            componentesCurriculares = componentesCurriculares.OrderBy(a => a.Disciplina).ToList();
            var gruposMatrizes = componentesCurriculares.Distinct().Where(c => c.GrupoMatriz != null).GroupBy(c => c.GrupoMatriz).ToList();

            MontarEstruturaGruposMatriz(relatorio, gruposMatrizes, periodosEscolares, areasDoConhecimento, ordenacaoGrupoArea);
            await MontarEstruturaLinhas(relatorio,
                                        alunos,
                                        gruposMatrizes,
                                        notasFinais,
                                        frequenciaAlunos,
                                        frequenciaAlunosGeral,
                                        pareceresConclusivos,
                                        periodosEscolares,
                                        turma,
                                        listaTurmasAlunos,
                                        bimestre,
                                        anotacoes,
                                        componentesCurriculares.Count(cc => cc.Frequencia),
                                        alunosComRegistroFrequencia);
            return relatorio;
        }

        private async Task MontarEstruturaLinhas(ConselhoClasseAtaBimestralDto relatorio, IEnumerable<AlunoSituacaoAtaFinalDto> alunos, IEnumerable<IGrouping<ComponenteCurricularGrupoMatriz,
            ComponenteCurricularPorTurma>> gruposMatrizes, IEnumerable<NotaConceitoBimestreComponente> notasFinais, IEnumerable<FrequenciaAluno> frequenciaAlunos, IEnumerable<FrequenciaAluno> frequenciaAlunosGeral,
            IEnumerable<ConselhoClasseParecerConclusivo> pareceresConclusivos, IEnumerable<PeriodoEscolar> periodosEscolares,
            Turma turma, IEnumerable<IGrouping<int, AlunoHistoricoEscolar>> listaTurmasAlunos, int bimestre, IEnumerable<AnotacoesPedagogicasAlunoIdsQueryDto> anotacoes, int qtdeDisciplinasLancamFrequencia = 0, IEnumerable<string> alunosComRegistroFrequencia = null)
        {
            var compensacaoAusenciaPercentualRegenciaClasse = double.Parse(await mediator.Send(new ObterParametroSistemaPorTipoQuery()
            {
                TipoParametro = TipoParametroSistema.CompensacaoAusenciaPercentualRegenciaClasse
            }));

            var compensacaoAusenciaPercentualFund2 = double.Parse(await mediator.Send(new ObterParametroSistemaPorTipoQuery()
            {
                TipoParametro = TipoParametroSistema.CompensacaoAusenciaPercentualFund2
            }));

            var periodoEscolar = periodosEscolares.FirstOrDefault(a => a.Bimestre == bimestre);

            // Primmeiro alunos Ativos
            var alunosAtivos = ObterAlunosAtivos(alunos, periodoEscolar);
            var alunosComNumeroChamada = await MontarLinhaAluno(alunosAtivos, true,
                gruposMatrizes, notasFinais, frequenciaAlunos, frequenciaAlunosGeral, pareceresConclusivos, periodosEscolares, turma, listaTurmasAlunos, bimestre, anotacoes, qtdeDisciplinasLancamFrequencia,
                compensacaoAusenciaPercentualRegenciaClasse, compensacaoAusenciaPercentualFund2, alunosComRegistroFrequencia);

            relatorio.Linhas.AddRange(alunosComNumeroChamada);

            // Depois alunos Inativos
            var alunosInativos = ObterAlunosInativos(alunos, periodoEscolar);
            var alunosSemNumeroChamada = await MontarLinhaAluno(alunosInativos, false,
                gruposMatrizes, notasFinais, frequenciaAlunos, frequenciaAlunosGeral, pareceresConclusivos, periodosEscolares, turma, listaTurmasAlunos, bimestre, anotacoes, qtdeDisciplinasLancamFrequencia,
                compensacaoAusenciaPercentualRegenciaClasse, compensacaoAusenciaPercentualFund2, alunosComRegistroFrequencia);

            relatorio.Linhas.AddRange(alunosSemNumeroChamada);
        }

        private IEnumerable<AlunoSituacaoAtaFinalDto> ObterAlunosInativos(IEnumerable<AlunoSituacaoAtaFinalDto> alunos, PeriodoEscolar periodoEscolar)
            => alunos
            .Where(a => a.Inativo && a.DataSituacaoAluno.Date < periodoEscolar.PeriodoFim)
            .Select(a => new AlunoSituacaoAtaFinalDto(a))
            .OrderBy(a => a.NumeroAlunoChamada);

        private IEnumerable<AlunoSituacaoAtaFinalDto> ObterAlunosAtivos(IEnumerable<AlunoSituacaoAtaFinalDto> alunos, PeriodoEscolar periodoEscolar)
            => alunos
            .Where(a => ((a.Ativo) && a.DataMatricula >= periodoEscolar.PeriodoInicio) || ((a.Ativo) && a.DataMatricula <= periodoEscolar.PeriodoFim))
            .Select(a => new AlunoSituacaoAtaFinalDto(a))
            .OrderBy(a => a.NumeroAlunoChamada);

        private async Task<List<ConselhoClasseAtaBimestralLinhaDto>> MontarLinhaAluno(IEnumerable<AlunoSituacaoAtaFinalDto> alunos, bool ativos, IEnumerable<IGrouping<ComponenteCurricularGrupoMatriz,
            ComponenteCurricularPorTurma>> gruposMatrizes, IEnumerable<NotaConceitoBimestreComponente> notasFinais, IEnumerable<FrequenciaAluno> frequenciaAlunos,
            IEnumerable<FrequenciaAluno> frequenciaAlunosGeral, IEnumerable<ConselhoClasseParecerConclusivo> pareceresConclusivos, IEnumerable<PeriodoEscolar> periodosEscolares, Turma turma,
            IEnumerable<IGrouping<int, AlunoHistoricoEscolar>> listaTurmasAlunos, int bimestre, IEnumerable<AnotacoesPedagogicasAlunoIdsQueryDto> anotacoes, int qtdeDisciplinasLancamFrequencia = 0, double compensacaoAusenciaPercentualRegenciaClasse = 0,
            double compensacaoAusenciaPercentualFund2 = 0, IEnumerable<string> alunosComRegistroFrequencia = null)
        {
            var linhas = new List<ConselhoClasseAtaBimestralLinhaDto>();
            for (var i = 0; i < alunos.Count(); i++)
            {
                var aluno = alunos.ElementAt(i);
                var linhaDto = new ConselhoClasseAtaBimestralLinhaDto()
                {
                    Id = long.Parse(aluno.NumeroAlunoChamada ?? "0"),
                    Nome = aluno.NomeAluno,
                    Situacao = aluno.SituacaoMatricula,
                    Inativo = aluno.Inativo
                };

                bool possuiComponente = true;
                int componentesCurricularesTotal = 0;
                List<(long codigoAluno, long codigoDisciplina, bool possuiConselho)> alunoComponenteConselhoClasse = new List<(long codigoAluno, long codigoDisciplina, bool possuiConselho)>();

                foreach (var grupoMatriz in gruposMatrizes)
                {
                    var componentes = ObterComponentesCurriculares(grupoMatriz.GroupBy(c => c.CodDisciplina).Select(x => x.FirstOrDefault()).ToList());
                    var componentesTurmas = ObterComponentesCurriculares(grupoMatriz.ToList());

                    componentesCurricularesTotal += componentesTurmas.Where(c => c.LancaNota).Select(a => a.CodDisciplina).Distinct().Count();

                    foreach (var componente in componentes.OrderBy(c => c.Disciplina))
                    {
                        var coluna = 0;

                        if (listaTurmasAlunos != null)
                        {
                            var turmasDoAluno = listaTurmasAlunos.SelectMany(a => a.Where(b => b.CodigoAluno == aluno.CodigoAluno)).Select(a => a.CodigoTurma).Distinct().ToArray();
                            var componentesDoAluno = componentesTurmas.Where(a => a.CodigoTurma != null && turmasDoAluno.Contains(int.Parse(a.CodigoTurma)) && a.CodDisciplina == componente.CodDisciplina).ToList();
                            possuiComponente = componentesDoAluno.Any();
                        }

                        // Monta Colunas notComponenteCurricularRepositoryas dos bimestres
                        var ultimoBimestreAtivo = aluno.Inativo ?
                            periodosEscolares.FirstOrDefault(p => p.PeriodoInicio <= aluno.DataSituacaoAluno && p.PeriodoFim >= aluno.DataSituacaoAluno)?.Bimestre : 4;

                        if (ultimoBimestreAtivo == null)
                            possuiComponente = false;

                        var turmaPossuiFrequenciaRegistrada = await mediator
                            .Send(new ExisteFrequenciaRegistradaPorTurmaComponenteCurricularEAnoQuery(turma.Codigo, componente.CodDisciplina.ToString(), turma.AnoLetivo));

                        var matriculadoDepois = aluno.Ativo ? periodosEscolares.FirstOrDefault(p => aluno.DataMatricula > p.PeriodoFim)?.Bimestre : null;
                        var possuiConselho = notasFinais.Any(n => n.Bimestre == bimestre
                        && n.AlunoCodigo == aluno.CodigoAluno.ToString() && n.ConselhoClasseAlunoId != 0 && n.ComponenteCurricularCodigo == componente.CodDisciplina);

                        if (matriculadoDepois != null && bimestre < matriculadoDepois)
                        {
                            linhaDto.AdicionaCelula(grupoMatriz.Key.Id, componente.CodDisciplina, "-", ++coluna);
                            linhaDto.AdicionaCelula(grupoMatriz.Key.Id, componente.CodDisciplina, "-", ++coluna);
                            linhaDto.AdicionaCelula(grupoMatriz.Key.Id, componente.CodDisciplina, "-", ++coluna);
                            linhaDto.AdicionaCelula(grupoMatriz.Key.Id, componente.CodDisciplina, "-", ++coluna);
                            continue;
                        }

                        if (bimestre > ultimoBimestreAtivo)
                        {
                            linhaDto.AdicionaCelula(grupoMatriz.Key.Id, componente.CodDisciplina, "-", ++coluna);
                            linhaDto.AdicionaCelula(grupoMatriz.Key.Id, componente.CodDisciplina, "-", ++coluna);
                            linhaDto.AdicionaCelula(grupoMatriz.Key.Id, componente.CodDisciplina, "-", ++coluna);
                            linhaDto.AdicionaCelula(grupoMatriz.Key.Id, componente.CodDisciplina, "-", ++coluna);
                            continue;
                        }

                        if (componente.LancaNota)
                            alunoComponenteConselhoClasse.Add((aluno.CodigoAluno, componente.CodDisciplina, possuiConselho));

                        var possuiConselhoParaExibirFrequencias = notasFinais.Any(n => n.AlunoCodigo == aluno.CodigoAluno.ToString() &&
                                                                                  n.ConselhoClasseAlunoId != 0 &&
                                                                                  n.ComponenteCurricularCodigo == componente.CodDisciplina);

                        var frequenciaAluno = ObterFrequenciaAluno(frequenciaAlunos, aluno.CodigoAluno.ToString(), componente, componentesTurmas);

                        var possuiFrequencia = alunosComRegistroFrequencia.Any(a => a == aluno.CodigoAluno.ToString());

                        if ((possuiConselhoParaExibirFrequencias || !componente.LancaNota) && frequenciaAluno != null)
                        {
                            linhaDto.AdicionaCelula(grupoMatriz.Key.Id, componente.CodDisciplina, frequenciaAluno.TotalAusencias.ToString(), ++coluna);
                            linhaDto.AdicionaCelula(grupoMatriz.Key.Id, componente.CodDisciplina, frequenciaAluno.TotalCompensacoes.ToString(), ++coluna);
                            linhaDto.AdicionaCelula(grupoMatriz.Key.Id, componente.CodDisciplina, frequenciaAluno.PercentualFrequenciaFormatado, ++coluna);
                        }
                        else
                        {
                            linhaDto.AdicionaCelula(grupoMatriz.Key.Id, componente.CodDisciplina, possuiFrequencia && componente.Frequencia ? 0.ToString($"N{PERCENTUAL_FREQUENCIA_PRECISAO}", CultureInfo.CurrentCulture) : "", ++coluna);
                            linhaDto.AdicionaCelula(grupoMatriz.Key.Id, componente.CodDisciplina, possuiFrequencia && componente.Frequencia ? 0.ToString($"N{PERCENTUAL_FREQUENCIA_PRECISAO}", CultureInfo.CurrentCulture) : "", ++coluna);
                            linhaDto.AdicionaCelula(grupoMatriz.Key.Id, componente.CodDisciplina, possuiFrequencia && componente.Frequencia ? frequencia100Formatada : "", ++coluna);
                        }

                        var notaConceito = notasFinais.OrderByDescending(n => n.NotaId).FirstOrDefault(c => c.AlunoCodigo == aluno.CodigoAluno.ToString()
                                                && c.ComponenteCurricularCodigo == componente.CodDisciplina
                                                && c.Bimestre == bimestre);

                        if (turma.RegularCodigo != null && !notasFinais.All(nf => nf.Nota.HasValue) && decimal.TryParse(notaConceito?.NotaConceito, out decimal valor))
                        {
                            var valorConvertido = ConverterNotaParaConceito(decimal.Parse(notaConceito.NotaConceito, CultureInfo.InvariantCulture));
                            notaConceito.ConceitoId = valorConvertido.conceitoId;
                            notaConceito.Conceito = valorConvertido.conceito;
                        }

                        linhaDto.AdicionaCelula(grupoMatriz.Key.Id,
                                                componente.CodDisciplina,
                                                possuiComponente ? (componente.LancaNota ?
                                                    notaConceito?.NotaConceito ?? "" :
                                                     notaConceito?.Sintese ?? "-") : "-",
                                                ++coluna);
                    }
                }
                
                linhaDto.ConselhoClasse = ativos ? TrataConselhoRegistrado(aluno.CodigoAluno, alunoComponenteConselhoClasse, componentesCurricularesTotal, anotacoes) : "";

                linhas.Add(linhaDto);
            }

            return linhas;
        }

        private string TrataConselhoRegistrado(long codigoAluno, List<(long codigoAluno, long codigoDisciplina, bool possuiConselho)> alunoComponenteConselhoClasse, int componentesCurricularesTotal, IEnumerable<AnotacoesPedagogicasAlunoIdsQueryDto> anotacoes)
        {
            var componentesConselhosDoAluno = alunoComponenteConselhoClasse
                .Where(a => a.codigoAluno == codigoAluno && a.possuiConselho == true)
                .Select(a => a.codigoDisciplina)
                .Count();

            if (componentesConselhosDoAluno == 0)
                return "PENDENTE";

            var regraNotasParaTodosOsComponentes = componentesConselhosDoAluno == componentesCurricularesTotal;

            if (regraNotasParaTodosOsComponentes)
                return "REGISTRADO";
            else
                return "EM ANDAMENTO";
        }

        private List<ComponenteCurricularPorTurma> ObterComponentesCurriculares(List<ComponenteCurricularPorTurma> componenteCurricularPorTurmas)
        {
            List<ComponenteCurricularPorTurma> componentes = new List<ComponenteCurricularPorTurma>();
            foreach (var componente in componenteCurricularPorTurmas)
            {
                if (componente.Regencia)
                {
                    foreach (var componenteCurricularRegencia in componente.ComponentesCurricularesRegencia)
                    {
                        if (!componentes.Any(a => a.CodDisciplina == componenteCurricularRegencia.CodDisciplina))
                            componentes.Add(new ComponenteCurricularPorTurma()
                            {
                                CodDisciplina = componenteCurricularRegencia.CodDisciplina,
                                CodDisciplinaPai = componenteCurricularRegencia.CodDisciplinaPai,
                                LancaNota = componenteCurricularRegencia.LancaNota,
                                Disciplina = componenteCurricularRegencia.Disciplina,
                                GrupoMatriz = componente.GrupoMatriz,
                                CodigoTurma = componente.CodigoTurma,
                                Regencia = componente.Regencia
                            });
                    }
                    componenteRegencia = componente;
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

            return frequenciaAlunos.FirstOrDefault(c => c.CodigoAluno == alunoCodigo
                                                && c.DisciplinaId == componenteFrequencia.CodDisciplina.ToString());
        }

        private void MontarEstruturaGruposMatriz(ConselhoClasseAtaBimestralDto relatorio, IEnumerable<IGrouping<ComponenteCurricularGrupoMatriz, ComponenteCurricularPorTurma>> gruposMatrizes, IEnumerable<PeriodoEscolar> periodosEscolares, IEnumerable<AreaDoConhecimento> areasDoConhecimento, IEnumerable<ComponenteCurricularGrupoAreaOrdenacaoDto> ordenacaoGrupoArea)
        {
            var bimestres = periodosEscolares.OrderBy(p => p.Bimestre).Select(a => a.Bimestre);
            if (gruposMatrizes != null)
            {
                foreach (var grupoMatriz in gruposMatrizes.OrderBy(gm => gm.Key.Id))
                {
                    var grupoMatrizDto = new ConselhoClasseAtaBimestralGrupoDto()
                    {
                        Id = grupoMatriz.Key.Id,
                        Nome = grupoMatriz.Key.Nome
                    };

                    var componentes = ObterComponentesCurriculares(grupoMatriz.GroupBy(c => c.CodDisciplina).Select(x => x.FirstOrDefault()).ToList());

                    foreach (var componenteCurricular in componentes.OrderBy(c => c.Disciplina))
                    {
                        if (!grupoMatrizDto.ComponentesCurriculares.Any(a => a.Id == componenteCurricular.CodDisciplina))
                            grupoMatrizDto.AdicionarComponente(componenteCurricular.CodDisciplina, componenteCurricular.Disciplina, grupoMatrizDto.Id, bimestres);
                    }
                    relatorio.GruposMatriz.Add(grupoMatrizDto);
                }

                foreach (var grupoMatriz in relatorio.GruposMatriz)
                {
                    var componentesDoGrupo = new List<ConselhoClasseAtaBimestralComponenteDto>();
                    var areasConhecimento = MapearAreasDoConhecimento(grupoMatriz.ComponentesCurriculares, areasDoConhecimento, ordenacaoGrupoArea, grupoMatriz.Id);

                    foreach (var area in areasConhecimento)
                        componentesDoGrupo.AddRange(ObterComponentesDasAreasDeConhecimento(grupoMatriz.ComponentesCurriculares, area));

                    grupoMatriz.ComponentesCurriculares = componentesDoGrupo.OrderBy(c => c.Nome).ToList();
                }
            }
        }

        private IEnumerable<ConselhoClasseAtaBimestralComponenteDto> ObterComponentesDasAreasDeConhecimento(IEnumerable<ConselhoClasseAtaBimestralComponenteDto> componentesCurricularesDaTurma, IEnumerable<AreaDoConhecimento> areaDoConhecimento)
            => componentesCurricularesDaTurma.Where(c => areaDoConhecimento.Select(a => a.CodigoComponenteCurricular).Contains(c.Id)).OrderBy(cc => cc.Nome);

        private IEnumerable<IGrouping<(string Nome, int? Ordem, long Id), AreaDoConhecimento>> MapearAreasDoConhecimento(IEnumerable<ConselhoClasseAtaBimestralComponenteDto> componentesCurricularesDaTurma,
                                                                                                           IEnumerable<AreaDoConhecimento> areasDoConhecimentos,
                                                                                                           IEnumerable<ComponenteCurricularGrupoAreaOrdenacaoDto> grupoAreaOrdenacao,
                                                                                                           long grupoMatrizId)
        {

            return areasDoConhecimentos.Where(a => ((componentesCurricularesDaTurma.Select(cc => cc.Id).Contains(a.CodigoComponenteCurricular)))).
                                                                     Select(a => { a.DefinirOrdem(grupoAreaOrdenacao, grupoMatrizId); return a; }).GroupBy(g => (g.Nome, g.Ordem, g.Id)).
                                                                     OrderByDescending(c => c.Key.Id > 0 && !string.IsNullOrEmpty(c.Key.Nome))
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

        private async Task<ConselhoClasseAtaBimestralCabecalhoDto> ObterCabecalho(string turmaCodigo)
            => await mediator.Send(new ObterAtaBimestralCabecalhoQuery(turmaCodigo));

        private async Task<Turma> ObterTurma(string turmaCodigo)
            => await mediator.Send(new ObterTurmaQuery(turmaCodigo));
        private async Task<IEnumerable<FrequenciaAluno>> ObterFrequenciaComponente(string[] turmasCodigo, IEnumerable<(string CodigoTurma, long ComponenteCurricularId)> componentesCurricularesPorTurma, int[] bimestres, long tipoCalendarioId, IEnumerable<(string codigoAluno, DateTime dataMatricula, DateTime? dataSituacao)> alunosDatasMatriculas)
            => await mediator.Send(new ObterFrequenciaComponenteGlobalPorTurmaQuery(turmasCodigo, componentesCurricularesPorTurma, bimestres, tipoCalendarioId, alunosDatasMatriculas));

        private async Task<IEnumerable<FrequenciaAluno>> ObterFrequenciaGeralPorAlunos(int anoletivo, string codigoTurma, long tipoCalendarioId, string[] alunosCodigo)
            => await mediator.Send(new ObterFrequenciasGeralPorAnoEAlunosQuery(anoletivo, codigoTurma, tipoCalendarioId, alunosCodigo));

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

        private ConselhoClasseAtaBimestralGrupoDto VerificarGrupoMatrizNaPagina(ConselhoClasseAtaBimestralDto modelCompleto, ConselhoClasseAtaBimestralPaginaDto modelPagina,
            ConselhoClasseAtaBimestralComponenteDto disciplina)
        {
            if (!modelPagina.GruposMatriz.Any(x => x.Id == disciplina.IdGrupoMatriz))
            {
                var grupoMatriz = modelCompleto.GruposMatriz.FirstOrDefault(x => x.Id == disciplina.IdGrupoMatriz);

                var novoGrupoMatriz = new ConselhoClasseAtaBimestralGrupoDto
                {
                    ComponentesCurriculares = new List<ConselhoClasseAtaBimestralComponenteDto>(),
                    Id = grupoMatriz.Id,
                    Nome = grupoMatriz.Nome
                };

                modelPagina.GruposMatriz.Add(novoGrupoMatriz);
            }

            return modelPagina.GruposMatriz.FirstOrDefault(x => x.Id == disciplina.IdGrupoMatriz);
        }
        private async Task<IEnumerable<IGrouping<string, NotasAlunoBimestre>>> ObterNotasAlunos(string[] alunosCodigo, int anoLetivo, Modalidade modalidade, int semestre, int[] tiposTurma, int bimestre)
            => await mediator.Send(new ObterNotasRelatorioAtaBimestralQuery(alunosCodigo, anoLetivo, (int)modalidade, semestre, tiposTurma, bimestre));

        private async Task<IEnumerable<IGrouping<string, ComponenteCurricularPorTurma>>> ObterComponentesCurricularesTurmasRelatorio(string[] turmaCodigo, string codigoUe, Modalidade modalidade,int[] bimestres = null)
            => await mediator.Send(new ObterComponentesCurricularesTurmasRelatorioAtaFinalResultadosQuery(turmaCodigo, codigoUe, modalidade, bimestres));      

        private (long conceitoId, string conceito) ConverterNotaParaConceito(decimal nota)
        {
            if (nota < 5)
                return (3, "NS");

            if (nota < 7)
                return (2, "S");

            return (1, "P");
        }
    }
}
