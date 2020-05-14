using Refit;
using SME.SR.Infra.Dtos.Resposta.ControlesEntrada;
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
        [Get("/jasperserver/rest_v2/reports/{**path}/inputControls")]
        Task<ListaControlesEntradaDto> GetObterControlesEntradaAsync(string path, string exclude);
    }
}
