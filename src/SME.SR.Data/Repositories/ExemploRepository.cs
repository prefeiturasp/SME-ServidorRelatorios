using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class ExemploRepository
    {

        public async Task<string> ObterGames()
        {
            return await Task.FromResult("Quake");
        }

    }
}
