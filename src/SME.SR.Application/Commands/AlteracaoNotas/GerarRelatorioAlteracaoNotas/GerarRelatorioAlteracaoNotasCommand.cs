using MediatR;
using SME.SR.Infra;
using System;

namespace SME.SR.Application
{
    public class GerarRelatorioAlteracaoNotasCommand : IRequest<bool>
    {
        public GerarRelatorioAlteracaoNotasCommand(FiltroRelatorioAlteracaoNotasDto filtros, Guid codigoCorrelacao)
        {
            Filtros = filtros;
            CodigoCorrelacao = codigoCorrelacao;
        }

        public FiltroRelatorioAlteracaoNotasDto Filtros { get; set; }
        public Guid CodigoCorrelacao { get; set; }
    }
}
