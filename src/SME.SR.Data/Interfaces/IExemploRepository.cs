using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public interface IExemploRepository
    {
        Task<string> ObterGames();
    }
}
