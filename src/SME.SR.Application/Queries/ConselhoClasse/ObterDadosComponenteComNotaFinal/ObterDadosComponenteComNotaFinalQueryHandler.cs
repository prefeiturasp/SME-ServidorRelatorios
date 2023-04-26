using MediatR;
using SME.SR.Data;
using SME.SR.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterDadosComponenteComNotaFinalQueryHandler : IRequestHandler<ObterDadosComponenteComNotaFinalQuery, IEnumerable<GrupoMatrizComponenteComNotaFinal>>
    {
        private readonly IMediator mediator;

        public ObterDadosComponenteComNotaFinalQueryHandler(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator)); 
        }

        public async Task<IEnumerable<GrupoMatrizComponenteComNotaFinal>> Handle(ObterDadosComponenteComNotaFinalQuery request, CancellationToken cancellationToken)
        {
            var notasFechamento = await ObterNotasAlunoBimestre(request.FechamentoTurmaId,
                                                          request.Turma.Codigo,
                                                          request.CodigoAluno,
                                                          request.PeriodoEscolar?.Bimestre);

            var notasConselhoClasse = await ObterNotasConselhoClasseAluno(request.ConselhoClasseId,
                                                                          request.CodigoAluno);

            var disciplinasPorTurma = await ObterComponentesCurricularesPorTurma(request.Turma.Codigo);

            var lstComponentesComNota = disciplinasPorTurma.Where(c => c.LancaNota)
                                                      .GroupBy(c => c.GrupoMatriz?.Nome);

            var lstGruposMatrizCompNota = new List<GrupoMatrizComponenteComNotaFinal>();

            foreach (var grupoDisciplinasMatriz in lstComponentesComNota.OrderBy(k => k.Key))
            {
                var lstCompComNota = new List<ComponenteComNotaFinal>();
                ComponenteFrequenciaRegenciaFinal compRegenciaComNota = null;

                foreach (var disciplina in grupoDisciplinasMatriz)
                {
                    // Carrega Frequencia Aluno
                    var frequenciaAluno = await mediator.Send(new ObterFrequenciaAlunoQuery()
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
                                                                                notasFechamento,
                                                                                request.Usuario);
                    else
                        lstCompComNota.Add(await ObterNotasFrequenciaComponenteComNotaFinal(disciplina,
                                                                    frequenciaAluno,
                                                                    request.PeriodoEscolar,
                                                                    request.Turma,
                                                                    notasConselhoClasse,
                                                                    notasFechamento));
                }

                var grupoMatriz = new GrupoMatrizComponenteComNotaFinal()
                {
                    Nome = grupoDisciplinasMatriz.Key,
                    ComponentesComNota = lstCompComNota,
                    ComponentesComNotaRegencia = compRegenciaComNota,
                    TipoNota = await ObterTipoNota(request.PeriodoEscolar, request.Turma)
                };

                lstGruposMatrizCompNota.Add(grupoMatriz);
            }

            return lstGruposMatrizCompNota;
        }

        private async Task<ComponenteFrequenciaRegenciaFinal> ObterNotasFrequenciaRegencia(ComponenteCurricularPorTurma disciplina, FrequenciaAluno frequenciaAluno, PeriodoEscolar periodoEscolar, Turma turma, IEnumerable<NotaConceitoBimestreComponente> notasConselhoClasse, IEnumerable<NotaConceitoBimestreComponente> notasFechamento, Usuario usuario)
        {            
            var turmaPossuiFrequenciaRegistrada = await mediator.Send(new ExisteFrequenciaRegistradaPorTurmaComponenteCurricularEAnoQuery(turma.Codigo, disciplina.CodDisciplina.ToString(), turma.AnoLetivo));

            string percentualFrequencia = null;

            if (disciplina.Frequencia)
            {
                percentualFrequencia = frequenciaAluno == null && turmaPossuiFrequenciaRegistrada
                ?
                FrequenciaAluno.FormatarPercentual(100)
                :
                frequenciaAluno.TotalAulas > 0
                ?
                frequenciaAluno?.PercentualFrequenciaFormatado
                :
                null;
            }

            //Frequência especifica para 2020.
            if (frequenciaAluno != null && turma.AnoLetivo.Equals(2020))

                percentualFrequencia = frequenciaAluno.PercentualFrequenciaFinal != null ? FrequenciaAluno.FormatarPercentual(frequenciaAluno.PercentualFrequenciaFinal??0) : null;

            var conselhoClasseComponente = new ComponenteFrequenciaRegenciaFinal()
            {
                Aulas = frequenciaAluno?.TotalAulas ?? 0,
                Faltas = frequenciaAluno?.TotalAusencias ?? 0,
                AusenciasCompensadas = frequenciaAluno?.TotalCompensacoes ?? 0,
                Frequencia = percentualFrequencia
            };

            var componentesRegencia = await mediator.Send(new ObterComponentesCurricularesRegenciaQuery()
            {
                Turma = turma,
                CdComponenteCurricular = disciplina.CodDisciplina,
                Usuario = usuario
            });

            foreach (var componenteRegencia in componentesRegencia)
            {
                conselhoClasseComponente.ComponentesCurriculares.Add(ObterNotasRegencia(componenteRegencia, periodoEscolar, notasConselhoClasse, notasFechamento, turma));
            }

            return conselhoClasseComponente;
        }

        private async Task<string> ObterTipoNota(PeriodoEscolar periodoEscolar, Turma turma)
        {
            return await mediator.Send(new ObterTipoNotaQuery()
            {
                PeriodoEscolar = periodoEscolar,
                Turma = turma
            });
        }

        private ComponenteRegenciaComNotaFinal ObterNotasRegencia(ComponenteCurricularPorTurma componenteCurricular, PeriodoEscolar periodoEscolar, IEnumerable<NotaConceitoBimestreComponente> notasConselhoClasse, IEnumerable<NotaConceitoBimestreComponente> notasFechamento, Turma turma)
        {
            var notasComponente = ObterNotasComponente(componenteCurricular, periodoEscolar, notasFechamento);

            return new ComponenteRegenciaComNotaFinal()
            {
                Componente = componenteCurricular.Disciplina,
                EhEja = turma.EhEja,
                NotaConceitoBimestre1 = notasComponente.FirstOrDefault(n => n.Bimestre == 1)?.NotaConceito,
                NotaConceitoBimestre2 = notasComponente.FirstOrDefault(n => n.Bimestre == 2)?.NotaConceito,
                NotaConceitoBimestre3 = notasComponente.FirstOrDefault(n => n.Bimestre == 3)?.NotaConceito,
                NotaConceitoBimestre4 = notasComponente.FirstOrDefault(n => n.Bimestre == 4)?.NotaConceito,
                NotaFinal = ObterNotaPosConselho(componenteCurricular, periodoEscolar?.Bimestre, notasConselhoClasse, notasFechamento)
            };
        }

        private async Task<ComponenteComNotaFinal> ObterNotasFrequenciaComponenteComNotaFinal(ComponenteCurricularPorTurma disciplina, FrequenciaAluno frequenciaAluno, PeriodoEscolar periodoEscolar, Turma turma, IEnumerable<NotaConceitoBimestreComponente> notasConselhoClasseAluno, IEnumerable<NotaConceitoBimestreComponente> notasFechamentoAluno)
        {
            var turmaPossuiFrequenciaRegistrada = await mediator.Send(new ExisteFrequenciaRegistradaPorTurmaComponenteCurricularEAnoQuery(turma.Codigo, disciplina.CodDisciplina.ToString(), turma.AnoLetivo));
            
            string percentualFrequencia = null;

            if (disciplina.Frequencia)
            {
                percentualFrequencia = frequenciaAluno == null && turmaPossuiFrequenciaRegistrada
                ?
                FrequenciaAluno.FormatarPercentual(100)
                :
                frequenciaAluno.TotalAulas > 0
                ?
                frequenciaAluno?.PercentualFrequenciaFormatado
                :
                null;
            }

            if (frequenciaAluno != null && turma.AnoLetivo.Equals(2020))

                percentualFrequencia = frequenciaAluno.PercentualFrequenciaFinal != null ? FrequenciaAluno.FormatarPercentual(frequenciaAluno.PercentualFrequenciaFinal??0) : null;

            var notasComponente = ObterNotasComponente(disciplina, periodoEscolar, notasFechamentoAluno);

            var conselhoClasseComponente = new ComponenteComNotaFinal()
            {
                EhEja = turma.EhEja,
                Componente = disciplina.Disciplina,
                Faltas = frequenciaAluno?.TotalAusencias ?? 0,
                AusenciasCompensadas = frequenciaAluno?.TotalCompensacoes ?? 0,
                Frequencia = percentualFrequencia,
                NotaConceitoBimestre1 = notasComponente.FirstOrDefault(n => n.Bimestre == 1)?.NotaConceito,
                NotaConceitoBimestre2 = notasComponente.FirstOrDefault(n => n.Bimestre == 2)?.NotaConceito,
                NotaConceitoBimestre3 = notasComponente.FirstOrDefault(n => n.Bimestre == 3)?.NotaConceito,
                NotaConceitoBimestre4 = notasComponente.FirstOrDefault(n => n.Bimestre == 4)?.NotaConceito,
                NotaFinal = ObterNotaPosConselho(disciplina, periodoEscolar?.Bimestre, notasConselhoClasseAluno, notasFechamentoAluno),
                PermiteRegistroFrequencia = disciplina.Frequencia
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
            return await mediator.Send(new ObterComponentesCurricularesPorTurmaQuery()
            {
                CodigoTurma = codigoTurma
            });
        }

        private async Task<IEnumerable<NotaConceitoBimestreComponente>> ObterNotasAlunoBimestre(long fechamentoTurmaId,
                                                                                                string codigoTurma,
                                                                                                string codigoAluno,
                                                                                                int? bimestre)
        {
            return await mediator.Send(new ObterNotasAlunoBimestreQuery()
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
            return await mediator.Send(new ObterNotasConselhoClasseAlunoQuery()
            {
                ConselhoClasseId = conselhoClasseId,
                CodigoAluno = codigoAluno
            });
        }
    }
}
