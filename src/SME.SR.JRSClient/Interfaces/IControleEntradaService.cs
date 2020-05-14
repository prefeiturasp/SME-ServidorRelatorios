using SME.SR.Infra.Dtos.Resposta.ControlesEntrada;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.JRSClient.Interfaces
{
    public interface IControleEntradaService
    {
        Task<ListaControlesEntradaDto> ObterControlesEntrada(string path, bool excludeState);
    }
}
