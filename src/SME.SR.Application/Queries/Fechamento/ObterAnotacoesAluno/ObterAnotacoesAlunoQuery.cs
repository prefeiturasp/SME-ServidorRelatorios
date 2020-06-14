using MediatR;
using SME.SR.Data;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterAnotacoesAlunoQuery : IRequest<IEnumerable<FechamentoAlunoAnotacaoConselho>>
    {
        public string CodigoAluno { get; set; }
        public long FechamentoTurmaId { get; set; }
    }
}
