using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System.Collections.Generic;
using System.Linq;

namespace SME.SR.Application
{
    public class ObterComponentesCurricularesTurmasRelatorioBoletimQuery : IRequest<IEnumerable<IGrouping<string, ComponenteCurricularPorTurma>>>
    {
        public string[] CodigosTurma { get; set; }

        public string CodigoUe { get; set; }

        public Modalidade Modalidade{ get; set; }

        public Usuario Usuario { get; set; }
    }
}
