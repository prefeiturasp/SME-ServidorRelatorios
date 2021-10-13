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
    public class MontarRelatorioAcompanhamentoRegistrosPedagogicosInfantilQueryHandler : IRequestHandler<MontarRelatorioAcompanhamentoRegistrosPedagogicosInfantilQuery, RelatorioAcompanhamentoRegistrosPedagogicosInfantilDto>
    {
        public async Task<RelatorioAcompanhamentoRegistrosPedagogicosInfantilDto> Handle(MontarRelatorioAcompanhamentoRegistrosPedagogicosInfantilQuery request, CancellationToken cancellationToken)
        {
            var relatorio = new RelatorioAcompanhamentoRegistrosPedagogicosInfantilDto();

            MontarCabecalho(relatorio, request.Dre, request.Ue, request.Turmas, request.Bimestres, request.UsuarioNome, request.UsuarioRF);

            if (request.Bimestres == null || !request.Bimestres.Any())
            {
                var bimestres = new List<int>();
                bimestres = bimestres.Distinct().OrderBy(b => b == 0).ThenBy(b => b).ToList();
                request.Bimestres = bimestres.ToArray();
            }
            else
                request.Bimestres = request.Bimestres.OrderBy(b => b == 0).ThenBy(b => b).ToArray();

            foreach (var bimestre in request.Bimestres)
            {
                var nomeBimestre = request.Bimestres != null && request.Bimestres.Any() &&
                                      request.Bimestres.Count() == 1 ? "" : (bimestre > 0 ? $"{bimestre}º BIMESTRE" : "FINAL");
                var bimestreRelatorio = new RelatorioAcompanhamentoRegistrosPedagogicosBimestreInfantilDto(nomeBimestre);

                foreach (var turma in request.DadosPedagogicosTurmas)
                {
                    var turmaRelatorio = new RelatorioAcompanhamentoRegistrosPedagogicosTurmaInfantilDto()
                    {
                        Nome = $"{turma.SiglaModalidade} - {turma.Nome} - {turma.ProfessorResponsavel} ({turma.ProfessorRF})",
                        Aulas = turma.Aulas,
                        FrequenciasPendentes = turma.FrequenciasPendentes, 
                        DataUltimoRegistroFrequencia = turma.DataUltimoRegistroFrequencia,
                        DiarioBordoPendentes = turma.DiarioBordoPendentes,
                        DataUltimoRegistroDiarioBordo = turma.DataUltimoRegistroDiarioBordo
                    };  
                    bimestreRelatorio.TurmasInfantil.Add(turmaRelatorio);
                }
            }
            return await Task.FromResult(relatorio);
        }

        private void MontarCabecalho(RelatorioAcompanhamentoRegistrosPedagogicosInfantilDto relatorio, Dre dre, Ue ue, IEnumerable<Turma> turmas, int[] bimestres, string nomeUsuario, string rfUsuario)
        {
            string turma = "TODAS";
            string bimestre = "TODOS";

            if (turmas != null && turmas.Any())
            {
                if (turmas.Count() == 1)
                    turma = turmas.FirstOrDefault().NomeRelatorio;
            }

            if (bimestres != null && bimestres.Any())
            {
                if (bimestres.Contains(0))
                {
                    var strBimestres = bimestres.Select(x => x.ToString()).ToList();

                    for (int i = 0; i < strBimestres.Count(); i++)
                    {
                        if (strBimestres[i].Equals("0"))
                            strBimestres[i] = "FINAL";
                        else
                            strBimestres[i] = $"{strBimestres[i]}º";
                    }

                    strBimestres = strBimestres.OrderBy(b => b).ToList();

                    bimestre = string.Join(",", strBimestres);
                }
                else if (bimestres.Count() == 1)
                    bimestre = $"{bimestres.FirstOrDefault()}º";
                else
                    bimestre = string.Join(", ", bimestres.Select(b => $"{b}º").OrderBy(b => b));
            }

            relatorio.Cabecalho = new RelatorioAcompanhamentoRegistrosPedagogicosCabecalhoDto()
            {
                Dre = dre != null ? dre.Abreviacao : "TODAS",
                Ue = ue != null ? ue.NomeRelatorio : "TODAS",
                Bimestre = bimestre,
                Turma = turma,
                UsuarioNome = nomeUsuario,
                UsuarioRF = rfUsuario
            };
        }
    }
}