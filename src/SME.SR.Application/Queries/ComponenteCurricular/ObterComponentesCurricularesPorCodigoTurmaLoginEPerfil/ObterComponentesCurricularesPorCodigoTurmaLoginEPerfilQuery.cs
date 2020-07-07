using MediatR;
using SME.SR.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Application
{
    public class ObterComponentesCurricularesPorCodigoTurmaLoginEPerfilQuery : IRequest<IEnumerable<ComponenteCurricularPorTurma>>
    {
        public ObterComponentesCurricularesPorCodigoTurmaLoginEPerfilQuery()
        {
        }

        public ObterComponentesCurricularesPorCodigoTurmaLoginEPerfilQuery(string codigoTurma, Usuario usuario)
        {
            CodigoTurma = codigoTurma;
            Usuario = usuario;
        }

        public string CodigoTurma { get; set; }
        public Usuario Usuario { get; set; }
    }
}
