﻿using MediatR;
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
            var mensagensErro = new StringBuilder();
            var relatoriosTurmas = new List<ConselhoClasseAtaFinalPaginaDto>();

            if (request.Filtro.Visualizacao == AtaFinalTipoVisualizacao.Turma || !request.Filtro.Visualizacao.HasValue)
            {
                var turmas = await mediator.Send(new ObterTurmasPorCodigoQuery(request.Filtro.TurmasCodigos.ToArray()));
               
                turmas.AsParallel().WithDegreeOfParallelism(variaveisAmbiente.ProcessamentoMaximoTurmas).ForAll(turma => 
                {
                try
                    {
                        Console.WriteLine($"Obtendo {turma.Codigo}");
                        var retorno = ObterRelatorioTurma(turma, request.Filtro, request.Filtro.Visualizacao).Result;
                        if (retorno != null && retorno.Any())
                            relatoriosTurmas.AddRange(retorno);
                        Console.WriteLine($"Processado {turma.Codigo}");
                    }
                    catch (Exception e)
                    {
                        mensagensErro.AppendLine($"<br/>Erro na carga de dados da turma {turma.NomeRelatorio}: {e.Message}");
                    }
                });
            }

            else if (request.Filtro.Visualizacao == AtaFinalTipoVisualizacao.Estudantes)
            {
                foreach (var turmaCodigo in request.Filtro.TurmasCodigos)
                {

                    var turma = await ObterTurma(turmaCodigo);
                    if (turma.TipoTurma == TipoTurma.Regular)
                    {
                        var retorno = await ObterRelatorioEstudante(turma, request.Filtro, request.Filtro.Visualizacao);
                        if (retorno != null && retorno.Any())
                            relatoriosTurmas.AddRange(retorno);
                    }
                }
            }



            if (mensagensErro.Length > 0 && relatoriosTurmas.Count() == 0)
                throw new NegocioException(mensagensErro.ToString());

            return relatoriosTurmas.OrderBy(a => a.Cabecalho.Turma).ToList();
        }

        private async Task<IEnumerable<ConselhoClasseAtaFinalPaginaDto>> ObterRelatorioTurma(Turma turma, FiltroConselhoClasseAtaFinalDto filtro, AtaFinalTipoVisualizacao? visualizacao)
        {
            try
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

                var componentesCurriculares = await ObterComponentesCurricularesTurmasRelatorio(listaTurmas.ToArray(), turma.Ue.Codigo, turma.ModalidadeCodigo);
                var frequenciaAlunosGeral = await ObterFrequenciaGeralPorAlunos(turma.AnoLetivo, tipoCalendarioId, alunosCodigos);

                var listaAlunos = await mediator.Send(new ObterDadosAlunosPorCodigosQuery(alunosCodigos.Select(long.Parse).ToArray()));
                listaAlunos = listaAlunos.Where(x => x.AnoLetivo == filtro.AnoLetivo);

                var listaTurmasAlunos = listaAlunos.GroupBy(x => x.CodigoTurma);

                listaTurmasAlunos = listaTurmasAlunos.Where(t => listaTurmas.Any(lt => lt == t.Key.ToString()));

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

                var componentesDaTurma = componentesCurriculares.SelectMany(cc => cc).ToList();
                var componentesCurricularesPorTurma = componentesDaTurma.Select(cc => (cc.CodigoTurma, cc.CodDisciplina)).Distinct();

                var bimestres = periodosEscolares.Select(p => p.Bimestre).ToArray();

                var frequenciaAlunos = await ObterFrequenciaComponente(listaTurmas.ToArray(), componentesCurricularesPorTurma, bimestres, tipoCalendarioId);

                var areasDoConhecimento = await ObterAreasConhecimento(componentesCurriculares);

                var ordenacaoGrupoArea = await ObterOrdenacaoAreasConhecimento(componentesCurriculares, areasDoConhecimento);

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
                        Sintese = nf.NotaConceito.Sintese,
                        ConselhoClasseAlunoId = nf.ConselhoClasseAlunoId
                    }));
                }

                var dadosRelatorio = await MontarEstruturaRelatorio(turma, cabecalho, alunos, componentesDaTurma, notasFinais, frequenciaAlunos, frequenciaAlunosGeral, pareceresConclusivos, periodosEscolares, listaTurmasAlunos, areasDoConhecimento, ordenacaoGrupoArea);
                return MontarEstruturaPaginada(dadosRelatorio);
            }
            catch (Exception ex)
            {
                throw ex;
            }

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

        private async Task<IEnumerable<ConselhoClasseAtaFinalPaginaDto>> ObterRelatorioEstudante(Turma turma, FiltroConselhoClasseAtaFinalDto filtro, AtaFinalTipoVisualizacao? visualizacao)
        {
            var alunos = await ObterAlunos(turma.Codigo);
            var alunosCodigos = alunos.Select(x => x.CodigoAluno.ToString()).ToArray();

            var notas = await ObterNotasAlunos(alunosCodigos, turma.AnoLetivo, turma.ModalidadeCodigo, turma.Semestre, new int[] { });
            if (notas == null || !notas.Any())
                return default;
            var tipoCalendarioId = await ObterIdTipoCalendario(turma.ModalidadeTipoCalendario, turma.AnoLetivo, turma.Semestre);
            var periodosEscolares = await ObterPeriodosEscolares(tipoCalendarioId);
            var cabecalho = await ObterCabecalho(turma.Codigo);

            var listaAlunos = await mediator.Send(new ObterDadosAlunosPorCodigosQuery(alunos.Select(x => x.CodigoAluno).ToArray()));
            listaAlunos = listaAlunos.Where(x => x.AnoLetivo == filtro.AnoLetivo);

            var listaTurmasAlunos = listaAlunos.GroupBy(x => x.CodigoTurma);
            List<string> listaTurmas = new List<string>();
            listaTurmas.Add(turma.Codigo);

            var turmaDetalhes = await ObterTurmaDetalhes(turma.Codigo);

            if (turmaDetalhes.EtapaEnsino != (int)EtapaEnsino.Magisterio)
            {
                foreach (var lta in listaTurmasAlunos)
                {
                    var turmaAluno = await ObterTurma(lta.Key.ToString());
                    if (turmaAluno.TipoTurma != TipoTurma.Regular)
                        listaTurmas.Add(turmaAluno.Codigo);
                }
            }
            else
            {
                foreach (var lta in listaTurmasAlunos)
                {
                    var turmaAluno = await ObterTurma(lta.Key.ToString());
                    if (turmaAluno.TipoTurma == TipoTurma.EdFisica)
                        listaTurmas.Add(turmaAluno.Codigo);
                }
            }

            listaTurmasAlunos = listaTurmasAlunos.Where(t => listaTurmas.Any(lt => lt == t.Key.ToString()));

            var componentesDaTurma = await ObterComponentesCurricularesTurmasRelatorio(listaTurmas.ToArray(), turma.Ue.Codigo, turma.ModalidadeCodigo);
            var frequenciaAlunosGeral = await ObterFrequenciaGeral(turma.AnoLetivo, tipoCalendarioId);
            var pareceresConclusivos = await ObterPareceresConclusivos(turma.Codigo);

            var componentesCurriculares = componentesDaTurma.SelectMany(cc => cc).ToList();
            var componentesCurricularesPorTurma = componentesCurriculares.Select(cc => (cc.CodigoTurma, cc.CodDisciplina)).Distinct();

            var bimestres = periodosEscolares.Select(p => p.Bimestre).ToArray();
            var frequenciaAlunos = await ObterFrequenciaComponente(listaTurmas.ToArray(), componentesCurricularesPorTurma, bimestres, tipoCalendarioId);

            var areasDoConhecimento = await ObterAreasConhecimento(componentesDaTurma);

            var ordenacaoGrupoArea = await ObterOrdenacaoAreasConhecimento(componentesDaTurma, areasDoConhecimento);

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
                    Sintese = nf.NotaConceito.Sintese,
                    ConselhoClasseAlunoId = nf.ConselhoClasseAlunoId
                }));
            }

            var dadosRelatorio = await MontarEstruturaRelatorio(turma, cabecalho, alunos, componentesCurriculares,
                notasFinais, frequenciaAlunos, frequenciaAlunosGeral, pareceresConclusivos, periodosEscolares, listaTurmasAlunos, areasDoConhecimento, ordenacaoGrupoArea);
            return MontarEstruturaPaginada(dadosRelatorio);
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

        private List<ConselhoClasseAtaFinalPaginaDto> MontarEstruturaPaginada(ConselhoClasseAtaFinalDto dadosRelatorio)
        {
            var maximoComponentesPorPagina = 8;
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

        private async Task<ConselhoClasseAtaFinalDto> MontarEstruturaRelatorio(Turma turma, ConselhoClasseAtaFinalCabecalhoDto cabecalho, IEnumerable<AlunoSituacaoAtaFinalDto> alunos, IEnumerable<ComponenteCurricularPorTurma> componentesCurriculares, IEnumerable<NotaConceitoBimestreComponente> notasFinais, IEnumerable<FrequenciaAluno> frequenciaAlunos, IEnumerable<FrequenciaAluno> frequenciaAlunosGeral,
            IEnumerable<ConselhoClasseParecerConclusivo> pareceresConclusivos, IEnumerable<PeriodoEscolar> periodosEscolares,
            IEnumerable<IGrouping<int, AlunoHistoricoEscolar>> listaTurmasAlunos, IEnumerable<AreaDoConhecimento> areasDoConhecimento, IEnumerable<ComponenteCurricularGrupoAreaOrdenacaoDto> ordenacaoGrupoArea)
        {
            var relatorio = new ConselhoClasseAtaFinalDto()
            {
                Modalidade = turma.ModalidadeCodigo,
                Cabecalho = cabecalho
            };

            componentesCurriculares = componentesCurriculares.OrderBy(a => a.Disciplina).ToList();
            var gruposMatrizes = componentesCurriculares.Distinct().Where(c => c.GrupoMatriz != null).GroupBy(c => c.GrupoMatriz).ToList();

            MontarEstruturaGruposMatriz(relatorio, gruposMatrizes, periodosEscolares, areasDoConhecimento, ordenacaoGrupoArea);
            await MontarEstruturaLinhas(relatorio, alunos, gruposMatrizes, notasFinais, frequenciaAlunos, frequenciaAlunosGeral, pareceresConclusivos, periodosEscolares, turma, listaTurmasAlunos, componentesCurriculares.Count(cc => cc.Frequencia));
            return relatorio;
        }

        private async Task MontarEstruturaLinhas(ConselhoClasseAtaFinalDto relatorio, IEnumerable<AlunoSituacaoAtaFinalDto> alunos, IEnumerable<IGrouping<ComponenteCurricularGrupoMatriz,
            ComponenteCurricularPorTurma>> gruposMatrizes, IEnumerable<NotaConceitoBimestreComponente> notasFinais, IEnumerable<FrequenciaAluno> frequenciaAlunos, IEnumerable<FrequenciaAluno> frequenciaAlunosGeral,
            IEnumerable<ConselhoClasseParecerConclusivo> pareceresConclusivos, IEnumerable<PeriodoEscolar> periodosEscolares,
            Turma turma, IEnumerable<IGrouping<int, AlunoHistoricoEscolar>> listaTurmasAlunos, int qtdeDisciplinasLancamFrequencia = 0)
        {
            var compensacaoAusenciaPercentualRegenciaClasse = double.Parse(await mediator.Send(new ObterParametroSistemaPorTipoQuery()
            {
                TipoParametro = TipoParametroSistema.CompensacaoAusenciaPercentualRegenciaClasse
            }));

            var compensacaoAusenciaPercentualFund2 = double.Parse(await mediator.Send(new ObterParametroSistemaPorTipoQuery()
            {
                TipoParametro = TipoParametroSistema.CompensacaoAusenciaPercentualFund2
            }));

            // Primmeiro alunos com numero de chamada
            var alunosComNumeroChamada = await MontarLinhaAlunoV2(alunos.Where(a => int.Parse(a.NumeroAlunoChamada ?? "0") > 0).Select(a => new AlunoSituacaoAtaFinalDto(a)).OrderBy(a => a.NumeroAlunoChamada),
                gruposMatrizes, notasFinais, frequenciaAlunos, frequenciaAlunosGeral, pareceresConclusivos, periodosEscolares, turma, listaTurmasAlunos, qtdeDisciplinasLancamFrequencia,
                compensacaoAusenciaPercentualRegenciaClasse, compensacaoAusenciaPercentualFund2);

            relatorio.Linhas.AddRange(alunosComNumeroChamada);

            // Depois alunos sem numero ordenados por nome
            var alunosSemNumeroChamada = await MontarLinhaAlunoV2(alunos.Where(a => int.Parse(a.NumeroAlunoChamada ?? "0") == 0).Select(a => new AlunoSituacaoAtaFinalDto(a)).OrderBy(a => a.NumeroAlunoChamada),
                gruposMatrizes, notasFinais, frequenciaAlunos, frequenciaAlunosGeral, pareceresConclusivos, periodosEscolares, turma, listaTurmasAlunos, qtdeDisciplinasLancamFrequencia,
                compensacaoAusenciaPercentualRegenciaClasse, compensacaoAusenciaPercentualFund2);

            relatorio.Linhas.AddRange(alunosSemNumeroChamada);
        }

        private async Task<List<ConselhoClasseAtaFinalLinhaDto>> MontarLinhaAlunoV2(IEnumerable<AlunoSituacaoAtaFinalDto> alunos, IEnumerable<IGrouping<ComponenteCurricularGrupoMatriz, ComponenteCurricularPorTurma>> gruposMatrizes, IEnumerable<NotaConceitoBimestreComponente> notasFinais, IEnumerable<FrequenciaAluno> frequenciaAlunos, IEnumerable<FrequenciaAluno> frequenciaAlunosGeral, IEnumerable<ConselhoClasseParecerConclusivo> pareceresConclusivos, IEnumerable<PeriodoEscolar> periodosEscolares, Turma turma, IEnumerable<IGrouping<int, AlunoHistoricoEscolar>> listaTurmasAlunos, int qtdeDisciplinasLancamFrequencia = 0, double compensacaoAusenciaPercentualRegenciaClasse = 0, double compensacaoAusenciaPercentualFund2 = 0)
        {
            List<ConselhoClasseAtaFinalLinhaDto> linhas = new List<ConselhoClasseAtaFinalLinhaDto>();
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

                foreach (var grupoMatriz in gruposMatrizes)
                {
                    var componentes = ObterComponentesCurriculares(grupoMatriz.GroupBy(c => c.CodDisciplina).Select(x => x.FirstOrDefault()).ToList());
                    var componentesTurmas = ObterComponentesCurriculares(grupoMatriz.ToList());
                    foreach (var componente in componentes)
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

                        var turmaPossuiFrequenciaRegistrada = await mediator.Send(new ExisteFrequenciaRegistradaPorTurmaComponenteCurricularEAnoQuery(turma.Codigo, componente.CodDisciplina.ToString(), turma.AnoLetivo));

                        var matriculadoDepois = !aluno.Inativo ? periodosEscolares.FirstOrDefault(p => p.PeriodoInicio <= aluno.DataSituacaoAluno && p.PeriodoFim >= aluno.DataSituacaoAluno)?.Bimestre : null;
                        var bimestres = periodosEscolares.OrderBy(p => p.Bimestre).Select(a => a.Bimestre).ToList();
                        foreach (var bimestre in bimestres)
                        {
                            var possuiConselho = notasFinais.Any(n => n.Bimestre == bimestre
                            && n.AlunoCodigo == aluno.CodigoAluno.ToString() && n.ConselhoClasseAlunoId != 0);

                            if (matriculadoDepois != null && bimestre < matriculadoDepois)
                            {
                                linhaDto.AdicionaCelula(grupoMatriz.Key.Id, componente.CodDisciplina, "-", ++coluna);
                                continue;
                            }

                            if (bimestre > ultimoBimestreAtivo)
                            {
                                linhaDto.AdicionaCelula(grupoMatriz.Key.Id, componente.CodDisciplina, "-", ++coluna);
                                continue;
                            }

                            if (possuiConselho)
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
                                continue;
                            }

                            linhaDto.AdicionaCelula(grupoMatriz.Key.Id, componente.CodDisciplina, possuiComponente ? "" : "-", ++coluna);

                        }

                        var possuiConselhoParaExibirFrequencias = notasFinais.Any(n => n.AlunoCodigo == aluno.CodigoAluno.ToString() &&
                                                                                n.ConselhoClasseAlunoId != 0 &&
                                                                                n.ComponenteCurricularCodigo == componente.CodDisciplina);

                        var frequenciaAluno = ObterFrequenciaAlunoV2(frequenciaAlunos, aluno.CodigoAluno.ToString(), componente, componentesTurmas);

                        var sintese = ObterSinteseAluno(frequenciaAluno?.PercentualFrequencia ?? 100, componente, compensacaoAusenciaPercentualRegenciaClasse, compensacaoAusenciaPercentualFund2);

                        if (possuiConselhoParaExibirFrequencias)
                        {
                            var notaConceitofinal = notasFinais.FirstOrDefault(c => c.AlunoCodigo == aluno.CodigoAluno.ToString()
                                                && c.ComponenteCurricularCodigo == componente.CodDisciplina
                                                && c.ConselhoClasseAlunoId != 0
                                                && (!c.Bimestre.HasValue || c.Bimestre.Value == 0));

                            linhaDto.AdicionaCelula(grupoMatriz.Key.Id,
                                                componente.CodDisciplina,
                                                possuiComponente ? (componente.LancaNota ?
                                                    notaConceitofinal?.NotaConceito ?? "" :
                                                    notaConceitofinal?.Sintese ?? sintese) : "-",
                                                ++coluna);

                            linhaDto.AdicionaCelula(grupoMatriz.Key.Id,
                                                componente.CodDisciplina,
                                               possuiComponente ? (frequenciaAluno?.TotalAusencias.ToString() ?? "0") : "-",
                                                ++coluna);
                            linhaDto.AdicionaCelula(grupoMatriz.Key.Id,
                                                    componente.CodDisciplina,
                                                     possuiComponente ? (frequenciaAluno?.TotalCompensacoes.ToString() ?? "0") : "-",
                                                    ++coluna);

                            var frequencia = "-";

                            if (possuiComponente)
                            {
                                if (turma.AnoLetivo.Equals(2020))
                                    frequencia = frequenciaAluno?.PercentualFrequenciaFinal.ToString();
                                else
                                {
                                    if (frequenciaAluno == null && turmaPossuiFrequenciaRegistrada)
                                        frequencia = "100";
                                    else if (frequenciaAluno != null)
                                        frequencia = frequenciaAluno?.PercentualFrequencia.ToString();
                                    else
                                        frequencia = string.Empty;
                                }
                            }

                            linhaDto.AdicionaCelula(grupoMatriz.Key.Id,
                                                    componente.CodDisciplina,
                                                    frequencia,
                                                    ++coluna);

                            continue;
                        }


                        var textoParaExibir = possuiComponente ? "" : "-";

                        linhaDto.AdicionaCelula(grupoMatriz.Key.Id, componente.CodDisciplina, (!aluno.Inativo && possuiComponente) ? "" : "-", ++coluna);
                        linhaDto.AdicionaCelula(grupoMatriz.Key.Id, componente.CodDisciplina, textoParaExibir, ++coluna);
                        linhaDto.AdicionaCelula(grupoMatriz.Key.Id, componente.CodDisciplina, textoParaExibir, ++coluna);
                        linhaDto.AdicionaCelula(grupoMatriz.Key.Id, componente.CodDisciplina, textoParaExibir, ++coluna);

                    }
                }

                TrataFrequenciaAnual(aluno, notasFinais, frequenciaAlunos, frequenciaAlunosGeral, pareceresConclusivos, linhaDto, turma, qtdeDisciplinasLancamFrequencia);

                linhas.Add(linhaDto);
            }

            return linhas;
        }

        private static void TrataFrequenciaAnual(AlunoSituacaoAtaFinalDto aluno, IEnumerable<NotaConceitoBimestreComponente> notasFinais, IEnumerable<FrequenciaAluno> frequenciaAlunos, IEnumerable<FrequenciaAluno> frequenciaAlunosGeral, IEnumerable<ConselhoClasseParecerConclusivo> pareceresConclusivos, ConselhoClasseAtaFinalLinhaDto linhaDto, Turma turma, int qtdeDisciplinasLancamFrequencia = 0)
        {
            var frequenciaGlobalAluno = frequenciaAlunosGeral
                .FirstOrDefault(c => c.CodigoAluno == aluno.CodigoAluno.ToString());

            var frequenciasAluno = frequenciaAlunos
                .Where(c => c.CodigoAluno == aluno.CodigoAluno.ToString());

            var possuiConselhoFinalParaAnual = notasFinais
                .Any(n => n.AlunoCodigo == aluno.CodigoAluno.ToString() && n.ConselhoClasseAlunoId != 0 && (!n.Bimestre.HasValue || n.Bimestre.Value == 0));

            var percentualFrequencia2020 = Math.Round((((qtdeDisciplinasLancamFrequencia - frequenciasAluno.Count()) * 100) + frequenciasAluno.Sum(f => f.PercentualFrequenciaFinal)) / qtdeDisciplinasLancamFrequencia, 2);

            if (possuiConselhoFinalParaAnual || aluno.CodigoSituacaoMatricula != SituacaoMatriculaAluno.Ativo)
            {
                linhaDto.AdicionaCelula(99, 99, frequenciasAluno?.Sum(f => f.TotalAusencias).ToString() ?? "0", 1);
                linhaDto.AdicionaCelula(99, 99, frequenciasAluno?.Sum(f => f.TotalCompensacoes).ToString() ?? "0", 2);
                linhaDto.AdicionaCelula(99, 99, (turma.AnoLetivo.Equals(2020) ? percentualFrequencia2020.ToString() : frequenciaGlobalAluno?.PercentualFrequencia.ToString()) ?? FREQUENCIA_100, 3);

                var parecerConclusivo = pareceresConclusivos.FirstOrDefault(c => c.AlunoCodigo == aluno.CodigoAluno.ToString());
                var textoParecer = parecerConclusivo?.ParecerConclusivo;
                if (textoParecer == null)
                    textoParecer = (aluno.CodigoSituacaoMatricula != SituacaoMatriculaAluno.Ativo) ? string.Concat(aluno.SituacaoMatricula, " em ", aluno.DataSituacaoAluno.ToString("dd/MM/yyyy")) : "Sem Parecer";
                linhaDto.AdicionaCelula(99, 99, textoParecer, 4);
                return;
            }


            linhaDto.AdicionaCelula(99, 99, "0", 1);
            linhaDto.AdicionaCelula(99, 99, "0", 2);
            linhaDto.AdicionaCelula(99, 99, string.Empty, 3);
            linhaDto.AdicionaCelula(99, 99, "Sem parecer", 4);
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
                                CodDisciplinaPai = componenteCurricularRegencia.CodDisciplinaPai,
                                LancaNota = componenteCurricularRegencia.LancaNota,
                                Disciplina = componenteCurricularRegencia.Disciplina,
                                GrupoMatriz = componente.GrupoMatriz,
                                CodigoTurma = componente.CodigoTurma
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

        private FrequenciaAluno ObterFrequenciaAlunoV2(IEnumerable<FrequenciaAluno> frequenciaAlunos, string alunoCodigo, ComponenteCurricularPorTurma componenteCurricular, List<ComponenteCurricularPorTurma> componentesTurmas)
        {
            var componenteFrequencia = componenteCurricular.Regencia ? ObterComponenteRegenciaTurmaV2(componentesTurmas) : componenteCurricular;

            return frequenciaAlunos.FirstOrDefault(c => c.CodigoAluno == alunoCodigo
                                                && c.DisciplinaId == componenteFrequencia.CodDisciplina.ToString());
        }

        private async Task<FrequenciaAluno> ObterFrequenciaAluno(IEnumerable<FrequenciaAluno> frequenciaAlunos, string alunoCodigo, ComponenteCurricularPorTurma componenteCurricular, string turmaCodigo)
        {
            var componenteFrequencia = componenteCurricular.Regencia ?
                await ObterComponenteRegenciaTurma(turmaCodigo) :
                componenteCurricular;

            return frequenciaAlunos.FirstOrDefault(c => c.CodigoAluno == alunoCodigo
                                                && c.DisciplinaId == componenteFrequencia.CodDisciplina.ToString());
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
                            grupoMatrizDto.AdicionarComponente(componenteCurricular.CodDisciplina, componenteCurricular.Disciplina, grupoMatrizDto.Id, bimestres);

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

                    grupoMatriz.ComponentesCurriculares = componentesDoGrupo;

                }
            }
        }

        private IEnumerable<ConselhoClasseAtaFinalComponenteDto> ObterComponentesDasAreasDeConhecimento(IEnumerable<ConselhoClasseAtaFinalComponenteDto> componentesCurricularesDaTurma,
                                                                                               IEnumerable<AreaDoConhecimento> areaDoConhecimento)
        {
            return componentesCurricularesDaTurma.Where(c => areaDoConhecimento.Select(a => a.CodigoComponenteCurricular).Contains(c.Id)).OrderBy(cc => cc.Nome);
        }

        private IEnumerable<IGrouping<(string Nome, int? Ordem, long Id), AreaDoConhecimento>> MapearAreasDoConhecimento(IEnumerable<ConselhoClasseAtaFinalComponenteDto> componentesCurricularesDaTurma,
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

        private ComponenteCurricularPorTurma ObterComponenteRegenciaTurmaV2(IEnumerable<ComponenteCurricularPorTurma> componentesTurma)
        {
            if (componenteRegencia == null)
            {
                componenteRegencia = componentesTurma.FirstOrDefault(c => c.Regencia);
            }

            return componenteRegencia;
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

        private async Task<Turma> ObterTurmaDetalhes(string turmaCodigo)
        {
            var turmas = await mediator.Send(new ObterTurmasDetalhePorCodigoQuery(new long[] { Convert.ToInt64(turmaCodigo) }));
            return turmas.FirstOrDefault();
        }

        private async Task<Turma> ObterTurma(string turmaCodigo)
            => await mediator.Send(new ObterTurmaQuery(turmaCodigo));
        private async Task<IEnumerable<FrequenciaAluno>> ObterFrequenciaComponente(string[] turmasCodigo, IEnumerable<(string CodigoTurma, long ComponenteCurricularId)> componentesCurricularesPorTurma, int[] bimestres, long tipoCalendarioId)
            => await mediator.Send(new ObterFrequenciaComponenteGlobalPorTurmaQuery(turmasCodigo, componentesCurricularesPorTurma, bimestres, tipoCalendarioId));

        private async Task<IEnumerable<FrequenciaAluno>> ObterFrequenciaGeral(int anoTurma, long tipoCalendarioId)
            => await mediator.Send(new ObterFrequenciasGeralAlunosNaTurmaQuery(anoTurma, tipoCalendarioId));

        private async Task<IEnumerable<FrequenciaAluno>> ObterFrequenciaGeralPorAlunos(int anoletivo, long tipoCalendarioId, string[] alunosCodigos)
            => await mediator.Send(new ObterFrequenciasGeralPorAnoEAlunosQuery(anoletivo, tipoCalendarioId, alunosCodigos));

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
        private async Task<IEnumerable<IGrouping<string, NotasAlunoBimestre>>> ObterNotasAlunos(string[] alunosCodigo, int anoLetivo, Modalidade modalidade, int semestre, int[] tiposTurma)
        {
            return await mediator.Send(new ObterNotasRelatorioAtaFinalQuery(alunosCodigo, anoLetivo, (int)modalidade, semestre, tiposTurma));
        }

        private async Task<IEnumerable<IGrouping<string, ComponenteCurricularPorTurma>>> ObterComponentesCurricularesTurmasRelatorio(string[] turmaCodigo, string codigoUe, Modalidade modalidade)
        {
            return await mediator.Send(new ObterComponentesCurricularesTurmasRelatorioAtaFinalResultadosQuery()
            {
                CodigosTurma = turmaCodigo,
                CodigoUe = codigoUe,
                Modalidade = modalidade
            });
        }
    }
}
