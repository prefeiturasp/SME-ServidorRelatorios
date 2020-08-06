using MediatR;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Application
{
    public class ObterRelatorioParecerConclusivoQuery : IRequest<RelatorioParecerConclusivoDto>
    {
        public FiltroRelatorioParecerConclusivoDto filtroRelatorioParecerConclusivoDto { get; set; }
        public string UsuarioRf { get; set; }
    }
}
