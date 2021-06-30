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

            MontarCabecalho(relatorio, request.Dre, request.Ue, request.TurmaCodigo, request.Turmas, request.Bimestres, request.Usuario);

            if (request.Bimestres == null)
            {
                var bimestresFechamento = request.ConsolidadoFechamento.Select(f => f.Bimestre);
                var bimestresConselho = request.ConsolidadoConselhosClasse.Select(f => f.Bimestre);

                var bimestres = new List<int>();
                bimestres.AddRange(bimestresFechamento);
                bimestres.AddRange(bimestresConselho);

                request.Bimestres = bimestres.Distinct().OrderBy(b => b).ToArray();
            }

            foreach (var turma in request.Turmas)
            {
                var turmaNome = !string.IsNullOrEmpty(request.TurmaCodigo) ? turma.NomeRelatorio : "";

                var turmaRelatorio = new RelatorioAcompanhamentoFechamentoTurmaDto(turmaNome);

                foreach (var bimestre in request.Bimestres)
                {
                    var nomeBimestre = request.Bimestres != null && request.Bimestres.Any() &&
                                       request.Bimestres.Count() == 1 ? "" : $"{bimestre}º BIMESTRE";

                    var bimestreRelatorio = new RelatorioAcompanhamentoFechamentoBimestreDto(nomeBimestre);

                    var fechamentos = request.ConsolidadoFechamento.Where(f => f.TurmaCodigo == turma.Codigo && f.Bimestre == bimestre);
                    var conselhos = request.ConsolidadoConselhosClasse.Where(f => f.TurmaCodigo == turma.Codigo && f.Bimestre == bimestre);

                    foreach (var fechamento in fechamentos)
                    {
                        var componenteNome = request.ComponentesCurriculares.FirstOrDefault(cc => cc.CodigoComponenteCurricular == fechamento.ComponenteCurricularCodigo).Nome;
                        var descricaoStatus = fechamento.Status.Name();
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

                    bimestreRelatorio.ConselhosClasse = MapearRetornoStatusAgrupado(conselhos.GroupBy(c => c.Status));

                    turmaRelatorio.Bimestres.Add(bimestreRelatorio);
                }

                relatorio.Turmas.Add(turmaRelatorio);
            }

            return await Task.FromResult(relatorio);
        }

        private void MontarCabecalho(RelatorioAcompanhamentoFechamentoPorUeDto relatorio, Dre dre, Ue ue, string turmaCodigo, IEnumerable<Turma> turmas, int[] bimestres, Usuario usuario)
        {
            Turma turma = null;
            string bimestre = "TODOS";

            if (!string.IsNullOrEmpty(turmaCodigo))
                turma = turmas.FirstOrDefault();

            if (bimestres != null && bimestres.Any())
                bimestre = string.Join("º,", bimestres);

            relatorio.Bimestre = bimestre;
            relatorio.Data = DateTime.Now.ToString("dd/MM/yyyy");
            relatorio.DreNome = dre != null ? dre.Nome : "TODAS";
            relatorio.UeNome = ue != null ? ue.Nome : "TODAS";
            relatorio.Turma = turma != null ? turma.NomeRelatorio : "TODAS";
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
