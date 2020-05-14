using Refit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.JRSClient.Grupos
{
    public interface IReports
    {
        [Headers("Accept: application/json")]

        [Get("/jasperserver/rest_v2/reportExecutions/123/status/")]
        Task<string> GetStatusAsync([Header("Authorization")] string authorization);
    }
}
