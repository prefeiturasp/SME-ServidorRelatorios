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
            var consolidacoesFiltradas = await registrosPedagogicosRepository.ObterDadosConsolidacaoRegistrosPedagogicosInfantil(request.AnoLetivo, request.TurmasId, request.ProfessorCodigo, request.ProfessorNome, request.Bimestres);
            var bimestres = new List<RelatorioAcompanhamentoRegistrosPedagogicosBimestreInfantilDto>();

            if (consolidacoesFiltradas.Any())
            {
                foreach (var consolidacoes in consolidacoesFiltradas.GroupBy(cf => cf.Bimestre))
                {
                    var bimestre = new RelatorioAcompanhamentoRegistrosPedagogicosBimestreInfantilDto();
                    bimestre.Bimestre = !consolidacoes.FirstOrDefault().Bimestre.Equals("0") ? $"{bimestre}º BIMESTRE" : "FINAL";

                    foreach (var turma in consolidacoes)
                    {
                        var dadosTurma = new RelatorioAcompanhamentoRegistrosPedagogicosTurmaInfantilDto()
                        {
                            Nome = $"{turma.TurmaModalidade} - {turma.TurmaNome}",
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
