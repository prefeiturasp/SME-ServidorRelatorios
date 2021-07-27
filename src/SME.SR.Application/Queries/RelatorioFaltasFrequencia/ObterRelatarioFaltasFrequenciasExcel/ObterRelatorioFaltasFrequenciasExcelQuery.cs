using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterRelatorioFaltasFrequenciasExcelQuery : IRequest<IEnumerable<RelatorioFaltasFrequenciasBaseExcelDto>>
    {
        public RelatorioFrequenciaDto RelatorioFaltasFrequencias { get; set; }

        public TipoRelatorioFaltasFrequencia TipoRelatorio{ get; set; }
    }
}
