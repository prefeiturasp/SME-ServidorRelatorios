using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterDadosPedagogicosTurmaQueryHandler : IRequestHandler<ObterDadosPedagogicosTurmaQuery, List<RelatorioAcompanhamentoRegistrosPedagogicosBimestreInfantilDto>>
    {
        private readonly IRegistrosPedagogicosRepository registrosPedagogicosRepository;

        public ObterDadosPedagogicosTurmaQueryHandler(IRegistrosPedagogicosRepository registrosPedagogicosSgpRepository)
        {
            this.registrosPedagogicosRepository = registrosPedagogicosSgpRepository ?? throw new ArgumentNullException(nameof(registrosPedagogicosSgpRepository));
        }

        public async Task<List<RelatorioAcompanhamentoRegistrosPedagogicosBimestreInfantilDto>> Handle(ObterDadosPedagogicosTurmaQuery request, CancellationToken cancellationToken)
        {
            var consolidacoesFiltradas = await registrosPedagogicosRepository.ObterDadosConsolidacaoRegistrosPedagogicosInfantil(request.DreCodigo, request.UeCodigo,
                request.AnoLetivo, request.ProfessorCodigo, request.Bimestres, request.TurmasId);

            var bimestres = new List<RelatorioAcompanhamentoRegistrosPedagogicosBimestreInfantilDto>();

            if (consolidacoesFiltradas.Any())
            {
                foreach (var consolidacoes in consolidacoesFiltradas.OrderBy(cf=> cf.Bimestre).GroupBy(cf => cf.Bimestre).Distinct())
                {
                    var bimestre = new RelatorioAcompanhamentoRegistrosPedagogicosBimestreInfantilDto();
                    bimestre.Bimestre = !consolidacoes.FirstOrDefault().Bimestre.Equals("0") ? $"{consolidacoes.FirstOrDefault().Bimestre}º BIMESTRE" : "FINAL";

                    foreach (var turma in consolidacoes.OrderBy(t=> t.TurmaNome).GroupBy(t=> t.TurmaId))
                    {
                        if(turma.Count() == 1)
                        {
                            var dadosTurma = turma.FirstOrDefault();
                            string nomeTurmaComplementar = dadosTurma.RFProfessor == "" ? $"{dadosTurma.NomeProfessor}"
                                                                               : $"{dadosTurma.NomeProfessor} ({dadosTurma.RFProfessor }) ";

                            if (dadosTurma.CJ)
                                nomeTurmaComplementar = $"{nomeTurmaComplementar} - CJ";

                            var registroTurma = new RelatorioAcompanhamentoRegistrosPedagogicosTurmaInfantilDto()
                            {
                                Nome = $"{dadosTurma.NomeTurmaFormatado} - {nomeTurmaComplementar}",
                                Aulas = dadosTurma.QuantidadeAulas,
                                FrequenciasPendentes = dadosTurma.FrequenciasPendentes,
                                DataUltimoRegistroFrequencia = dadosTurma.DataUltimaFrequencia != null ? dadosTurma.DataUltimaFrequencia?.ToString("dd/MM/yyyy HH:mm:ss") : "",
                                DiarioBordoPendentes = dadosTurma.DiarioBordoPendentes,
                                DataUltimoRegistroDiarioBordo = dadosTurma.DataUltimoDiarioBordo != null ? dadosTurma.DataUltimoDiarioBordo?.ToString("dd/MM/yyyy HH:mm:ss") : ""
                            };

                            bimestre.TurmasInfantil.Add(registroTurma);
                        }
                        else
                        {
                            string nomeProfessor = string.Empty;
                            string[] nomesProfessores = turma.Where(t => !string.IsNullOrEmpty(t.RFProfessor)).Select(t => t.NomeProfessor).Distinct().ToArray();
                            string[] rfProfessores = turma.Where(t => !string.IsNullOrEmpty(t.RFProfessor)).Select(t => t.RFProfessor).Distinct().ToArray();
                            var dadosTurma = turma.FirstOrDefault();
                            var listaProfessores = new List<string>();

                            for (int i=0; i<2; i++)
                            {
                                nomeProfessor = $"{nomesProfessores[i]} - ({rfProfessores[i]})";
                                listaProfessores.Add(nomeProfessor);
                            }

                            var registroTurma = new RelatorioAcompanhamentoRegistrosPedagogicosTurmaInfantilDto()
                            {
                                Nome = dadosTurma.NomeTurmaFormatado,
                                Professores = listaProfessores,
                                Aulas = dadosTurma.QuantidadeAulas,
                                FrequenciasPendentes = dadosTurma.FrequenciasPendentes,
                                DataUltimoRegistroFrequencia = dadosTurma.DataUltimaFrequencia != null ? dadosTurma.DataUltimaFrequencia?.ToString("dd/MM/yyyy HH:mm:ss") : "",
                                DiarioBordoPendentes = dadosTurma.DiarioBordoPendentes,
                                DataUltimoRegistroDiarioBordo = dadosTurma.DataUltimoDiarioBordo != null ? dadosTurma.DataUltimoDiarioBordo?.ToString("dd/MM/yyyy HH:mm:ss") : ""
                            };

                            bimestre.TurmasInfantil.Add(registroTurma);
                        }
                        
                    }
                    bimestres.Add(bimestre);
                }
            }

            return bimestres;
        }

    }
}
