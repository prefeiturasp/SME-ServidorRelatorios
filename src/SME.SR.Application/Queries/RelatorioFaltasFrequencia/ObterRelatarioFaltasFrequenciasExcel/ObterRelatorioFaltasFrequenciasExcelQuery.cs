using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterRelatorioFaltasFrequenciasExcelQuery : IRequest<IEnumerable<RelatorioFaltasFrequenciasExcelDto>>
    {
        public FiltroRelatorioFaltasFrequenciasDto RelatorioFaltasFrequencias { get; set; }
    }
}
