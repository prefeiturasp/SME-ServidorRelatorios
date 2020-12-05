using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SME.SR.Data
{
   public class AtribuicaoEsporadica
    {
        public DateTime DataFim { get; set; }
        public DateTime DataInicio { get; set; }
        public string DreId { get; set; }
        public bool Excluido { get; set; }
        public bool Migrado { get; set; }
        public string ProfessorRf { get; set; }
        public string UeId { get; set; }
        public string CriadoPor { get; set; }
        public DateTime CriadoEm { get; set; }
    }
}
