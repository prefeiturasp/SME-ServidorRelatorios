﻿using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;
using System.Data;

namespace SME.SR.Application
{
    public class ObterRelatorioConselhoClasseAtaFinalExcelQuery : IRequest<IEnumerable<DataTable>>
    {
        public IEnumerable<ConselhoClasseAtaFinalPaginaDto> ObjetoExportacao { get; set; }
    }
}
