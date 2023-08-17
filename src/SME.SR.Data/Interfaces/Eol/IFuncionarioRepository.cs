using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public interface IFuncionarioRepository
    {
        Task<IEnumerable<Funcionario>> ObterFuncionariosPorCargoUe(string codigoCargo, string codigoUe);
        Task<IEnumerable<Funcionario>> ObterNomesServidoresPorRfs(string[] codigosRfs);
        Task<IEnumerable<Guid>> ObterPerfisUsuarioPorRf(string usuarioRf);
    }
}
