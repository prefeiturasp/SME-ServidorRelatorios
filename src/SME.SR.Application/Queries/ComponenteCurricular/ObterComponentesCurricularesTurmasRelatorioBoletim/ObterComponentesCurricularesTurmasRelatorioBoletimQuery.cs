using MediatR;
using Microsoft.Extensions.Primitives;
using SME.SR.Data;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterComponentesCurricularesTurmasRelatorioBoletimQuery : IRequest<IEnumerable<ComponenteCurricularPorTurma>>
    {
        public string[] CodigosTurma { get; set; }

        public string CodigoUe { get; set; }

        public Modalidade Modalidade{ get; set; }

        public Usuario Usuario { get; set; }
    }
}
