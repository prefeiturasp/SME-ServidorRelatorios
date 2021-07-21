﻿using MediatR;
using SME.SR.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterDadosComponenteComNotaBimestreQueryHandler : IRequestHandler<ObterDadosComponenteComNotaBimestreQuery, IEnumerable<GrupoMatrizComponenteComNotaBimestre>>
    {
        private IMediator mediator;

        public ObterDadosComponenteComNotaBimestreQueryHandler(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator)); 
        }

        public async Task<IEnumerable<GrupoMatrizComponenteComNotaBimestre>> Handle(ObterDadosComponenteComNotaBimestreQuery request, CancellationToken cancellationToken)
        {
            var notasFechamento = await ObterNotasAlunoBimestre(request.FechamentoTurmaId,
                                              request.Turma.Codigo,
                                              request.CodigoAluno,
                                              request.PeriodoEscolar.Bimestre);

            var notasConselhoClasse = await ObterNotasConselhoClasseAluno(request.ConselhoClasseId,
                                                                          request.CodigoAluno);

            var disciplinasPorTurma = await ObterComponentesCurricularesPorTurma(request.Turma.Codigo);

            var lstComponentesComNota = disciplinasPorTurma.Where(c => c.LancaNota)
                                                        .GroupBy(c => c.GrupoMatriz?.Nome);

            var lstGruposMatrizCompNota = new List<GrupoMatrizComponenteComNotaBimestre>();

            foreach (var grupoDisciplinasMatriz in lstComponentesComNota.OrderBy(k => k.Key))
            {
                var lstCompComNota = new List<ComponenteComNotaBimestre>();
                ComponenteFrequenciaRegenciaBimestre compRegenciaComNota = null;

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
                        lstCompComNota.Add(await ObterNotasFrequenciaComponenteComNotaBimestre(disciplina,
                                                                        frequenciaAluno,
                                                                        request.PeriodoEscolar,
                                                                        request.Turma,
                                                                        notasConselhoClasse,
                                                                        notasFechamento));
                }

                var grupoMatriz = new GrupoMatrizComponenteComNotaBimestre()
                {
                    Nome = grupoDisciplinasMatriz.Key,
                    ComponentesComNota = lstCompComNota,
                    ComponenteComNotaRegencia = compRegenciaComNota,
                    TipoNota = await ObterTipoNota(request.PeriodoEscolar, request.Turma)
                };

                lstGruposMatrizCompNota.Add(grupoMatriz);
            }

            return lstGruposMatrizCompNota.AsEnumerable();
        }

        private async Task<ComponenteFrequenciaRegenciaBimestre> ObterNotasFrequenciaRegencia(ComponenteCurricularPorTurma disciplina, FrequenciaAluno frequenciaAluno, PeriodoEscolar periodoEscolar, Turma turma, IEnumerable<NotaConceitoBimestreComponente> notasConselhoClasse, IEnumerable<NotaConceitoBimestreComponente> notasFechamento, Usuario usuario)
        {
            var componentesRegencia = await mediator.Send(new ObterComponentesCurricularesRegenciaQuery()
            {
                Turma = turma,
                CdComponenteCurricular = disciplina.CodDisciplina,
                Usuario = usuario
            });

            if (componentesRegencia == null || !componentesRegencia.Any())
                return null;            

            var turmaPossuiFrequenciaRegistrada = await mediator.Send(new ExisteFrequenciaRegistradaPorTurmaComponenteCurricularQuery(turma.Codigo, disciplina.CodDisciplina.ToString(), periodoEscolar.Id));

            
            double? percentualFrequencia = null;

            if (disciplina.Frequencia)
            {
                percentualFrequencia = frequenciaAluno == null && turmaPossuiFrequenciaRegistrada
                ?
                100
                :
                frequenciaAluno.TotalAulas > 0
                ?
                frequenciaAluno?.PercentualFrequencia
                :
                null;
            }

            if (frequenciaAluno != null && periodoEscolar != null && turma.AnoLetivo.Equals(2020))
            {
                var percentualFrequencia2020 = frequenciaAluno?.TotalAulas > 0 ? frequenciaAluno?.PercentualFrequencia ?? 100 : 100;
                frequenciaAluno.AdicionarFrequenciaBimestre(periodoEscolar.Bimestre, percentualFrequencia2020);
                percentualFrequencia = frequenciaAluno.PercentualFrequenciaFinal;
            }

            var conselhoClasseComponente = new ComponenteFrequenciaRegenciaBimestre()
            {
                Aulas = frequenciaAluno?.TotalAulas ?? 0,
                Faltas = frequenciaAluno?.TotalAusencias ?? 0,
                AusenciasCompensadas = frequenciaAluno?.TotalCompensacoes ?? 0,
                Frequencia = percentualFrequencia,
                PermiteRegistroFrequencia = disciplina.Frequencia
            };

            foreach (var componenteRegencia in componentesRegencia)
            {
                conselhoClasseComponente.ComponentesCurriculares.Add(ObterNotasRegencia(componenteRegencia, periodoEscolar, notasConselhoClasse, notasFechamento, turma));
            }

            return conselhoClasseComponente;
        }

        private ComponenteRegenciaComNotaBimestre ObterNotasRegencia(ComponenteCurricularPorTurma componenteCurricular, PeriodoEscolar periodoEscolar, IEnumerable<NotaConceitoBimestreComponente> notasConselhoClasse, IEnumerable<NotaConceitoBimestreComponente> notasFechamento, Turma turma)
        {
            return new ComponenteRegenciaComNotaBimestre()
            {
                Componente = componenteCurricular.Disciplina,
                EhEja = turma.EhEja,
                NotaConceito = ObterNotasComponente(componenteCurricular, periodoEscolar, notasFechamento).FirstOrDefault()?.NotaConceito,
                NotaPosConselho = ObterNotaPosConselho(componenteCurricular, periodoEscolar?.Bimestre, notasConselhoClasse, notasFechamento)
            };
        }

        private async Task<ComponenteComNotaBimestre> ObterNotasFrequenciaComponenteComNotaBimestre(ComponenteCurricularPorTurma disciplina, FrequenciaAluno frequenciaAluno, PeriodoEscolar periodoEscolar, Turma turma, IEnumerable<NotaConceitoBimestreComponente> notasConselhoClasseAluno, IEnumerable<NotaConceitoBimestreComponente> notasFechamentoAluno)
        {
            var turmaPossuiFrequenciaRegistrada = await mediator.Send(new ExisteFrequenciaRegistradaPorTurmaComponenteCurricularQuery(turma.Codigo, disciplina.CodDisciplina.ToString(), periodoEscolar.Id));
            
            double? percentualFrequencia = null;

            if (disciplina.Frequencia)
            {
                percentualFrequencia = frequenciaAluno == null && turmaPossuiFrequenciaRegistrada
                ?
                100
                :
                frequenciaAluno.TotalAulas > 0
                ?
                frequenciaAluno?.PercentualFrequencia
                :
                null;
            }

            if (frequenciaAluno != null && periodoEscolar != null && turma.AnoLetivo.Equals(2020))
            {
                var percentualFrequencia2020 = frequenciaAluno?.TotalAulas > 0 ? frequenciaAluno?.PercentualFrequencia ?? 100 : 100;
                frequenciaAluno.AdicionarFrequenciaBimestre(periodoEscolar.Bimestre, percentualFrequencia2020);
                percentualFrequencia = frequenciaAluno.PercentualFrequenciaFinal;
            }

            var conselhoClasseComponente = new ComponenteComNotaBimestre()
            {
                Componente = disciplina.Disciplina,
                EhEja = turma.EhEja,
                Aulas = frequenciaAluno?.TotalAulas ?? 0,
                Faltas = frequenciaAluno?.TotalAusencias ?? 0,
                AusenciasCompensadas = frequenciaAluno?.TotalCompensacoes ?? 0,
                Frequencia = percentualFrequencia,
                NotaConceito = ObterNotasComponente(disciplina, periodoEscolar, notasFechamentoAluno).FirstOrDefault()?.NotaConceito,
                NotaPosConselho = ObterNotaPosConselho(disciplina, periodoEscolar?.Bimestre, notasConselhoClasseAluno, notasFechamentoAluno),
                PermiteRegistroFrequencia = disciplina.Frequencia
            };

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
