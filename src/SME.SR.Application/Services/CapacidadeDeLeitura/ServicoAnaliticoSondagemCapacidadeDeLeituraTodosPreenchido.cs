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
    public class ServicoAnaliticoSondagemCapacidadeDeLeituraTodosPreenchido : ServicoAnaliticoSondagemCapacidadeDeLeituraAbstract, IServicoAnaliticoSondagemCapacidadeDeLeituraTodosPreenchido
    {
        public ServicoAnaliticoSondagemCapacidadeDeLeituraTodosPreenchido(
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

            foreach (var anoTurmaItem in relatorioAgrupadoPorAno)
            {
                respostas.Add(ObterRespostaCapacidade(
                                        anoTurmaItem.GroupBy(x => x.Ordem),
                                        ues,
                                        codigoUe,
                                        int.Parse(anoTurmaItem.Key),
                                        totalDeTurmas));
            }

            return respostas;
        }

        protected override int ObterValorSemPreenchimento(List<OrdemPerguntaRespostaDto> perguntaResposta, int totalDeAlunos)
        {
            return perguntaResposta.Where(f => f.RespostaDescricao == RespostaDescricaoSondagem.SemPreenchimento).Select(x => x.QtdRespostas).Sum();
        }
    }
}
