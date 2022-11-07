using MediatR;
using SME.SR.Data;
using System;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterAlunosTurmaPorTurmaDataSituacaoMatriculaQuery : IRequest<IEnumerable<Aluno>>
    {
        public ObterAlunosTurmaPorTurmaDataSituacaoMatriculaQuery(string turmaCodigo, DateTime dataReferencia)
        {
            TurmaCodigo = turmaCodigo;
            Referencia = dataReferencia;
        }

        public string TurmaCodigo { get; set; }
        public DateTime Referencia { get; set; }
    }
}
