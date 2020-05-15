using Refit;
using SME.SR.Infra.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.JRSClient.Grupos
{
    public interface IReports
    {
        [Headers("Accept: application/json", "Authorization: Basic")]
        [Get("/jasperserver/rest_v2/reportExecutions/123/status/")]
        Task<string> GetStatusAsync();

        [Headers("Accept: application/json", "Authorization: Basic")]
        [Get("/jasperserver/rest_v2/reports/{**caminhoRelatorio}/inputControls")]
        Task<ListaControlesEntradaDto> GetObterControlesEntradaAsync(string caminhoRelatorio, [AliasAs("exclude")] string ignorarEstados);

        [Headers("Accept: application/json", "Authorization: Basic")]
        [Put("/jasperserver/rest_v2/reports/{**caminhoRelatorio}/inputControls")]
        Task<ListaControlesEntradaDto> MudarOrdemControlesEntradaAsync(string caminhoRelatorio, [Body] ListaControlesEntradaDto listaControlesEntradaDto);

        [Headers("Accept: application/json", "Authorization: Basic")]
        [Post("/jasperserver/rest_v2/reports/{**caminhoRelatorio}/inputControls")]
        Task<ListaControlesEntradaDto> SetarValoresControleEntradaAsync(string caminhoRelatorio, [Body] IDictionary<string, object[]> valoresControles, [Query,AliasAs("freshData")]bool atualizarCache);

        [Headers("Accept: application/json", "Authorization: Basic")]
        [Get("/jasperserver/rest_v2/reports/{**caminhoRelatorio}/inputControls/values")]
        Task<ListaEstadosControleEntradaDto> GetObterEstadosControlesEntradaAsync(string caminhoRelatorio, [AliasAs("freshData")] bool ignorarCache);
    }
}
