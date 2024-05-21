using SME.SR.Infra;

namespace SME.SR.Application.Interfaces
{
    public interface IFabricaDeServicoAnaliticoSondagem
    {
        IServicoRepositorioAnalitico CriarServico(FiltroRelatorioAnaliticoSondagemDto filtro);
    }
}
