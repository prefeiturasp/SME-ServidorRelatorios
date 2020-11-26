using System;

namespace SME.SR.Infra
{
    public class FiltroAlteracaoNotasDto
    {
        public string Dre { get; set; }
        public string Ue { get; set; }
        public string Turma { get; set; }        
        public string Bimestre { get; set; }
        public string Usuario { get; set; }
        public string RF { get; set; }
        public DateTime Data => DateTime.Now;
    }
}
