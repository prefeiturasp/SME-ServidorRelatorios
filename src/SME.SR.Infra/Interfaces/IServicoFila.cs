using System.Threading.Tasks;

namespace SME.SR.Infra
{
    public interface IServicoFila
    {
        Task PublicaFila(PublicaFilaDto publicaFilaDto);
    }
}