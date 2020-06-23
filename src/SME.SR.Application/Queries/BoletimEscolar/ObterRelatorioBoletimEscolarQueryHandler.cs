using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterRelatorioBoletimEscolarQueryHandler : IRequestHandler<ObterRelatorioBoletimEscolarQuery, RelatorioBoletimEscolarDto>
    {
        private IMediator _mediator;

        public ObterRelatorioBoletimEscolarQueryHandler(IMediator mediator)
        {
            this._mediator = mediator;
        }

        public async Task<RelatorioBoletimEscolarDto> Handle(ObterRelatorioBoletimEscolarQuery request, CancellationToken cancellationToken)
        {
            BoletimEscolarDto relatorio = new BoletimEscolarDto();

            if (!string.IsNullOrWhiteSpace(request.TurmaCodigo))
            {
                Turma dadosTurma = await ObterDadosTurma(request.TurmaCodigo);
                DreUe dreUe = await ObterDadosDreUePorTurma(request.TurmaCodigo);

                // preencher o relatório para os alunos selecionados;
                if (request.AlunosCodigo != null && request.AlunosCodigo.Any())
                {
                    IEnumerable<FechamentoTurma> fechamentos = await ObterFechamentoPorCodigoTurma(request.TurmaCodigo);

                    foreach (string codigoAluno in request.AlunosCodigo)
                    {
                        Aluno dadosAluno = await ObterDadosAluno(request.TurmaCodigo, codigoAluno);

                        BoletimEscolarAlunoDto boletim = await MontarRelatorio(dadosAluno, dadosTurma, dreUe, fechamentos, request.Usuario);

                        relatorio.Boletins.Add(boletim);
                    }
                }
                // preenchero relatório para os alunos da turma;
                else
                {
                    IEnumerable<FechamentoTurma> fechamentos = await ObterFechamentoPorCodigoTurma(request.TurmaCodigo);

                    IEnumerable<Aluno> dadosAlunos = await ObterDadosAlunos(request.TurmaCodigo);

                    foreach (Aluno dadosAluno in dadosAlunos)
                    {
                        try
                        {


                            BoletimEscolarAlunoDto boletim = await MontarRelatorio(dadosAluno, dadosTurma, dreUe, fechamentos, request.Usuario);

                            relatorio.Boletins.Add(boletim);

                        }
                        catch (Exception ex)
                        {

                            throw ex;
                        }
                    }
                }
            }
            // preenchero relatório para os alunos de cada turma da ue;
            else
            {
                IEnumerable<Turma> turmas = await ObterTurmasPorFiltro(request.UeCodigo, request.Modalidade, request.AnoLetivo, request.Semestre);

                if (turmas != null && turmas.Any())
                {
                    foreach (Turma dadosTurma in turmas)
                    {
                        IEnumerable<FechamentoTurma> fechamentos = await ObterFechamentoPorCodigoTurma(request.TurmaCodigo);

                        DreUe dreUe = await ObterDadosDreUePorTurma(dadosTurma.CodigoTurma);

                        IEnumerable<Aluno> dadosAlunos = await ObterDadosAlunos(dadosTurma.CodigoTurma);

                        foreach (Aluno dadosAluno in dadosAlunos)
                        {
                            BoletimEscolarAlunoDto boletim = await MontarRelatorio(dadosAluno, dadosTurma, dreUe, fechamentos, request.Usuario);

                            relatorio.Boletins.Add(boletim);
                        }
                    }
                }
            }

            return new RelatorioBoletimEscolarDto(relatorio);
        }

        private async Task<BoletimEscolarAlunoDto> MontarRelatorio(Aluno dadosAluno, Turma dadosTurma, DreUe dreUe, IEnumerable<FechamentoTurma> fechamentosTurma, Usuario usuario)
        {
            try
            {
                BoletimEscolarAlunoDto boletim = InicializarRelatorioBoletim(dadosTurma, dreUe, dadosAluno);

                List<GrupoMatrizComponenteCurricularDto> grupos = await ProcessarFechamentos(dadosAluno.CodigoAluno.ToString(), dadosTurma, fechamentosTurma, usuario);

                boletim.DescricaoGrupos = string.Join(" | ", grupos.Select(x => $"{x.Nome}: {x.Descricao}").ToArray());
                boletim.TipoNota = await ObterTipoNota(fechamentosTurma.FirstOrDefault().PeriodoEscolar, dadosTurma, dreUe.DreId, dreUe.UeId);
                boletim.Grupos.AddRange(grupos);

                return boletim;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private async Task<string> ObterTipoNota(PeriodoEscolar periodoEscolar, Turma turma,
                                              long dreId, long ueId)
        {
            return await _mediator.Send(new ObterTipoNotaQuery()
            {
                PeriodoEscolar = periodoEscolar,
                Turma = turma
            });
        }

        private static BoletimEscolarAlunoDto InicializarRelatorioBoletim(Turma dadosTurma, DreUe dreUe, Aluno dadosAluno)
        {
            return new BoletimEscolarAlunoDto
            {
                Cabecalho = new BoletimEscolarCabecalhoDto
                {
                    CodigoEol = dadosAluno.CodigoAluno.ToString(),
                    Aluno = dadosAluno.NomeRelatorio,
                    NomeDre = dreUe.DreNome,
                    NomeUe = dreUe.UeNome,
                    NomeTurma = dadosTurma?.NomeRelatorio,
                    Data = DateTime.Now.ToString("dd/MM/yyyy")
                }
            };
        }

        private async Task<List<GrupoMatrizComponenteCurricularDto>> ProcessarFechamentos(string codigoAluno, Turma dadosTurma, IEnumerable<FechamentoTurma> fechamentosTurma, Usuario usuario)
        {
            List<GrupoMatrizComponenteCurricularDto> gruposMatriz = new List<GrupoMatrizComponenteCurricularDto>();

            foreach (var fechamento in fechamentosTurma)
            {
                try
                {
                    var conselhoClasseId = await ObterConselhoPorFechamentoTurmaId(fechamento.Id);

                    await ObterComponentesComNotaAsync(gruposMatriz, codigoAluno, dadosTurma, fechamento, conselhoClasseId, usuario);
                    await ObterComponentesSemNotaAsync(gruposMatriz, codigoAluno, dadosTurma.CodigoTurma, fechamento.PeriodoEscolar?.Bimestre);

                }
                catch (Exception ex)
                {

                    throw ex;
                }

            }

            return gruposMatriz;
        }

        public async Task ObterComponentesComNotaAsync(List<GrupoMatrizComponenteCurricularDto> gruposMatriz, string codigoAluno, Turma dadosTurma, FechamentoTurma fechamento, long conselhoClasseId, Usuario usuario)
        {
            if (fechamento.PeriodoEscolarId.HasValue)
            {
                var dadosComponentesComNota = await _mediator.Send(new ObterDadosComponenteComNotaBimestreQuery()
                {
                    CodigoAluno = codigoAluno,
                    ConselhoClasseId = conselhoClasseId,
                    FechamentoTurmaId = fechamento.Id,
                    PeriodoEscolar = fechamento.PeriodoEscolar,
                    Turma = dadosTurma,
                    Usuario = usuario
                });

                int bimestre = fechamento.PeriodoEscolar.Bimestre;

                foreach (var grupoMatriz in dadosComponentesComNota)
                {
                    var grupoNaLista = gruposMatriz.FirstOrDefault(g => g.Descricao == grupoMatriz.Nome);

                    if (grupoNaLista == null)
                    {
                        grupoNaLista = new GrupoMatrizComponenteCurricularDto()
                        {
                            Id = gruposMatriz.Count() + 1,
                            Nome = $"GRUPO {gruposMatriz.Count() + 1}",
                            Descricao = grupoMatriz.Nome,
                            ComponentesCurriculares = new List<ComponenteCurricularDto>()
                        };

                        gruposMatriz.Add(grupoNaLista);
                    }

                    if (grupoMatriz.ComponenteComNotaRegencia != null)
                    {
                        if (grupoNaLista.ComponenteCurricularRegencia == null)
                            grupoNaLista.ComponenteCurricularRegencia = new ComponenteCurricularRegenciaDto();

                        SetarFrequenciaRegencia(grupoNaLista.ComponenteCurricularRegencia, bimestre,
                                                grupoMatriz.ComponenteComNotaRegencia.Frequencia);

                        foreach (var componenteCurricular in grupoMatriz.ComponenteComNotaRegencia.ComponentesCurriculares)
                        {
                            var componenteNaRegencia = gruposMatriz.FirstOrDefault(g => g.Descricao == grupoMatriz.Nome).
                                                            ComponenteCurricularRegencia?.ComponentesCurriculares?.
                                                            FirstOrDefault(cc => cc.Nome == componenteCurricular.Componente);

                            var notaConceito = componenteCurricular.NotaPosConselho ?? componenteCurricular.NotaConceito;

                            if (componenteNaRegencia != null)
                            {
                                SetarNotaRegencia(componenteNaRegencia, fechamento.PeriodoEscolar.Bimestre, notaConceito);
                            }
                            else
                            {
                                componenteNaRegencia = new ComponenteCurricularRegenciaNotaDto()
                                {
                                    Nome = componenteCurricular.Componente
                                };

                                SetarNotaRegencia(componenteNaRegencia, fechamento.PeriodoEscolar.Bimestre, notaConceito);

                                grupoNaLista.ComponenteCurricularRegencia.ComponentesCurriculares.Add(componenteNaRegencia);
                            }
                        }
                    }

                    foreach (var componenteCurricular in grupoMatriz.ComponentesComNota)
                    {
                        var componenteNoGrupo = gruposMatriz.FirstOrDefault(g => g.Descricao == grupoMatriz.Nome)
                         .ComponentesCurriculares.FirstOrDefault(cc => cc.Nome == componenteCurricular.Componente);

                        var notaConceito = componenteCurricular.NotaPosConselho ?? componenteCurricular.NotaConceito;

                        if (componenteNoGrupo != null)
                        {
                            SetarNotaFrequencia(componenteNoGrupo, fechamento.PeriodoEscolar.Bimestre,
                                                componenteCurricular.Frequencia, notaConceito);
                        }
                        else
                        {
                            componenteNoGrupo = new ComponenteCurricularDto()
                            {
                                Nome = componenteCurricular.Componente
                            };

                            SetarNotaFrequencia(componenteNoGrupo, fechamento.PeriodoEscolar.Bimestre,
                                                componenteCurricular.Frequencia, notaConceito);

                            grupoNaLista.ComponentesCurriculares.Add(componenteNoGrupo);
                        }
                    }
                }
            }
            else
            {
                var dadosComponentesComNota = await _mediator.Send(new ObterDadosComponenteComNotaFinalQuery()
                {
                    CodigoAluno = codigoAluno,
                    ConselhoClasseId = conselhoClasseId,
                    FechamentoTurmaId = fechamento.Id,
                    PeriodoEscolar = fechamento.PeriodoEscolar,
                    Turma = dadosTurma,
                    Usuario = usuario
                });

                foreach (var grupoMatriz in dadosComponentesComNota)
                {
                    if(grupoMatriz.ComponentesComNotaRegencia != null)
                    {
                        gruposMatriz.FirstOrDefault(c => c.Descricao == grupoMatriz.Nome).
                            ComponenteCurricularRegencia.FrequenciaFinal =
                             grupoMatriz.ComponentesComNotaRegencia.Frequencia.ToString();

                        foreach (var componenteCurricular in grupoMatriz.ComponentesComNotaRegencia.ComponentesCurriculares)
                        {
                            gruposMatriz.FirstOrDefault(c => c.Descricao == grupoMatriz.Nome).
                             ComponenteCurricularRegencia.ComponentesCurriculares.FirstOrDefault(cc =>
                                                       cc.Nome == componenteCurricular.Componente)
                                                       .NotaFinal = componenteCurricular.NotaFinal;
                        }
                    }

                    foreach (var componenteCurricular in grupoMatriz.ComponentesComNota)
                    {
                        gruposMatriz.FirstOrDefault(c => c.Descricao == grupoMatriz.Nome).
                            ComponentesCurriculares.FirstOrDefault(cc =>
                                                   cc.Nome == componenteCurricular.Componente)
                                                   .NotaFinal = componenteCurricular.NotaFinal;

                        gruposMatriz.FirstOrDefault(c => c.Descricao == grupoMatriz.Nome).
                            ComponentesCurriculares.FirstOrDefault(cc =>
                                                   cc.Nome == componenteCurricular.Componente)
                                                   .FrequenciaFinal = componenteCurricular.Frequencia.ToString();
                    }
                }
            }
        }

        public async Task ObterComponentesSemNotaAsync(List<GrupoMatrizComponenteCurricularDto> gruposMatriz, string codigoAluno, string codigoTurma, int? bimestre)
        {
            if (bimestre.HasValue)
            {
                var dadosComponentesSemNota = await _mediator.Send(new ObterDadosComponenteSemNotaBimestreQuery()
                {
                    Bimestre = bimestre,
                    CodigoTurma = codigoTurma,
                    CodigoAluno = codigoAluno
                });

                foreach (var grupoMatriz in dadosComponentesSemNota)
                {
                    var grupoNaLista = gruposMatriz.FirstOrDefault(g => g.Descricao == grupoMatriz.Nome);

                    if (grupoNaLista == null)
                    {
                        grupoNaLista = new GrupoMatrizComponenteCurricularDto()
                        {
                            Id = gruposMatriz.Count() + 1,
                            Nome = $"GRUPO {gruposMatriz.Count() + 1}",
                            Descricao = grupoMatriz.Nome,
                            ComponentesCurriculares = new List<ComponenteCurricularDto>()
                        };

                        gruposMatriz.Add(grupoNaLista);
                    }

                    foreach (var componenteCurricular in grupoMatriz.ComponentesSemNota)
                    {
                        var componenteNoGrupo = gruposMatriz.FirstOrDefault(g => g.Descricao == grupoMatriz.Nome)
                         .ComponentesCurriculares.FirstOrDefault(cc => cc.Nome == componenteCurricular.Componente);

                        if (componenteNoGrupo != null)
                        {
                            SetarNotaFrequencia(componenteNoGrupo, bimestre.Value,
                                                componenteCurricular.Frequencia, null);
                        }
                        else
                        {
                            componenteNoGrupo = new ComponenteCurricularDto()
                            {
                                Nome = componenteCurricular.Componente
                            };

                            SetarNotaFrequencia(componenteNoGrupo, bimestre.Value,
                                                componenteCurricular.Frequencia, null);

                            grupoNaLista.ComponentesCurriculares.Add(componenteNoGrupo);
                        }
                    }
                }
            }
            else
            {
                var dadosComponentesSemNota = await _mediator.Send(new ObterDadosComponenteSemNotaFinalQuery()
                {
                    Bimestre = bimestre,
                    CodigoTurma = codigoTurma,
                    CodigoAluno = codigoAluno
                });

                foreach (var grupoMatriz in dadosComponentesSemNota)
                {
                    foreach (var componenteCurricular in grupoMatriz.ComponentesSemNota)
                    {
                        gruposMatriz.FirstOrDefault(c => c.Descricao == grupoMatriz.Nome).
                                ComponentesCurriculares.FirstOrDefault(cc =>
                                              cc.Nome == componenteCurricular.Componente)
                                              .NotaFinal = componenteCurricular.Parecer
                                              .Equals("Frequente") ? "F" : "NF";

                        gruposMatriz.FirstOrDefault(c => c.Descricao == grupoMatriz.Nome).
                               ComponentesCurriculares.FirstOrDefault(cc =>
                                             cc.Nome == componenteCurricular.Componente)
                                             .FrequenciaFinal = componenteCurricular.Frequencia.ToString();
                    }
                }
            }
        }

        private void SetarNotaFrequencia(ComponenteCurricularDto grupoComponente, int bimestre, double? frequencia, string notaConceito)
        {
            PropertyInfo propriedadeComponenteCurricular = grupoComponente.GetType().GetProperty($"FrequenciaBimestre{bimestre}");
            propriedadeComponenteCurricular.SetValue(grupoComponente, Convert.ChangeType(frequencia, propriedadeComponenteCurricular.PropertyType), null);

            propriedadeComponenteCurricular = grupoComponente.GetType().GetProperty($"NotaBimestre{bimestre}");
            propriedadeComponenteCurricular.SetValue(grupoComponente, Convert.ChangeType(notaConceito, propriedadeComponenteCurricular.PropertyType), null);
        }

        private void SetarNotaRegencia(ComponenteCurricularRegenciaNotaDto componenteNaRegencia, int bimestre, string notaConceito)
        {
            PropertyInfo propriedadeComponenteCurricular = componenteNaRegencia.GetType().GetProperty($"NotaBimestre{bimestre}");
            propriedadeComponenteCurricular.SetValue(componenteNaRegencia, Convert.ChangeType(notaConceito, propriedadeComponenteCurricular.PropertyType), null);
        }

        private void SetarFrequenciaRegencia(ComponenteCurricularRegenciaDto grupoComponente, int bimestre, double? frequencia)
        {
            PropertyInfo propriedadeComponenteCurricular = grupoComponente.GetType().GetProperty($"FrequenciaBimestre{bimestre}");
            propriedadeComponenteCurricular.SetValue(grupoComponente, Convert.ChangeType(frequencia, propriedadeComponenteCurricular.PropertyType), null);

        }

        private async Task<Aluno> ObterDadosAluno(string codigoTurma, string codigoAluno)
        {
            return await _mediator.Send(new ObterDadosAlunoQuery
            {
                CodigoTurma = codigoTurma,
                CodigoAluno = codigoAluno
            });
        }

        private async Task<IEnumerable<Aluno>> ObterDadosAlunos(string codigoTurma)
        {
            IEnumerable<Aluno> alunos = await _mediator.Send(new ObterAlunosPorTurmaQuery
            {
                TurmaCodigo = codigoTurma
            });

            return alunos;
        }

        private async Task<Turma> ObterDadosTurma(string codigoTurma)
        {
            Turma turma = await _mediator.Send(new ObterTurmaQuery { CodigoTurma = codigoTurma });

            return turma;
        }

        private async Task<DreUe> ObterDadosDreUePorTurma(string codigoTurma)
        {
            DreUe dreUe = await _mediator.Send(new ObterDreUePorTurmaQuery { CodigoTurma = codigoTurma });

            return dreUe;
        }

        private async Task<IEnumerable<Turma>> ObterTurmasPorFiltro(string codigoUe, Modalidade? modalidade, int? anoLetivo, int? semestre)
        {
            IEnumerable<Turma> turmas = await _mediator.Send(
                new ObterTurmasPorFiltroQuery
                {
                    CodigoUe = codigoUe,
                    Modalidade = modalidade,
                    Semestre = semestre,
                    AnoLetivo = anoLetivo
                }
             );

            return turmas;
        }

        private async Task<IEnumerable<FechamentoTurma>> ObterFechamentoPorCodigoTurma(string turmaCodigo)
        {
            IEnumerable<FechamentoTurma> fechamento = await _mediator.Send(new ObterFechamentosPorCodigoTurmaQuery
            {
                TurmaCodigo = turmaCodigo,
            });

            return fechamento;
        }

        private async Task<long> ObterConselhoPorFechamentoTurmaId(long fechamentoTurmaId)
        {
            long fechamento = await _mediator.Send(new ObterConselhoClassePorFechamentoTurmaIdQuery
            {
                FechamentoTurmaId = fechamentoTurmaId,
            });

            return fechamento;
        }
    }
}
