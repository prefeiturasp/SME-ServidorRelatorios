using MediatR;
using SME.SR.Infra.Dtos.FrequenciaMensal;
using System;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class GerarRelatoricoControleDeFrequenciaMensalExcelCommand : IRequest
    {
        public IEnumerable<ControleFrequenciaMensalDto> ControlesFrequenciasMensais { get; set; }
        public Guid CodigoCorrelacao { get; set; }

        public GerarRelatoricoControleDeFrequenciaMensalExcelCommand(IEnumerable<ControleFrequenciaMensalDto> controlesFrequenciasMensais, Guid codigoCorrelacao)
        {
            ControlesFrequenciasMensais = controlesFrequenciasMensais;
            CodigoCorrelacao = codigoCorrelacao;
        }
    }
}
