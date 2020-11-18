using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Application
{
    public class ObterUltimoPlanoAulaProfessorQuery : IRequest<DateTime>
    {
        public ObterUltimoPlanoAulaProfessorQuery(string professorRf)
        {
            ProfessorRf = professorRf;
        }

        public string ProfessorRf { get; set; }
    }
}
