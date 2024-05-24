using SME.SR.Application.Interfaces;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using SME.SR.Data.Interfaces.Sondagem;
using SME.SR.Infra;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Application.Services
{
    public class ServicoAnaliticoSondagemNumeros : ServicoAnaliticoSondagemIADNumerosAbstract, IServicoAnaliticoSondagemNumeros
    {
        public ServicoAnaliticoSondagemNumeros(IAlunoRepository alunoRepository, IDreRepository dreRepository, IUeRepository ueRepository, ISondagemRelatorioRepository sondagemRelatorioRepository, ISondagemAnaliticaRepository sondagemAnaliticaRepository, ITurmaRepository turmaRepository) : base(alunoRepository, dreRepository, ueRepository, sondagemRelatorioRepository, sondagemAnaliticaRepository, turmaRepository)
        {
        }

        protected override bool EhTodosPreenchidos()
        {
            return EhPreenchimentoDeTodosEstudantes();
        }

        protected override Task<IEnumerable<PerguntaRespostaOrdemDto>> ObterPerguntasRespostaAnterior()
        {
            return sondagemRelatorioRepository.MatematicaNumerosAntes2022(
                                                    filtro.AnoLetivo, 
                                                    filtro.Periodo, 
                                                    filtro.UeCodigo, 
                                                    filtro.DreCodigo, 
                                                    periodoFixoSondagem.PeriodoId);
        }

        protected override ProficienciaSondagemEnum ObterProficiencia()
        {
            return ProficienciaSondagemEnum.Numeros;
        }
    }
}
