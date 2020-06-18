using MediatR;
using SME.SR.Application.Queries.ComponenteCurricular.ObterComponentesCurricularesRegencia;
using SME.SR.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterDadosComponenteComNotaFinalQueryHandler : IRequestHandler<ObterDadosComponenteComNotaFinalQuery, IEnumerable<GrupoMatrizComponenteComNotaFinal>>
    {
        private IMediator _mediator;

        public ObterDadosComponenteComNotaFinalQueryHandler(IMediator mediator)
        {
            this._mediator = mediator;
        }

        public async Task<IEnumerable<GrupoMatrizComponenteComNotaFinal>> Handle(ObterDadosComponenteComNotaFinalQuery request, CancellationToken cancellationToken)
        {
            var notasFechamento = await ObterNotasAlunoBimestre(request.FechamentoTurmaId,
                                                          request.Turma.CodigoTurma,
                                                          request.CodigoAluno,
                                                          request.PeriodoEscolar?.Bimestre);

            var notasConselhoClasse = await ObterNotasConselhoClasseAluno(request.ConselhoClasseId,
                                                                          request.CodigoAluno);

            var disciplinasPorTurma = await ObterComponentesCurricularesPorTurma(request.Turma.CodigoTurma);

            var lstComponentesComNota = disciplinasPorTurma.Where(c => c.LancaNota)
                                                      .GroupBy(c => c.GrupoMatriz?.Nome);

            var lstGruposMatrizCompNota = new List<GrupoMatrizComponenteComNotaFinal>();

            foreach (var grupoDisciplinasMatriz in lstComponentesComNota.OrderBy(k => k.Key))
            {
                var lstCompComNota = new List<ComponenteComNotaFinal>();
                var compRegenciaComNota = new ComponenteFrequenciaRegenciaBimestre();

                foreach (var disciplina in grupoDisciplinasMatriz)
                {
                    // Carrega Frequencia Aluno
                    var frequenciaAluno = await _mediator.Send(new ObterFrequenciaAlunoQuery()
                    {
                        Turma = request.Turma,
                        CodigoAluno = request.CodigoAluno,
                        ComponenteCurricularCodigo = disciplina.CodDisciplina.ToString(),
                        PeriodoEscolar = request.PeriodoEscolar
                    });

                    if (disciplina.Regencia)
                        compRegenciaComNota = await ObterNotasFrequenciaRegencia(disciplina,
                                                                                frequenciaAluno,
                                                                                request.PeriodoEscolar,
                                                                                request.Turma,
                                                                                notasConselhoClasse,
                                                                                notasFechamento);
                    else
                        lstCompComNota.Add(ObterNotasFrequenciaComponenteComNotaFinal(disciplina,
                                                                    frequenciaAluno,
                                                                    request.PeriodoEscolar,
                                                                    request.Turma,
                                                                    notasConselhoClasse,
                                                                    notasFechamento));
                }

                var grupoMatriz = new GrupoMatrizComponenteComNotaFinal()
                {
                    Nome = grupoDisciplinasMatriz.Key,
                    ComponentesComNota = lstCompComNota
                };

                lstGruposMatrizCompNota.Add(grupoMatriz);
            }

            return lstGruposMatrizCompNota;
        }

        private async Task<ComponenteFrequenciaRegenciaBimestre> ObterNotasFrequenciaRegencia(ComponenteCurricularPorTurma disciplina, FrequenciaAluno frequenciaAluno, PeriodoEscolar periodoEscolar, Turma turma, IEnumerable<NotaConceitoBimestreComponente> notasConselhoClasse, IEnumerable<NotaConceitoBimestreComponente> notasFechamento)
        {
            var conselhoClasseComponente = new ComponenteFrequenciaRegenciaBimestre()
            {
                Aulas = frequenciaAluno.TotalAulas,
                Faltas = frequenciaAluno?.TotalAusencias ?? 0,
                AusenciasCompensadas = frequenciaAluno?.TotalCompensacoes ?? 0,
                Frequencia = (frequenciaAluno.TotalAulas > 0 ? frequenciaAluno?.PercentualFrequencia ?? 100 : 100)
            };

            var componentesRegencia = await _mediator.Send(new ObterComponentesCurricularesRegenciaQuery()
            {
                Turma = turma,
                CdComponenteCurricular = disciplina.CodDisciplina
            });

            foreach (var componenteRegencia in componentesRegencia)
            {
                conselhoClasseComponente.ComponentesCurriculares.Add(ObterNotasRegencia(componenteRegencia, periodoEscolar, notasConselhoClasse, notasFechamento));
            }

            return conselhoClasseComponente;
        }

        private ComponenteRegenciaComNotaBimestre ObterNotasRegencia(ComponenteCurricularPorTurma componenteCurricular, PeriodoEscolar periodoEscolar, IEnumerable<NotaConceitoBimestreComponente> notasConselhoClasse, IEnumerable<NotaConceitoBimestreComponente> notasFechamento)
        {
            return new ComponenteRegenciaComNotaBimestre()
            {
                Nome = componenteCurricular.Disciplina,
                NotaConceito = ObterNotasComponente(componenteCurricular, periodoEscolar, notasFechamento).FirstOrDefault()?.NotaConceito,
                NotaPosConselho = ObterNotaPosConselho(componenteCurricular, periodoEscolar?.Bimestre, notasConselhoClasse, notasFechamento)
            };
        }

        private ComponenteComNotaFinal ObterNotasFrequenciaComponenteComNotaFinal(ComponenteCurricularPorTurma disciplina, FrequenciaAluno frequenciaAluno, PeriodoEscolar periodoEscolar, Turma turma, IEnumerable<NotaConceitoBimestreComponente> notasConselhoClasseAluno, IEnumerable<NotaConceitoBimestreComponente> notasFechamentoAluno)
        {
            var conselhoClasseComponente = new ComponenteComNotaFinal()
            {
                Componente = disciplina.Disciplina,
                Faltas = frequenciaAluno?.TotalAusencias ?? 0,
                AusenciasCompensadas = frequenciaAluno?.TotalCompensacoes ?? 0,
                Frequencia = (frequenciaAluno.TotalAulas > 0 ? frequenciaAluno?.PercentualFrequencia ?? 100 : 100),
                NotasBimestre = ObterNotasComponente(disciplina, periodoEscolar, notasFechamentoAluno),
                NotaFinal = ObterNotaPosConselho(disciplina, periodoEscolar?.Bimestre, notasConselhoClasseAluno, notasFechamentoAluno)
            };

            return conselhoClasseComponente;
        }
        private List<NotaBimestre> ObterNotasComponente(ComponenteCurricularPorTurma componenteCurricular, PeriodoEscolar periodoEscolar, IEnumerable<NotaConceitoBimestreComponente> notasFechamentoAluno)
        {
            var notasFinais = new List<NotaBimestre>();

            if (periodoEscolar != null)
                notasFinais.Add(ObterNotaFinalComponentePeriodo(componenteCurricular.CodDisciplina, periodoEscolar.Bimestre, notasFechamentoAluno));
            else
                notasFinais.AddRange(ObterNotasFinaisComponentePeriodos(componenteCurricular.CodDisciplina, notasFechamentoAluno));

            return notasFinais;
        }

        private NotaBimestre ObterNotaFinalComponentePeriodo(long codigoComponenteCurricular, int bimestre, IEnumerable<NotaConceitoBimestreComponente> notasFechamentoAluno)
        {
            string notaConceito = null;
            // Busca nota do FechamentoNota
            var notaFechamento = notasFechamentoAluno.FirstOrDefault(c => c.ComponenteCurricularCodigo == codigoComponenteCurricular);
            if (notaFechamento != null)
                notaConceito = notaFechamento.NotaConceito;

            return new NotaBimestre()
            {
                Bimestre = bimestre,
                NotaConceito = notaConceito
            };
        }

        private IEnumerable<NotaBimestre> ObterNotasFinaisComponentePeriodos(long codigoComponenteCurricular, IEnumerable<NotaConceitoBimestreComponente> notasFechamentoAluno)
        {
            var notasPeriodos = new List<NotaBimestre>();

            var notasFechamentoBimestres = notasFechamentoAluno.Where(c => c.ComponenteCurricularCodigo == codigoComponenteCurricular && c.Bimestre.HasValue);
            foreach (var notaFechamento in notasFechamentoBimestres)
            {
                notasPeriodos.Add(new NotaBimestre()
                {
                    Bimestre = notaFechamento.Bimestre.Value,
                    NotaConceito = notaFechamento.NotaConceito.ToString()
                });
            }

            return notasPeriodos;
        }

        private string ObterNotaPosConselho(ComponenteCurricularPorTurma componenteCurricular, int? bimestre, IEnumerable<NotaConceitoBimestreComponente> notasConselhoClasseAluno, IEnumerable<NotaConceitoBimestreComponente> notasFechamentoAluno)
        {
            var componenteCurricularCodigo = componenteCurricular.CodDisciplina;
            // Busca nota do conselho de classe consultado
            var notaComponente = notasConselhoClasseAluno.FirstOrDefault(c => c.ComponenteCurricularCodigo == componenteCurricularCodigo);
            if (notaComponente == null)
                // Sugere nota final do fechamento
                notaComponente = notasFechamentoAluno.FirstOrDefault(c => c.ComponenteCurricularCodigo == componenteCurricularCodigo && c.Bimestre == bimestre);

            return notaComponente?.NotaConceito.ToString();
        }

        private async Task<IEnumerable<ComponenteCurricularPorTurma>> ObterComponentesCurricularesPorTurma(string codigoTurma)
        {
            return await _mediator.Send(new ObterComponentesCurricularesPorTurmaQuery()
            {
                CodigoTurma = codigoTurma
            });
        }

        private async Task<IEnumerable<NotaConceitoBimestreComponente>> ObterNotasAlunoBimestre(long fechamentoTurmaId,
                                                                                                string codigoTurma,
                                                                                                string codigoAluno,
                                                                                                int? bimestre)
        {
            return await _mediator.Send(new ObterNotasAlunoBimestreQuery()
            {
                FechamentoTurmaId = fechamentoTurmaId,
                CodigoTurma = codigoTurma,
                CodigoAluno = codigoAluno,
                Bimestre = bimestre
            });
        }

        private async Task<IEnumerable<NotaConceitoBimestreComponente>> ObterNotasConselhoClasseAluno(long conselhoClasseId,
                                                                                                      string codigoAluno)
        {
            return await _mediator.Send(new ObterNotasConselhoClasseAlunoQuery()
            {
                ConselhoClasseId = conselhoClasseId,
                CodigoAluno = codigoAluno
            });
        }
    }
}
