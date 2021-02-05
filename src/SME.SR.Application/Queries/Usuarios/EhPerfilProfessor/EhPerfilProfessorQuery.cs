using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Application
{
    public class EhPerfilProfessorQuery : IRequest<bool>
    {
        public EhPerfilProfessorQuery(Guid perfil)
        {
            Perfil = Perfil;
        }

        public Guid Perfil { get; set; }
    }
}
