using SME.SR.Data;
using SME.SR.Data.Interfaces;
using SME.SR.Data.Interfaces.Sondagem;
using SME.SR.Infra;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SME.SR.Application.Services
{
    public abstract class ServicoAnaliticoSondagemIADNumerosAbstract : ServicoAnaliticoSondagemAbstract
    {
        protected ServicoAnaliticoSondagemIADNumerosAbstract(IAlunoRepository alunoRepository, IDreRepository dreRepository, IUeRepository ueRepository, ISondagemRelatorioRepository sondagemRelatorioRepository, ISondagemAnaliticaRepository sondagemAnaliticaRepository, ITurmaRepository turmaRepository) : base(alunoRepository, dreRepository, ueRepository, sondagemRelatorioRepository, sondagemAnaliticaRepository, turmaRepository)
        {
        }

        protected abstract ProficienciaSondagemEnum ObterProficiencia();

        protected abstract Task<IEnumerable<PerguntaRespostaOrdemDto>> ObterPerguntasRespostaAnterior();

        public async Task<IEnumerable<RelatorioSondagemAnaliticoPorDreDto>> ObterRelatorio(FiltroRelatorioAnaliticoSondagemDto filtro)
        {
            this.filtro = filtro;
            periodoFixoSondagem = await ObterPeriodoFixoSondagemMatematica(EhIAD());

            var retorno = new List<RelatorioSondagemAnaliticoNumeroIadDto>();
            var perguntasRespostas = await ObterPerguntasResposta();
            //var agrupadoPorDre = perguntasRespostas.Where(x => x.CodigoDre != null).GroupBy(x => x.CodigoDre);
            var dres = filtro.DreCodigo == TODOS ?
                await dreRepository.ObterTodas() : new Dre[] { await dreRepository.ObterPorCodigo(filtro.DreCodigo) };

            if (perguntasRespostas.Any(x => x.CodigoDre != null))
            {
                //var dres = await ObterDres(agrupadoPorDre.Select(x => x.Key).ToArray());
                var agrupadoPorDre = (from dre in dres
                                      from c in perguntasRespostas
                                      select dre.Codigo == c.CodigoDre ? c : new PerguntaRespostaOrdemDto() { CodigoDre = dre.Codigo })
                                 .GroupBy(x => x.CodigoDre).Distinct();

                var cabecalhos = ObterCabecalho(perguntasRespostas).ToList();

                foreach (var itemDre in agrupadoPorDre)
                {
                    var dre = dres.FirstOrDefault(x => x.Codigo == itemDre.Key);
                    //var agrupamentoPorUe = itemDre.GroupBy(x => x.CodigoUe).ToList();
                    var uesDre = filtro.UeCodigo == TODOS ? await ueRepository
                        .ObterPorDresId(new long[] { dres.First(d => d.Codigo == itemDre.Key).Id }) :
                        new UePorDresIdResultDto[] { new UePorDresIdResultDto() { Codigo = filtro.UeCodigo } };

                    var agrupamentoPorUe = (from ue in uesDre
                                            from i in itemDre
                                            select ue.Codigo == i.CodigoUe ? i : new PerguntaRespostaOrdemDto() { CodigoUe = ue.Codigo, AnoTurma = i.AnoTurma })
                                         .GroupBy(x => x.CodigoUe).Distinct().ToList();

                    var respostas = await ObterRespostas(agrupamentoPorUe, dre, cabecalhos);

                    retorno.Add(new RelatorioSondagemAnaliticoNumeroIadDto(filtro.TipoSondagem)
                    {
                        Dre = dre.Nome,
                        DreSigla = dre.Abreviacao,
                        AnoLetivo = filtro.AnoLetivo,
                        Periodo = filtro.Periodo,
                        ColunasDoCabecalho = cabecalhos,
                        Respostas = respostas.ToList()
                    });
                }
            }

            return retorno;
        }

        private async Task<IEnumerable<RespostaSondagemAnaliticaNumeroIadDto>> ObterRespostas(
                                                                                        List<IGrouping<string, PerguntaRespostaOrdemDto>> agrupadoRespostasPorUe,
                                                                                        Dre dre,
                                                                                        IEnumerable<CabecalhoSondagemAnaliticaDto> cabecalhos)
        {
            var ues = await ObterUe(agrupadoRespostasPorUe.Select(x => x.Key).ToArray());
            var totalDeTurmas = await ObterQuantidadeTurmaPorAnoDre(dre.Id);
            var respostas = new List<RespostaSondagemAnaliticaNumeroIadDto>();
            var totalDeAlunosPorAno = await ObterTotalDeAlunosPorDre(dre.Codigo);

            foreach (var itemUe in agrupadoRespostasPorUe)
            {
                //var agrupamentoPorAnoTurma = itemUe.OrderBy(x => x.AnoTurma).ThenBy(x => x.OrdemPergunta).GroupBy(x => x.AnoTurma);
                var totalDeAlunosUe = await ObterTotalDeAlunosPorUe(dre.Codigo, itemUe.Key, totalDeAlunosPorAno);
                var totalTurmasUe = await ObterQuantidadeTurmaPorAnoUe(dre.Id, itemUe.Key, totalDeTurmas);
                var ue = ues.FirstOrDefault(x => x.Codigo == itemUe.Key);
                var turmasUe = (await turmaRepository
                            .ObterTurmasPorUeEAnoLetivo(itemUe.Key, filtro.AnoLetivo))
                            .Where(t => t.Ano.All(x => char.IsDigit(x)) && int.Parse(t.Ano) > 0 && t.ModalidadeCodigo == Modalidade.Fundamental);

                var agrupamentoPorAnoTurma = (from t in turmasUe
                                              from i in itemUe
                                              where (i.AnoTurma != null && i.AnoTurma != "0" && i.AnoTurma.All(x => char.IsDigit(x)) && int.Parse(i.AnoTurma) > 0) || (t.Ano == i.AnoTurma)
                                              select t.Codigo == i.CodigoTurma ? i : new PerguntaRespostaOrdemDto() { AnoTurma = t.Ano, CodigoTurma = t.Codigo })
                                                       .OrderBy(p => p.AnoTurma)
                                                       .ThenBy(p => p.OrdemPergunta)
                                                       .GroupBy(p => p.AnoTurma);

                foreach (var anoTurma in agrupamentoPorAnoTurma)
                {
                    var agrupamentoOrdemPergunta = anoTurma.GroupBy(p => new { p.OrdemPergunta, p.PerguntaDescricao });
                    var reposta = new RespostaSondagemAnaliticaNumeroIadDto()
                    {
                        TotalDeAlunos = ObterTotalDeAlunos(anoTurma.ToList(), anoTurma.Key, totalDeAlunosUe),
                        Ano = int.Parse(anoTurma.Key),
                        TotalDeTurma = ObterTotalDeTurmas(totalTurmasUe, anoTurma.Key),
                        Ue = ue.TituloTipoEscolaNome
                    };

                    foreach (var ordemPergunta in agrupamentoOrdemPergunta)
                    {
                        var cabecalho = cabecalhos.FirstOrDefault(c => c.Ordem == ordemPergunta.Key.OrdemPergunta && c.Descricao == ordemPergunta.Key.PerguntaDescricao);
                        if (cabecalho != null)
                          reposta.Respostas.AddRange(ObterRespostas(ordemPergunta, cabecalho.SubCabecalhos, reposta.TotalDeAlunos));
                    }

                    respostas.Add(reposta);
                }
            }

            return respostas;
        }

        private IEnumerable<CabecalhoSondagemAnaliticaDto> ObterCabecalho(IEnumerable<PerguntaRespostaOrdemDto> respostasPerguntas)
        {
            var cabecalhos = new List<CabecalhoSondagemAnaliticaDto>();
            var perguntas = respostasPerguntas.GroupBy(x => new { x.OrdemPergunta, x.PerguntaDescricao });

            foreach(var pergunta in perguntas)
            {
                cabecalhos.Add(new CabecalhoSondagemAnaliticaDto
                {
                    Descricao = pergunta.Key.PerguntaDescricao,
                    Ordem = pergunta.Key.OrdemPergunta,
                    SubCabecalhos = ObterSubsCabecalhos(pergunta.ToList())
                });
            }

            return cabecalhos;
        }

        private List<SubCabecalhoSondagemAnaliticaDto> ObterSubsCabecalhos(IEnumerable<PerguntaRespostaOrdemDto> respostasPerguntas)
        {
            var subsCabecalhos = respostasPerguntas.GroupBy(x => x.RespostaDescricao)
                                                   .Select(perguntaResposta => ObterSubCabecalho(perguntaResposta.FirstOrDefault()))
                                                   .ToList();

            if (subsCabecalhos.Exists(x => x.Descricao == RespostaDescricaoSondagem.SemPreenchimento))
                return subsCabecalhos;

            var respostaPergunta = respostasPerguntas.FirstOrDefault();

            var resposta = new PerguntaRespostaOrdemDto()
            {
                OrdemResposta = subsCabecalhos.Count() + 1,
                RespostaDescricao = RespostaDescricaoSondagem.SemPreenchimento,
                OrdemPergunta = respostaPergunta.OrdemPergunta,
                PerguntaDescricao = respostaPergunta.PerguntaDescricao
            };

            subsCabecalhos.Add(ObterSubCabecalho(resposta));

            return subsCabecalhos;
        }

        private SubCabecalhoSondagemAnaliticaDto ObterSubCabecalho(PerguntaRespostaOrdemDto resposta)
        {
            return new SubCabecalhoSondagemAnaliticaDto
            {
                Ordem = resposta.OrdemResposta,
                IdPerguntaResposta = @$"{resposta.OrdemPergunta}_{resposta.PerguntaDescricao}_{resposta.RespostaDescricao}",
                Descricao = resposta.RespostaDescricao
            };
        }

        private IEnumerable<RespostaSondagemAnaliticaDto> ObterRespostas(
                                                            IEnumerable<PerguntaRespostaOrdemDto> perguntasRespostas,
                                                            List<SubCabecalhoSondagemAnaliticaDto> subsCabecalhos,
                                                            int totalAlunos)
        {
            var respostas = perguntasRespostas.Select(perguntaResposta =>
                                 new RespostaSondagemAnaliticaDto
                                 {
                                     IdPerguntaResposta = $"{perguntaResposta.OrdemPergunta}_{perguntaResposta.PerguntaDescricao}_{perguntaResposta.RespostaDescricao}",
                                     Valor = perguntaResposta.QtdRespostas
                                 }).ToList();

            var respostaSemPreenchimento = ObterRespostaSemPreenchimento(perguntasRespostas, subsCabecalhos, totalAlunos);

            if (respostaSemPreenchimento is RespostaSondagemAnaliticaDto)
                respostas.Add(respostaSemPreenchimento);

            return respostas;
        }

        private RespostaSondagemAnaliticaDto ObterRespostaSemPreenchimento(
                                                            IEnumerable<PerguntaRespostaOrdemDto> perguntasRespostas,
                                                            List<SubCabecalhoSondagemAnaliticaDto> subsCabecalhos,
                                                            int totalAlunos)
        {
            if (EhTodosPreenchidos())
                return null;

            var subSemPreenchimento = subsCabecalhos.Find(sub => sub.Descricao == RespostaDescricaoSondagem.SemPreenchimento);

            return new RespostaSondagemAnaliticaDto()
            {
                IdPerguntaResposta = subSemPreenchimento.IdPerguntaResposta,
                Valor = ObterValorSemPreenchimento(perguntasRespostas, totalAlunos)
            };
        }

        private int ObterValorSemPreenchimento(
                                IEnumerable<PerguntaRespostaOrdemDto> perguntasRespostas,
                                int totalAlunos)
        {
            var totalRespostas = perguntasRespostas?.Sum(c => c.QtdRespostas) ?? 0;

            return totalAlunos > totalRespostas ? totalAlunos - totalRespostas : 0;
        }

        private Task<IEnumerable<PerguntaRespostaOrdemDto>> ObterPerguntasResposta()
        {
            return filtro.AnoLetivo >= ANO_ESCOLAR_2022 ?
                sondagemRelatorioRepository.MatematicaIADNumeroBimestre(filtro.AnoLetivo, SondagemComponenteCurricular.MATEMATICA, filtro.Periodo, filtro.UeCodigo, filtro.DreCodigo, ObterProficiencia()) :
                ObterPerguntasRespostaAnterior();
        }

        private bool EhIAD()
        {
            return ObterProficiencia() == ProficienciaSondagemEnum.IAD;
        }
    }
}
