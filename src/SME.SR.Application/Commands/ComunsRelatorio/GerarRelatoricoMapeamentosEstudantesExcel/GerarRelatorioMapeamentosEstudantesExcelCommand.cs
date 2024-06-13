using MediatR;
using SME.SR.Infra;
using System;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class GerarRelatorioMapeamentosEstudantesExcelCommand : IRequest
    {
        public IEnumerable<MapeamentoEstudanteUltimoBimestreDto> MapeamentosEstudantes { get; set; }
        public Guid CodigoCorrelacao { get; set; }

        public GerarRelatorioMapeamentosEstudantesExcelCommand(IEnumerable<MapeamentoEstudanteUltimoBimestreDto> mapeamentosEstudantes, Guid codigoCorrelacao)
        {
            MapeamentosEstudantes = mapeamentosEstudantes;
            CodigoCorrelacao = codigoCorrelacao;
        }
    }
}
