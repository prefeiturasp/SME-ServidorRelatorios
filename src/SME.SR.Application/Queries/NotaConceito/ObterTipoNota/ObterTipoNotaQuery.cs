using MediatR;
using SME.SR.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Application
{
   public class ObterTipoNotaQuery :IRequest<string>
    {
        public long DreId { get; set; }
        public long UeId { get; set; }
        public PeriodoEscolar PeriodoEscolar { get; set; }
        public Turma Turma { get; set; }
    }
}
