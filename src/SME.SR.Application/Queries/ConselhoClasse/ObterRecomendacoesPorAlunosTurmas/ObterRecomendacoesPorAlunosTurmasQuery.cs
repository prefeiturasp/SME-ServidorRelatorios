using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterRecomendacoesPorAlunosTurmasQuery : IRequest<IEnumerable<RecomendacaoConselhoClasseAluno>>
    {
        public ObterRecomendacoesPorAlunosTurmasQuery(string[] codigosAluno, string[] codigosTurma, int anoLetivo, Modalidade modalidade, int semestre)
        {
            CodigosAluno = codigosAluno;
            CodigosTurma = codigosTurma;
            AnoLetivo = anoLetivo;
            Modalidade = modalidade;
            Semestre = semestre;
        }

        public string[] CodigosAluno { get; set; }
        public string[] CodigosTurma { get; set; }
        public int AnoLetivo { get; set; }
        public Modalidade Modalidade { get; set; }
        public  int Semestre { get; set; }
    }
}
