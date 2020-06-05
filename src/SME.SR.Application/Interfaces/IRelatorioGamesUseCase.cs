using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace SME.SR.Workers.SGP
{
    public interface IRelatorioGamesUseCase
    {
        Task Executar(JObject request);
    }
}
