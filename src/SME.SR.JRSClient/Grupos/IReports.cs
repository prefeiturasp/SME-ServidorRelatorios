using Microsoft.AspNetCore.Http;
using Refit;
using SME.SR.Infra.Dtos.Requisicao;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.JRSClient.Grupos
{
    public interface IReports
    {
        [Headers("Accept: application/json")]
        [Get("/jasperserver/rest_v2/reports/{Dto.Relatorio}/to/{Dto.Nome}.{Dto.Formato}")]
        Task<HttpResponse> GetRelatorioSincrono([Query]ExecutarRelatorioSincronoDto Dto);
    }
}
