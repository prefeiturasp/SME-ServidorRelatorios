using System.Threading.Tasks;

namespace SME.SR.Infra
{
    public interface IServicoFila
    {
        void PublicaFila(PublicaFilaDto publicaFilaDto);
    }
}