using MediatR;
using SME.SR.Data;
using System;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterAlunosPorTurmaDataSituacaoMatriculaQuery : IRequest<IEnumerable<Aluno>>
    {
        public ObterAlunosPorTurmaDataSituacaoMatriculaQuery(long turmaCodigo, DateTime dataReferencia)
        {
            TurmaCodigo = turmaCodigo;
            Referencia = dataReferencia;
        }

        public long TurmaCodigo { get; set; }
        public DateTime Referencia { get; set; }
    }
}
