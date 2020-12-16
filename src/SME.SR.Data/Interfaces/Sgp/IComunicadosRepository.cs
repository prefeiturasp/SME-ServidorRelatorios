using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SME.SR.Infra;

namespace SME.SR.Data.Interfaces
{
    public interface IComunicadosRepository
    {
        Task<IEnumerable<LeituraComunicadoDto>> ObterComunicadosPorFiltro(FiltroRelatorioLeituraComunicadosDto filtro);
        Task<IEnumerable<LeituraComunicadoTurmaDto>> ObterComunicadoTurmasPorComunicadosIds(IEnumerable<long> comunicados);
        Task<IEnumerable<LeituraComunicadoTurmaDto>> ObterComunicadoTurmasAppPorComunicadosIds(IEnumerable<long> comunicados);

        Task<string[]> ObterUsuariosApp();

        Task<long[]> ObterComunicadoTurmasAlunosPorComunicadoId(long comunicado);
        Task<IEnumerable<LeituraComunicadoResponsavelDto>> ObterResponsaveisPorAlunosIds(int[] estudantes);

        Task<IEnumerable<LeituraComunicadoEstudanteDto>> ObterComunicadoTurmasEstudanteAppPorComunicadosIds(long[] comunicados);
        Task<IEnumerable<LeituraComunicadoDto>> ObterComunicadoDadosSMEPorComunicadosIds(IEnumerable<long> comunicados);
    }
}
