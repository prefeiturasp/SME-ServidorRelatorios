using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using SME.SR.Infra.Utilitarios;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterDadosPedagogicosTurmaComponenteQueryHandler : IRequestHandler<ObterDadosPedagogicosTurmaComponenteQuery, List<RelatorioAcompanhamentoRegistrosPedagogicosBimestreInfantilComponenteDto>>
    {
        private readonly IRegistrosPedagogicosRepository registrosPedagogicosRepository;

        public ObterDadosPedagogicosTurmaComponenteQueryHandler(IRegistrosPedagogicosRepository registrosPedagogicosSgpRepository)
        {
            this.registrosPedagogicosRepository = registrosPedagogicosSgpRepository ?? throw new ArgumentNullException(nameof(registrosPedagogicosSgpRepository));
        }

        public async Task<List<RelatorioAcompanhamentoRegistrosPedagogicosBimestreInfantilComponenteDto>> Handle(ObterDadosPedagogicosTurmaComponenteQuery request, CancellationToken cancellationToken)
        {
            var consolidacoesFiltradas = await registrosPedagogicosRepository.ObterDadosConsolidacaoRegistrosPedagogicosInfantil(request.DreCodigo, request.UeCodigo,
                request.AnoLetivo, request.ProfessorCodigo, request.Bimestres, request.TurmasId, request.ComponentesCurricularesIds);

            var bimestres = new List<RelatorioAcompanhamentoRegistrosPedagogicosBimestreInfantilComponenteDto>();

            if (consolidacoesFiltradas.Any())
            {
                foreach (var consolidacao in consolidacoesFiltradas.OrderBy(cf => cf.Bimestre).GroupBy(cf => cf.Bimestre).Distinct())
                {
                    var bimestre = new RelatorioAcompanhamentoRegistrosPedagogicosBimestreInfantilComponenteDto
                    {
                        Bimestre = !consolidacao.FirstOrDefault().Bimestre.Equals("0") ? $"{consolidacao.FirstOrDefault().Bimestre}º BIMESTRE" : "FINAL"
                    };

                    var frequenciaTurmaInfantil = consolidacao.Select(c => new RelatorioAcompanhamentoRegistrosPedagogicosTurmaInfantilComponenteDto()
                    {
                        Nome = $"{((Modalidade)Enum.ToObject(typeof(Modalidade), c.ModalidadeCodigo)).GetAttribute<DisplayAttribute>().ShortName} - {c.TurmaNome}",
                        Aulas = c.QuantidadeAulas,
                        FrequenciasPendentes = c.FrequenciasPendentes,
                        DataUltimoRegistroFrequencia = c.DataUltimaFrequencia?.ToString("dd/MM/yyyy")
                    }).DistinctBy(c => c.Nome);

                    bimestre.TurmasInfantilComponente.AddRange(frequenciaTurmaInfantil);

                    var turmarAgrupadas = consolidacao.GroupBy(c => $"{((Modalidade)Enum.ToObject(typeof(Modalidade), c.ModalidadeCodigo)).GetAttribute<DisplayAttribute>().ShortName} - {c.TurmaNome}")
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
                                DataUltimoRegistroDiarioBordo = c.DataUltimoDiarioBordo?.ToString("dd/MM/yyyy")
                            }).ToList()
                        });
                    }

                    bimestres.Add(bimestre);
                }               
            }

            return bimestres;
        }
    }
}
