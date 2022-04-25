using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;
using System.Data;

namespace SME.SR.Application
{
    public class ObterRelatorioDeFrequenciaGlobalExcelQuery : IRequest<IEnumerable<DataTable>>
    {
        public List<FrequenciaGlobalDto> ObjetoExportacao { get; set; }

        public ObterRelatorioDeFrequenciaGlobalExcelQuery(List<FrequenciaGlobalDto> objetoExportacao)
        {
            ObjetoExportacao = objetoExportacao;
        }
    }
}
