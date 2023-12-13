using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterDadosComponenteSemNotaFinalQueryHandler : IRequestHandler<ObterDadosComponenteSemNotaFinalQuery, IEnumerable<GrupoMatrizComponenteSemNotaFinal>>
    {
        private IMediator mediator;

        public ObterDadosComponenteSemNotaFinalQueryHandler(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<IEnumerable<GrupoMatrizComponenteSemNotaFinal>> Handle(ObterDadosComponenteSemNotaFinalQuery request, CancellationToken cancellationToken)
        {
            var disciplinasPorTurma = await ObterComponentesCurricularesPorTurma(request.CodigoTurma);
            var turma = await mediator.Send(new ObterTurmaQuery(request.CodigoTurma));

            var lstComponentesSemNota = disciplinasPorTurma.Where(x => !x.LancaNota && x.GrupoMatriz != null)
                                              .GroupBy(c => new { c.GrupoMatriz?.Id, c.GrupoMatriz?.Nome });

            var frequenciaAluno = await mediator.Send(new ObterFrequenciaPorDisciplinaBimestresQuery()
            {
                CodigoTurma = request.CodigoTurma,
                CodigoAluno = request.CodigoAluno,
                Bimestre = request.Bimestre
            });

            var lstGruposMatrizCompSemNota = new List<GrupoMatrizComponenteSemNotaFinal>();

            foreach (var grupoDisciplinasMatriz in lstComponentesSemNota.OrderBy(k => k.Key.Nome))
            {
                var lstCompSemNota = new List<ComponenteSemNotaFinal>();

                foreach (var disciplina in grupoDisciplinasMatriz)
                {
                    var frequenciaDisciplina = frequenciaAluno.Where(x =>
                        x.DisciplinaId == disciplina.CodDisciplina.ToString());

                    var frequenciaAlunoDaDisciplina = new FrequenciaAluno()
                    {
                        TotalAulas = frequenciaDisciplina?.Sum(f => f.TotalAulas) ?? 0,
                        TotalAusencias = frequenciaDisciplina?.Sum(f => f.TotalAusencias) ?? 0,
                        TotalCompensacoes = frequenciaDisciplina?.Sum(f => f.TotalCompensacoes) ?? 0
                    };                                   
                    
                    var sintese = frequenciaAlunoDaDisciplina.PercentualFrequencia >=
                                    await ObterFrequenciaMediaPorComponenteCurricular(disciplina.Regencia,
                                                                                      disciplina.LancaNota) ?
                                 "Frequente" : "Não Frequente";

                    var componenteSemNota = new ComponenteSemNotaFinal()
                    {
                        Componente = disciplina.Disciplina,
                        Faltas = frequenciaDisciplina?.Sum(x => x.TotalAusencias),
                        Frequencia = frequenciaAlunoDaDisciplina.PercentualFrequenciaFormatado,
                        Parecer = sintese
                    };

                    lstCompSemNota.Add(componenteSemNota);
                }

                var grupoMatriz = new GrupoMatrizComponenteSemNotaFinal()
                {
                    Nome = grupoDisciplinasMatriz.Key.Nome ?? "",
                    ComponentesSemNota = lstCompSemNota
                };

                lstGruposMatrizCompSemNota.Add(grupoMatriz);
            }

            return lstGruposMatrizCompSemNota;
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

        private async Task<IEnumerable<ComponenteCurricularPorTurma>> ObterComponentesCurricularesPorTurma(string codigoTurma)
        {
            return await mediator.Send(new ObterComponentesCurricularesPorTurmaQuery()
            {
                CodigoTurma = codigoTurma
            });
        }
    }
}
