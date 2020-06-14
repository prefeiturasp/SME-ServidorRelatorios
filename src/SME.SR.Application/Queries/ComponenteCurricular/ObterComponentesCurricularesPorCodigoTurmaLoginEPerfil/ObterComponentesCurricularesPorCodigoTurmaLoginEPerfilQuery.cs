using MediatR;
using SME.SR.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Application
{
    public class ObterComponentesCurricularesPorCodigoTurmaLoginEPerfilQuery : IRequest<IEnumerable<ComponenteCurricularPorTurma>>
    {
        public string Login { get; set; }
        public Guid IdPerfil { get; set; }
        public string CodigoTurma { get; set; }
    }
}
