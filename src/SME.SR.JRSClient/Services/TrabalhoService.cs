using Newtonsoft.Json;
using SME.SR.Infra;
using SME.SR.Infra.Dtos;
using SME.SR.JRSClient.Grupos;
using SME.SR.JRSClient.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SME.SR.JRSClient.Services
{
    public class TrabalhoService : ServiceBase<IReports>, ITrabalhoService
    {
        public TrabalhoService(Configuracoes configuracoes) : base(configuracoes)
        {
        }

        public async Task<TrabalhoDefinicaoDto> AgendarTrabalhoRelatorio(TrabalhoDefinicaoDto definicoes)
        {
            return await restService.PutAgendarTrabalhoRelatorioAsync(definicoes);
        }

        public async Task<TrabalhoDefinicaoDto> AlterarDefinicaoTrabalho(int trabalhoId, TrabalhoDefinicaoDto trabalhoDefinicaoDto)
        {
            return await restService.PostAlterarDefinicaoTrabalhoAsync(trabalhoId, trabalhoDefinicaoDto);
        }

        public async Task<TrabalhoListaIdsDto> AtualizarTrabalhosEmLote(IEnumerable<int> ids, bool sobrescreverGatilhoIgnorandoTipo, TrabalhoDefinicaoDto trabalhoDefinicao)
        {
            var trabalhoDefinicaoJson = UtilJson.ConverterApenasCamposNaoNulos(trabalhoDefinicao);

            return await restService.PostAtualizarTrabalhosEmLoteAsync(ids, sobrescreverGatilhoIgnorandoTipo, trabalhoDefinicaoJson);
        }

        public Task<int> DeletarTrabalhoPorId(int trabalhoId)
        {
            return restService.DeleteTrabalhoPorIdAsync(trabalhoId);
        }

        public Task<TrabalhoListaIdsDto> DeletarTrabalhosPorListaId(IEnumerable<int> ids)
        {
            return restService.DeleteTrabalhosPorListaIdAsync(ids);
        }

        public async Task<TrabalhoListaIdsDto> InciarTrabalhos(TrabalhoListaIdsDto trabalhoListaIdsDto)
        {
            return await restService.PostInciarTrabalhosAsync(trabalhoListaIdsDto);
        }

        public async Task<TrabalhoEstadoDto> ObterTrabalhoEstado(int trabalhoId)
        {
            return await restService.GetTrabalhoEstadoAsync(trabalhoId);
        }

        public async Task<TrabalhoDefinicaoDto> ObterTrabalhoRelatorioPorId(int trabalhoId)
        {
            return await restService.GetTrabalhoRelatorioPorIdAsync(trabalhoId);
        }

        public async Task<TrabalhoListaResumoDto> ObterTrabalhosRelatorios()
        {
            return await restService.GetTrabalhosRelatoriosAsync(default);
        }

        public async Task<TrabalhoListaResumoDto> ObterTrabalhosRelatorios(string caminhoRelatorio)
        {
            return await restService.GetTrabalhosRelatoriosAsync(caminhoRelatorio);
        }

        public async Task<TrabalhoListaResumoDto> ObterTrabalhosRelatorios(TrabalhoFiltroDto trabalhoFiltroDto, TrabalhoModelDto model)
        {
            var modelJson = UtilJson.ConverterApenasCamposNaoNulos<TrabalhoModelDto>(model);

            return await restService.GetTrabalhosRelatoriosAsync(trabalhoFiltroDto, modelJson);
        }

        public async Task<TrabalhoListaIdsDto> PausarTrabalhos(TrabalhoListaIdsDto trabalhoListaIdsDto)
        {
            return await restService.PostPausarTrabalhosAsync(trabalhoListaIdsDto);
        }

        public async Task<TrabalhoListaIdsDto> ReinicarTrabalhosFalhados(TrabalhoListaIdsDto trabalhoListaIdsDto)
        {
            return await restService.PostReinicarTrabalhosFalhadosAsync(trabalhoListaIdsDto);
        }
    }
}
