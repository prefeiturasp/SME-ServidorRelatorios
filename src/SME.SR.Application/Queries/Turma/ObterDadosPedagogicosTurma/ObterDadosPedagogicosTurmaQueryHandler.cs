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
            var consolidacoesFiltradas = await registrosPedagogicosRepository.ObterDadosConsolidacaoRegistrosPedagogicosInfantil(request.DreCodigo, request.UeCodigo, request.AnoLetivo, request.ProfessorCodigo, request.ProfessorNome, request.Bimestres, request.TurmasId);
            var bimestres = new List<RelatorioAcompanhamentoRegistrosPedagogicosBimestreInfantilDto>();

            if (consolidacoesFiltradas.Any())
            {
                foreach (var consolidacoes in consolidacoesFiltradas.OrderBy(cf=> cf.Bimestre).GroupBy(cf => cf.Bimestre))
                {
                    var bimestre = new RelatorioAcompanhamentoRegistrosPedagogicosBimestreInfantilDto();
                    bimestre.Bimestre = !consolidacoes.FirstOrDefault().Bimestre.Equals("0") ? $"{consolidacoes.FirstOrDefault().Bimestre}º BIMESTRE" : "FINAL";

                    foreach (var turma in consolidacoes.OrderBy(t=> t.TurmaNome))
                    {
                        string nomeTurmaComplementar = turma.RFProfessor == "" ? $"{turma.NomeProfessor}"
                                                                               : $"{turma.NomeProfessor} ({turma.RFProfessor })";
                        var dadosTurma = new RelatorioAcompanhamentoRegistrosPedagogicosTurmaInfantilDto()
                        {
                            Nome = $"{turma.NomeTurmaFormatado} - {nomeTurmaComplementar}",
                            Aulas = turma.QuantidadeAulas,
                            FrequenciasPendentes = turma.FrequenciasPendentes,
                            DataUltimoRegistroFrequencia = turma.DataUltimaFrequencia,
                            DiarioBordoPendentes = turma.DiarioBordoPendentes,
                            DataUltimoRegistroDiarioBordo = turma.DataUltimoDiarioBordo
                        };

                        bimestre.TurmasInfantil.Add(dadosTurma);
                    }
                    bimestres.Add(bimestre);
                }
            }

            return bimestres;
        }

    }
}
