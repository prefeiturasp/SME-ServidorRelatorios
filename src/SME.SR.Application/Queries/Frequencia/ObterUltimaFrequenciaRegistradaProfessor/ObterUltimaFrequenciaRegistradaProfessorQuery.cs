using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Application
{
    public class ObterUltimaFrequenciaRegistradaProfessorQuery : IRequest<DateTime>
    {
        public ObterUltimaFrequenciaRegistradaProfessorQuery(string professorRf)
        {
            ProfessorRf = professorRf;
        }

        public string ProfessorRf { get; set; }
    }
}
