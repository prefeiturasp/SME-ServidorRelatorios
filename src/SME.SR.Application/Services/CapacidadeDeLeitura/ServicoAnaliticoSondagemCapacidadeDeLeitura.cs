using SME.SR.Application.Interfaces;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using SME.SR.Data.Interfaces.Sondagem;
using SME.SR.Infra;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SME.SR.Application.Services.CapacidadeDeLeitura
{
    public class ServicoAnaliticoSondagemCapacidadeDeLeitura : ServicoAnaliticoSondagemCapacidadeDeLeituraAbstract, IServicoAnaliticoSondagemCapacidadeDeLeitura
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

        protected override async Task<List<RespostaSondagemAnaliticoCapacidadeDeLeituraDto>> ObterRespostas(
                                                                                                List<IGrouping<string, OrdemPerguntaRespostaDto>> relatorioAgrupadoPorAno,
                                                                                                IEnumerable<Ue> ues,
                                                                                                IEnumerable<TotalDeTurmasPorAnoDto> totalDeTurmas,
                                                                                                string codigoDre,
                                                                                                string codigoUe)
        {
            var respostas = new List<RespostaSondagemAnaliticoCapacidadeDeLeituraDto>();
            var totalDeAlunosPorAno = (await ObterTotalDeAlunosAnoTurma(codigoDre, codigoUe)).ToList();

            foreach (var anoTurmaItem in relatorioAgrupadoPorAno)
            {
                var totalDeAlunos = totalDeAlunosPorAno.Where(x => x.AnoTurma == anoTurmaItem.Key).Select(x => x.QuantidadeAluno).Sum();

                respostas.Add(ObterRespostaCapacidade(
                                        anoTurmaItem.GroupBy(x => x.Ordem),
                                        ues,
                                        codigoUe,
                                        int.Parse(anoTurmaItem.Key),
                                        totalDeTurmas,
                                        totalDeAlunos));
            }

            return respostas;
        }
        protected override int ObterValorSemPreenchimento(List<OrdemPerguntaRespostaDto> perguntaResposta, int totalDeAlunos)
        {
            var totalRespostas = perguntaResposta.Select(s => s.QtdRespostas).ToList().Sum();
            totalDeAlunos = totalDeAlunos >= totalRespostas ? totalDeAlunos : totalRespostas;

            return totalDeAlunos - totalRespostas;
        }
    }
}
