using MediatR;
using SME.SR.Data;
using System;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterAlunosPorTurmaDataSituacaoMatriculaQuery : IRequest<IEnumerable<Aluno>>
    {
        public ObterAlunosPorTurmaDataSituacaoMatriculaQuery(long turmaCodigo, DateTime dataReferenciaFim, DateTime? dataReferenciaInicio = null)
        {
            TurmaCodigo = turmaCodigo;
            DataReferenciaFim = dataReferenciaFim;
            DataReferenciaInicio = dataReferenciaInicio;
        }

        public long TurmaCodigo { get; set; }
        public DateTime DataReferenciaFim { get; set; }
        public DateTime? DataReferenciaInicio { get; set; }
    }
}
