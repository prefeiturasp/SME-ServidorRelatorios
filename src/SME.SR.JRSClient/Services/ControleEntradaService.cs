using SME.SR.Infra.Dtos;
using SME.SR.JRSClient.Grupos;
using SME.SR.JRSClient.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.JRSClient.Services
{
    public class ControleEntradaService : ServiceBase<IReports>, IControleEntradaService
    {
        public ControleEntradaService(Configuracoes configuracoes) : base(configuracoes)
        {
        }

        public async Task<ListaControlesEntradaDto> MudarOrdemControlesEntrada(string caminhoRelatorio, ListaControlesEntradaDto listaControlesEntradaDto)
        {
            return await restService.MudarOrdemControlesEntradaAsync(caminhoRelatorio, listaControlesEntradaDto);
        }

        public async Task<ListaControlesEntradaDto> ObterControlesEntrada(string caminhoRelatorio, bool ignorarEstados)
        {
            string ignorarEstadosString = ignorarEstados ? "state" : "";

            return await restService.GetObterControlesEntradaAsync(caminhoRelatorio, ignorarEstadosString);
        }

        public async Task<ListaEstadosControleEntradaDto> ObterEstadosControlesEntrada(string caminhoRelatorio, bool ignorarCache)
        {
            return await restService.GetObterEstadosControlesEntradaAsync(caminhoRelatorio, ignorarCache);
        }

        public async Task<ListaControlesEntradaDto> SetarValoresControleEntrada(string caminhoRelatorio, IDictionary<string, object[]> valoresControles, bool atualizarCache)
        {
            return await restService.SetarValoresControleEntradaAsync(caminhoRelatorio, valoresControles, atualizarCache); ;
        }
    }
}
