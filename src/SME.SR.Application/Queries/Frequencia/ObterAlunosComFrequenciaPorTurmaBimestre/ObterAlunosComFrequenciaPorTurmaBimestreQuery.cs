using MediatR;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterAlunosComFrequenciaPorTurmaBimestreQuery : IRequest<IEnumerable<string>>
    {
        public ObterAlunosComFrequenciaPorTurmaBimestreQuery(string turmaCodigo, int bimestre)
        {
            TurmaCodigo = turmaCodigo;
            Bimestre = bimestre;
        }

        public string TurmaCodigo { get; }
        public int Bimestre { get; }
    }
}
