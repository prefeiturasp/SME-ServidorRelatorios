using MediatR;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Application
{
    public class ObterTotalAlunosSemFrequenciaPorTurmaBimestreQuery : IRequest<IEnumerable<TotalAulasTurmaDisciplinaDto>>
    {
        public ObterTotalAlunosSemFrequenciaPorTurmaBimestreQuery(string[] disciplinaId, string[] codigoTurma, int[] bimestre)
        {
            CodigoTurma = codigoTurma;
            DisciplinaId = disciplinaId;
            Bimestre = bimestre;
        }
        public string[] CodigoTurma { get; set; }
        public string[] DisciplinaId { get; set; }
        public int[] Bimestre { get; set; }

    }
}
