using MediatR;
using SME.SR.Infra.Dtos;
using System.Collections.Generic;

namespace SME.SR.Application
{
    class ObterAusenciaPorAlunoTurmaBimestreQuery : IRequest<IEnumerable<AusenciaBimestreDto>>
    {
        public ObterAusenciaPorAlunoTurmaBimestreQuery(string[] codigosAlunos, string codigoTurma, string bimestre)
        {
            CodigosAlunos = codigosAlunos;
            CodigoTurma = codigoTurma;
            Bimestre = bimestre;
        }

        public string[] CodigosAlunos { get; set; }
        public string CodigoTurma { get; set; }
        public string Bimestre { get; set; }
    }
}
