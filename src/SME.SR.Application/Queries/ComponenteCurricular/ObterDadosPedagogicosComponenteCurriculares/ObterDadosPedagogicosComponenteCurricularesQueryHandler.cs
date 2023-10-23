using DocumentFormat.OpenXml.Bibliography;
using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterDadosPedagogicosComponenteCurricularesQueryHandler : IRequestHandler<ObterDadosPedagogicosComponenteCurricularesQuery, List<RelatorioAcompanhamentoRegistrosPedagogicosBimestreDto>>
    {
        private readonly IRegistrosPedagogicosRepository registrosPedagogicosRepository;
        private readonly IMediator mediator;

        public ObterDadosPedagogicosComponenteCurricularesQueryHandler(IRegistrosPedagogicosRepository registrosPedagogicosSgpRepository, IMediator mediator)
        {
            this.registrosPedagogicosRepository = registrosPedagogicosSgpRepository ?? throw new ArgumentNullException(nameof(registrosPedagogicosSgpRepository));
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<List<RelatorioAcompanhamentoRegistrosPedagogicosBimestreDto>> Handle(ObterDadosPedagogicosComponenteCurricularesQuery request, CancellationToken cancellationToken)
        {
            var consolidacoesFiltradas = await registrosPedagogicosRepository.ObterDadosConsolidacaoRegistrosPedagogicos(request.DreCodigo, request.UeCodigo, request.AnoLetivo, request.ComponentesCurriculares, request.TurmasCodigo, request.ProfessorCodigo, request.Bimestres, request.Modalidade, request.Semestre);
            var bimestres = new List<RelatorioAcompanhamentoRegistrosPedagogicosBimestreDto>();

            if (consolidacoesFiltradas.Any())
            {
                consolidacoesFiltradas = await TratarInformacoesComponentesCurriculares(consolidacoesFiltradas.ToList());
                foreach (var consolidacoes in consolidacoesFiltradas.OrderBy(cf=> cf.Bimestre).GroupBy(cf => cf.Bimestre).Distinct())
                {
                    var bimestre = new RelatorioAcompanhamentoRegistrosPedagogicosBimestreDto();
                    bimestre.Bimestre = !consolidacoes.FirstOrDefault().Bimestre.Equals("0") ? $"{consolidacoes.FirstOrDefault().Bimestre}º BIMESTRE" : "FINAL";

                    foreach (var turmas in consolidacoes.OrderBy(t=> t.TurmaNome).GroupBy(t => t.TurmaId))
                    {
                        var turma = new RelatorioAcompanhamentoRegistrosPedagogicosTurmaDto();
                        turma.Nome = $"{turmas.FirstOrDefault().NomeTurmaFormatado}";

                        var turmasComponenteAgrupados = turmas.GroupBy(t => t.ComponenteCurricularId);

                        foreach (var turmaComponentes in turmasComponenteAgrupados)
                        {
                            bool existeConsolidacaoSemProfessorEComProfessor = turmasComponenteAgrupados.Any(t => t.Any(c => string.IsNullOrEmpty(c.RFProfessor))) && turmaComponentes.Count() > 1;
                            if (existeConsolidacaoSemProfessorEComProfessor)
                            {
                                var compCurricular = turmaComponentes.Where(t => !string.IsNullOrEmpty(t.RFProfessor)).FirstOrDefault();

                                string nomeComponente = compCurricular.RFProfessor == "" ? $"{compCurricular.ComponenteCurricularNome} - {compCurricular.NomeProfessor}"
                                                                                     : $"{compCurricular.ComponenteCurricularNome} - {compCurricular.NomeProfessor} ({compCurricular.RFProfessor}) ";

                                if (compCurricular.CJ)
                                    nomeComponente = $"{nomeComponente} - CJ";

                                string dataUltimoRegistroFrequencia = turmaComponentes.Any(c => c.DataUltimaFrequencia != null) ?
                                                                      turmaComponentes.Max(t => t.DataUltimaFrequencia)?.ToString("dd/MM/yyyy HH:mm:ss")
                                                                      : "";
                                string dataUltimoRegistroPlanoAula = turmaComponentes.Any(t => t.DataUltimoPlanoAula != null) ?
                                                                      turmaComponentes.Max(t => t.DataUltimoPlanoAula)?.ToString("dd/MM/yyyy HH:mm:ss")
                                                                      : "";

                                var componente = new RelatorioAcompanhamentoRegistrosPedagogicosCompCurricularesDto()
                                {
                                    Nome = nomeComponente.ToUpper(),
                                    QuantidadeAulas = turmaComponentes.Sum(t => t.QuantidadeAulas),
                                    FrequenciasPendentes = turmaComponentes.Sum(t => t.FrequenciasPendentes),
                                    DataUltimoRegistroFrequencia = dataUltimoRegistroFrequencia,
                                    PlanosAulaPendentes = turmaComponentes.Sum(t => t.PlanoAulaPendentes),
                                    DataUltimoRegistroPlanoAula = dataUltimoRegistroPlanoAula
                                };
                                turma.ComponentesCurriculares.Add(componente);
                            }
                            else
                            {
                                foreach (var valorTurma in turmaComponentes)
                                {
                                    string nomeComponente = valorTurma.RFProfessor == "" ? $"{valorTurma.ComponenteCurricularNome} - {valorTurma.NomeProfessor}"
                                                                                   : $"{valorTurma.ComponenteCurricularNome} - {valorTurma.NomeProfessor} ({valorTurma.RFProfessor}) ";

                                    if (valorTurma.CJ)
                                        nomeComponente = $"{nomeComponente} - CJ";

                                    string dataUltimoRegistroFrequencia = valorTurma.DataUltimaFrequencia != null ?
                                                                          valorTurma.DataUltimaFrequencia?.ToString("dd/MM/yyyy HH:mm:ss")
                                                                          : "";
                                    string dataUltimoRegistroPlanoAula = valorTurma.DataUltimoPlanoAula != null ?
                                                                          valorTurma.DataUltimoPlanoAula?.ToString("dd/MM/yyyy HH:mm:ss")
                                                                          : "";

                                    var componente = new RelatorioAcompanhamentoRegistrosPedagogicosCompCurricularesDto()
                                    {
                                        Nome = nomeComponente.ToUpper(),
                                        QuantidadeAulas = valorTurma.QuantidadeAulas,
                                        FrequenciasPendentes = valorTurma.FrequenciasPendentes,
                                        DataUltimoRegistroFrequencia = dataUltimoRegistroFrequencia,
                                        PlanosAulaPendentes = valorTurma.PlanoAulaPendentes,
                                        DataUltimoRegistroPlanoAula = dataUltimoRegistroPlanoAula
                                    };
                                    turma.ComponentesCurriculares.Add(componente);
                                }
                            }
                        }
                        turma.ComponentesCurriculares = turma.ComponentesCurriculares.OrderBy(c => c.Nome).ToList();
                        bimestre.Turmas.Add(turma);
                    }
                    bimestres.Add(bimestre);
                }
            }

            return bimestres;
        }

        private async Task<IEnumerable<ConsolidacaoRegistrosPedagogicosDto>> TratarInformacoesComponentesCurriculares(List<ConsolidacaoRegistrosPedagogicosDto> consolidacoesRegistrosPedagogicos)
        {

            var idsComponentesSemNome = consolidacoesRegistrosPedagogicos.Where(ff => string.IsNullOrEmpty(ff.ComponenteCurricularNome)).Select(ff => ff.ComponenteCurricularId).Distinct();
            if (!idsComponentesSemNome.Any())
                return consolidacoesRegistrosPedagogicos;

            var informacoesComponentesCurriculares = await mediator.Send(new ObterComponentesCurricularesEolPorIdsQuery(idsComponentesSemNome.ToArray()));
            foreach (var consolidacaoRegistroPedagogico in consolidacoesRegistrosPedagogicos.Where(ff => string.IsNullOrEmpty(ff.ComponenteCurricularNome)))
            {
                var componenteCurricular = informacoesComponentesCurriculares.Where(cc => cc.CodDisciplina == consolidacaoRegistroPedagogico.ComponenteCurricularId).FirstOrDefault();
                if (!(componenteCurricular is null))
                    consolidacaoRegistroPedagogico.ComponenteCurricularNome = componenteCurricular.Disciplina;
            }
            return consolidacoesRegistrosPedagogicos;
        }

    }
}
