using SME.SR.Infra.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.JRSClient.Interfaces
{
    public interface ITrabalhoService
    {
        Task<TrabalhoListaResumoDto> ObterTrabalhosRelatorios();
        Task<TrabalhoListaResumoDto> ObterTrabalhosRelatorios(string caminhoRelatorio);
        Task<TrabalhoListaResumoDto> ObterTrabalhosRelatorios(TrabalhoFiltroDto trabalhoFiltroDto, TrabalhoModelDto model);
        Task<TrabalhoDefinicaoDto> ObterTrabalhoRelatorioPorId(int trabalhoId);
        Task<TrabalhoDefinicaoDto> AgendarTrabalhoRelatorio(TrabalhoDefinicaoDto definicoes);
        Task<TrabalhoEstadoDto> ObterTrabalhoEstado(int trabalhoId);
        Task<TrabalhoDefinicaoDto> AlterarDefinicaoTrabalho(int trabalhoId, TrabalhoDefinicaoDto trabalhoDefinicaoDto);
        Task<TrabalhoListaIdsDto> PausarTrabalhos(TrabalhoListaIdsDto trabalhoListaIdsDto);
        Task<TrabalhoListaIdsDto> InciarTrabalhos(TrabalhoListaIdsDto trabalhoListaIdsDto);
        Task<TrabalhoListaIdsDto> ReinicarTrabalhosFalhados(TrabalhoListaIdsDto trabalhoListaIdsDto);
        Task<TrabalhoListaIdsDto> AtualizarTrabalhosEmLote(IEnumerable<int>? ids, bool sobrescreverGatilhoIgnorandoTipo, TrabalhoDefinicaoDto? trabalhoDefinicao);
        Task<int> DeletarTrabalhoPorId(int trabalhoId);
        Task<TrabalhoListaIdsDto> DeletarTrabalhosPorListaId(IEnumerable<int> ids);
    }
}
