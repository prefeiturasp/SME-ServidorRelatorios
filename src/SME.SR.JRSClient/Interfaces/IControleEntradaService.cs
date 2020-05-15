using SME.SR.Infra.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.JRSClient.Interfaces
{
    public interface IControleEntradaService
    {
        Task<ListaControlesEntradaDto> ObterControlesEntrada(string caminhoRelatorio, bool ignorarEstados);

        Task<ListaEstadosControleEntradaDto> ObterEstadosControlesEntrada(string caminhoRelatorio, bool ignorarCache);

        Task<ListaControlesEntradaDto> MudarOrdemControlesEntrada(string caminhoRelatorio, ListaControlesEntradaDto listaControlesEntradaDto);

        Task<ListaControlesEntradaDto> SetarValoresControleEntrada(string caminhoRelatorio, IDictionary<string, object[]> valoresControles, bool atualizarCache);
    }
}
