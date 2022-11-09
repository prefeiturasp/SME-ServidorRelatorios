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
    public class MontarRelatorioAcompanhamentoRegistrosPedagogicosInfantilComponenteQueryHandler : IRequestHandler<MontarRelatorioAcompanhamentoRegistrosPedagogicosInfantilComponenteQuery, RelatorioAcompanhamentoRegistrosPedagogicosInfantilComponenteDto>
    {
        public async Task<RelatorioAcompanhamentoRegistrosPedagogicosInfantilComponenteDto> Handle(MontarRelatorioAcompanhamentoRegistrosPedagogicosInfantilComponenteQuery request,
            CancellationToken cancellationToken)
        {
            var cabecalho = MontarCabecalho(request.Dre, request.Ue, request.Turmas, request.Bimestres, request.UsuarioNome, request.UsuarioRF);
            return await Task.FromResult(new RelatorioAcompanhamentoRegistrosPedagogicosInfantilComponenteDto(request.DadosBimestre, cabecalho));
        }

        private RelatorioAcompanhamentoRegistrosPedagogicosCabecalhoDto MontarCabecalho(Dre dre, Ue ue, IEnumerable<Turma> turmas, int[] bimestres, string nomeUsuario, string rfUsuario)
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

            return new RelatorioAcompanhamentoRegistrosPedagogicosCabecalhoDto()
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