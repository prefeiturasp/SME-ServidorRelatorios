using System;
using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class AtendimentoClinicoAlunoDto
    {
        public AtendimentoClinicoAlunoDto()
        { }

        public string DiaSemana { get; set; }
        public string AtendimentoAtividade { get; set; }
        public string LocalRealizacao { get; set; }
        public DateTime HorarioInicio { get; set; }
        public DateTime HorarioTermino { get; set; }   
    }
}