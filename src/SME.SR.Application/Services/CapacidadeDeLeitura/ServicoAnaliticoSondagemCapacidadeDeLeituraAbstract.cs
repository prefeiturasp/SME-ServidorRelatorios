using SME.SR.Data;
using SME.SR.Data.Interfaces;
using SME.SR.Data.Interfaces.Sondagem;
using SME.SR.Data.Models;
using SME.SR.Infra;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SME.SR.Application.Services.CapacidadeDeLeitura
{
    public abstract class ServicoAnaliticoSondagemCapacidadeDeLeituraAbstract : ServicoAnaliticoSondagemAbstract
    {
        protected ServicoAnaliticoSondagemCapacidadeDeLeituraAbstract(IAlunoRepository alunoRepository, IDreRepository dreRepository, IUeRepository ueRepository, ISondagemRelatorioRepository sondagemRelatorioRepository, ISondagemAnaliticaRepository sondagemAnaliticaRepository, ITurmaRepository turmaRepository) : base(alunoRepository, dreRepository, ueRepository, sondagemRelatorioRepository, sondagemAnaliticaRepository, turmaRepository)
        {
        }

        protected abstract Task<List<RespostaSondagemAnaliticoCapacidadeDeLeituraDto>> ObterRespostas(
                                                                                    List<IGrouping<string, OrdemPerguntaRespostaDto>> itemUe,
                                                                                    IEnumerable<Ue> ues,
                                                                                    IEnumerable<TotalDeTurmasPorAnoDto> totalDeTurmas,
                                                                                    string codigoDre,
                                                                                    string codigoUe);

        protected abstract int ObterValorSemPreenchimento(List<OrdemPerguntaRespostaDto> perguntaResposta, int totalDeAlunos);

        protected RespostaSondagemAnaliticoCapacidadeDeLeituraDto ObterRespostaCapacidade(
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

        public async Task<IEnumerable<RelatorioSondagemAnaliticoPorDreDto>> ObterRelatorio(FiltroRelatorioAnaliticoSondagemDto filtro)
        {
            this.filtro = filtro;
            periodoFixoSondagem = await ObterPeriodoFixoSondagem();

            var retorno = new List<RelatorioSondagemAnaliticoPorDreDto>();
            var modalidades = new List<int> { 5, 13 };
            var consultaDados = await ConsolidadoCapacidadeLeitura(periodoFixoSondagem.PeriodoId);
            var agrupadoPorDre = consultaDados.Where(x => x.CodigoDre != null).GroupBy(x => x.CodigoDre).Distinct().ToList();

            if (agrupadoPorDre.Any())
            {
                var listaDres = await dreRepository.ObterPorCodigos(agrupadoPorDre.Select(x => x.Key).ToArray());
                foreach (var itemDre in agrupadoPorDre)
                {
                    var perguntas = new RelatorioSondagemAnaliticoCapacidadeDeLeituraDto();
                    var agrupadoPorUe = itemDre.GroupBy(x => x.CodigoUe).Distinct().ToList();
                    var listaUes = await ueRepository.ObterPorCodigos(agrupadoPorUe.Select(x => x.Key).ToArray());
                    foreach (var itemUe in agrupadoPorUe)
                    {
                        var totalTurmas = await ObterQuantidadeTurmaPorAno(itemUe.Key, filtro.AnoLetivo);
                        var relatorioAgrupadoPorAno = itemUe.Where(x => x.AnoTurma != null).GroupBy(p => p.AnoTurma).ToList();

                        perguntas.Respostas.AddRange(await ObterRespostas(
                                                                relatorioAgrupadoPorAno,
                                                                listaUes,
                                                                totalTurmas,
                                                                itemDre.Key,
                                                                itemUe.Key));
                    }

                    var dre = listaDres.FirstOrDefault(x => x.Codigo == itemDre.Key);
                    perguntas.Dre = dre.Nome;
                    perguntas.DreSigla = dre.Abreviacao;
                    perguntas.AnoLetivo = filtro.AnoLetivo;
                    perguntas.Periodo = filtro.Periodo;
                    retorno.Add(perguntas);
                }
            }

            return retorno;
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

            if (perguntasResposta != null)
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
    }
}
