using MediatR;
using SME.SR.Data;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterNotasFinaisPorTurmaBimestreQuery : IRequest<IEnumerable<NotaConceitoBimestreComponente>>
    {
        public ObterNotasFinaisPorTurmaBimestreQuery(string turmaCodigo, int[] bimestres)
        {
            TurmaCodigo = turmaCodigo;
            Bimestres = bimestres;
        }

        public string TurmaCodigo { get; set; }

        public int[] Bimestres { get; set; }
    }
}
