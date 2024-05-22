using SME.SR.Application.Interfaces;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using SME.SR.Data.Interfaces.Sondagem;
using SME.SR.Data.Models;
using SME.SR.Infra;
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
            periodoFixoSondagem = await ObterPeriodoFixoSondagem();

            var retorno = new List<RelatorioSondagemAnaliticoPorDreDto>();
            var consultaDados = await ConsolidadoCapacidadeLeitura(periodoFixoSondagem.PeriodoId);
            var agrupadoPorDre = consultaDados.Where(x => x.CodigoDre != null).GroupBy(x => x.CodigoDre).Distinct().ToList();

            if (agrupadoPorDre.Any())
            {
                var listaDres = await ObterDres(agrupadoPorDre.Select(x => x.Key).ToArray());
                foreach (var itemDre in agrupadoPorDre)
                {
                    var dre = listaDres.FirstOrDefault(x => x.Codigo == itemDre.Key);
                    var perguntas = new RelatorioSondagemAnaliticoCapacidadeDeLeituraDto();
                    var agrupadoPorUe = itemDre.GroupBy(x => x.CodigoUe).Distinct().ToList();
                    var listaUes = await ObterUe(agrupadoPorUe.Select(x => x.Key).ToArray());
                    var totalTurmas = await ObterQuantidadeTurmaPorAno(dre.Id, filtro.AnoLetivo);

                    perguntas.Respostas.AddRange(await ObterRespostas(
                                        agrupadoPorUe,
                                        listaUes,
                                        totalTurmas,
                                        itemDre.Key));

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
            return ComPreenchimentoDeTodosEstudantesIAD();
        }

        private RespostaSondagemAnaliticoCapacidadeDeLeituraDto ObterRespostaCapacidade(
                                                            IEnumerable<IGrouping<string, OrdemPerguntaRespostaDto>> respostasAnoTurma,
                                                            IEnumerable<Ue> ues,
                                                            string codigoUe,
                                                            int anoTurma,
                                                            IEnumerable<TotalDeTurmasPorAnoDto> totalDeTurmas,
                                                            int totalDeAlunos = 0)
        {
            var resposta = new RespostaSondagemAnaliticoCapacidadeDeLeituraDto();

            resposta.OrdemDoNarrar = ObterResposta(OrdemSondagem.ORDEM_DO_NARRAR, respostasAnoTurma, totalDeAlunos);
            resposta.OrdemDoArgumentar = ObterResposta(OrdemSondagem.ORDEM_DO_ARGUMENTAR, respostasAnoTurma, totalDeAlunos);
            resposta.OrdemDoRelatar = ObterResposta(OrdemSondagem.ORDEM_DO_RELATAR, respostasAnoTurma, totalDeAlunos);

            var Ue = ues.FirstOrDefault(x => x.Codigo == codigoUe);
            resposta.Ue = Ue.TituloTipoEscolaNome;
            resposta.Ano = anoTurma;
            resposta.TotalDeTurma = totalDeTurmas?.FirstOrDefault(t => t.Ano == anoTurma.ToString()).Quantidade ?? 0;
            resposta.TotalDeAlunos = totalDeAlunos > 0 ? totalDeAlunos : resposta.TotalDeReposta;

            return resposta;
        }

        private async Task<List<RespostaSondagemAnaliticoCapacidadeDeLeituraDto>> ObterRespostas(
                                                                            List<IGrouping<string, OrdemPerguntaRespostaDto>> relatorioAgrupadoPorUe,
                                                                            IEnumerable<Ue> ues,
                                                                            IEnumerable<TotalDeTurmasPorAnoDto> totalDeTurmas,
                                                                            string codigoDre)
        {
            var respostas = new List<RespostaSondagemAnaliticoCapacidadeDeLeituraDto>();
            var totalDeAlunosPorAno = await ObterTotalDeAlunosAnoTurma(codigoDre);

            foreach (var itemUe in relatorioAgrupadoPorUe)
            {
                var totalDeAlunosUe = ObterTotalDeAlunosPorUe(totalDeAlunosPorAno, itemUe.Key);
                var totalTurmasUe = totalDeTurmas.Where(x => x.CodigoUe == itemUe.Key);
                var relatorioAgrupadoPorAno = itemUe.Where(x => x.AnoTurma != null).GroupBy(p => p.AnoTurma).ToList();

                foreach (var anoTurmaItem in relatorioAgrupadoPorAno)
                {
                    respostas.Add(ObterRespostaCapacidade(
                                            anoTurmaItem.GroupBy(x => x.Ordem),
                                            ues,
                                            itemUe.Key,
                                            int.Parse(anoTurmaItem.Key),
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

        private Task<PeriodoFixoSondagem> ObterPeriodoFixoSondagem()
        {
            return ObterPeriodoFixoSondagem(ObterTituloSemestreBimestrePortugues(true));
        }

        private int ObterValorSemPreenchimento(List<OrdemPerguntaRespostaDto> perguntaResposta, int totalDeAlunos)
        {
            if (ComPreenchimentoDeTodosEstudantesIAD())
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
    }
}
