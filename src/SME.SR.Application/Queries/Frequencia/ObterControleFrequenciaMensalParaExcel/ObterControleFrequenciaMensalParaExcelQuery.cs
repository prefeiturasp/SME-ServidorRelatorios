using MediatR;
using SME.SR.Infra;
using SME.SR.Infra.Dtos.FrequenciaMensal;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterControleFrequenciaMensalParaExcelQuery : IRequest<IEnumerable<RelatorioControleFrequenciaMensalExcelDto>>
    {
        public List<ControleFrequenciaMensalDto> ControlesFrequenciasMensais { get; set; }

        public ObterControleFrequenciaMensalParaExcelQuery(List<ControleFrequenciaMensalDto> controlesFrequenciasMensais)
        {
            ControlesFrequenciasMensais = controlesFrequenciasMensais;
        }
    }
}
