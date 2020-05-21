using Microsoft.AspNetCore.Http;
using Refit;
using SME.SR.Infra.Dtos;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace SME.SR.JRSClient.Grupos
{
    public interface IReports
    {
        [Headers("Accept: application/json", "Authorization: Basic")]
        [Get("/jasperserver/rest_v2/reportExecutions/{reportExecutionId}/status")]
        Task<string> GetStatusAsync(int reportExecutionId);

        [Headers("Accept: application/json", "Authorization: Basic")]
        [Get("/jasperserver/rest_v2/reports/{**caminhoRelatorio}/inputControls")]
        Task<ListaControlesEntradaDto> GetObterControlesEntradaAsync(string caminhoRelatorio, [AliasAs("exclude")] string ignorarEstados);

        [Headers("Accept: application/json", "Authorization: Basic")]
        [Put("/jasperserver/rest_v2/reports/{**caminhoRelatorio}/inputControls")]
        Task<ListaControlesEntradaDto> MudarOrdemControlesEntradaAsync(string caminhoRelatorio, [Body] ListaControlesEntradaDto listaControlesEntradaDto);

        [Headers("Accept: application/json", "Authorization: Basic")]
        [Post("/jasperserver/rest_v2/reports/{**caminhoRelatorio}/inputControls")]
        Task<ListaControlesEntradaDto> SetarValoresControleEntradaAsync(string caminhoRelatorio, [Body] IDictionary<string, object[]> valoresControles, [Query, AliasAs("freshData")]bool atualizarCache);

        [Headers("Accept: application/json", "Authorization: Basic")]
        [Get("/jasperserver/rest_v2/reports/{**caminhoRelatorio}/inputControls/values")]
        Task<ListaEstadosControleEntradaDto> GetObterEstadosControlesEntradaAsync(string caminhoRelatorio, [AliasAs("freshData")] bool ignorarCache);

        [Headers("Accept: application/json", "Authorization: Basic")]
        [Get("/jasperserver/rest_v2/reports/{**caminhoCompleto}")]
        Task<Stream> GetRelatorioSincrono(string caminhoCompleto, ExecutarRelatorioSincronoDto Dto);

        [Headers("Accept: application/json", "Authorization: Basic")]
        [Get("/jasperserver/rest_v2/jobs")]
        Task<TrabalhoListaResumoDto> GetTrabalhosRelatoriosAsync([AliasAs("reportUnitURI")] string caminhoRelatorio);

        [Headers("Accept: application/json", "Authorization: Basic")]
        [Post("/jasperserver/rest_v2/jobs/pause")]
        Task<TrabalhoListaIdsDto> PostPausarTrabalhosAsync(TrabalhoListaIdsDto trabalhoListaIdsDto);

        [Headers("Accept: application/json", "Authorization: Basic")]
        [Post("/jasperserver/rest_v2/jobs/resume")]
        Task<TrabalhoListaIdsDto> PostInciarTrabalhosAsync(TrabalhoListaIdsDto trabalhoListaIdsDto);

        [Headers("Accept: application/json", "Authorization: Basic")]
        [Post("/jasperserver/rest_v2/jobs/restart")]
        Task<TrabalhoListaIdsDto> PostReinicarTrabalhosFalhadosAsync(TrabalhoListaIdsDto trabalhoListaIdsDto);

        [Headers("Accept: application/json", "Authorization: Basic")]
        [Delete("/jasperserver/rest_v2/jobs/{trabalhoId}")]
        Task<int> DeleteTrabalhoPorIdAsync(int trabalhoId);

        [Headers("Accept: application/json", "Authorization: Basic")]
        [Delete("/jasperserver/rest_v2/jobs")]
        Task<TrabalhoListaIdsDto> DeleteTrabalhosPorListaIdAsync([AliasAs("id"), Query(CollectionFormat.Multi)] IEnumerable<int> ids);

        [Headers("Accept: application/json", "Authorization: Basic")]
        [Get("/jasperserver/rest_v2/jobs")]
        Task<TrabalhoListaResumoDto> GetTrabalhosRelatoriosAsync(TrabalhoFiltroDto filtro, [AliasAs("example")] string? exemplo);

        [Headers("Accept: application/json", "Authorization: Basic")]
        [Get("/jasperserver/rest_v2/jobs/{trabalhoId}")]
        Task<TrabalhoDefinicaoDto> GetTrabalhoRelatorioPorIdAsync(int trabalhoId);

        [Headers("Accept: application/json", "Authorization: Basic", "Content-Type: application/json")]
        [Post("/jasperserver/rest_v2/jobs/{trabalhoId}")]
        Task<TrabalhoDefinicaoDto> PostAlterarDefinicaoTrabalhoAsync(int trabalhoId, TrabalhoDefinicaoDto trabalhoDefinicaoDto);

        [Headers("Accept: application/json", "Authorization: Basic", "Content-Type: application/json")]
        [Post("/jasperserver/rest_v2/jobs")]
        Task<TrabalhoListaIdsDto> PostAtualizarTrabalhosEmLoteAsync([AliasAs("id"),Query(CollectionFormat.Multi)] IEnumerable<int>? ids, bool replaceTriggerIgnoreType, [Body]string? trabalhoDefinicaoJson);

        [Headers("Accept: application/json", "Authorization: Basic")]
        [Get("/jasperserver/rest_v2/jobs/{trabalhoId}/state")]
        Task<TrabalhoEstadoDto> GetTrabalhoEstadoAsync(int trabalhoId);

        [Headers("Accept: application/json", "Authorization: Basic", "Content-Type: application/json")]
        [Put("/jasperserver/rest_v2/jobs")]
        Task<TrabalhoDefinicaoDto> PutAgendarTrabalhoRelatorioAsync(TrabalhoDefinicaoDto definicoesJson);
    }
}
