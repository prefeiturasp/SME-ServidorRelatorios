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
    public class ObterDadosPedagogicosTurmaQueryComponenteHandler : IRequestHandler<ObterDadosPedagogicosTurmaComponenteQuery, List<RelatorioAcompanhamentoRegistrosPedagogicosBimestreInfantilComponenteDto>>
    {
        private readonly IRegistrosPedagogicosRepository registrosPedagogicosRepository;

        public ObterDadosPedagogicosTurmaQueryComponenteHandler(IRegistrosPedagogicosRepository registrosPedagogicosSgpRepository)
        {
            this.registrosPedagogicosRepository = registrosPedagogicosSgpRepository ?? throw new ArgumentNullException(nameof(registrosPedagogicosSgpRepository));
        }

        public async Task<List<RelatorioAcompanhamentoRegistrosPedagogicosBimestreInfantilComponenteDto>> Handle(ObterDadosPedagogicosTurmaComponenteQuery request, CancellationToken cancellationToken)
        {
            var consolidacoesFiltradas = await registrosPedagogicosRepository.ObterDadosConsolidacaoRegistrosPedagogicosInfantil(request.DreCodigo, request.UeCodigo,
                request.AnoLetivo, request.ProfessorCodigo, request.Bimestres, request.TurmasId);

            var bimestres = new List<RelatorioAcompanhamentoRegistrosPedagogicosBimestreInfantilComponenteDto>();

            if (consolidacoesFiltradas.Any())
            {
                foreach (var consolidacao in consolidacoesFiltradas.OrderBy(cf => cf.Bimestre).GroupBy(cf => cf.Bimestre).Distinct())
                {
                    var bimestre = new RelatorioAcompanhamentoRegistrosPedagogicosBimestreInfantilComponenteDto
                    {
                        Bimestre = !consolidacao.FirstOrDefault().Bimestre.Equals("0") ? $"{consolidacao.FirstOrDefault().Bimestre}º BIMESTRE" : "FINAL"
                    };

                    bimestre.TurmasInfantilComponente.AddRange(consolidacao.Select(c => new RelatorioAcompanhamentoRegistrosPedagogicosTurmaInfantilComponenteDto()
                    {
                        Nome = c.TurmaNome,
                        Aulas = c.QuantidadeAulas,
                        FrequenciasPendentes = c.FrequenciasPendentes,
                        DataUltimoRegistroFrequencia = c.DataUltimaFrequencia?.ToString("dd/MM/yyyy")
                    }));

                    var turmarAgrupadas = consolidacao.GroupBy(c => c.TurmaNome)
                        .Distinct();

                    foreach (var turma in turmarAgrupadas)
                    {
                        bimestre.TurmasInfantilDiarioBordoComponente.Add(new RelatorioAcompanhamentoRegistrosPedagogicosTurmaInfantilDiarioBordoComponenteDto()
                        {
                            NomeTurma = turma.Key,
                            Aulas = turma.FirstOrDefault().QuantidadeAulas,
                            Componentes = turma.Select(c => new RelatorioAcompanhamentoRegistrosPedagogicosDiarioBordoComponenteDto()
                            {
                                NomeComponente = c.ComponenteCurricularNome,
                                DiarioBordoPendentes = c.DiarioBordoPendentes,
                                DataUltimoRegistroDiarioBordo = c.DataUltimaFrequencia?.ToString("dd/MM/yyyy")
                            }).ToList()
                        });
                    }
                }               
            }

            return bimestres;
        }
    }
}
