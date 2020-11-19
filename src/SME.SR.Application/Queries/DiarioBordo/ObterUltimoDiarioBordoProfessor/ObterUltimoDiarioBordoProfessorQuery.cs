using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Application
{
    public class ObterUltimoDiarioBordoProfessorQuery : IRequest<DateTime?>
    {
        public ObterUltimoDiarioBordoProfessorQuery(string professorRf)
        {
            ProfessorRf = professorRf;
        }

        public string ProfessorRf { get; set; }
    }
}
