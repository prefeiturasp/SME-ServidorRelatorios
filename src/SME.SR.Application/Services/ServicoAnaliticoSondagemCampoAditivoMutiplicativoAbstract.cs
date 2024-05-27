using SME.SR.Data;
using SME.SR.Data.Interfaces;
using SME.SR.Data.Interfaces.Sondagem;
using SME.SR.Infra;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SME.SR.Application.Services
{
    public abstract class ServicoAnaliticoSondagemCampoAditivoMutiplicativoAbstract : ServicoAnaliticoSondagemAbstract
    {
        private readonly List<Pergunta> perguntas;

        public ServicoAnaliticoSondagemCampoAditivoMutiplicativoAbstract(IAlunoRepository alunoRepository, IDreRepository dreRepository, IUeRepository ueRepository, ISondagemRelatorioRepository sondagemRelatorioRepository, ISondagemAnaliticaRepository sondagemAnaliticaRepository, ITurmaRepository turmaRepository) : base(alunoRepository, dreRepository, ueRepository, sondagemRelatorioRepository, sondagemAnaliticaRepository, turmaRepository)
        {
            perguntas = ObterPerguntas();
        }

        protected abstract ProficienciaSondagemEnum ObterProficiencia();

        public async Task<IEnumerable<RelatorioSondagemAnaliticoPorDreDto>> ObterRelatorio(FiltroRelatorioAnaliticoSondagemDto filtro)
        {
            this.filtro = filtro;
            periodoFixoSondagem = await ObterPeriodoFixoSondagemMatematica(false);

            var retorno = new List<RelatorioSondagemAnaliticoCampoAditivoMultiplicativoDto>();
            var perguntasRespostas = await ObterPerguntaResposta();
            var agrupadoPorDre = perguntasRespostas.Where(x => x.CodigoDre != null).GroupBy(x => x.CodigoDre);

            if (agrupadoPorDre.Any())
            {
                var dres = await ObterDres(agrupadoPorDre.Select(x => x.Key).ToArray());

                foreach (var itemDre in agrupadoPorDre)
                {
                    var dre = dres.FirstOrDefault(x => x.Codigo == itemDre.Key);
                    var agrupamentoPorUe = itemDre.GroupBy(x => x.CodigoUe).Distinct().ToList();

                    var relatorio = new RelatorioSondagemAnaliticoCampoAditivoMultiplicativoDto(filtro.TipoSondagem);
                    relatorio.Dre = dre.Nome;
                    relatorio.DreSigla = dre.Abreviacao;
                    relatorio.AnoLetivo = filtro.AnoLetivo;
                    relatorio.Periodo = filtro.Periodo;
                    relatorio.Respostas.AddRange(await ObterRespostas(agrupamentoPorUe, dre));
                    retorno.Add(relatorio);
                }
            }

            return retorno;
        }

        protected override bool EhTodosPreenchidos()
        {
            return EhPreenchimentoDeTodosEstudantes();
        }

        private Task<IEnumerable<PerguntaRespostaOrdemDto>> ObterPerguntaRespostaAtual()
        {
            return sondagemRelatorioRepository.ConsolidacaoCampoAditivoMultiplicativo(new RelatorioMatematicaFiltroDto
            {
                AnoLetivo = filtro.AnoLetivo,
                CodigoDre = filtro.DreCodigo,
                CodigoUe = filtro.UeCodigo,
                ComponenteCurricularId = SondagemComponenteCurricular.MATEMATICA,
                Proficiencia = ObterProficiencia(),
                Bimestre = filtro.Periodo
            });
        }

        private Task<IEnumerable<PerguntaRespostaOrdemDto>> ObterPerguntaRespostaAnterior2022()
        {
            return sondagemRelatorioRepository.ConsolidacaoCampoAditivoMultiplicativoAntes2022(new RelatorioMatematicaFiltroDto
            {
                AnoLetivo = filtro.AnoLetivo,
                CodigoDre = filtro.DreCodigo,
                CodigoUe = filtro.UeCodigo,
                Proficiencia = ObterProficiencia(),
                Bimestre = filtro.Periodo
            });
        }

        private Task<IEnumerable<PerguntaRespostaOrdemDto>> ObterPerguntaResposta()
        {
            if (filtro.AnoLetivo >= ANO_ESCOLAR_2022)
                return ObterPerguntaRespostaAtual();

            return ObterPerguntaRespostaAnterior2022();
        }

        private async Task<List<RespostaSondagemAnaliticoCampoAditivoMultiplicativoDto>> ObterRespostas(
                                                                                            List<IGrouping<string, PerguntaRespostaOrdemDto>> agrupadoRespostasPorUe,
                                                                                            Dre dre)
        {
            var ues = await ObterUe(agrupadoRespostasPorUe.Select(x => x.Key).ToArray());
            var totalDeTurmas = await ObterQuantidadeTurmaPorAnoDre(dre.Id);
            var respostas = new List<RespostaSondagemAnaliticoCampoAditivoMultiplicativoDto>();
            var totalDeAlunosPorAno = await ObterTotalDeAlunosPorDre(dre.Codigo);

            foreach (var itemUe in agrupadoRespostasPorUe)
            {
                var agrupamentoPorAnoTurma = itemUe.OrderBy(x => x.AnoTurma).ThenBy(x => x.OrdemPergunta).GroupBy(x => x.AnoTurma);
                var totalDeAlunosUe = await ObterTotalDeAlunosPorUe(dre.Codigo, itemUe.Key, totalDeAlunosPorAno);
                var totalTurmasUe = await ObterQuantidadeTurmaPorAnoUe(dre.Id, itemUe.Key, totalDeTurmas);
                var ue = ues.FirstOrDefault(x => x.Codigo == itemUe.Key);

                foreach (var anoTurma in agrupamentoPorAnoTurma)
                {
                    var agrupamentoOrdemPergunta = anoTurma.GroupBy(p => new { p.OrdemPergunta, p.PerguntaDescricao });
                    var reposta = new RespostaSondagemAnaliticoCampoAditivoMultiplicativoDto()
                    {
                        TotalDeAlunos = ObterTotalDeAlunos(anoTurma.ToList(), anoTurma.Key, totalDeAlunosUe),
                        Ano = int.Parse(anoTurma.Key),
                        TotalDeTurma = ObterTotalDeTurmas(totalTurmasUe, anoTurma.Key),
                        Ue = ue.TituloTipoEscolaNome
                    };

                    foreach (var ordemPergunta in agrupamentoOrdemPergunta)
                    {
                        var descricaoPergunta = ObterDescricaoPergunta(ordemPergunta.Key.PerguntaDescricao, ordemPergunta.Key.OrdemPergunta, int.Parse(anoTurma.Key));
                        if (string.IsNullOrEmpty(descricaoPergunta)) continue;

                        reposta.Ordens.Add(ObterRespostaSondagemAnaliticoOrdemDto(
                                                    ordemPergunta.ToList(),
                                                    ordemPergunta.Key.OrdemPergunta, 
                                                    descricaoPergunta,
                                                    reposta.TotalDeAlunos));
                    }

                    respostas.Add(reposta);
                }
            }

            return respostas;
        }

        private RespostaOrdemMatematicaDto ObterRespostaSondagemAnaliticoOrdemDto(List<PerguntaRespostaOrdemDto> perguntasRespostas,
                                                                                 int ordemPergunta, 
                                                                                 string descricaoPergunta, 
                                                                                 int totalDeAlunos)
        {
            return new RespostaOrdemMatematicaDto
            {
                Ordem = ordemPergunta,
                Descricao = descricaoPergunta,
                Resultado = ObterRespostaMatematica("Resultado", perguntasRespostas, totalDeAlunos),
                Ideia = ObterRespostaMatematica("Ideia", perguntasRespostas, totalDeAlunos)
            };
        }

        private RespostaMatematicaDto ObterRespostaMatematica(
                                        string descricaoSubPergunta, 
                                        List<PerguntaRespostaOrdemDto> perguntasRepostasOrdem,
                                        int totalDeAlunos)
        {
            var resposta = new RespostaMatematicaDto();
            var perguntasRespostas = perguntasRepostasOrdem.Where(x => x.SubPerguntaDescricao == descricaoSubPergunta).ToList();
            
            resposta.Acertou = perguntasRespostas?.Where(x => x.RespostaDescricao == RespostaDescricaoSondagem.Acertou).Select(x => x.QtdRespostas).Sum() ?? 0;
            resposta.Errou = perguntasRespostas?.Where(x => x.RespostaDescricao == RespostaDescricaoSondagem.Errou).Select(x => x.QtdRespostas).Sum() ?? 0;
            resposta.NaoResolveu = perguntasRespostas?.Where(x => x.RespostaDescricao == RespostaDescricaoSondagem.NaoResolveu).Select(x => x.QtdRespostas).Sum() ?? 0;
            resposta.SemPreenchimento = ObterValorSemPreenchimento(resposta, totalDeAlunos, perguntasRespostas);

            return resposta;
        }

        private int ObterValorSemPreenchimento(
                                RespostaMatematicaDto resposta, 
                                int totalDeAlunos,
                                List<PerguntaRespostaOrdemDto> perguntasRepostasOrdem)
        {
            if (EhTodosPreenchidos())
                return perguntasRepostasOrdem?.Where(x => x.RespostaDescricao == RespostaDescricaoSondagem.SemPreenchimento).Select(x => x.QtdRespostas).Sum() ?? 0; 

            return ObterValorSemPreenchimentoCalculado(resposta, totalDeAlunos);
        }

        private int ObterValorSemPreenchimentoCalculado(
                                RespostaMatematicaDto resposta,
                                int totalDeAlunos)
        {
            var totalRespostas = resposta.Acertou + resposta.Errou + resposta.NaoResolveu;

            return totalDeAlunos >= totalRespostas ? totalDeAlunos - totalRespostas : 0;
        }

        private string ObterDescricaoPergunta(
                            string perguntaDescricao, 
                            int ordem, 
                            int anoTurma)
        {
            if (!string.IsNullOrEmpty(perguntaDescricao))
                return perguntaDescricao;

            var pergunta = perguntas.FirstOrDefault(p => p.AnoTurma == anoTurma && p.Ordem == ordem && p.Proficiencia == ObterProficiencia());

            return pergunta?.Descricao ?? string.Empty;
        }

        private List<Pergunta> ObterPerguntas()
        {
            return new List<Pergunta>()
            {
                new Pergunta(1, ProficienciaSondagemEnum.CampoAditivo, "COMPOSIÇÃO"),
                new Pergunta(2, ProficienciaSondagemEnum.CampoAditivo, "COMPOSIÇÃO", 1),
                new Pergunta(2, ProficienciaSondagemEnum.CampoAditivo, "TRANSFORMAÇÃO", 2),
                new Pergunta(2, ProficienciaSondagemEnum.CampoMultiplicativo, "PROPORCIONALIDADE", 3),
                new Pergunta(3, ProficienciaSondagemEnum.CampoAditivo, "COMPOSIÇÃO", 1),
                new Pergunta(3, ProficienciaSondagemEnum.CampoAditivo, "TRANSFORMAÇÃO", 2),
                new Pergunta(3, ProficienciaSondagemEnum.CampoAditivo, "COMPARAÇÃO", 3),
                new Pergunta(3, ProficienciaSondagemEnum.CampoMultiplicativo, "CONFIGURAÇÃO RETANGULAR", 4),
                new Pergunta(3, ProficienciaSondagemEnum.CampoMultiplicativo, "PROPORCIONALIDADE", 5),
                new Pergunta(4, ProficienciaSondagemEnum.CampoAditivo, "COMPARAÇÃO", 1),
                new Pergunta(4, ProficienciaSondagemEnum.CampoAditivo, "TRANSFORMAÇÃO", 2),
                new Pergunta(4, ProficienciaSondagemEnum.CampoAditivo, "COMPOSIÇÃO DE TRANSF.", 3),
                new Pergunta(4, ProficienciaSondagemEnum.CampoAditivo, "COMPARAÇÃO", 4),
                new Pergunta(4, ProficienciaSondagemEnum.CampoMultiplicativo, "CONFIGURAÇÃO RETANGULAR", 5),
                new Pergunta(4, ProficienciaSondagemEnum.CampoMultiplicativo, "PROPORCIONALIDADE", 6),
                new Pergunta(4, ProficienciaSondagemEnum.CampoMultiplicativo, "COMBINATÓRIA", 7),
                new Pergunta(5, ProficienciaSondagemEnum.CampoAditivo, "COMPARAÇÃO", 1),
                new Pergunta(5, ProficienciaSondagemEnum.CampoAditivo, "TRANSFORMAÇÃO", 2),
                new Pergunta(5, ProficienciaSondagemEnum.CampoAditivo, "COMPOSIÇÃO DE TRANSF.", 3),
                new Pergunta(5, ProficienciaSondagemEnum.CampoAditivo, "COMPARAÇÃO", 4),
                new Pergunta(5, ProficienciaSondagemEnum.CampoMultiplicativo, "COMBINATÓRIA", 5),
                new Pergunta(5, ProficienciaSondagemEnum.CampoMultiplicativo, "CONFIGURAÇÃO RETANGULAR", 6),
                new Pergunta(5, ProficienciaSondagemEnum.CampoMultiplicativo, "PROPORCIONALIDADE", 7),
                new Pergunta(5, ProficienciaSondagemEnum.CampoMultiplicativo, "MULTIPLICAÇÃO COMPARATIVA", 8),
                new Pergunta(6, ProficienciaSondagemEnum.CampoAditivo, "COMPARAÇÃO", 1),
                new Pergunta(6, ProficienciaSondagemEnum.CampoAditivo, "TRANSFORMAÇÃO", 2),
                new Pergunta(6, ProficienciaSondagemEnum.CampoAditivo, "COMPOSIÇÃO DE TRANSF.", 3),
                new Pergunta(6, ProficienciaSondagemEnum.CampoAditivo, "COMPARAÇÃO", 4),
                new Pergunta(6, ProficienciaSondagemEnum.CampoMultiplicativo, "COMBINATÓRIA", 5),
                new Pergunta(6, ProficienciaSondagemEnum.CampoMultiplicativo, "CONFIGURAÇÃO RETANGULAR", 6),
                new Pergunta(6, ProficienciaSondagemEnum.CampoMultiplicativo, "PROPORCIONALIDADE", 7),
                new Pergunta(6, ProficienciaSondagemEnum.CampoMultiplicativo, "MULTIPLICAÇÃO COMPARATIVA", 8)
            };
        }

        private class Pergunta
        {
            public Pergunta(int anoTurma, ProficienciaSondagemEnum proficiencia, string descricao, int ordem = 0)
            {
                AnoTurma = anoTurma;
                Proficiencia = proficiencia;
                Ordem = ordem;
                Descricao = descricao;
            }

            public int AnoTurma { get; set; }
            public ProficienciaSondagemEnum Proficiencia { get; set; }
            public string Descricao { get; set; }
            public int Ordem { get; set; }
        } 
    }
}
