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
            var agrupadoPorDre = perguntasRespostas.Where(x => x.CodigoDre != null).GroupBy(x => x.CodigoDre);
            
            if (agrupadoPorDre.Any())
            {
                var dres = await ObterDres(agrupadoPorDre.Select(x => x.Key).ToArray());
                
                foreach (var itemDre in agrupadoPorDre)
                {
                    var perguntas = new RelatorioSondagemAnaliticoProducaoDeTextoDto();
                    var agrupadoPorUe = itemDre.GroupBy(x => x.CodigoUe).ToList();
                    var dre = dres.FirstOrDefault(x => x.Codigo == itemDre.Key);

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
                var relatorioAgrupadoPorAnoTurma = itemUe.OrderBy(x => x.AnoTurma)
                                                         .GroupBy(x => x.AnoTurma);
                var ue = listaUes.FirstOrDefault(x => x.Codigo == itemUe.Key);

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
