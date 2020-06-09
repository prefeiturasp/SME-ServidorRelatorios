using SME.SR.Workers.SGP.Infra;
using System.Threading.Tasks;

namespace SME.SR.Workers.SGP.Commons
{
    public interface ITipoCalendarioRepository
    {
        Task<long> ObterPorAnoLetivoEModalidade(int anoLetivo, ModalidadeTipoCalendario modalidade, int semestre = 0);
    }
}
