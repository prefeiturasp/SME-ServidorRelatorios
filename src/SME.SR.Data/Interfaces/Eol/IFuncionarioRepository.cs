using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SME.SR.Infra;

namespace SME.SR.Data
{
    public interface IFuncionarioRepository
    {
        Task<IEnumerable<Funcionario>> ObterFuncionariosPorCargoUe(string codigoCargo, string codigoUe);
        Task<IEnumerable<Funcionario>> ObterNomesServidoresPorRfs(string[] codigosRfs);
        Task<IEnumerable<Guid>> ObterPerfisUsuarioPorRf(string usuarioRf);
        Task<IEnumerable<DisciplinaTerritorioSaberDto>> BuscarDisciplinaTerritorioDosSaberesAsync(string codigoTurma, IEnumerable<long> codigosComponentesCurriculares);
    }
}
