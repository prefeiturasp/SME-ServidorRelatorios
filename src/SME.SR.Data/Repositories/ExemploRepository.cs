using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class ExemploRepository : IExemploRepository
    {

        public async Task<string> ObterGames()
        {
            return await Task.FromResult("Quake");
        }

    }
}
