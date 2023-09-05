using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class AlunoConselhoClasseCadastradoBimestresQuery : IRequest<IEnumerable<int>>
    {
        public AlunoConselhoClasseCadastradoBimestresQuery()
        {

        }

        public AlunoConselhoClasseCadastradoBimestresQuery(string codigoAluno, int anoLetivo, Modalidade modalidade, int semestre, string codigoTurma)
        {
            AnoLetivo = anoLetivo;
            Modalidade = modalidade;
            Semestre = semestre;
            CodigoAluno = codigoAluno;
            CodigoTurma = codigoTurma;
        }
        public string CodigoAluno { get; set; }
        public int AnoLetivo { get; set; }
        public Modalidade Modalidade { get; set; }
        public int Semestre { get; set; }
        public string CodigoTurma { get; set; }
    }
}
