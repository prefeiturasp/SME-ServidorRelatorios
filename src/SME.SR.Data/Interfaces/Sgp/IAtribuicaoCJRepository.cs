using SME.SR.Infra;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data.Interfaces
{
    public interface IAtribuicaoCJRepository
    {
        Task<IEnumerable<AtribuicaoCJ>> ObterPorFiltros(Modalidade modalidade, string turmaId, string ueId, long componenteCurricularId, string usuarioRf, string usuarioNome, bool? substituir, string dreCodigo = "", string[] turmaIds = null, long[] componentesCurricularresId = null, int? anoLetivo = null);
    }
}
