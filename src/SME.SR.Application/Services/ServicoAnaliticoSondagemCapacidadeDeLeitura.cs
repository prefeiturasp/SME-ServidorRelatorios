﻿using SME.SR.Application.Interfaces;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using SME.SR.Data.Interfaces.Sondagem;
using SME.SR.Data.Models;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SME.SR.Application.Services
{
    public class ServicoAnaliticoSondagemCapacidadeDeLeitura : ServicoAnaliticoSondagemAbstract, IServicoAnaliticoSondagemCapacidadeDeLeitura
    {
        public ServicoAnaliticoSondagemCapacidadeDeLeitura(
                                IAlunoRepository alunoRepository,
                                IDreRepository dreRepository,
                                IUeRepository ueRepository,
                                ISondagemRelatorioRepository sondagemRelatorioRepository,
                                ISondagemAnaliticaRepository sondagemAnaliticaRepository,
                                ITurmaRepository turmaRepository) : base(alunoRepository, dreRepository, ueRepository, sondagemRelatorioRepository, sondagemAnaliticaRepository, turmaRepository)
        {
        }

        public async Task<IEnumerable<RelatorioSondagemAnaliticoPorDreDto>> ObterRelatorio(FiltroRelatorioAnaliticoSondagemDto filtro)
        {
            this.filtro = filtro;
            periodoFixoSondagem = await ObterPeriodoFixoSondagemPortugues(true);

            var retorno = new List<RelatorioSondagemAnaliticoPorDreDto>();
            var perguntasRespostas = await ConsolidadoCapacidadeLeitura(periodoFixoSondagem.PeriodoId);
            var dres = await ObterDres();

            if (perguntasRespostas.Any(x => x.CodigoDre != null))
            {
                var agrupadoPorDre = ObterAgrupamentoPorDre(dres, perguntasRespostas);
                foreach (var itemDre in agrupadoPorDre)
                {
                    var dre = dres.FirstOrDefault(x => x.Codigo == itemDre.Key);
                    var perguntas = new RelatorioSondagemAnaliticoCapacidadeDeLeituraDto();
                    var uesDre = await ObterUesDre(dres.First(d => d.Codigo == itemDre.Key).Id);                       
                    var agrupadoPorUe = ObterAgrupamentoPorUe(uesDre, itemDre).ToList();

                    perguntas.Respostas.AddRange(await ObterRespostas(agrupadoPorUe,dre));

                    perguntas.Dre = dre.Nome;
                    perguntas.DreSigla = dre.Abreviacao;
                    perguntas.AnoLetivo = filtro.AnoLetivo;
                    perguntas.Periodo = filtro.Periodo;
                    retorno.Add(perguntas);
                }
            }

            return retorno;
        }

        protected override bool EhTodosPreenchidos()
        {
            return EhPreenchimentoDeTodosEstudantesIAD();
        }

        private RespostaSondagemAnaliticoCapacidadeDeLeituraDto ObterRespostaCapacidade(
                                                            IGrouping<string, OrdemPerguntaRespostaDto> respostasAnoTurma,
                                                            Ue ue,
                                                            IEnumerable<TotalDeTurmasPorAnoDto> totalDeTurmas,
                                                            int totalDeAlunos)
        {
            var resposta = new RespostaSondagemAnaliticoCapacidadeDeLeituraDto();
            var respostaOrdem = respostasAnoTurma.GroupBy(x => x.Ordem);

            resposta.OrdemDoNarrar = ObterResposta(OrdemSondagem.ORDEM_DO_NARRAR, respostaOrdem, totalDeAlunos);
            resposta.OrdemDoArgumentar = ObterResposta(OrdemSondagem.ORDEM_DO_ARGUMENTAR, respostaOrdem, totalDeAlunos);
            resposta.OrdemDoRelatar = ObterResposta(OrdemSondagem.ORDEM_DO_RELATAR, respostaOrdem, totalDeAlunos);

            resposta.Ue = ue.TituloTipoEscolaNome;
            resposta.Ano = int.Parse(respostasAnoTurma.Key);
            resposta.TotalDeTurma = ObterTotalDeTurmas(totalDeTurmas, respostasAnoTurma.Key);
            resposta.TotalDeAlunos = totalDeAlunos > 0 ? totalDeAlunos : resposta.TotalDeReposta;

            return resposta;
        }



        private async Task<List<RespostaSondagemAnaliticoCapacidadeDeLeituraDto>> ObterRespostas(
                                                                            List<IGrouping<string, OrdemPerguntaRespostaDto>> relatorioAgrupadoPorUe,
                                                                            Dre dre)
        {
            var ues = await ObterUe(relatorioAgrupadoPorUe.Select(x => x.Key).ToArray());
            var totalDeTurmas = await ObterQuantidadeTurmaPorAnoDre(dre.Id);
            var respostas = new List<RespostaSondagemAnaliticoCapacidadeDeLeituraDto>();
            var totalDeAlunosPorAno = await ObterTotalDeAlunosPorDre(dre.Codigo);

            foreach (var itemUe in relatorioAgrupadoPorUe)
            {
                var totalDeAlunosUe = await ObterTotalDeAlunosPorUe(dre.Codigo, itemUe.Key, totalDeAlunosPorAno);
                var totalTurmasUe = await ObterQuantidadeTurmaPorAnoUe(dre.Id, itemUe.Key, totalDeTurmas);
                var turmasUe = await ObterTurmasUe(itemUe.Key);
                var relatorioAgrupadoPorAno = ObterAgrupamentoPorAnoTurma(turmasUe, itemUe);
                var ue = ues.FirstOrDefault(x => x.Codigo == itemUe.Key);

                foreach (var anoTurmaItem in relatorioAgrupadoPorAno)
                {
                    respostas.Add(ObterRespostaCapacidade(
                                            anoTurmaItem,
                                            ue,
                                            totalTurmasUe,
                                            ObterTotalDeAluno(totalDeAlunosUe, anoTurmaItem.Key)));
                }
            }

            return respostas;
        }

        private Task<IEnumerable<OrdemPerguntaRespostaDto>> ConsolidadoCapacidadeLeitura(string periodoId)
        {
            return sondagemRelatorioRepository.ConsolidadoCapacidadeLeitura(new RelatorioPortuguesFiltroDto
            {
                AnoLetivo = filtro.AnoLetivo,
                CodigoDre = filtro.DreCodigo,
                CodigoUe = filtro.UeCodigo,
                ComponenteCurricularId = SondagemComponenteCurricular.LINGUA_PORTUGUESA,
                GrupoId = GrupoSondagem.CAPACIDADE_DE_LEITURA,
                PeriodoId = periodoId
            });
        }

        private RespostaCapacidadeDeLeituraDto ObterResposta(string descricaoOrdem, IEnumerable<IGrouping<string, OrdemPerguntaRespostaDto>> agrupadoOrdemPergunta, int totalDeAlunos)
        {
            var ordemPergunta = agrupadoOrdemPergunta.Where(x => x.Key == descricaoOrdem).ToList();

            return new RespostaCapacidadeDeLeituraDto
            {
                Localizacao = ObterItemResposta(ObterPerguntasPorDescricao(ordemPergunta, PerguntaDescricaoSondagem.Localizacao), totalDeAlunos),
                Inferencia = ObterItemResposta(ObterPerguntasPorDescricao(ordemPergunta, PerguntaDescricaoSondagem.Inferencia), totalDeAlunos),
                Reflexao = ObterItemResposta(ObterPerguntasPorDescricao(ordemPergunta, PerguntaDescricaoSondagem.Reflexao), totalDeAlunos),
            };
        }

        private List<OrdemPerguntaRespostaDto> ObterPerguntasPorDescricao(List<IGrouping<string, OrdemPerguntaRespostaDto>> agrupadoOrdemPergunta, string perguntaDescricao)
        {
            return agrupadoOrdemPergunta.FirstOrDefault()?.Where(x => x.PerguntaDescricao == perguntaDescricao).ToList();
        }

        private ItemRespostaCapacidadeDeLeituraDto ObterItemResposta(List<OrdemPerguntaRespostaDto> perguntasResposta, int totalDeAlunos)
        {
            var resposta = new ItemRespostaCapacidadeDeLeituraDto();

            if (perguntasResposta != null && perguntasResposta.Any())
            {
                resposta.Adequada = perguntasResposta.Where(f => f.RespostaDescricao == RespostaDescricaoSondagem.Adequada).Select(x => x.QtdRespostas).Sum();
                resposta.Inadequada = perguntasResposta.Where(f => f.RespostaDescricao == RespostaDescricaoSondagem.Inadequada).Select(x => x.QtdRespostas).Sum();
                resposta.NaoResolveu = perguntasResposta.Where(f => f.RespostaDescricao == RespostaDescricaoSondagem.NaoResolveu).Select(x => x.QtdRespostas).Sum();
                resposta.SemPreenchimento = ObterValorSemPreenchimento(perguntasResposta, totalDeAlunos);
            }
            else
            {
                resposta.SemPreenchimento = totalDeAlunos;
            }

            return resposta;
        }

        private int ObterValorSemPreenchimento(List<OrdemPerguntaRespostaDto> perguntaResposta, int totalDeAlunos)
        {
            if (EhTodosPreenchidos())
                return ObterValorSemPreenchimento(perguntaResposta);

            return ObterValorSemPreenchimentoCalculado(perguntaResposta, totalDeAlunos);
        }

        private int ObterValorSemPreenchimentoCalculado(List<OrdemPerguntaRespostaDto> perguntaResposta, int totalDeAlunos)
        {
            var totalRespostas = perguntaResposta.Select(s => s.QtdRespostas).ToList().Sum();

            totalDeAlunos = totalDeAlunos >= totalRespostas ? totalDeAlunos : totalRespostas;

            return totalDeAlunos - totalRespostas;
        }

        private int ObterValorSemPreenchimento(List<OrdemPerguntaRespostaDto> perguntaResposta)
        {
            return perguntaResposta.Where(f => f.RespostaDescricao == RespostaDescricaoSondagem.SemPreenchimento).Select(x => x.QtdRespostas).Sum();
        }

        private IEnumerable<IGrouping<string, OrdemPerguntaRespostaDto>> ObterAgrupamentoPorDre(IEnumerable<Dre> dres, IEnumerable<OrdemPerguntaRespostaDto> perguntasRespostas)
            => filtro.ApresentarTurmasUesDresSemLancamento  
                ?  (from dre in dres
                        from c in perguntasRespostas
                        select dre.Codigo == c.CodigoDre ? c : new OrdemPerguntaRespostaDto() { CodigoDre = dre.Codigo })
                                            .GroupBy(x => x.CodigoDre).Distinct()
                : perguntasRespostas.Where(x => x.CodigoDre != null).GroupBy(x => x.CodigoDre);


        private IEnumerable<IGrouping<string, OrdemPerguntaRespostaDto>> ObterAgrupamentoPorUe(IEnumerable<UePorDresIdResultDto> ues, IEnumerable<OrdemPerguntaRespostaDto> perguntasRespostas)
            => filtro.ApresentarTurmasUesDresSemLancamento
                ? (from ue in ues
                   from i in perguntasRespostas
                   select ue.Codigo == i.CodigoUe ? i : new OrdemPerguntaRespostaDto() { CodigoUe = ue.Codigo, AnoTurma = i.AnoTurma })
                                             .GroupBy(x => x.CodigoUe).Distinct()
                : perguntasRespostas.GroupBy(x => x.CodigoUe);


        private IEnumerable<IGrouping<string, OrdemPerguntaRespostaDto>> ObterAgrupamentoPorAnoTurma(IEnumerable<Turma> turmas, IEnumerable<OrdemPerguntaRespostaDto> perguntasRespostas)
         => filtro.ApresentarTurmasUesDresSemLancamento
                ? (from t in turmas
                 from i in perguntasRespostas
                 where (i.AnoTurma != null && i.AnoTurma != "0" && i.AnoTurma.All(x => char.IsDigit(x)) && int.Parse(i.AnoTurma) > 0) || (t.Ano == i.AnoTurma)
                 select t.Codigo == i.CodigoTurma ? i : new OrdemPerguntaRespostaDto() { AnoTurma = t.Ano, Ordem = i.Ordem })
                                                           .GroupBy(p => p.AnoTurma)
                                                           .OrderBy(p => p.Key)
                : perguntasRespostas.Where(x => x.AnoTurma != null)
                                    .OrderBy(x => x.AnoTurma)
                                    .GroupBy(p => p.AnoTurma);

    }
}
