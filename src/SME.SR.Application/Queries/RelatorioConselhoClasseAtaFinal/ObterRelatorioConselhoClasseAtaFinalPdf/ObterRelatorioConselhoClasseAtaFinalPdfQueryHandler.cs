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
       
            foreach (var turmaCodigo in request.Filtro.TurmasCodigos)
            {
                try
                {
                    var turma = await ObterTurma(turmaCodigo);
                    

                    if (turma.TipoTurma == TipoTurma.Itinerarios2AAno &&
                        request.FiltroConselhoClasseAtaFinal.Visualizacao == AtaFinalTipoVisualizacao.Estudantes)
                        continue;

                    if (request.FiltroConselhoClasseAtaFinal.Visualizacao == AtaFinalTipoVisualizacao.Estudantes)
                    {
                        //obter relatorio estudante 
                        //só com turmas regulares 
                    }
                    else
                    {
                        var retorno = await ObterRelatorioTurma(turma, request);

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

            if (mensagensErro.Length > 0 && relatoriosTurmas.Count() == 0)
                throw new NegocioException(mensagensErro.ToString());

            return relatoriosTurmas;
        }

        private async Task<IEnumerable<ConselhoClasseAtaFinalPaginaDto>> ObterRelatorioTurma(Turma turma, ObterRelatorioConselhoClasseAtaFinalPdfQuery filtro)
        {
            var alunos = await ObterAlunos(turma.Codigo);
            var alunosCodigos = alunos.Select(x => x.CodigoAluno.ToString()).ToArray();
            var notas = await ObterNotasAlunos(alunosCodigos, turma.AnoLetivo, turma.ModalidadeCodigo, turma.Semestre);
            if (notas == null || !notas.Any())
                return Enumerable.Empty<ConselhoClasseAtaFinalPaginaDto>();
            var tipoCalendarioId = await ObterIdTipoCalendario(turma.ModalidadeTipoCalendario, turma.AnoLetivo, turma.Semestre);
            var periodosEscolares = await ObterPeriodosEscolares(tipoCalendarioId);
            var cabecalho = await ObterCabecalho(turma.Codigo);
            var listaturmas  = notas.Select(n => n.Key).ToArray();
            var componentesCurriculares = await ObterComponentesCurricularesTurmasRelatorio(listaturmas.ToArray(), turma.UeCodigo, turma.ModalidadeCodigo, filtro.UsuarioLogadoRF);
            var frequenciaAlunos = await ObterFrequenciaComponente(turma.Codigo, tipoCalendarioId, periodosEscolares);
            var frequenciaAlunosGeral = await ObterFrequenciaGeral(turma.Codigo);
            var pareceresConclusivos = await ObterPareceresConclusivos(turma.Codigo);



            var notasFinais = await ObterNotasFinaisPorTurma(turma.Codigo);
            var componentesCurriculares = await ObterComponentesCurriculares(turma.Codigo, filtro.UsuarioLogadoRF, filtro.PerfilUsuario);
            var dadosRelatorio = await MontarEstruturaRelatorio(turma.ModalidadeCodigo, cabecalho, alunos, componentesCurriculares, notasFinais, frequenciaAlunos, frequenciaAlunosGeral, pareceresConclusivos, periodosEscolares, turma.Codigo);
            return MontarEstruturaPaginada(dadosRelatorio);


            //var turma = await ObterTurma(turmaCodigo);
            //

            //

          
            //
            //var frequenciaAlunos = await ObterFrequenciaComponente(turmaCodigo, tipoCalendarioId, periodosEscolares);
            //var frequenciaAlunosGeral = await ObterFrequenciaGeral(turmaCodigo);
            //var pareceresConclusivos = await ObterPareceresConclusivos(turmaCodigo);

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

        private async Task<ConselhoClasseAtaFinalDto> MontarEstruturaRelatorio(Modalidade modalidadeCodigo, ConselhoClasseAtaFinalCabecalhoDto cabecalho, IEnumerable<AlunoSituacaoAtaFinalDto> alunos, IEnumerable<ComponenteCurricularPorTurma> componentesCurriculares, IEnumerable<NotaConceitoBimestreComponente> notasFinais, IEnumerable<FrequenciaAluno> frequenciaAlunos, IEnumerable<FrequenciaAluno> frequenciaAlunosGeral, IEnumerable<ConselhoClasseParecerConclusivo> pareceresConclusivos, IEnumerable<PeriodoEscolar> periodosEscolares, string turmaCodigo)
        {
            var relatorio = new ConselhoClasseAtaFinalDto();
            relatorio.Modalidade = modalidadeCodigo;

            relatorio.Cabecalho = cabecalho;
            var gruposMatrizes = componentesCurriculares.Where(c => c.GrupoMatriz != null).GroupBy(c => c.GrupoMatriz);

            MontarEstruturaGruposMatriz(relatorio, gruposMatrizes, periodosEscolares);
            await MontarEstruturaLinhas(relatorio, alunos, gruposMatrizes, notasFinais, frequenciaAlunos, frequenciaAlunosGeral, pareceresConclusivos, periodosEscolares, turmaCodigo);
            return relatorio;
        }

        private async Task MontarEstruturaLinhas(ConselhoClasseAtaFinalDto relatorio, IEnumerable<AlunoSituacaoAtaFinalDto> alunos, IEnumerable<IGrouping<ComponenteCurricularGrupoMatriz, ComponenteCurricularPorTurma>> gruposMatrizes, IEnumerable<NotaConceitoBimestreComponente> notasFinais, IEnumerable<FrequenciaAluno> frequenciaAlunos, IEnumerable<FrequenciaAluno> frequenciaAlunosGeral, IEnumerable<ConselhoClasseParecerConclusivo> pareceresConclusivos, IEnumerable<PeriodoEscolar> periodosEscolares, string turmaCodigo)
        {
            // Primmeiro alunos com numero de chamada
            foreach (var aluno in alunos.Where(a => int.Parse(a.NumeroAlunoChamada ?? "0") > 0).Select(a => new AlunoSituacaoAtaFinalDto(a)).OrderBy(a => a.NumeroAlunoChamada))
            {
                relatorio.Linhas.Add(await MontarLinhaAluno(aluno, gruposMatrizes, notasFinais, frequenciaAlunos, frequenciaAlunosGeral, pareceresConclusivos, periodosEscolares, turmaCodigo));
            }

            // Depois alunos sem numero ordenados por nome
            foreach (var aluno in alunos.Where(a => int.Parse(a.NumeroAlunoChamada ?? "0") == 0).Select(a => new AlunoSituacaoAtaFinalDto(a)).OrderBy(a => a.NomeAluno))
            {
                relatorio.Linhas.Add(await MontarLinhaAluno(aluno, gruposMatrizes, notasFinais, frequenciaAlunos, frequenciaAlunosGeral, pareceresConclusivos, periodosEscolares, turmaCodigo));
            }
        }

        private async Task<ConselhoClasseAtaFinalLinhaDto> MontarLinhaAluno(AlunoSituacaoAtaFinalDto aluno, IEnumerable<IGrouping<ComponenteCurricularGrupoMatriz, ComponenteCurricularPorTurma>> gruposMatrizes, IEnumerable<NotaConceitoBimestreComponente> notasFinais, IEnumerable<FrequenciaAluno> frequenciaAlunos, IEnumerable<FrequenciaAluno> frequenciaAlunosGeral, IEnumerable<ConselhoClasseParecerConclusivo> pareceresConclusivos, IEnumerable<PeriodoEscolar> periodosEscolares, string turmaCodigo)
        {
            var linhaDto = new ConselhoClasseAtaFinalLinhaDto()
            {
                Id = long.Parse(aluno.NumeroAlunoChamada ?? "0"),
                Nome = aluno.NomeAluno,
                Situacao = aluno.SituacaoMatricula,
                Inativo = aluno.Inativo
            };

            var turma = await mediator.Send(new ObterTurmaQuery(turmaCodigo));

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

                    var frequenciaAluno = await ObterFrequenciaAluno(frequenciaAlunos, aluno.CodigoAluno.ToString(), componente, turmaCodigo);

                    var sintese = await ObterSinteseAluno(frequenciaAluno?.PercentualFrequencia ?? 100, componente);

                    linhaDto.AdicionaCelula(grupoMatriz.Key.Id,
                                            componente.CodDisciplina,
                                            componente.LancaNota ?
                                                notaConceitofinal?.NotaConceito ?? "" :
                                                notaConceitofinal?.Sintese ?? sintese,
                                            ++coluna);

                    // Monta colunas frequencia F - CA - %


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
                                            (turma.AnoLetivo.Equals(2020) ? frequenciaAluno?.PercentualFrequenciaFinal.ToString() : frequenciaAluno?.PercentualFrequencia.ToString()) ?? FREQUENCIA_100,
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

        private async Task<IEnumerable<IGrouping<string, NotasAlunoBimestre>>> ObterNotasAlunos(string[] alunosCodigo, int anoLetivo, Modalidade modalidade, int semestre)
        {
            return await mediator.Send(new ObterNotasRelatorioHistoricoEscolarQuery(alunosCodigo, anoLetivo, (int)modalidade, semestre));
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
