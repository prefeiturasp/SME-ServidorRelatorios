using MediatR;
using SME.SR.Infra;
using SME.SR.Infra.Dtos.AE.Adesao;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterListaRelatorioAdessaoAEQuery : IRequest<AdesaoAERetornoDto>
    {
        public IEnumerable<AdesaoAEQueryConsolidadoRetornoDto> ListaConsolida { get; set; }
        public AdesaoAEFiltroDto RelatorioFiltros { get; set; }

        public ObterListaRelatorioAdessaoAEQuery(IEnumerable<AdesaoAEQueryConsolidadoRetornoDto> listaConsolida, AdesaoAEFiltroDto relatorioFiltros)
        {
            ListaConsolida = listaConsolida;
            RelatorioFiltros = relatorioFiltros;
        }
    }
}