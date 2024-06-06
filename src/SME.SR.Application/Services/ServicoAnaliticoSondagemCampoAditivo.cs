using SME.SR.Application.Interfaces;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using SME.SR.Data.Interfaces.Sondagem;
using SME.SR.Infra;

namespace SME.SR.Application.Services
{
    public class ServicoAnaliticoSondagemCampoAditivo : ServicoAnaliticoSondagemCampoAditivoMutiplicativoAbstract, IServicoAnaliticoSondagemCampoAditivo
    {
        public ServicoAnaliticoSondagemCampoAditivo(IAlunoRepository alunoRepository, IDreRepository dreRepository, IUeRepository ueRepository, ISondagemRelatorioRepository sondagemRelatorioRepository, ISondagemAnaliticaRepository sondagemAnaliticaRepository, ITurmaRepository turmaRepository) : base(alunoRepository, dreRepository, ueRepository, sondagemRelatorioRepository, sondagemAnaliticaRepository, turmaRepository)
        {
        }

        protected override ProficienciaSondagemEnum ObterProficiencia()
        {
            return ProficienciaSondagemEnum.CampoAditivo;
        }
    }
}
