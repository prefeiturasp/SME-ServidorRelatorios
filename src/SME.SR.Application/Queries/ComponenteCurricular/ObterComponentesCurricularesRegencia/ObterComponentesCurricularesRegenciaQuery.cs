using MediatR;
using SME.SR.Data;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterComponentesCurricularesRegenciaQuery : IRequest<IEnumerable<ComponenteCurricularPorTurma>>
    {
        public Turma Turma { get; set; }

        public long CdComponenteCurricular { get; set; }

        public Usuario Usuario { get; set; }
    }
}
