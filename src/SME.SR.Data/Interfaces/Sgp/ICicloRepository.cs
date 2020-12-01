using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public interface ICicloRepository
    {
        Task<long?> ObterCicloIdPorAnoModalidade(string ano, Modalidade modalidadeCodigo);
        Task<IEnumerable<TipoCiclo>> ObterCiclosPorAnosModalidade(string[] anos, Modalidade modalidadeCodigo);
        Task<IEnumerable<TipoCiclo>> ObterPorUeId(long ueId);
        Task<TipoCiclo> ObterPorId(long id);
        Task<CicloTurmaDto> ObterCicloPorAnoModalidade(string ano, Modalidade modalidade);
        Task<NotaTipoValor> ObterPorCicloIdDataAvalicacao(long cicloId, DateTime dataAvalicao);
    }
}
