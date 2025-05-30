﻿using SME.SR.Application.Interfaces;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using SME.SR.Data.Interfaces.Sondagem;
using SME.SR.Data.Models;
using SME.SR.Infra.Dtos.Sondagem;
using SME.SR.Infra;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SME.SR.Application.Services
{
    public class ServicoAnaliticoSondagemLeitura : ServicoAnaliticoSondagemAbstract, IServicoAnaliticoSondagemLeitura
    {
        public ServicoAnaliticoSondagemLeitura(IAlunoRepository alunoRepository, IDreRepository dreRepository, IUeRepository ueRepository, ISondagemRelatorioRepository sondagemRelatorioRepository, ISondagemAnaliticaRepository sondagemAnaliticaRepository, ITurmaRepository turmaRepository) : base(alunoRepository, dreRepository, ueRepository, sondagemRelatorioRepository, sondagemAnaliticaRepository, turmaRepository)
        {
        }

        public async Task<IEnumerable<RelatorioSondagemAnaliticoPorDreDto>> ObterRelatorio(FiltroRelatorioAnaliticoSondagemDto filtro)
        {
            this.filtro = filtro;
            periodoFixoSondagem = await ObterPeriodoFixoSondagemPortugues(false);

            var retorno = new List<RelatorioSondagemAnaliticoPorDreDto>();
            var respostasLeitura = await sondagemAnaliticaRepository.ObterRespostasRelatorioAnaliticoDeLeitura(filtro, EhTodosPreenchidos());
            var dres = await ObterDres();

            if (respostasLeitura.Any(x => x.CodigoDre != null))
            {
                var agrupadoPorDre = ObterAgrupamentoPorDre(dres, respostasLeitura);

                foreach (var itemDre in agrupadoPorDre)
                {
                    var relatorioSondagemAnaliticoLeituraDto = new RelatorioSondagemAnaliticoLeituraDto();
                    var dre = dres.FirstOrDefault(x => x.Codigo == itemDre.Key);
                    var totalTurmas = await ObterQuantidadeTurmaPorAnoDre(dre.Id);
                    var totalAlunosDre = await ObterTotalDeAlunosPorDre(dre.Codigo);

                    var uesDre = await ObterUesDre(dres.First(d => d.Codigo == itemDre.Key).Id);
                    var agrupadoPorUe = ObterAgrupamentoPorUe(uesDre, itemDre).ToList();
                    var listaUes = await ObterUe(agrupadoPorUe.Select(x => x.Key).ToArray());

                    foreach (var itemUe in agrupadoPorUe)
                    {
                        var totalDeAlunosUe = await ObterTotalDeAlunosPorUe(dre.Codigo, itemUe.Key, totalAlunosDre);
                        var totalTurmasUe = await ObterQuantidadeTurmaPorAnoUe(dre.Id, itemUe.Key, totalTurmas);
                        var turmasUe = await ObterTurmasUe(itemUe.Key);
                        var agrupamentoPorAnoTurma = ObterAgrupamentoPorAnoTurma(turmasUe, itemUe);

                        foreach (var anoTurma in agrupamentoPorAnoTurma)
                        {
                            var ue = listaUes.FirstOrDefault(x => x.Codigo == itemUe.Key);
                            var respostaSondagemAnaliticoLeituraDto = ObterResposta(
                                                                                    anoTurma, 
                                                                                    ue, 
                                                                                    totalTurmasUe, 
                                                                                    ObterTotalDeAluno(totalDeAlunosUe, anoTurma.Key));

                            relatorioSondagemAnaliticoLeituraDto.Respostas.Add(respostaSondagemAnaliticoLeituraDto);
                        }
                    }

                    relatorioSondagemAnaliticoLeituraDto.Dre = dre.Nome;
                    relatorioSondagemAnaliticoLeituraDto.DreSigla = dre.Abreviacao;
                    relatorioSondagemAnaliticoLeituraDto.AnoLetivo = filtro.AnoLetivo;
                    relatorioSondagemAnaliticoLeituraDto.Periodo = filtro.Periodo;
                    retorno.Add(relatorioSondagemAnaliticoLeituraDto);
                }
            }

            return retorno;
        }

        protected override bool EhTodosPreenchidos()
        {
            return EhPreenchimentoDeTodosEstudantes();
        }

        private RespostaSondagemAnaliticoLeituraDto ObterResposta(
                                                                IGrouping<string, TotalRespostasAnaliticoLeituraDto> respostaAnoTurma,
                                                                Ue ue,
                                                                IEnumerable<TotalDeTurmasPorAnoDto> totalTurmaUe,
                                                                int totalDeAlunos)
        {
            var resposta = new RespostaSondagemAnaliticoLeituraDto();

            return new RespostaSondagemAnaliticoLeituraDto
            {
                Nivel1 = respostaAnoTurma.Select(x => x.Nivel1).Sum(),
                Nivel2 = respostaAnoTurma.Select(x => x.Nivel2).Sum(),
                Nivel3 = respostaAnoTurma.Select(x => x.Nivel3).Sum(),
                Nivel4 = respostaAnoTurma.Select(x => x.Nivel4).Sum(),
                SemPreenchimento = ObterTotalSemPreenchimento(respostaAnoTurma, totalDeAlunos),
                TotalDeAlunos = totalDeAlunos,
                Ano = int.Parse(respostaAnoTurma.Key),
                TotalDeTurma = totalTurmaUe?.FirstOrDefault(t => t.Ano == respostaAnoTurma.Key).Quantidade ?? 0,
                Ue = ue.TituloTipoEscolaNome
            };
        }

        private int ObterTotalSemPreenchimento(
                            IGrouping<string, TotalRespostasAnaliticoLeituraDto> respostaAnoTurma,
                            int totalDeAlunos)
        {
            if (EhTodosPreenchidos())
                return ObterTotalSemPreenchimento(respostaAnoTurma);

            return ObterTotalSemPreenchimentoCalculado(respostaAnoTurma, totalDeAlunos);
        }

        private int ObterTotalSemPreenchimento(IGrouping<string, TotalRespostasAnaliticoLeituraDto> respostaAnoTurma)
        {
            return respostaAnoTurma.Select(x => x.SemPreenchimento).Sum();
        }

        private int ObterTotalSemPreenchimentoCalculado(
                            IGrouping<string, TotalRespostasAnaliticoLeituraDto> respostaAnoTurma,
                            int totalDeAlunos)
        {
            var totalRepostas = respostaAnoTurma.Select(x => x.Nivel1).Sum()
                                + respostaAnoTurma.Select(x => x.Nivel2).Sum()
                                + respostaAnoTurma.Select(x => x.Nivel3).Sum()
                                + respostaAnoTurma.Select(x => x.Nivel4).Sum();

            return totalDeAlunos <= totalRepostas ? 0 : totalDeAlunos - totalRepostas;
        }

        private IEnumerable<IGrouping<string, TotalRespostasAnaliticoLeituraDto>> ObterAgrupamentoPorDre(IEnumerable<Dre> dres, IEnumerable<TotalRespostasAnaliticoLeituraDto> perguntasRespostas)
            => filtro.ApresentarTurmasUesDresSemLancamento
                ? (from dre in dres
                    from c in perguntasRespostas
                    select dre.Codigo == c.CodigoDre ? c : new TotalRespostasAnaliticoLeituraDto() { CodigoDre = dre.Codigo })
                                     .GroupBy(x => x.CodigoDre).Distinct()
                : perguntasRespostas.Where(x => x.CodigoDre != null).GroupBy(x => x.CodigoDre);


        private IEnumerable<IGrouping<string, TotalRespostasAnaliticoLeituraDto>> ObterAgrupamentoPorUe(IEnumerable<UePorDresIdResultDto> ues, IEnumerable<TotalRespostasAnaliticoLeituraDto> perguntasRespostas)
            => filtro.ApresentarTurmasUesDresSemLancamento
                ? (from ue in ues
                    from i in perguntasRespostas
                    select ue.Codigo == i.CodigoUe ? i : new TotalRespostasAnaliticoLeituraDto() { CodigoUe = ue.Codigo, AnoTurma = i.AnoTurma })
                                             .GroupBy(x => x.CodigoUe).Distinct()
                : perguntasRespostas.GroupBy(x => x.CodigoUe);


        private IEnumerable<IGrouping<string, TotalRespostasAnaliticoLeituraDto>> ObterAgrupamentoPorAnoTurma(IEnumerable<Turma> turmas, IEnumerable<TotalRespostasAnaliticoLeituraDto> perguntasRespostas)
         => filtro.ApresentarTurmasUesDresSemLancamento
                ? (from t in turmas
                 from i in perguntasRespostas
                 where (i.AnoTurma != null && i.AnoTurma != "0" && i.AnoTurma.All(x => char.IsDigit(x)) && int.Parse(i.AnoTurma) > 0) || (t.Ano == i.AnoTurma)
                 select t.Codigo == i.TurmaCodigo ? i : new TotalRespostasAnaliticoLeituraDto() { AnoTurma = t.Ano, TurmaCodigo = t.Codigo })
                                                                   .GroupBy(p => p.AnoTurma)
                                                                   .OrderBy(p => p.Key)
                : perguntasRespostas.OrderBy(x => x.AnoTurma)
                                    .GroupBy(x => x.AnoTurma);
    }
}
