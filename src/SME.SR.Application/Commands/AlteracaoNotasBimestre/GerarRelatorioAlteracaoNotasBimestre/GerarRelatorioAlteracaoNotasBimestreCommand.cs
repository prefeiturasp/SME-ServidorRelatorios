using MediatR;
using SME.SR.Infra;
using System;

namespace SME.SR.Application
{
    public class GerarRelatorioAlteracaoNotasBimestreCommand : IRequest<bool>
    {
        public GerarRelatorioAlteracaoNotasBimestreCommand(FiltroRelatorioAlteracaoNotasBimestreDto filtros, Guid codigoCorrelacao)
        {
            Filtros = filtros;
            CodigoCorrelacao = codigoCorrelacao;
        }

        public FiltroRelatorioAlteracaoNotasBimestreDto Filtros { get; set; }
        public Guid CodigoCorrelacao { get; set; }
    }
}
