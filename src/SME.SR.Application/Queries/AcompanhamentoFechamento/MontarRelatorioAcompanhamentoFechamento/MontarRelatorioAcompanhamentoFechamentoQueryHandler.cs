using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using SME.SR.Infra.Utilitarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class MontarRelatorioAcompanhamentoFechamentoQueryHandler : IRequestHandler<MontarRelatorioAcompanhamentoFechamentoQuery, RelatorioAcompanhamentoFechamentoPorUeDto>
    {
        public async Task<RelatorioAcompanhamentoFechamentoPorUeDto> Handle(MontarRelatorioAcompanhamentoFechamentoQuery request, CancellationToken cancellationToken)
        {
            var relatorio = new RelatorioAcompanhamentoFechamentoPorUeDto();
            var lstComponentesCurriculares = request.ComponentesCurriculares.ToList();

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

                var turmaRelatorio = new RelatorioAcompanhamentoFechamentoTurmaDto(turmaNome);

                foreach (var bimestre in request.Bimestres)
                {
                    var nomeBimestre = request.Bimestres != null && request.Bimestres.Any() &&
                                       request.Bimestres.Count() == 1 ? "" : (bimestre > 0 ? $"{bimestre}º BIMESTRE" : "FINAL");

                    var bimestreRelatorio = new RelatorioAcompanhamentoFechamentoBimestreDto(nomeBimestre);

                    var fechamentos = request.ConsolidadoFechamento.Where(f => f.TurmaCodigo == turma.Codigo && f.Bimestre == bimestre);
                    var conselhos = request.ConsolidadoConselhosClasse.Where(f => f.TurmaCodigo == turma.Codigo && f.Bimestre == bimestre);

                    foreach (var fechamento in fechamentos)
                    {
                        var componenteNome = lstComponentesCurriculares.FirstOrDefault(cc => cc.CodDisciplina == fechamento.ComponenteCurricularCodigo).Disciplina;
                        var descricaoStatus = fechamento.StatusRelatorio.Name();
                        var pendencias = new List<string>();

                        if (request.ListarPendencias)
                        {
                            var lstPendencias = request.Pendencias.Where(p => p.TurmaCodigo == turma.Codigo &&
                                                                              p.Bimestre == bimestre &&
                                                                              p.ComponenteCurricularId == fechamento.ComponenteCurricularCodigo);

                            pendencias = lstPendencias.Select(p => p.TipoPendencia.Name()).ToList();
                        }

                        bimestreRelatorio.FechamentosComponente.Add(
                            new RelatorioAcompanhamentoFechamentoComponenteDto(componenteNome, descricaoStatus, pendencias));
                    }

                    bimestreRelatorio.FechamentosComponente = bimestreRelatorio.FechamentosComponente.OrderBy(f => lstComponentesCurriculares.FindIndex(c => c.Disciplina == f.Componente)).ToList();
                    bimestreRelatorio.ConselhosClasse = MapearRetornoStatusAgrupado(conselhos.GroupBy(c => c.Status));

                    turmaRelatorio.Bimestres.Add(bimestreRelatorio);
                }

                relatorio.Turmas.Add(turmaRelatorio);
            }

            return await Task.FromResult(relatorio);
        }

        private void MontarCabecalho(RelatorioAcompanhamentoFechamentoPorUeDto relatorio, Dre dre, Ue ue, string[] turmasCodigo, IEnumerable<Turma> turmas, int[] bimestres, Usuario usuario)
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

        private List<RelatorioAcompanhamentoFechamentoConselhoDto> MapearRetornoStatusAgrupado(IEnumerable<IGrouping<SituacaoConselhoClasse, ConselhoClasseConsolidadoTurmaAlunoDto>> statusAgrupados)
        {
            var lstStatus = new List<RelatorioAcompanhamentoFechamentoConselhoDto>();

            foreach (var status in statusAgrupados)
            {
                lstStatus.Add(new RelatorioAcompanhamentoFechamentoConselhoDto()
                {
                    Status = (int)status.Key,
                    Descricao = status.Key.Name(),
                    Quantidade = status.Count()
                });
            }

            var lstTodosStatus = Enum.GetValues(typeof(SituacaoConselhoClasse)).Cast<SituacaoConselhoClasse>();

            var statusNaoEncontrados = lstTodosStatus.Where(ls => !lstStatus.Select(s => (SituacaoConselhoClasse)s.Status).Contains(ls));

            if (statusNaoEncontrados != null && statusNaoEncontrados.Any())
            {
                foreach (var status in statusNaoEncontrados)
                {
                    lstStatus.Add(new RelatorioAcompanhamentoFechamentoConselhoDto()
                    {
                        Status = (int)status,
                        Descricao = status.Name(),
                        Quantidade = 0
                    });
                }
            }

            return lstStatus.OrderBy(o => o.Status).ToList();
        }
    }
}
