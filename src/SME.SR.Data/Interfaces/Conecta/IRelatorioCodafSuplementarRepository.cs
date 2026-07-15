using SME.SR.Infra.Dtos.Relatorios.Conecta;
using System.Threading.Tasks;

namespace SME.SR.Data.Interfaces.Conecta
{
    public interface IRelatorioCodafSuplementarRepository
    {
        Task<DadosPrincipaisRelatorioCodafDto> ObterDadosRelatorioSuplementarAsync(long codafId);
    }
}