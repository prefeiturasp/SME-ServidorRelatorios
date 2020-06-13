using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using SME.SR.Infra.Dtos.Relatorios.BoletimEscolar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application.Queries.BoletimEscolar
{
    public class ObterRelatorioBoletimEscolarQueryHandler : IRequestHandler<ObterRelatorioBoletimEscolarQuery, BoletimEscolarDto>
    {
        private IMediator _mediator;

        public ObterRelatorioBoletimEscolarQueryHandler(IMediator mediator)
        {
            this._mediator = mediator;
        }

        public async Task<BoletimEscolarDto> Handle(ObterRelatorioBoletimEscolarQuery request, CancellationToken cancellationToken)
        {
            BoletimEscolarDto relatorio = new BoletimEscolarDto();

            if (!string.IsNullOrWhiteSpace(request.TurmaCodigo))
            {
                Turma dadosTurma = await ObterDadosTurma(request.TurmaCodigo);
                DreUe dreUe = await ObterDadosDreUePorTurma(request.TurmaCodigo);

                // preenchero relatório para os alunos selecionados;
                if (request.AlunosCodigo != null && request.AlunosCodigo.Any())
                {
                    IEnumerable<FechamentoTurma> fechamentos = await ObterFechamentoPorCodigoTurma(request.TurmaCodigo);

                    foreach (string codigoAluno in request.AlunosCodigo)
                    {
                        Aluno dadosAluno = await ObterDadosAluno(request.TurmaCodigo, codigoAluno);

                        BoletimEscolarAlunoDto boletim = await MontarRelatorio(dadosAluno, dadosTurma, dreUe, fechamentos);

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
                        BoletimEscolarAlunoDto boletim = await MontarRelatorio(dadosAluno, dadosTurma, dreUe, fechamentos);

                        relatorio.Boletins.Add(boletim);
                    }
                }
            }
            // preenchero relatório para os alunos de cada turma da ue;
            else
            {
                IEnumerable<Turma> turmas = await ObterTurmasPorUe(request.UeCodigo, request.Modalidade, request.AnoLetivo, request.PeriodoEscolarId); // falta adicionar semestre e ano letivo

                if (turmas != null && turmas.Any())
                {
                    foreach (Turma dadosTurma in turmas)
                    {
                        IEnumerable<FechamentoTurma> fechamentos = await ObterFechamentoPorCodigoTurma(request.TurmaCodigo);

                        DreUe dreUe = await ObterDadosDreUePorTurma(dadosTurma.CodigoTurma);

                        IEnumerable<Aluno> dadosAlunos = await ObterDadosAlunos(dadosTurma.CodigoTurma);

                        foreach (Aluno dadosAluno in dadosAlunos)
                        {
                            BoletimEscolarAlunoDto boletim = await MontarRelatorio(dadosAluno, dadosTurma, dreUe, fechamentos);

                            relatorio.Boletins.Add(boletim);
                        }
                    }
                }
            }

            return relatorio;
        }

        private async Task<BoletimEscolarAlunoDto> MontarRelatorio(Aluno dadosAluno, Turma dadosTurma, DreUe dreUe, IEnumerable<FechamentoTurma> fechamentosTurma)
        {
            BoletimEscolarAlunoDto boletim = InicializarRelatorioBoletim(dadosTurma, dreUe, dadosAluno);

            List<ComponenteCurricularDto> componentes = await ProcessarFechamentos(dadosAluno.CodigoAluno.ToString(), dadosTurma, fechamentosTurma);

            boletim.ComponentesCurriculares.AddRange(componentes);

            return boletim;
        }

        private static BoletimEscolarAlunoDto InicializarRelatorioBoletim(Turma dadosTurma, DreUe dreUe, Aluno dadosAluno)
        {
            return new BoletimEscolarAlunoDto
            {
                Cabecalho = new BoletimEscolarCabecalhoDto
                {
                    Aluno = dadosAluno.NomeAluno,
                    NomeDre = dreUe.Dre,
                    NomeUe = dreUe.Ue,
                    NomeTurma = dadosTurma?.Nome
                }
            };
        }

        private async Task<List<ComponenteCurricularDto>> ProcessarFechamentos(string codigoAluno, Turma dadosTurma, IEnumerable<FechamentoTurma> fechamentosTurma)
        {
            List<ComponenteCurricularDto> componentes = new List<ComponenteCurricularDto>();

            foreach (var fechamento in fechamentosTurma)
            {
                var conselhoClasseId = await ObterConselhoPorFechamentoTurmaId(fechamento.Id);

                componentes.AddRange(await ObterComponentesComNotaAsync(codigoAluno, dadosTurma, fechamento, conselhoClasseId));
                componentes.AddRange(await ObterComponentesSemNotaAsync(codigoAluno, dadosTurma.CodigoTurma, fechamento.PeriodoEscolar.Bimestre));
            }

            return componentes;
        }

        public async Task<IEnumerable<ComponenteCurricularDto>> ObterComponentesComNotaAsync(string codigoAluno, Turma dadosTurma, FechamentoTurma fechamento, long conselhoClasseId)
        {
            List<ComponenteCurricularDto> componentes = new List<ComponenteCurricularDto>();

            if (fechamento.PeriodoEscolarId.HasValue)
            {
                var dadosComponentesComNota = await _mediator.Send(new ObterDadosComponenteComNotaBimestreQuery()
                {
                    CodigoAluno = codigoAluno,
                    ConselhoClasseId = conselhoClasseId,
                    FechamentoTurmaId = fechamento.Id,
                    PeriodoEscolar = fechamento.PeriodoEscolar,
                    Turma = dadosTurma
                });

                foreach (var grupoMatriz in dadosComponentesComNota)
                {
                    foreach (var componenteCurricular in grupoMatriz.ComponentesComNota)
                    {
                        var grupoComponente = componentes.FirstOrDefault(c => c.Grupo == grupoMatriz.Nome &&
                                                                              c.Nome == componenteCurricular.Componente);

                        var notaConceito = componenteCurricular.NotaPosConselho ?? componenteCurricular.NotaConceito;

                        if (grupoComponente != null)
                        {
                            SetarNotaFrequencia(grupoComponente, fechamento.PeriodoEscolar.Bimestre,
                                                componenteCurricular.Frequencia, notaConceito);
                        }
                        else
                        {
                            grupoComponente = new ComponenteCurricularDto()
                            {
                                Grupo = grupoMatriz.Nome,
                                Nome = componenteCurricular.Componente
                            };

                            SetarNotaFrequencia(grupoComponente, fechamento.PeriodoEscolar.Bimestre,
                                                componenteCurricular.Frequencia, notaConceito);

                            componentes.Add(grupoComponente);
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
                    Turma = dadosTurma
                });

                foreach (var grupoMatriz in dadosComponentesComNota)
                {
                    foreach (var componenteCurricular in grupoMatriz.ComponentesComNota)
                    {
                        componentes.FirstOrDefault(c => c.Grupo == grupoMatriz.Nome &&
                                                   c.Nome == componenteCurricular.Componente)
                                                   .NotaFinal = componenteCurricular.NotaFinal;
                    }
                }
            }

            return componentes;
        }

        public async Task<IEnumerable<ComponenteCurricularDto>> ObterComponentesSemNotaAsync(string codigoAluno, string codigoTurma, int? bimestre)
        {
            List<ComponenteCurricularDto> componentes = new List<ComponenteCurricularDto>();

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
                    foreach (var componenteCurricular in grupoMatriz.ComponentesSemNota)
                    {
                        var grupoComponente = componentes.FirstOrDefault(c => c.Grupo == grupoMatriz.Nome &&
                                                                              c.Nome == componenteCurricular.Componente);

                        if (grupoComponente != null)
                        {
                            SetarNotaFrequencia(grupoComponente, bimestre.Value,
                                                componenteCurricular.Frequencia, null);
                        }
                        else
                        {
                            grupoComponente = new ComponenteCurricularDto()
                            {
                                Grupo = grupoMatriz.Nome,
                                Nome = componenteCurricular.Componente
                            };

                            SetarNotaFrequencia(grupoComponente, bimestre.Value,
                                                componenteCurricular.Frequencia, null);

                            componentes.Add(grupoComponente);
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
                        componentes.FirstOrDefault(c => c.Grupo == grupoMatriz.Nome &&
                                                   c.Nome == componenteCurricular.Componente)
                                                   .NotaFinal = componenteCurricular.Parecer;
                    }
                }
            }

            return componentes;
        }

        private void SetarNotaFrequencia(ComponenteCurricularDto grupoComponente, int bimestre, double? frequencia, string notaConceito)
        {
            PropertyInfo propriedadeComponenteCurricular = grupoComponente.GetType().GetProperty($"FrequenciaBimestre{bimestre}");
            propriedadeComponenteCurricular.SetValue(grupoComponente, Convert.ChangeType(frequencia, propriedadeComponenteCurricular.PropertyType), null);

            propriedadeComponenteCurricular = grupoComponente.GetType().GetProperty($"NotaBimestre{bimestre}");
            propriedadeComponenteCurricular.SetValue(grupoComponente, Convert.ChangeType(notaConceito, propriedadeComponenteCurricular.PropertyType), null);
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

        private async Task<IEnumerable<Turma>> ObterTurmasPorUe(string codigoUe, Modalidade? modalidade, int? anoLetivo, long? periodoEscolarId)
        {
            IEnumerable<Turma> turmas = await _mediator.Send(new ObterTurmasPorUeQuery { CodigoUe = codigoUe });

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
