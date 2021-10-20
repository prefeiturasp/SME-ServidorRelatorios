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

        public ObterDadosPedagogicosComponenteCurricularesQueryHandler(IRegistrosPedagogicosRepository registrosPedagogicosSgpRepository)
        {
            this.registrosPedagogicosRepository = registrosPedagogicosSgpRepository ?? throw new ArgumentNullException(nameof(registrosPedagogicosSgpRepository));
        }

        public async Task<List<RelatorioAcompanhamentoRegistrosPedagogicosBimestreDto>> Handle(ObterDadosPedagogicosComponenteCurricularesQuery request, CancellationToken cancellationToken)
        {
            var consolidacoesFiltradas = await registrosPedagogicosRepository.ObterDadosConsolidacaoRegistrosPedagogicos(request.DreCodigo, request.UeCodigo, request.AnoLetivo, request.ComponentesCurriculares, request.TurmasId, request.ProfessorCodigo, request.ProfessorNome, request.Bimestres);
            var bimestres = new List<RelatorioAcompanhamentoRegistrosPedagogicosBimestreDto>();

            if (consolidacoesFiltradas.Any())
            {
                foreach (var consolidacoes in consolidacoesFiltradas.OrderBy(cf=> cf.Bimestre).GroupBy(cf => cf.Bimestre))
                {
                    var bimestre = new RelatorioAcompanhamentoRegistrosPedagogicosBimestreDto();
                    bimestre.Bimestre = !consolidacoes.FirstOrDefault().Bimestre.Equals("0") ? $"{consolidacoes.FirstOrDefault().Bimestre}º BIMESTRE" : "FINAL";

                    foreach (var turmas in consolidacoes.OrderBy(t=> t.TurmaNome).GroupBy(t => t.TurmaId))
                    {
                        var turma = new RelatorioAcompanhamentoRegistrosPedagogicosTurmaDto();
                        turma.Nome = $"{turmas.FirstOrDefault().NomeTurmaFormatado}";

                        foreach (var compCurricular in turmas.OrderBy(c=> c.ComponenteCurricularNome))
                        {
                            string nomeComponente = compCurricular.RFProfessor == "" ? $"{compCurricular.ComponenteCurricularNome} - {compCurricular.NomeProfessor}"
                                                                                     : $"{compCurricular.ComponenteCurricularNome} - {compCurricular.NomeProfessor} ({compCurricular.RFProfessor})";
                            var componente = new RelatorioAcompanhamentoRegistrosPedagogicosCompCurricularesDto()
                            {
                                Nome = nomeComponente,
                                QuantidadeAulas = compCurricular.QuantidadeAulas,
                                FrequenciasPendentes = compCurricular.FrequenciasPendentes,
                                DataUltimoRegistroFrequencia = compCurricular.DataUltimaFrequencia,
                                PlanosAulaPendentes = compCurricular.PlanoAulaPendentes,
                                DataUltimoRegistroPlanoAula = compCurricular.DataUltimoPlanoAula
                            };
                            turma.ComponentesCurriculares.Add(componente);
                        }
                        bimestre.Turmas.Add(turma);
                    }
                    bimestres.Add(bimestre);
                }
            }

            return bimestres;
        }

    }
}
