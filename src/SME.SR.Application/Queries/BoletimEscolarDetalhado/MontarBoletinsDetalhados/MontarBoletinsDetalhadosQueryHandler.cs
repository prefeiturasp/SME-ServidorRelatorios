using DocumentFormat.OpenXml.Office2010.PowerPoint;
using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using SME.SR.Data.Models;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class
        MontarBoletinsDetalhadosQueryHandler : IRequestHandler<MontarBoletinsDetalhadosQuery,
            BoletimEscolarDetalhadoDto>
    {
        private const double FREQUENCIA_100 = 100;
        private readonly IMediator mediator;
        private readonly ITipoCalendarioRepository tipoCalendarioRepository;

        private string frequencia100Formatada = FrequenciaAluno.FormatarPercentual(FREQUENCIA_100);

        public MontarBoletinsDetalhadosQueryHandler(IMediator mediator,
            ITipoCalendarioRepository tipoCalendarioRepository)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this.tipoCalendarioRepository = tipoCalendarioRepository ??
                                            throw new ArgumentNullException(nameof(tipoCalendarioRepository));
        }

        public async Task<BoletimEscolarDetalhadoDto> Handle(MontarBoletinsDetalhadosQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var turmas = request.Turmas;
                var dre = request.Dre;
                var ue = request.Ue;
                var ultimoBimestrePeriodoFechamento = request.UltimoBimestrePeriodoFechamento;
                var ciclos = request.TiposCiclo;
                var componentesCurriculares = request.ComponentesCurriculares;
                var alunos = request.AlunosPorTuma;
                var fotos = request.AlunosFoto;
                var notas = request.Notas;
                var frequencia = request.Frequencias;
                var frequenciasGlobal = request.FrequenciasGlobal;
                var tiposNota = request.TiposNota;
                var pareceresConclusivos = request.PareceresConclusivos;
                var mediasFrequencia = request.MediasFrequencia;
                var recomendacoes = request.RecomendacoesAlunos;

                var registroFrequencia =
                    await mediator.Send(new ObterTotalAulasTurmaEBimestreEComponenteCurricularQuery(
                        turmas.Select(a => a.Codigo).ToArray(), 0, new string[] { }, new int[] { 1, 2, 3, 4 }));
                var periodoAtual =
                    await mediator.Send(new ObterBimestrePeriodoFechamentoAtualQuery(request.AnoLetivo));

                var boletinsAlunos = new List<BoletimEscolarDetalhadoAlunoDto>();

                foreach (var aluno in alunos)
                {
                    // verifica PossuiConselho bimestre
                    var turma = turmas.First(t => aluno.Any(a => a.CodigoTurma.ToString() == t.Codigo));

                    var conselhoClassBimestres =
                        await mediator.Send(new AlunoConselhoClasseCadastradoBimestresQuery(aluno.Key, turma.AnoLetivo,
                            turma.ModalidadeCodigo, turma.Semestre));

                    if (conselhoClassBimestres != null && conselhoClassBimestres.Any())
                    {
                        var tipoNota = tiposNota[turma.Codigo];
                        var boletimEscolarAlunoDto = new BoletimEscolarDetalhadoAlunoDto()
                        {
                            TipoNota = tipoNota
                        };

                        var componentesAluno = componentesCurriculares.FirstOrDefault(c => c.Key == aluno.Key);
                        foreach (var turmaAluno in aluno)
                        {
                            if (componentesAluno != null && componentesAluno.Any())
                            {
                                await MapearGruposEComponentes(
                                componentesAluno.Where(cc => cc.CodigoTurma == turmaAluno.CodigoTurma.ToString()),
                                boletimEscolarAlunoDto);
                            }
                        }

                        var notasAluno = notas.FirstOrDefault(t => t.Key == aluno.First().CodigoAluno.ToString());
                        var frequenciasAluno =
                            frequencia?.FirstOrDefault(t => t.Key == aluno.First().CodigoAluno.ToString());
                        var frequenciasTurma = frequencia?.SelectMany(a => a).Where(f => f.TurmaId == turma.Codigo);

                        if (notasAluno != null && notasAluno.Any())
                            SetarNotasFrequencia(boletimEscolarAlunoDto, notasAluno, frequenciasAluno, frequenciasTurma,
                                mediasFrequencia, conselhoClassBimestres, ultimoBimestrePeriodoFechamento,
                                registroFrequencia, periodoAtual);

                        var frequeciaGlobal = frequenciasGlobal?.FirstOrDefault(t => t.Key == aluno.First().CodigoAluno.ToString());
                        var percentualFrequenciaGlobal = frequeciaGlobal != null ? $"{FrequenciaAluno.FormatarPercentual(frequeciaGlobal.First().PercentualFrequencia)}" : string.Empty;

                        var parecerConclusivo = pareceresConclusivos.FirstOrDefault(c =>
                            c.TurmaId.ToString() == turma.Codigo && c.AlunoCodigo.ToString() == aluno.Key);
                        var recomendacao = recomendacoes?.FirstOrDefault(r =>
                            r.TurmaCodigo == turma.Codigo && r.AlunoCodigo == aluno.Key);
                        var ciclo = ciclos.FirstOrDefault(c =>
                            c.Modalidade == turma.ModalidadeCodigo && c.Ano == turma.Ano);
                        var foto = fotos.FirstOrDefault(c => c.CodigoAluno.ToString() == aluno.Key);

                        boletimEscolarAlunoDto.Cabecalho = ObterCabecalhoInicial(dre, ue, ciclo, turma, aluno.Key, foto,
                            aluno.FirstOrDefault().NomeRelatorio, aluno.FirstOrDefault().ObterNomeFinal(), percentualFrequenciaGlobal, request.AnoLetivo);
                        boletimEscolarAlunoDto.ParecerConclusivo = conselhoClassBimestres.Any(b => b == 0)
                            ? (parecerConclusivo?.ParecerConclusivo ?? "")
                            : null;
                        boletimEscolarAlunoDto.RecomendacoesEstudante = recomendacao?.RecomendacoesAluno;
                        boletimEscolarAlunoDto.RecomendacoesFamilia = recomendacao?.RecomendacoesFamilia;
                        boletimEscolarAlunoDto.ExibirRecomendacoes = request.ExibirRecomendacao;
                        boletinsAlunos.Add(boletimEscolarAlunoDto);
                    }
                }

                if (!boletinsAlunos.Any())
                    throw new NegocioException("Não foram encontradas informações para geração do boletim");

                return await Task.FromResult(new BoletimEscolarDetalhadoDto(boletinsAlunos, request.ExibirRecomendacao));
            }
            catch (Exception ex)
            {
                var mensagem = $"Não foi possível montar boletim - Motivo: {ex.Message}";

                if (ex is NegocioException)
                    throw new NegocioException(mensagem);

                throw new Exception(mensagem);
            }
        }

        private BoletimEscolarDetalhadoCabecalhoDto ObterCabecalhoInicial(Dre dre, Ue ue, TipoCiclo ciclo, Turma turma,
            string alunoCodigo, AlunoFotoArquivoDto foto, string nome, string nomeAluno, string frequenciaGlobal, int anoLetivo)
        {
            return new BoletimEscolarDetalhadoCabecalhoDto()
            {
                Data = DateTime.Now.ToString("dd/MM/yyyy"),
                NomeDre = dre.Nome,
                NomeUe = ue.TituloTipoEscolaNome,
                NomeTurma = turma.NomeRelatorio,
                CodigoEol = alunoCodigo,
                Aluno = nome,
                FrequenciaGlobal = frequenciaGlobal,
                Ciclo = ciclo.Descricao,
                Foto = foto?.FotoBase64,
                AnoLetivo = anoLetivo,
                NomeAluno = nomeAluno
            };
        }

        private async Task MapearGruposEComponentes(
            IEnumerable<ComponenteCurricularPorTurma> componentesCurricularesPorTurma,
            BoletimEscolarDetalhadoAlunoDto boletim)
        {
            var componentesOrdenados =
                await mediator.Send(
                    new OrdenarComponentesPorGrupoMatrizAreaConhecimentoQuery(componentesCurricularesPorTurma));

            var gruposMatrizes = componentesCurricularesPorTurma.GroupBy(cc => cc.GrupoMatriz).ToList();

            var grupos = boletim.Grupos;

            var gruposRetorno = new List<GrupoMatrizComponenteCurricularDto>();

            foreach (var grupoMatriz in gruposMatrizes)
            {
                GrupoMatrizComponenteCurricularDto grupo = null;

                if (grupos.Any(g => g.Id == (int)grupoMatriz.Key.Id))
                    grupo = grupos.FirstOrDefault(g => g.Id == (int)grupoMatriz.Key.Id);
                else
                {
                    grupo = new GrupoMatrizComponenteCurricularDto()
                    {
                        Id = (int)grupoMatriz.Key.Id,
                        Nome = $"GRUPO {gruposRetorno.Count() + 1}",
                        Descricao = grupoMatriz.Key.Nome
                    };

                    grupos.Add(grupo);
                }

                var grupoMatrizAgrupadosAreaOrdem = grupoMatriz.GroupBy(a => a.AreaDoConhecimento.Ordem != null);

                foreach (var gruposOrdem in grupoMatrizAgrupadosAreaOrdem.OrderByDescending(g => g.Key))
                {
                    foreach (var componente in gruposOrdem.OrderBy(a => a.Disciplina))
                    {
                        if (componente.Regencia && componente.ComponentesCurricularesRegencia != null &&
                            componente.ComponentesCurricularesRegencia.Any())
                        {
                            boletim.ComponenteCurricularRegencia = new ComponenteCurricularRegenciaDto()
                            {
                                Codigo = componente.CodDisciplina.ToString(),
                                Frequencia = componente.Frequencia
                            };

                            foreach (var componenteRegencia in componente.ComponentesCurricularesRegencia.OrderBy(a =>
                                a.Disciplina))
                            {
                                if (!boletim.ComponenteCurricularRegencia.ComponentesCurriculares.Any(g =>
                                    g.Codigo == componenteRegencia.CodDisciplina.ToString()))
                                    boletim.ComponenteCurricularRegencia.ComponentesCurriculares.Add(
                                        new ComponenteCurricularRegenciaNotaDto()
                                        {
                                            Codigo = componenteRegencia.CodDisciplina.ToString(),
                                            Nome = componenteRegencia.Disciplina,
                                            Nota = componenteRegencia.LancaNota
                                        });
                            }
                        }
                        else if (!componente.Regencia)
                        {
                            if (grupo.ComponentesCurriculares == null)
                                grupo.ComponentesCurriculares = new List<ComponenteCurricularDto>();

                            if (!grupo.ComponentesCurriculares.Any(g => g.Codigo == componente.CodDisciplina.ToString()))
                                grupo.ComponentesCurriculares.Add(
                                    new ComponenteCurricularDto()
                                    {
                                        Codigo = componente.CodDisciplina.ToString(),
                                        Nome = componente.Disciplina,
                                        Nota = componente.LancaNota,
                                        Frequencia = componente.Frequencia
                                    });
                        }
                    }
                }


                if (boletim.ComponenteCurricularRegencia != null)
                    boletim.ComponenteCurricularRegencia.ComponentesCurriculares = boletim.ComponenteCurricularRegencia
                        .ComponentesCurriculares.OrderBy(c => c.Nome).ToList();
            }
        }

        private void SetarNotasFrequencia(BoletimEscolarDetalhadoAlunoDto boletimEscolar,
            IEnumerable<NotasAlunoBimestre> notas, IEnumerable<FrequenciaAluno> frequenciasAluno,
            IEnumerable<FrequenciaAluno> frequenciasTurma, IEnumerable<MediaFrequencia> mediasFrequencia,
            IEnumerable<int> conselhoClasseBimestres, int ultimoBimestrePeriodoFechamento,
            IEnumerable<TurmaComponenteQtdAulasDto> registroFrequencia, int periodoAtual)
        {
            boletimEscolar.PossuiNotaFinal = false;
            boletimEscolar.PossuiNotaFinalRegencia = false;
            if (boletimEscolar.ComponenteCurricularRegencia != null)
            {
                if (boletimEscolar.ComponenteCurricularRegencia.Frequencia)
                {
                    var frequenciasAlunoRegencia = frequenciasAluno?.Where(f =>
                        f.DisciplinaId == boletimEscolar.ComponenteCurricularRegencia.Codigo);
                    var frequenciasTurmaRegencia = frequenciasTurma?.Where(f =>
                        f.DisciplinaId == boletimEscolar.ComponenteCurricularRegencia.Codigo);
                    var aulasCadastradas = registroFrequencia?.Where(f =>
                        f.ComponenteCurricularCodigo == boletimEscolar.ComponenteCurricularRegencia.Codigo);

                    boletimEscolar.ComponenteCurricularRegencia.FrequenciaBimestre1 = ObterFrequenciaBimestre(
                        conselhoClasseBimestres, frequenciasAlunoRegencia, frequenciasTurmaRegencia, 1,
                        aulasCadastradas, periodoAtual);
                    boletimEscolar.ComponenteCurricularRegencia.FrequenciaBimestre2 = ObterFrequenciaBimestre(
                        conselhoClasseBimestres, frequenciasAlunoRegencia, frequenciasTurmaRegencia, 2,
                        aulasCadastradas, periodoAtual);
                    boletimEscolar.ComponenteCurricularRegencia.FrequenciaBimestre3 = ObterFrequenciaBimestre(
                        conselhoClasseBimestres, frequenciasAlunoRegencia, frequenciasTurmaRegencia, 3,
                        aulasCadastradas, periodoAtual);
                    boletimEscolar.ComponenteCurricularRegencia.FrequenciaBimestre4 = ObterFrequenciaBimestre(
                        conselhoClasseBimestres, frequenciasAlunoRegencia, frequenciasTurmaRegencia, 4,
                        aulasCadastradas, periodoAtual);

                    boletimEscolar.ComponenteCurricularRegencia.FrequenciaFinal =
                        ObterFrequenciaFinalAluno(frequenciasAlunoRegencia, frequenciasTurmaRegencia,
                            conselhoClasseBimestres);
                }

                foreach (var componenteCurricular in
                    boletimEscolar.ComponenteCurricularRegencia.ComponentesCurriculares)
                {
                    if (componenteCurricular.Nota)
                    {
                        var notaFrequenciaComponente = notas?.Where(nf =>
                            nf.CodigoComponenteCurricular == componenteCurricular.Codigo);

                        var notaFrequenciaComponenteComPeriodo = notaFrequenciaComponente
                            .Where(nf => nf.PeriodoEscolar != null);

                        if (!notaFrequenciaComponente.Any())
                        {
                            notaFrequenciaComponente = notas?.Where(nf =>
                                nf.CodigoComponenteCurricular == boletimEscolar.ComponenteCurricularRegencia.Codigo);
                        }

                        componenteCurricular.NotaBimestre1 = ObterNotaBimestre(conselhoClasseBimestres,
                            ultimoBimestrePeriodoFechamento, notaFrequenciaComponenteComPeriodo, 1, periodoAtual);
                        componenteCurricular.NotaBimestre2 = ObterNotaBimestre(conselhoClasseBimestres,
                            ultimoBimestrePeriodoFechamento, notaFrequenciaComponenteComPeriodo, 2, periodoAtual);
                        componenteCurricular.NotaBimestre3 = ObterNotaBimestre(conselhoClasseBimestres,
                            ultimoBimestrePeriodoFechamento, notaFrequenciaComponenteComPeriodo, 3, periodoAtual);
                        componenteCurricular.NotaBimestre4 = ObterNotaBimestre(conselhoClasseBimestres,
                            ultimoBimestrePeriodoFechamento, notaFrequenciaComponenteComPeriodo, 4, periodoAtual);

                        componenteCurricular.NotaFinal =
                            ObterNotaBimestreFinal(conselhoClasseBimestres, notaFrequenciaComponente);

                        if (!string.IsNullOrEmpty(componenteCurricular.NotaFinal))
                            boletimEscolar.PossuiNotaFinalRegencia = true;
                    }
                }
            }

            boletimEscolar.PossuiNotaFinal = boletimEscolar.PossuiNotaFinalRegencia;
            foreach (var grupoMatriz in boletimEscolar.Grupos)
            {
                if (grupoMatriz.ComponentesCurriculares != null && grupoMatriz.ComponentesCurriculares.Any())
                {
                    foreach (var componenteCurricular in grupoMatriz.ComponentesCurriculares)
                    {
                        var frequenciasAlunoComponente =
                            frequenciasAluno?.Where(f => f.DisciplinaId == componenteCurricular.Codigo);
                        var frequenciasTurmaComponente =
                            frequenciasTurma?.Where(f => f.DisciplinaId == componenteCurricular.Codigo);

                        if (componenteCurricular.Nota)
                        {
                            var notasComponente =
                                notas?.Where(n => n.CodigoComponenteCurricular == componenteCurricular.Codigo) ?? null;

                            var notasComponenteComPeriodoEscolar =
                                notasComponente?.Where(n => n.PeriodoEscolar != null) ?? null;

                            bool transformarNotaEmConceito = !notasComponente.All(nc => nc.NotaConceito.Nota.HasValue);

                            componenteCurricular.NotaBimestre1 = ObterNotaBimestre(conselhoClasseBimestres,
                                 ultimoBimestrePeriodoFechamento, notasComponenteComPeriodoEscolar, 1, periodoAtual, transformarNotaEmConceito);
                            componenteCurricular.NotaBimestre2 = ObterNotaBimestre(conselhoClasseBimestres,
                                ultimoBimestrePeriodoFechamento, notasComponenteComPeriodoEscolar, 2, periodoAtual, transformarNotaEmConceito);
                            componenteCurricular.NotaBimestre3 = ObterNotaBimestre(conselhoClasseBimestres,
                                ultimoBimestrePeriodoFechamento, notasComponenteComPeriodoEscolar, 3, periodoAtual, transformarNotaEmConceito);
                            componenteCurricular.NotaBimestre4 = ObterNotaBimestre(conselhoClasseBimestres,
                                ultimoBimestrePeriodoFechamento, notasComponenteComPeriodoEscolar, 4, periodoAtual, transformarNotaEmConceito);

                            componenteCurricular.NotaFinal =
                                ObterNotaBimestreFinal(conselhoClasseBimestres, notasComponente);
                            if (!string.IsNullOrEmpty(componenteCurricular.NotaFinal))
                                boletimEscolar.PossuiNotaFinal = true;
                        }
                        else
                            componenteCurricular.NotaFinal = ObterSintese(frequenciasAlunoComponente, mediasFrequencia, false, false);

                        if (componenteCurricular.Frequencia)
                        {
                            var aulasCadastradas = registroFrequencia?.Where(f =>
                                f.ComponenteCurricularCodigo == componenteCurricular.Codigo);

                            componenteCurricular.FrequenciaBimestre1 = ObterFrequenciaBimestre(conselhoClasseBimestres,
                                frequenciasAlunoComponente, frequenciasTurmaComponente, 1, aulasCadastradas,
                                periodoAtual);
                            componenteCurricular.FrequenciaBimestre2 = ObterFrequenciaBimestre(conselhoClasseBimestres,
                                frequenciasAlunoComponente, frequenciasTurmaComponente, 2, aulasCadastradas,
                                periodoAtual);
                            componenteCurricular.FrequenciaBimestre3 = ObterFrequenciaBimestre(conselhoClasseBimestres,
                                frequenciasAlunoComponente, frequenciasTurmaComponente, 3, aulasCadastradas,
                                periodoAtual);
                            componenteCurricular.FrequenciaBimestre4 = ObterFrequenciaBimestre(conselhoClasseBimestres,
                                frequenciasAlunoComponente, frequenciasTurmaComponente, 4, aulasCadastradas,
                                periodoAtual);

                            componenteCurricular.FrequenciaFinal = ObterFrequenciaFinalAluno(frequenciasAlunoComponente,
                                frequenciasTurmaComponente, conselhoClasseBimestres);

                            if (!componenteCurricular.Nota)
                                componenteCurricular.NotaFinal = ObterSintese(frequenciasAlunoComponente,
                                    mediasFrequencia, false, false);
                        }
                        else
                        {
                            componenteCurricular.FrequenciaBimestre1 = "-";
                            componenteCurricular.FrequenciaBimestre2 = "-";
                            componenteCurricular.FrequenciaBimestre3 = "-";
                            componenteCurricular.FrequenciaBimestre4 = "-";
                            componenteCurricular.FrequenciaFinal = "-";
                        }
                    }
                }
            }
        }

        private string ObterNotaBimestre(IEnumerable<int> conselhoClassBimestres, int ultimoBimestrePeriodoFechamento,
            IEnumerable<NotasAlunoBimestre> notasComponente, int bimestre, int periodoAtual, bool transformarNotaEmConceito = false)
        {
            var possuiConselho = VerificaPossuiConselho(conselhoClassBimestres, bimestre);

            var nota = !possuiConselho
                ? ""
                : notasComponente
                    ?.FirstOrDefault(nf => nf.PeriodoEscolar != null && nf.PeriodoEscolar.Bimestre == bimestre)
                    ?.NotaConceito?.NotaConceito;

            if (bimestre > periodoAtual && string.IsNullOrEmpty(nota) && notasComponente.Any(x => x.PeriodoEscolar.PeriodoInicio.Year == DateTime.Now.Year))
                nota = "-";
            else if (transformarNotaEmConceito && decimal.TryParse(nota, out decimal valor))
                nota = ConverterNotaParaConceito(decimal.Parse(nota, CultureInfo.InvariantCulture)).conceito;

            return nota;
        }

        private string ObterNotaBimestreFinal(IEnumerable<int> conselhoClassBimestres,
            IEnumerable<NotasAlunoBimestre> notasComponente)
        {
            if (!VerificaPossuiConselho(conselhoClassBimestres, 0))
                return "";

            var nota = notasComponente
                .OrderByDescending(nf => nf.fechamentoDisciplina)?
                .FirstOrDefault(nf => nf.PeriodoEscolar == null)?.NotaConceito?.NotaConceito;

            if (!notasComponente.All(nc => nc.NotaConceito.Nota.HasValue) && decimal.TryParse(nota, out decimal valor))
                nota = ConverterNotaParaConceito(decimal.Parse(nota)).conceito;

            return !string.IsNullOrEmpty(nota) ? nota : "-";
        }

        private bool VerificaPossuiConselho(IEnumerable<int> conselhoClassBimestres, int bimestre)
        {
            return conselhoClassBimestres.Any(a => a == bimestre);
        }

        private string ObterFrequenciaBimestre(IEnumerable<int> conselhoClassBimestres,
            IEnumerable<FrequenciaAluno> frequenciasAlunoComponente,
            IEnumerable<FrequenciaAluno> frequenciasTurmaComponente, int bimestre,
            IEnumerable<TurmaComponenteQtdAulasDto> aulasCadastradas, int periodoAtual)
        {
            var possuiFrequenciaTurma = aulasCadastradas?.Any(nf => nf.Bimestre == bimestre);

            var frequencia = !VerificaPossuiConselho(conselhoClassBimestres, bimestre)
                ? ""
                : frequenciasAlunoComponente?.FirstOrDefault(nf => nf.Bimestre == bimestre)?.PercentualFrequenciaFormatado
                     ?? (possuiFrequenciaTurma.HasValue && possuiFrequenciaTurma.Value
                    ? frequencia100Formatada
                    : string.Empty);

            if (!String.IsNullOrEmpty(frequencia))
                frequencia = frequencia += "%";

            if (bimestre > periodoAtual && String.IsNullOrEmpty(frequencia))
                frequencia = "-";

            return frequencia;
        }

        private string ObterFrequenciaFinalAluno(IEnumerable<FrequenciaAluno> frequenciasAluno,
            IEnumerable<FrequenciaAluno> frequenciasTurma, IEnumerable<int> conselhoClassBimestres)
        {
            if (!conselhoClassBimestres.Any(a => a == 0) ||
                frequenciasAluno == null || !frequenciasAluno.Any())
            {
                var possuiNotaTurma = frequenciasTurma?.Any(nf => nf.PeriodoEscolarId == null);

                if (possuiNotaTurma.HasValue && possuiNotaTurma.Value)
                    return frequencia100Formatada;
                else
                    return "";
            }
            else if (frequenciasAluno.FirstOrDefault(nf => nf.PeriodoEscolarId == null) != null)
                return frequenciasAluno.FirstOrDefault(nf => nf.PeriodoEscolarId == null).PercentualFrequenciaFormatado;
            else
            {
                var frequenciaFinal = new FrequenciaAluno()
                {
                    TotalAulas = frequenciasAluno.Sum(f => f.TotalAulas),
                    TotalAusencias = frequenciasAluno.Sum(f => f.TotalAusencias),
                    TotalCompensacoes = frequenciasAluno.Sum(f => f.TotalCompensacoes)
                };

                //Particularidade de cálculo de frequência para 2020.
                if (frequenciasAluno.First().AnoTurma.Equals(2020))
                {
                    var idTipoCalendario = tipoCalendarioRepository
                        .ObterPorAnoLetivoEModalidade(frequenciasAluno.First().AnoTurma,
                            frequenciasAluno.First().ModalidadeTurma).Result;
                    var periodos = mediator.Send(new ObterPeriodosEscolaresPorTipoCalendarioQuery(idTipoCalendario))
                        .Result;

                    periodos.ToList().ForEach(p =>
                    {
                        var frequencia = frequenciasAluno.SingleOrDefault(f => f.Bimestre.Equals(p.Bimestre));
                        frequenciaFinal.AdicionarFrequenciaBimestre(p.Bimestre,
                            frequencia != null ? frequencia.PercentualFrequencia : 100);
                    });

                    return FrequenciaAluno.FormatarPercentual(frequenciaFinal.PercentualFrequenciaFinal.GetValueOrDefault());
                }

                return frequenciaFinal.PercentualFrequenciaFormatado;
            }
        }

        private string ObterSintese(IEnumerable<FrequenciaAluno> frequenciasComponente,
            IEnumerable<MediaFrequencia> mediaFrequencias, bool regencia, bool lancaNota)
        {
            var percentualFrequencia = ObterPercentualDeFrequencia(frequenciasComponente);

            var sintese = percentualFrequencia >= ObterFrequenciaMedia(mediaFrequencias, regencia, lancaNota)
                ? "F"
                : "NF";

            return sintese;
        }

        private double ObterPercentualDeFrequencia(IEnumerable<FrequenciaAluno> frequenciaDisciplina)
        {
            return frequenciaDisciplina != null && frequenciaDisciplina.Any()
                ? frequenciaDisciplina.Sum(x => x.PercentualFrequencia) / frequenciaDisciplina.Count()
                : 100;
        }

        private double ObterFrequenciaMedia(IEnumerable<MediaFrequencia> mediaFrequencias, bool regencia,
            bool lancaNota)
        {
            if (regencia || !lancaNota)
                return mediaFrequencias.FirstOrDefault(mf =>
                    mf.Tipo == TipoParametroSistema.CompensacaoAusenciaPercentualRegenciaClasse).Media;
            else
                return mediaFrequencias
                    .FirstOrDefault(mf => mf.Tipo == TipoParametroSistema.CompensacaoAusenciaPercentualFund2).Media;
        }

        private (int conceitoId, string conceito) ConverterNotaParaConceito(decimal nota)
        {
            if (nota < 5)
                return (3, "NS");

            if (nota < 7)
                return (2, "S");

            return (1, "P");
        }
    }
}