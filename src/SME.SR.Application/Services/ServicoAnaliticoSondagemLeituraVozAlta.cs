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
    public class ServicoAnaliticoSondagemLeituraVozAlta : ServicoAnaliticoSondagemAbstract, IServicoAnaliticoSondagemLeituraVozAlta
    {
        public ServicoAnaliticoSondagemLeituraVozAlta(IAlunoRepository alunoRepository, IDreRepository dreRepository, IUeRepository ueRepository, ISondagemRelatorioRepository sondagemRelatorioRepository, ISondagemAnaliticaRepository sondagemAnaliticaRepository, ITurmaRepository turmaRepository) : base(alunoRepository, dreRepository, ueRepository, sondagemRelatorioRepository, sondagemAnaliticaRepository, turmaRepository)
        {
        }

        public async Task<IEnumerable<RelatorioSondagemAnaliticoPorDreDto>> ObterRelatorio(FiltroRelatorioAnaliticoSondagemDto filtro)
        {
            this.filtro = filtro;
            this.periodoFixoSondagem = await ObterPeriodoFixoSondagem();

            var retorno = new List<RelatorioSondagemAnaliticoPorDreDto>();
            var perguntasRespostas = await ObterPerguntasRespostas();
            var agrupadoPorDre = perguntasRespostas.Where(x => x.CodigoDre != null).GroupBy(x => x.CodigoDre).Distinct();
            
            if (agrupadoPorDre.Any())
            {
                var listaDre = await ObterDres(agrupadoPorDre.Select(x => x.Key).ToArray());

                foreach (var itemDre in agrupadoPorDre)
                {
                    var perguntas = new RelatorioSondagemAnaliticoLeituraDeVozAltaDto();
                    var agrupadoPorUe = itemDre.GroupBy(x => x.CodigoUe).Distinct();
                    var dre = listaDre.FirstOrDefault(x => x.Codigo == itemDre.Key);

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
            return ComPreenchimentoDeTodosEstudantesIAD();
        }

        private Task<IEnumerable<PerguntaRespostaProducaoTextoDto>> ObterPerguntasRespostas()
        {
            return sondagemRelatorioRepository.ObterDadosProducaoTexto(new RelatorioPortuguesFiltroDto
            {
                AnoLetivo = filtro.AnoLetivo,
                CodigoDre = filtro.DreCodigo,
                CodigoUe = filtro.UeCodigo,
                ComponenteCurricularId = SondagemComponenteCurricular.LINGUA_PORTUGUESA,
                GrupoId = GrupoSondagem.LEITURA_EM_VOZ_ALTA,
                PeriodoId = periodoFixoSondagem.PeriodoId
            });
        }

        private async Task<List<RespostaSondagemAnaliticoLeituraDeVozAltaDto>> ObterRespostas(
                                                                    IEnumerable<IGrouping<string, PerguntaRespostaProducaoTextoDto>> relatorioAgrupadoPorUe,
                                                                    Dre dre)
        {
            var respostas = new List<RespostaSondagemAnaliticoLeituraDeVozAltaDto>();
            var listaUes = await ObterUe(relatorioAgrupadoPorUe.Select(x => x.Key).ToArray());
            var totalAlunosDre = await ObterTotalDeAlunosPorDre(dre.Codigo);
            var totalTurmasDre = await ObterQuantidadeTurmaPorAnoDre(dre.Id);
            
            foreach (var itemUe in relatorioAgrupadoPorUe)
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

        private RespostaSondagemAnaliticoLeituraDeVozAltaDto ObterRespostaDto(
                                                                    IGrouping<string, PerguntaRespostaProducaoTextoDto> perguntaResposta,
                                                                    Ue ue,
                                                                    IEnumerable<TotalDeTurmasPorAnoDto> totalDeTurmas,
                                                                    int totalDeAlunos)
        {
            return new RespostaSondagemAnaliticoLeituraDeVozAltaDto
            {
                NaoConseguiuOuNaoQuisLer = perguntaResposta.Count(p => p.Pergunta == PerguntaDescricaoSondagem.NaoConseguiuOuNaoQuisLer),
                LeuComMuitaDificuldade = perguntaResposta.Count(p => p.Pergunta == PerguntaDescricaoSondagem.LeuComMuitaDificuldade),
                LeuComAlgumaFluencia = perguntaResposta.Count(p => p.Pergunta == PerguntaDescricaoSondagem.LeuComAlgumaFluencia),
                LeuComFluencia = perguntaResposta.Count(p => p.Pergunta == PerguntaDescricaoSondagem.LeuComFluencia),
                SemPreenchimento = ObterValorSemPreenchimento(perguntaResposta, totalDeAlunos),
                Ano = int.Parse(perguntaResposta.Key),
                TotalDeTurma = ObterTotalDeTurmas(totalDeTurmas, perguntaResposta.Key),
                TotalDeAlunos = totalDeAlunos > 0 ? totalDeAlunos : TotalDeAlunos(perguntaResposta),
                Ue = ue.TituloTipoEscolaNome
            };
        }

        private int ObterValorSemPreenchimento(IGrouping<string, PerguntaRespostaProducaoTextoDto> perguntaResposta, int totalDeAlunos)
        {
            if (EhTodosPreenchidos())
                return ObterValorSemPreenchimento(perguntaResposta);

            return ObterValorSemPreenchimentoCalculado(perguntaResposta, totalDeAlunos);
        }

        private int ObterValorSemPreenchimentoCalculado(IGrouping<string, PerguntaRespostaProducaoTextoDto> perguntaResposta, int totalDeAlunos)
        {
            var totalRespostas = TotalDeAlunos(perguntaResposta);

            totalDeAlunos = totalDeAlunos >= totalRespostas ? totalDeAlunos : totalRespostas;

            return totalDeAlunos - totalRespostas;
        }

        private int TotalDeAlunos(IGrouping<string, PerguntaRespostaProducaoTextoDto> perguntaResposta)
        {
            var alunoNaSondagem = perguntaResposta.GroupBy(x => x.CodigoAluno);

            return alunoNaSondagem.Select(x => x.Key).Count();
        }

        private int ObterValorSemPreenchimento(IGrouping<string, PerguntaRespostaProducaoTextoDto> perguntaResposta)
        {
            return perguntaResposta.Count(p => p.Pergunta == PerguntaDescricaoSondagem.SemPreenchimento);
        }

        private Task<PeriodoFixoSondagem> ObterPeriodoFixoSondagem()
        {
            return ObterPeriodoFixoSondagem(ObterTituloSemestreBimestrePortugues(true));
        }
    }
}
