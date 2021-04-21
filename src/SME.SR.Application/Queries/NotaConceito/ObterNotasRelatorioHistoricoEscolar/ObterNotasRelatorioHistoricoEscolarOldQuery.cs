using MediatR;
using SME.SR.Data;
using System.Collections.Generic;
using System.Linq;

namespace SME.SR.Application
{
    public class ObterNotasRelatorioHistoricoEscolarOldQuery : IRequest<IEnumerable<IGrouping<string, NotasAlunoBimestre>>>
    {
        public string[] CodigosTurma { get; set; }
        public string[] CodigosAlunos { get; set; }
    }
}