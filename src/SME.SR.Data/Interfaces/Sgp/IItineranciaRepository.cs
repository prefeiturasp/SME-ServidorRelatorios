using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public interface IItineranciaRepository
    {
        Task<IEnumerable<Itinerancia>> ObterComUEDREPorIds(long[] ids);
        Task<IEnumerable<ItineranciaObjetivoDto>> ObterObjetivosPorItineranciaIds(long[] ids);
        Task<IEnumerable<ItineranciaQuestaoDto>> ObterQuestoesPorItineranciaIds(long[] ids);
        Task<IEnumerable<ItineranciaAlunoDto>> ObterAlunosPorItineranciaIds(long[] ids);
        Task<IEnumerable<ListagemItineranciaDto>> ObterItinerancias(FiltroRelatorioListagemItineranciasDto filtro);
    }
}
