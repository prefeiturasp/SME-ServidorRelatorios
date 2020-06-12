using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using SME.SR.Infra.Dtos.Relatorios.BoletimEscolar;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application.Queries.BoletimEscolar
{
    public class ObterRelatorioBoletimEscolarQueryHandler : IRequestHandler<ObterRelatorioBoletimEscolarQuery, BoletimEscolarDto>
    {
        private IMediator _mediator;

        public async Task<BoletimEscolarDto> Handle(ObterRelatorioBoletimEscolarQuery request, CancellationToken cancellationToken)
        {
            BoletimEscolarDto relatorio = new BoletimEscolarDto();

            if (!string.IsNullOrWhiteSpace(request.TurmaCodigo))
            {
                Turma dadosTurma = await ObterDadosTurma(request.TurmaCodigo);
                DreUe dreUe = await ObterDadosDreUePorTurma(request.TurmaCodigo);

                // preenchero relatório para os alunos selecionados;
                if (request.AlunosCodigo.Any())
                {
                    IEnumerable<FechamentoTurma> fechamentos = await ObterFechamentoPorCodigoTurma(request.TurmaCodigo);

                    foreach (FechamentoTurma fechamento in fechamentos)
                    {
                        long conselhoClasseId = await ObterConselhoPorFechamentoTurmaId(fechamento.Id);

                        foreach (string codigoAluno in request.AlunosCodigo)
                        {
                            Aluno dadosAluno = await ObterDadosAluno(request.TurmaCodigo, codigoAluno);

                            BoletimEscolarAlunoDto boletim = await MontarRelatorio(dadosAluno, dadosTurma, dreUe, fechamento, conselhoClasseId);

                            relatorio.Alunos.Add(boletim);
                        }
                    }

                }
                // preenchero relatório para os alunos da turma;
                else
                {
                    IEnumerable<FechamentoTurma> fechamentos = await ObterFechamentoPorCodigoTurma(request.TurmaCodigo);

                    foreach (FechamentoTurma fechamento in fechamentos)
                    {
                        long conselhoClasseId = await ObterConselhoPorFechamentoTurmaId(fechamento.Id);

                        IEnumerable<Aluno> dadosAlunos = await ObterDadosAlunos(request.TurmaCodigo);

                        foreach (Aluno dadosAluno in dadosAlunos)
                        {
                            BoletimEscolarAlunoDto boletim = await MontarRelatorio(dadosAluno, dadosTurma, dreUe, fechamento, conselhoClasseId);

                            relatorio.Alunos.Add(boletim);
                        }
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

                        foreach (FechamentoTurma fechamento in fechamentos)
                        {
                            long conselhoClasseId = await ObterConselhoPorFechamentoTurmaId(fechamento.Id);

                            foreach (Aluno dadosAluno in dadosAlunos)
                            {
                                BoletimEscolarAlunoDto boletim = await MontarRelatorio(dadosAluno, dadosTurma, dreUe, fechamento, conselhoClasseId);

                                relatorio.Alunos.Add(boletim);
                            }
                        }
                    }
                }
            }

            return relatorio;
        }

        private async Task<BoletimEscolarAlunoDto> MontarRelatorio(Aluno dadosAluno, Turma dadosTurma, DreUe dreUe, FechamentoTurma fechamento, long conselhoClasseId)
        {
            BoletimEscolarAlunoDto boletim = InicializarRelatorioBoletim(dadosTurma, dreUe, dadosAluno);

            List<ComponenteCurricularDto> componentes = await ProcessarFechamentos(dadosAluno, dadosTurma, fechamento, conselhoClasseId);

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

        private async Task<List<ComponenteCurricularDto>> ProcessarFechamentos(Aluno dadosAluno, Turma dadosTurma, FechamentoTurma fechamento, long conselhoClasseId)
        {
            List<ComponenteCurricularDto> componentes = new List<ComponenteCurricularDto>();

            IEnumerable<GrupoMatrizComponenteComNotaFinal> fechamentoNotas = await ObterFechamento(fechamento.Id, conselhoClasseId, dadosTurma, dadosAluno.CodigoAluno.ToString(), fechamento.PeriodoEscolar);

            foreach (GrupoMatrizComponenteComNotaFinal fechamentoNota in fechamentoNotas)
            {
                foreach (ComponenteComNotaFinal componente in fechamentoNota.ComponentesComNota)
                {
                    // preencher componentescurriculares
                    ComponenteCurricularDto dto = new ComponenteCurricularDto
                    {
                        Nome = componente.Componente,
                        Grupo = fechamentoNota.Nome,
                        NotaBimestre1 = componente.NotasBimestre.FirstOrDefault(x => x.Bimestre == 1).NotaConceito,
                        NotaBimestre2 = componente.NotasBimestre.FirstOrDefault(x => x.Bimestre == 2).NotaConceito,
                        NotaBimestre3 = componente.NotasBimestre.FirstOrDefault(x => x.Bimestre == 3).NotaConceito,
                        NotaBimestre4 = componente.NotasBimestre.FirstOrDefault(x => x.Bimestre == 4).NotaConceito,
                        NotaFinal = componente.NotaFinal
                    };

                    IEnumerable<FrequenciaAluno> frequencias = await ObterFrequenciaPorDisciplinaBimestres(dadosAluno.CodigoAluno.ToString(), dadosTurma.CodigoTurma);

                    dto.FrequenciaBimestre1 = frequencias.FirstOrDefault(x => x.Bimestre == 1)?.PercentualFrequencia.ToString();
                    dto.FrequenciaBimestre2 = frequencias.FirstOrDefault(x => x.Bimestre == 2)?.PercentualFrequencia.ToString();
                    dto.FrequenciaBimestre3 = frequencias.FirstOrDefault(x => x.Bimestre == 3)?.PercentualFrequencia.ToString();
                    dto.FrequenciaBimestre4 = frequencias.FirstOrDefault(x => x.Bimestre == 4)?.PercentualFrequencia.ToString();

                    componentes.Add(dto);
                }
            }

            return componentes;
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

        private async Task<IEnumerable<GrupoMatrizComponenteComNotaFinal>> ObterFechamento(long fechamentoTurmaId, long conselhoClasseId, Turma turma, string codigoAluno, PeriodoEscolar periodoEscolar)
        {
            IEnumerable<GrupoMatrizComponenteComNotaFinal> fechamento = await _mediator.Send(new ObterDadosComponenteComNotaFinalQuery
            {
                FechamentoTurmaId = fechamentoTurmaId,
                ConselhoClasseId = conselhoClasseId,
                Turma = turma,
                CodigoAluno = codigoAluno,
                PeriodoEscolar = periodoEscolar
            });

            return fechamento;
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

        private async Task<IEnumerable<FrequenciaAluno>> ObterFrequenciaPorDisciplinaBimestres(string codigoAluno, string codigoTurma)
        {
            IEnumerable<FrequenciaAluno> fechamento = await _mediator.Send(new ObterFrequenciaPorDisciplinaBimestresQuery
            {
                CodigoAluno = codigoAluno,
                CodigoTurma = codigoTurma
            });

            return fechamento;
        }
    }
}
