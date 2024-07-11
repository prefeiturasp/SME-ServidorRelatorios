using SME.SR.Application.Interfaces;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using SME.SR.Data.Interfaces.Sondagem;
using SME.SR.Infra;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SME.SR.Application.Services
{
    public class ServicoAnaliticoSondagemProducaoDeTexto : ServicoAnaliticoSondagemAbstract, IServicoAnaliticoSondagemProducaoDeTexto
    {
        public ServicoAnaliticoSondagemProducaoDeTexto(IAlunoRepository alunoRepository, IDreRepository dreRepository, IUeRepository ueRepository, ISondagemRelatorioRepository sondagemRelatorioRepository, ISondagemAnaliticaRepository sondagemAnaliticaRepository, ITurmaRepository turmaRepository) : base(alunoRepository, dreRepository, ueRepository, sondagemRelatorioRepository, sondagemAnaliticaRepository, turmaRepository)
        {
        }

        public async Task<IEnumerable<RelatorioSondagemAnaliticoPorDreDto>> ObterRelatorio(FiltroRelatorioAnaliticoSondagemDto filtro)
        {
            this.filtro = filtro;
            this.periodoFixoSondagem = await ObterPeriodoFixoSondagemPortugues(true);

            var retorno = new List<RelatorioSondagemAnaliticoPorDreDto>();
            var perguntasRespostas = await ObterPerguntaRespostaProducaoTexto();
            //var agrupadoPorDre = perguntasRespostas.Where(x => x.CodigoDre != null).GroupBy(x => x.CodigoDre);
            var dres = filtro.DreCodigo == TODOS ?
                await dreRepository.ObterTodas() : new Dre[] { await dreRepository.ObterPorCodigo(filtro.DreCodigo) };

            if (perguntasRespostas.Any(x => x.CodigoDre != null))
            {
                //var dres = await ObterDres(agrupadoPorDre.Select(x => x.Key).ToArray());
                var agrupadoPorDre = (from dre in dres
                                      from c in perguntasRespostas
                                      select dre.Codigo == c.CodigoDre ? c : new PerguntaRespostaProducaoTextoDto() { CodigoDre = dre.Codigo })
                                 .GroupBy(x => x.CodigoDre).Distinct();

                foreach (var itemDre in agrupadoPorDre)
                {
                    var perguntas = new RelatorioSondagemAnaliticoProducaoDeTextoDto();
                    //var agrupadoPorUe = itemDre.GroupBy(x => x.CodigoUe).ToList();
                    var dre = dres.FirstOrDefault(x => x.Codigo == itemDre.Key);
                    var uesDre = filtro.UeCodigo == TODOS ? await ueRepository
                        .ObterPorDresId(new long[] { dres.First(d => d.Codigo == itemDre.Key).Id }) :
                        new UePorDresIdResultDto[] { new UePorDresIdResultDto() { Codigo = filtro.UeCodigo } };

                    var agrupadoPorUe = (from ue in uesDre
                                         from i in itemDre
                                         select ue.Codigo == i.CodigoUe ? i : new PerguntaRespostaProducaoTextoDto() { CodigoUe = ue.Codigo, AnoTurma = i.AnoTurma })
                                         .GroupBy(x => x.CodigoUe).Distinct().ToList();

                    perguntas.Respostas.AddRange(await ObterRespostas(agrupadoPorUe, dre));

                    perguntas.DreSigla = dre.Abreviacao;
                    perguntas.Dre = dre.Nome;
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

        private async Task<List<RespostaSondagemAnaliticoProducaoDeTextoDto>> ObterRespostas(
                                                            IEnumerable<IGrouping<string, PerguntaRespostaProducaoTextoDto>> agrupadoPorUe,
                                                            Dre dre)
        {
            var respostas = new List<RespostaSondagemAnaliticoProducaoDeTextoDto>();
            var listaUes = await ObterUe(agrupadoPorUe.Select(x => x.Key).ToArray());
            var totalAlunosDre = await ObterTotalDeAlunosPorDre(dre.Codigo);
            var totalTurmasDre = await ObterQuantidadeTurmaPorAnoDre(dre.Id);

            foreach (var itemUe in agrupadoPorUe)
            {
                var totalDeAlunosUe = await ObterTotalDeAlunosPorUe(dre.Codigo, itemUe.Key, totalAlunosDre);
                var totalTurmasUe = await ObterQuantidadeTurmaPorAnoUe(dre.Id, itemUe.Key, totalTurmasDre);
                /*var relatorioAgrupadoPorAnoTurma = itemUe.OrderBy(x => x.AnoTurma)
                                                         .GroupBy(x => x.AnoTurma);*/
                var ue = listaUes.FirstOrDefault(x => x.Codigo == itemUe.Key);
                var turmasUe = (await turmaRepository
                            .ObterTurmasPorUeEAnoLetivo(itemUe.Key, filtro.AnoLetivo))
                            .Where(t => t.Ano.All(x => char.IsDigit(x)) && int.Parse(t.Ano) > 0 && t.ModalidadeCodigo == Modalidade.Fundamental);

                var relatorioAgrupadoPorAnoTurma = (from t in turmasUe
                                                    from i in itemUe
                                                    where (i.AnoTurma != null && i.AnoTurma != "0" && i.AnoTurma.All(x => char.IsDigit(x)) && int.Parse(i.AnoTurma) > 0) || (t.Ano == i.AnoTurma)
                                                    select t.Codigo == i.CodigoTurma ? i : new PerguntaRespostaProducaoTextoDto() { AnoTurma = t.Ano, CodigoTurma = t.Codigo })
                                                       .GroupBy(p => p.AnoTurma)
                                                       .OrderBy(p => p.Key);

                foreach (var anoTurmaItem in relatorioAgrupadoPorAnoTurma)
                {
                    respostas.Add(ObterRespostaDto(
                                            anoTurmaItem, 
                                            ue, 
                                            totalTurmasUe, 
                                            ObterTotalDeAluno(totalDeAlunosUe, anoTurmaItem.Key)));
                }
            }

            return respostas;
        }

        private Task<IEnumerable<PerguntaRespostaProducaoTextoDto>> ObterPerguntaRespostaProducaoTexto()
        {
            return sondagemRelatorioRepository.ObterDadosProducaoTexto(new RelatorioPortuguesFiltroDto
            {
                AnoLetivo = filtro.AnoLetivo,
                CodigoDre = filtro.DreCodigo,
                CodigoUe = filtro.UeCodigo,
                ComponenteCurricularId = SondagemComponenteCurricular.LINGUA_PORTUGUESA,
                GrupoId = GrupoSondagem.PRODUCAO_DE_TEXTO,
                PeriodoId = periodoFixoSondagem.PeriodoId
            });
        }

        private RespostaSondagemAnaliticoProducaoDeTextoDto ObterRespostaDto(
                                                            IGrouping<string, PerguntaRespostaProducaoTextoDto> perguntaResposta,
                                                            Ue ue,
                                                            IEnumerable<TotalDeTurmasPorAnoDto> totalDeTurmas,
                                                            int totalDeAlunos)
        {
            return new RespostaSondagemAnaliticoProducaoDeTextoDto()
            {
                NaoProduziuEntregouEmBranco = perguntaResposta.Count(p => p.Pergunta == PerguntaDescricaoSondagem.NaoProduziuEntregouEmBranco),
                NaoApresentouDificuldades = perguntaResposta.Count(p => p.Pergunta == PerguntaDescricaoSondagem.NaoApresentouDificuldades),
                EscritaNaoAlfabetica = perguntaResposta.Count(p => p.Pergunta == PerguntaDescricaoSondagem.EscritaNaoAlfabetica),
                DificuldadesComAspectosSemanticos = perguntaResposta.Count(p => p.Pergunta == PerguntaDescricaoSondagem.DificuldadesComAspectosSemanticos),
                DificuldadesComAspectosTextuais = perguntaResposta.Count(p => p.Pergunta == PerguntaDescricaoSondagem.DificuldadesComAspectosTextuais),
                DificuldadesComAspectosOrtograficosNotacionais = perguntaResposta.Count(p => p.Pergunta == PerguntaDescricaoSondagem.DificuldadesComAspectosOrtograficosNotacionais),
                SemPreenchimento = ObterTotalSemPreenchimento(perguntaResposta, totalDeAlunos),
                Ano = int.Parse(perguntaResposta.Key),
                TotalDeTurma = ObterTotalDeTurmas(totalDeTurmas, perguntaResposta.Key),
                TotalDeAlunos = ObterTotalDeAlunos(perguntaResposta, totalDeAlunos),
                Ue = ue.TituloTipoEscolaNome
            };
        }

        private int ObterTotalSemPreenchimento(
                                IGrouping<string, PerguntaRespostaProducaoTextoDto> perguntaResposta,
                                int totalDeAlunos)
        {
            if (EhTodosPreenchidos())
                return perguntaResposta.Count(p => p.Pergunta == PerguntaDescricaoSondagem.SemPreenchimento);

            return totalDeAlunos - ObterTotalDeAlunosSondagem(perguntaResposta);
        }

        private int ObterTotalDeAlunos(
                                IGrouping<string, PerguntaRespostaProducaoTextoDto> perguntaResposta,
                                int totalDeAlunos)
        {
            if (totalDeAlunos > 0)
                return totalDeAlunos;

            return ObterTotalDeAlunosSondagem(perguntaResposta);
        }

        private int ObterTotalDeAlunosSondagem(
                            IGrouping<string, PerguntaRespostaProducaoTextoDto> perguntaResposta)
        {
            var alunoNaSondagem = perguntaResposta.GroupBy(x => x.CodigoAluno);

            return alunoNaSondagem.Select(x => x.Key).Count();
        }
    }
}
