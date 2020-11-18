using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Application
{
    public class ObterUltimaAulaCadastradaProfessorQuery : IRequest<DateTime>
    {
        public ObterUltimaAulaCadastradaProfessorQuery(string professorRf)
        {
            ProfessorRf = professorRf;
        }

        public string ProfessorRf { get; set; }
    }
}
