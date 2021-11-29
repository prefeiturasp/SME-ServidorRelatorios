using MediatR;
using SME.SR.Infra;
using System;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterAlunosPorTurmaEDataMatriculaQuery : IRequest<IEnumerable<AlunoPorTurmaRespostaDto>>
    {
        public string CodigoTurma { get; set; }
        public DateTime DataMatricula { get; set; }

        public ObterAlunosPorTurmaEDataMatriculaQuery(string codigoTurma, DateTime dataMatricula)
        {
            CodigoTurma = codigoTurma;
            DataMatricula = dataMatricula;
        }
    }
}
