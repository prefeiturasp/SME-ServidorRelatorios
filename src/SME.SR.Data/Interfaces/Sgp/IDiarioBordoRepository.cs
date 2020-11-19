using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Data.Interfaces
{
    public interface IDiarioBordoRepository
    {
        Task<DateTime?> ObterUltimoDiarioBordoProfessor(string professorRf);
    }
}
