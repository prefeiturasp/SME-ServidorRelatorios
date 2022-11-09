using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System.Collections.Generic;
using System.Linq;

namespace SME.SR.Application
{
    public class ObterComponentesCurricularesTurmasRelatorioAtaFinalResultadosQuery : IRequest<IEnumerable<IGrouping<string, ComponenteCurricularPorTurma>>>
    {
        public ObterComponentesCurricularesTurmasRelatorioAtaFinalResultadosQuery(string[] codigosTurma, string codigoUe, Modalidade modalidade, int[] bimestres = null)
        {
            CodigosTurma = codigosTurma;
            CodigoUe = codigoUe;
            Modalidade = modalidade;
            Bimestres = bimestres;
        }

        public string[] CodigosTurma { get; set; }
        public string CodigoUe { get; set; }
        public Modalidade Modalidade{ get; set; }
        public int[] Bimestres { get; set; }
    }
}
