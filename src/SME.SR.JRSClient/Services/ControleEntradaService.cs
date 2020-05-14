using SME.SR.Infra.Dtos.Resposta.ControlesEntrada;
using SME.SR.JRSClient.Grupos;
using SME.SR.JRSClient.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.JRSClient.Services
{
    public class ControleEntradaService : ServiceBase<IReports> , IControleEntradaService
    {
        public ControleEntradaService(Configuracoes configuracoes) : base(configuracoes)
        {
        }

        public async Task<ListaControlesEntradaDto> ObterControlesEntrada(string path, bool excludeState)
        {
            string exclude = excludeState ? "state" : "";

            return await restService.GetObterControlesEntradaAsync(path, exclude);
        }
    }
}
