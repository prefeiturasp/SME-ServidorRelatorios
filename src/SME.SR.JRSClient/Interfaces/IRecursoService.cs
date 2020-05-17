using SME.SR.Infra.Dtos;
using System.Threading.Tasks;

namespace SME.SR.JRSClient.Interfaces
{
    public interface IRecursoService
    {
        Task<BuscaRepositorioRespostaDto> BuscarRepositorio(BuscaRepositorioRequisicaoDto requisicaoDto);

        Task<DetalhesRecursoDto> ObterDetalhesRecurso(string caminhoRelatorio, bool expanded);

        Task<DetalhesRecursoDto> Post(string caminhoRelatorio, bool criarCaminho, bool? sobrescrever = null);
    }
}
