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
    public class MontarRelatorioAcompanhamentoRegistrosPedagogicosQueryHandler : IRequestHandler<MontarRelatorioAcompanhamentoRegistrosPedagogicosQuery, RelatorioAcompanhamentoRegistrosPedagogicosDto>
    {
        public async Task<RelatorioAcompanhamentoRegistrosPedagogicosDto> Handle(MontarRelatorioAcompanhamentoRegistrosPedagogicosQuery request, CancellationToken cancellationToken)
        {
            var relatorio = new RelatorioAcompanhamentoRegistrosPedagogicosDto();
            var lstComponentesCurriculares = request.ComponentesCurriculares;

            MontarCabecalho(relatorio, request.Dre, request.Ue,  request.Turmas, request.Bimestres, request.UsuarioNome, request.UsuarioRF);

            if (request.Bimestres == null || !request.Bimestres.Any())
            {
                var bimestres = new List<int>();
                bimestres = bimestres.Distinct().OrderBy(b => b == 0).ThenBy(b => b).ToList();
                request.Bimestres = bimestres.ToArray();
            }
            else
                request.Bimestres = request.Bimestres.OrderBy(b => b == 0).ThenBy(b => b).ToArray();

            foreach (var turma in request.Turmas)
            {
                var turmaNome = request.Turmas != null && request.Turmas.Any() &&
                                request.Turmas.Count() == 1 ? "" : turma.NomeRelatorio;

                var turmaRelatorio = new RelatorioAcompanhamentoFechamentoTurmaDto(turmaNome);

                foreach (var bimestre in request.Bimestres)
                {
                    var nomeBimestre = request.Bimestres != null && request.Bimestres.Any() &&
                                       request.Bimestres.Count() == 1 ? "" : (bimestre > 0 ? $"{bimestre}º BIMESTRE" : "FINAL");

                    var bimestreRelatorio = new RelatorioAcompanhamentoFechamentoBimestreDto(nomeBimestre);

                    turmaRelatorio.Bimestres.Add(bimestreRelatorio);
                }

               // relatorio.Turmas.Add(turmaRelatorio);
            }

            return await Task.FromResult(relatorio);
        }

        private void MontarCabecalho(RelatorioAcompanhamentoRegistrosPedagogicosDto relatorio, Dre dre, Ue ue, IEnumerable<Turma> turmas, int[] bimestres, string nomeUsuario, string rfUsuario)
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

            //relatorio.Bimestre = bimestre;
            //relatorio.Data = DateTime.Now.ToString("dd/MM/yyyy");
            //relatorio.DreNome = dre != null ? dre.Abreviacao : "TODAS";
            //relatorio.UeNome = ue != null ? ue.NomeRelatorio : "TODAS";
            //relatorio.Turma = turma;
            //relatorio.Usuario = usuario.Nome;
            //relatorio.RF = usuario.CodigoRf;
        }
    }
}
