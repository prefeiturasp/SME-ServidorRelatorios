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
    public class MontasRelatorioAcompanhamentoFechamentoConselhoClasseConsolidadoQueryHandler : IRequestHandler<MontasRelatorioAcompanhamentoFechamentoConselhoClasseConsolidadoQuery, RelatorioAcompanhamentoFechamentoConsolidadoPorUeDto>
    {
        public async Task<RelatorioAcompanhamentoFechamentoConsolidadoPorUeDto> Handle(MontasRelatorioAcompanhamentoFechamentoConselhoClasseConsolidadoQuery request, CancellationToken cancellationToken)
        {
            var relatorio = new RelatorioAcompanhamentoFechamentoConsolidadoPorUeDto();
            MontarCabecalho(relatorio, request.Dre, request.Ue, request.TurmasCodigo, request.Turmas, request.Bimestres, request.Usuario);
            if (request.Bimestres == null || !request.Bimestres.Any())
            {
                var bimestresFechamento = request.ConsolidadoFechamento.Select(f => f.Bimestre);
                var bimestresConselho = request.ConsolidadoConselhosClasse.Select(f => f.Bimestre);

                var bimestres = new List<int>();
                bimestres.AddRange(bimestresFechamento);
                bimestres.AddRange(bimestresConselho);

                bimestres = bimestres.Distinct().OrderBy(b => b == 0).ThenBy(b => b).ToList();

                request.Bimestres = bimestres.ToArray();
            }
            else
                request.Bimestres = request.Bimestres.OrderBy(b => b == 0).ThenBy(b => b).ToArray();

            foreach (var turma in request.Turmas)
            {
                var turmaNome = request.TurmasCodigo != null && request.TurmasCodigo.Any() &&
                                request.TurmasCodigo.Count() == 1 ? "" : turma.NomeRelatorio;
                var nomeUe = request.ConsolidadoFechamento?.Where(x => x.TurmaCodigo == turma.Codigo).FirstOrDefault()?.NomeUe;
                var uesRelatorio = new RelatorioAcompanhamentoFechamentoConsolidadoUesDto(nomeUe);
                var fechamentoConsolidadoTurmas = new RelatorioAcompanhamentoFechamentoConsolidadoTurmasDto(turmaNome);

                foreach (var bimestre in request.Bimestres)
                {
                    var nomeBimestre = request.Bimestres != null && request.Bimestres.Any() &&
                                request.Bimestres.Count() == 1 ? "" : (bimestre > 0 ? $"{bimestre}º BIMESTRE" : "FINAL");

                    var fechamentos = request.ConsolidadoFechamento.Where(f => f.TurmaCodigo == turma.Codigo && f.Bimestre == bimestre).OrderBy(x => x.NomeUe);
                    var conselhos = request.ConsolidadoConselhosClasse.Where(f => f.TurmaCodigo == turma.Codigo && f.Bimestre == bimestre).OrderBy(x => x.NomeUe);
                    var bimestres = new RelatorioAcompanhamentoFechamentoConsolidadoBimestresDto(nomeBimestre);

                    foreach (var fechamento in fechamentos)
                    {
                        var fechamentoConselhoClasseConsolidado = new RelatorioAcompanhamentoFechamentoConselhoClasseConsolidadoDto(fechamento.NomeTurma);
                        fechamentoConselhoClasseConsolidado.FechamentoConsolidado.Add(new RelatorioAcompanhamentoFechamentoConsolidadoDto()
                        {
                            NaoIniciado = fechamento.NaoIniciado,
                            ProcessadoComPendencia = fechamento.ProcessadoComPendencia,
                            ProcessadoComSucesso = fechamento.ProcessadoComPendencia
                        });
                        bimestres.FechamentoConselhoClasseConsolidado.Add(fechamentoConselhoClasseConsolidado);
                    }
                    foreach (var conselho in conselhos)
                    {
                        var fechamentoConselhoClasseConsolidado = new RelatorioAcompanhamentoFechamentoConselhoClasseConsolidadoDto(conselho.NomeTurma);
                        fechamentoConselhoClasseConsolidado.ConselhoDeClasseConsolidado.Add(new RelatorioAcompanhamentoConselhoClasseConsolidadoDto()
                        {
                            NaoIniciado = conselho.NaoIniciado,
                            EmAndamento = conselho.EmAndamento,
                            Concluido = conselho.Concluido
                        });
                    }
                    if (bimestres?.FechamentoConselhoClasseConsolidado.Count() > 0)
                        fechamentoConsolidadoTurmas.Bimestres.Add(bimestres);

                    if (fechamentoConsolidadoTurmas?.Bimestres.Count() > 0)
                        uesRelatorio.Turmas.Add(fechamentoConsolidadoTurmas);
                }
                if (uesRelatorio?.Turmas?.Count() > 0)
                    relatorio.Ues.Add(uesRelatorio);
            }

            return await Task.FromResult(relatorio);
        }
        private void MontarCabecalho(RelatorioAcompanhamentoFechamentoConsolidadoPorUeDto relatorio, Dre dre, Ue ue, string[] turmasCodigo, IEnumerable<Turma> turmas, int[] bimestres, Usuario usuario)
        {
            string turma = "TODAS";
            string bimestre = "TODOS";

            if (turmasCodigo != null && turmasCodigo.Any())
            {
                if (turmasCodigo.Count() == 1)
                    turma = turmas.FirstOrDefault().NomeRelatorio;
                else
                    turma = string.Join(",", turmas.Select(t => t.NomeRelatorio));
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

            relatorio.Bimestre = bimestre;
            relatorio.Data = DateTime.Now.ToString("dd/MM/yyyy");
            relatorio.DreNome = dre != null ? dre.Abreviacao : "TODAS";
            relatorio.UeNome = ue != null ? ue.NomeRelatorio : "TODAS";
            relatorio.Turma = turma;
            relatorio.Usuario = usuario.Nome;
            relatorio.RF = usuario.CodigoRf;
        }
    }
}
