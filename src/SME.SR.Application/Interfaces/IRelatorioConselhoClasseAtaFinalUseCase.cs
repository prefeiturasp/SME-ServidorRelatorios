using SME.SR.Infra;
using SME.SR.Infra.Dtos.Relatorios.ConselhoClasse;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public interface IRelatorioConselhoClasseAtaFinalUseCase
    {
        Task Executar(FiltroRelatorioDto request);
    }
}
