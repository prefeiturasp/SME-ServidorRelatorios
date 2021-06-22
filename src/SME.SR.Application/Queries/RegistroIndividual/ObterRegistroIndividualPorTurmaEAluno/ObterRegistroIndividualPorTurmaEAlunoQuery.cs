using MediatR;
using SME.SR.Infra;
using System;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterRegistroIndividualPorTurmaEAlunoQuery : IRequest<IEnumerable<AcompanhamentoAprendizagemRegistroIndividualDto>>
    {
        public ObterRegistroIndividualPorTurmaEAlunoQuery(long turmaId, long? alunoCodigo, DateTime dataInicio, DateTime dataFim)
        {
            TurmaId = turmaId;
            AlunoCodigo = alunoCodigo;
            DataInicio = dataInicio;
            DataFim = dataFim;
        }

        public long TurmaId { get; set; }
        public long? AlunoCodigo { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
    }
}
