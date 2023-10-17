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
        MontarBoletinsDetalhadosQueryHandler : IRequestHandler<MontarBoletinsDetalhadosQuery,BoletimEscolarDetalhadoDto>
    {
        private const double FREQUENCIA_100 = 100;
        private const int BIMESTRE_1 = 1;
        private const int BIMESTRE_2 = 2;
        private const int BIMESTRE_3 = 3;
        private const int BIMESTRE_4 = 4;
        private const int BIMESTRE_FINAL = 0;
        private const int PERCENTUAL_FREQUENCIA_PRECISAO = 2;
        private readonly IMediator mediator;
        private readonly ITipoCalendarioRepository tipoCalendarioRepository;

        private string frequencia100Formatada = FREQUENCIA_100.ToString($"N{PERCENTUAL_FREQUENCIA_PRECISAO}", CultureInfo.CurrentCulture);

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
                    var turma = turmas.FirstOrDefault(t => aluno.Any(a => t.TipoTurma == TipoTurma.Regular && VerificaTurmaEnsinoMedioRegular(t.EtapaEnsino) && a.CodigoTurma.ToString() == t.Codigo));

                    if(turma == null)
                        turma = turmas.First(t => aluno.Any(a => a.CodigoTurma.ToString() == t.Codigo));


                    var conselhoClasseBimestres = await mediator.Send(new AlunoConselhoClasseCadastradoBimestresQuery(aluno.Key, turma.AnoLetivo, turma.ModalidadeCodigo, turma.Semestre, turma.Codigo));

                    if (conselhoClasseBimestres != null && conselhoClasseBimestres.Any())
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
                                boletimEscolarAlunoDto, turmas.Any(turma => turma.EhEja));
                            }
                        }

                        var notasAluno = notas.FirstOrDefault(t => t.Key == aluno.First().CodigoAluno.ToString());
                        var frequenciasAluno =
                            frequencia?.FirstOrDefault(t => t.Key == aluno.First().CodigoAluno.ToString());
                        var frequenciasTurma = frequencia?.SelectMany(a => a).Where(f => f.TurmaId == turma.Codigo);

                    if (notasAluno != null && notasAluno.Any())
                        SetarNotasFrequencia(boletimEscolarAlunoDto, notasAluno, frequenciasAluno, frequenciasTurma, mediasFrequencia, registroFrequencia, periodoAtual, turmas, conselhoClasseBimestres);

                        var frequeciaGlobal =
                            frequenciasGlobal?.FirstOrDefault(t => t.Key == aluno.First().CodigoAluno.ToString());
                        var percentualFrequenciaGlobalFormatado = frequeciaGlobal?.First()?.PercentualFrequenciaFormatado;
                        var parecerConclusivo = pareceresConclusivos.FirstOrDefault(c =>
                            c.TurmaId.ToString() == turma.Codigo && c.AlunoCodigo.ToString() == aluno.Key);
                        var recomendacao = recomendacoes?.FirstOrDefault(r =>
                            r.TurmaCodigo == turma.Codigo && r.AlunoCodigo == aluno.Key);
                        var ciclo = ciclos.FirstOrDefault(c =>
                            c.Modalidade == turma.ModalidadeCodigo && c.Ano == turma.Ano);
                        var foto = fotos.FirstOrDefault(c => c.CodigoAluno.ToString() == aluno.Key);

                        boletimEscolarAlunoDto.Cabecalho = ObterCabecalhoInicial(dre, ue, ciclo, turma, aluno.Key, foto,
                            aluno.FirstOrDefault(b => 
                            b.CodigoTurma.ToString() == turma.Codigo).NomeRelatorio,
                            aluno.FirstOrDefault().ObterNomeFinal(),
                            String.IsNullOrEmpty(percentualFrequenciaGlobalFormatado) ? percentualFrequenciaGlobalFormatado : $"{percentualFrequenciaGlobalFormatado}%",
                            request.AnoLetivo);
                        boletimEscolarAlunoDto.ParecerConclusivo = parecerConclusivo?.ParecerConclusivo;
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

        private bool VerificaTurmaEnsinoMedioRegular(int etapa)
        {
            var etapaEnsinoMedioRegularEnsinoMedio = EtapaEnsino.EnsinoMedio;
            var etapaEnsinoMedioRegularMagisterio = EtapaEnsino.Magisterio;
            var etapaEnsinoMedioRegularEnsinoMedioEspecial = EtapaEnsino.EnsinoMedioEspecial;

            if (etapa.Equals((int)etapaEnsinoMedioRegularEnsinoMedio) || etapa.Equals((int)etapaEnsinoMedioRegularMagisterio) || etapa.Equals((int)etapaEnsinoMedioRegularEnsinoMedioEspecial))
                return true;

            return false;
        }

        private async Task MapearGruposEComponentes(
            IEnumerable<ComponenteCurricularPorTurma> componentesCurricularesPorTurma,
            BoletimEscolarDetalhadoAlunoDto boletim,
            bool ehEja)
        {
            var componentesOrdenados =
                await mediator.Send(
                    new OrdenarComponentesPorGrupoMatrizAreaConhecimentoQuery(componentesCurricularesPorTurma));

            var gruposMatrizes = componentesCurricularesPorTurma.OrderBy(cc => cc.GrupoMatriz.Id).GroupBy(cc => cc.GrupoMatriz).ToList();

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
                if (grupo.ComponentesCurriculares == null)
                    grupo.ComponentesCurriculares = new List<ComponenteCurricularDto>();

                var grupoMatrizAgrupadosAreaOrdem = grupoMatriz.Any(g=> g.TerritorioSaber)
                                                    ? grupoMatriz.OrderBy(g=> g.DescricaoCompletaTerritorio).GroupBy(a => a.AreaDoConhecimento.Ordem != null)
                                                    : grupoMatriz.GroupBy(a => a.AreaDoConhecimento.Ordem != null);

                foreach (var gruposOrdem in grupoMatrizAgrupadosAreaOrdem.OrderByDescending(g=> g.Key))
                {
                    foreach (var componente in gruposOrdem.OrderBy(a => a.Disciplina))
                    {
                        if (componente.Regencia && componente.ComponentesCurricularesRegencia != null &&
                            componente.ComponentesCurricularesRegencia.Any())
                        {
                            var componenteCurricular = new ComponenteCurricularRegenciaDto()
                            {
                                Codigo = componente.CodDisciplina.ToString(),
                                Frequencia = componente.Frequencia
                            };

                            foreach (var componenteRegencia in componente.ComponentesCurricularesRegencia.OrderBy(a =>
                                a.Disciplina))
                            {
                                if (!componenteCurricular.ComponentesCurriculares.Any(g =>
                                    g.Codigo == componenteRegencia.CodDisciplina.ToString()))
                                    componenteCurricular.ComponentesCurriculares.Add(
                                        new ComponenteCurricularRegenciaNotaDto()
                                        {
                                            Codigo = componenteRegencia.CodDisciplina.ToString(),
                                            Nome = componenteRegencia.Disciplina,
                                            Nota = componenteRegencia.LancaNota
                                        });
                            }

                            if (ehEja)
                                boletim.ComponenteCurricularRegencia = componenteCurricular;
                            else
                                grupo.ComponenteCurricularRegencia = componenteCurricular;
                        }
                        else if (!componente.Regencia)
                        {
                            if (!grupo.ComponentesCurriculares.Any(g => !g.TerritorioSaber && g.Codigo == componente.CodDisciplina.ToString()) ||
                                !grupo.ComponentesCurriculares.Any(g => g.TerritorioSaber && g.Codigo == componente.CodDisciplina.ToString() && g.Professor == componente.Professor))
                            {
                                grupo.ComponentesCurriculares.Add(
                                    new ComponenteCurricularDto()
                                    {
                                        Codigo = componente.CodDisciplina.ToString(),
                                        CodigoTerritorioSaber = componente.CodigoTerritorioSaber.ToString(),
                                        Nome = componente.Disciplina,
                                        Nota = componente.LancaNota,
                                        Frequencia = componente.Frequencia,
                                        TerritorioSaber = componente.TerritorioSaber,
                                        Professor = componente.Professor
                                    });
                            }                                
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
            IEnumerable<TurmaComponenteQtdAulasDto> registroFrequencia, int periodoAtual,
            IEnumerable<Turma> turmas, IEnumerable<int> conselhoClasseBimestres)
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

                    boletimEscolar.ComponenteCurricularRegencia.FrequenciaBimestre1 = ObterFrequenciaBimestre(frequenciasAlunoRegencia, BIMESTRE_1, aulasCadastradas, periodoAtual, conselhoClasseBimestres);
                    boletimEscolar.ComponenteCurricularRegencia.FrequenciaBimestre2 = ObterFrequenciaBimestre(frequenciasAlunoRegencia, BIMESTRE_2, aulasCadastradas, periodoAtual, conselhoClasseBimestres);
                    boletimEscolar.ComponenteCurricularRegencia.FrequenciaBimestre3 = ObterFrequenciaBimestre(frequenciasAlunoRegencia, BIMESTRE_3, aulasCadastradas, periodoAtual, conselhoClasseBimestres);
                    boletimEscolar.ComponenteCurricularRegencia.FrequenciaBimestre4 = ObterFrequenciaBimestre(frequenciasAlunoRegencia, BIMESTRE_4, aulasCadastradas, periodoAtual, conselhoClasseBimestres);
                    boletimEscolar.ComponenteCurricularRegencia.FrequenciaFinal = ObterFrequenciaFinalAluno(frequenciasAlunoRegencia, frequenciasTurmaRegencia, BIMESTRE_FINAL, conselhoClasseBimestres);
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

                        componenteCurricular.NotaBimestre1 = ObterNotaBimestre(notaFrequenciaComponenteComPeriodo, BIMESTRE_1, periodoAtual, conselhoClasseBimestres);
                        componenteCurricular.NotaBimestre2 = ObterNotaBimestre(notaFrequenciaComponenteComPeriodo, BIMESTRE_2, periodoAtual, conselhoClasseBimestres);
                        componenteCurricular.NotaBimestre3 = ObterNotaBimestre(notaFrequenciaComponenteComPeriodo, BIMESTRE_3, periodoAtual, conselhoClasseBimestres);
                        componenteCurricular.NotaBimestre4 = ObterNotaBimestre(notaFrequenciaComponenteComPeriodo, BIMESTRE_4, periodoAtual, conselhoClasseBimestres);
                        componenteCurricular.NotaFinal = ObterNotaBimestreFinal(notaFrequenciaComponente, BIMESTRE_FINAL, conselhoClasseBimestres);

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
                            frequenciasAluno?.Where(f => f.DisciplinaId == componenteCurricular.Codigo || f.DisciplinaId == componenteCurricular.CodigoTerritorioSaber
                             && (!componenteCurricular.TerritorioSaber 
                                  || (componenteCurricular.TerritorioSaber && !string.IsNullOrEmpty(componenteCurricular.Professor) ? f.Professor == componenteCurricular.Professor : true)));
                      
                        var frequenciasTurmaComponente =
                            frequenciasTurma?.Where(f => f.DisciplinaId == componenteCurricular.Codigo);

                        if (componenteCurricular.Nota)
                        {
                            var notasComponente = ObterNotasAluno(componenteCurricular.Codigo, notas, turmas);

                            var notasComponenteComPeriodoEscolar =
                                notasComponente?.Where(n => n.PeriodoEscolar != null) ?? null;

                            componenteCurricular.NotaBimestre1 = ObterNotaBimestre(notasComponenteComPeriodoEscolar, BIMESTRE_1, periodoAtual, conselhoClasseBimestres);
                            componenteCurricular.NotaBimestre2 = ObterNotaBimestre(notasComponenteComPeriodoEscolar, BIMESTRE_2, periodoAtual, conselhoClasseBimestres);
                            componenteCurricular.NotaBimestre3 = ObterNotaBimestre(notasComponenteComPeriodoEscolar, BIMESTRE_3, periodoAtual, conselhoClasseBimestres);
                            componenteCurricular.NotaBimestre4 = ObterNotaBimestre(notasComponenteComPeriodoEscolar, BIMESTRE_4, periodoAtual, conselhoClasseBimestres);
                            componenteCurricular.NotaFinal = ObterNotaBimestreFinal(notasComponente, BIMESTRE_FINAL, conselhoClasseBimestres);
                            
                            if (!string.IsNullOrEmpty(componenteCurricular.NotaFinal))
                                boletimEscolar.PossuiNotaFinal = true;
                        }
                        else
                            componenteCurricular.NotaFinal = ObterSintese(frequenciasAlunoComponente, mediasFrequencia, false, false, BIMESTRE_FINAL, conselhoClasseBimestres);

                        if (componenteCurricular.Frequencia)
                        {
                            var aulasCadastradas = registroFrequencia?.Where(f =>
                                f.ComponenteCurricularCodigo == componenteCurricular.Codigo && (!componenteCurricular.TerritorioSaber || (componenteCurricular.TerritorioSaber && f.Professor == componenteCurricular.Professor)));

                            componenteCurricular.FrequenciaBimestre1 = ObterFrequenciaBimestre(frequenciasAlunoComponente, BIMESTRE_1, aulasCadastradas, periodoAtual, conselhoClasseBimestres);
                            componenteCurricular.FrequenciaBimestre2 = ObterFrequenciaBimestre(frequenciasAlunoComponente, BIMESTRE_2, aulasCadastradas, periodoAtual, conselhoClasseBimestres);
                            componenteCurricular.FrequenciaBimestre3 = ObterFrequenciaBimestre(frequenciasAlunoComponente, BIMESTRE_3, aulasCadastradas, periodoAtual, conselhoClasseBimestres);
                            componenteCurricular.FrequenciaBimestre4 = ObterFrequenciaBimestre(frequenciasAlunoComponente, BIMESTRE_4, aulasCadastradas, periodoAtual, conselhoClasseBimestres);

                            componenteCurricular.FrequenciaFinal = ObterFrequenciaFinalAluno(frequenciasAlunoComponente, frequenciasTurmaComponente, BIMESTRE_FINAL, conselhoClasseBimestres);

                            if (!componenteCurricular.Nota)
                                componenteCurricular.NotaFinal = ObterSintese(frequenciasAlunoComponente,mediasFrequencia, false, false, BIMESTRE_FINAL, conselhoClasseBimestres);
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
                if (grupoMatriz.ComponenteCurricularRegencia != null)
                {
                    if (grupoMatriz.ComponenteCurricularRegencia.Frequencia)
                    {
                        var frequenciasAlunoRegencia = frequenciasAluno?.Where(f =>
                            f.DisciplinaId == grupoMatriz.ComponenteCurricularRegencia.Codigo);
                        var frequenciasTurmaRegencia = frequenciasTurma?.Where(f =>
                            f.DisciplinaId == grupoMatriz.ComponenteCurricularRegencia.Codigo);
                        var aulasCadastradas = registroFrequencia?.Where(f =>
                            f.ComponenteCurricularCodigo == grupoMatriz.ComponenteCurricularRegencia.Codigo);

                        grupoMatriz.ComponenteCurricularRegencia.FrequenciaBimestre1 = ObterFrequenciaBimestre(frequenciasAlunoRegencia, BIMESTRE_1, aulasCadastradas, periodoAtual, conselhoClasseBimestres);
                        grupoMatriz.ComponenteCurricularRegencia.FrequenciaBimestre2 = ObterFrequenciaBimestre(frequenciasAlunoRegencia, BIMESTRE_2, aulasCadastradas, periodoAtual, conselhoClasseBimestres);
                        grupoMatriz.ComponenteCurricularRegencia.FrequenciaBimestre3 = ObterFrequenciaBimestre(frequenciasAlunoRegencia, BIMESTRE_3, aulasCadastradas, periodoAtual, conselhoClasseBimestres);
                        grupoMatriz.ComponenteCurricularRegencia.FrequenciaBimestre4 = ObterFrequenciaBimestre(frequenciasAlunoRegencia, BIMESTRE_4, aulasCadastradas, periodoAtual, conselhoClasseBimestres);
                        grupoMatriz.ComponenteCurricularRegencia.FrequenciaFinal = ObterFrequenciaFinalAluno(frequenciasAlunoRegencia, frequenciasTurmaRegencia, BIMESTRE_FINAL, conselhoClasseBimestres);
                    }



                    foreach (var componenteCurricular in
                         grupoMatriz.ComponenteCurricularRegencia.ComponentesCurriculares)
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
                                    nf.CodigoComponenteCurricular == grupoMatriz.ComponenteCurricularRegencia.Codigo);
                            }

                            componenteCurricular.NotaBimestre1 = ObterNotaBimestre(notaFrequenciaComponenteComPeriodo, BIMESTRE_1, periodoAtual, conselhoClasseBimestres);
                            componenteCurricular.NotaBimestre2 = ObterNotaBimestre(notaFrequenciaComponenteComPeriodo, BIMESTRE_2, periodoAtual, conselhoClasseBimestres);
                            componenteCurricular.NotaBimestre3 = ObterNotaBimestre(notaFrequenciaComponenteComPeriodo, BIMESTRE_3, periodoAtual, conselhoClasseBimestres);
                            componenteCurricular.NotaBimestre4 = ObterNotaBimestre(notaFrequenciaComponenteComPeriodo, BIMESTRE_4, periodoAtual, conselhoClasseBimestres);
                            componenteCurricular.NotaFinal = ObterNotaBimestreFinal(notaFrequenciaComponente, BIMESTRE_FINAL, conselhoClasseBimestres);



                            if (!string.IsNullOrEmpty(componenteCurricular.NotaFinal))
                                boletimEscolar.PossuiNotaFinalRegencia = true;
                        }
                    }
                }
            }
        }

        private IEnumerable<NotasAlunoBimestre> ObterNotasAluno(
                                                                string codigoComponente,
                                                                IEnumerable<NotasAlunoBimestre> notas,
                                                                IEnumerable<Turma> turmas)
        {
            if (TurmaEhEjaEdFisica(codigoComponente, turmas))
            {
                var codigoTurmaRegular = turmas.FirstOrDefault(t => t.EhEja && t.TipoTurma == TipoTurma.EdFisica)?.RegularCodigo;

                return notas?.Where(n => n.CodigoTurma == codigoTurmaRegular && n.CodigoComponenteCurricular == codigoComponente) ?? null;
            }

            return notas?.Where(n => n.CodigoComponenteCurricular == codigoComponente) ?? null;
        }

        private bool TurmaEhEjaEdFisica(string codigoComponente, IEnumerable<Turma> turmas)
        {
            const string ED_FISICA = "6";

            return codigoComponente == ED_FISICA && turmas.Any(t => t.EhEja && t.TipoTurma == TipoTurma.EdFisica);
        }

        private string ObterNotaBimestre(IEnumerable<NotasAlunoBimestre> notasComponente, int bimestre, int periodoAtual, IEnumerable<int> conselhoClasseBimestres)
        {
            var notaConceito =!VerificaPossuiConselho(conselhoClasseBimestres, bimestre) ? new NotasAlunoBimestre().NotaConceito : notasComponente
                    ?.FirstOrDefault(nf => nf.PeriodoEscolar != null && nf.PeriodoEscolar.Bimestre == bimestre)
                    ?.NotaConceito;

            if (bimestre > periodoAtual && string.IsNullOrEmpty(notaConceito?.NotaConceito) && notasComponente.Any(x => x.PeriodoEscolar.PeriodoInicio.Year == DateTime.Now.Year))
                return "-";

            return notaConceito?.NotaConceito;
        }

        private string ObterNotaBimestreFinal(IEnumerable<NotasAlunoBimestre> notasComponente, int bimestre, IEnumerable<int> conselhoClasseBimestres)
        {
            var nota = !VerificaPossuiConselho(conselhoClasseBimestres, bimestre) ? "" : notasComponente
                .FirstOrDefault(nf => nf.PeriodoEscolar.Bimestre == bimestre)?.NotaConceito?.NotaConceito;

            if (!notasComponente.All(nc => nc.NotaConceito.Nota.HasValue) && decimal.TryParse(nota, out decimal valor))
                nota = ConverterNotaParaConceito(decimal.Parse(nota)).conceito;

            return !string.IsNullOrEmpty(nota) ? nota : "-";
        }

        private string ObterFrequenciaBimestre(IEnumerable<FrequenciaAluno> frequenciasAlunoComponente, int bimestre, IEnumerable<TurmaComponenteQtdAulasDto> aulasCadastradas, int periodoAtual, IEnumerable<int> conselhoClasseBimestres)
        {
            var frequencia = !VerificaPossuiConselho(conselhoClasseBimestres,bimestre) 
                ? ""
                : frequenciasAlunoComponente?.FirstOrDefault(nf => nf.Bimestre == bimestre)?.PercentualFrequenciaFormatado ?? string.Empty;

            if (!String.IsNullOrEmpty(frequencia))
                frequencia = frequencia += "%";

            if (bimestre > periodoAtual && String.IsNullOrEmpty(frequencia))
                frequencia = "-";

            return frequencia;
        }

        private string ObterFrequenciaFinalAluno(IEnumerable<FrequenciaAluno> frequenciasAluno,IEnumerable<FrequenciaAluno> frequenciasTurma, int bimestre, IEnumerable<int> conselhoClasseBimestres)
        {
            if (frequenciasAluno == null || !frequenciasAluno.Any())
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

                    return frequenciaFinal.PercentualFrequenciaFinal?.ToString($"N{PERCENTUAL_FREQUENCIA_PRECISAO}", CultureInfo.CurrentCulture);
                }
                if (!VerificaPossuiConselho(conselhoClasseBimestres, bimestre))
                    return "";
                return frequenciaFinal.PercentualFrequenciaFormatado;
            }
        }

        private string ObterSintese(IEnumerable<FrequenciaAluno> frequenciasComponente,
            IEnumerable<MediaFrequencia> mediaFrequencias, bool regencia, bool lancaNota, int bimestre, IEnumerable<int> conselhoClasseBimestres)
        {
            if (!VerificaPossuiConselho(conselhoClasseBimestres, bimestre))
                return "-";
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
        private bool VerificaPossuiConselho(IEnumerable<int> conselhoClasseBimestres, int bimestre)
        {
            return conselhoClasseBimestres.Any(a => a == bimestre);
        }
    }
}