using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class AulaDiarioBordoDto
    {
        public long AulaId { get; set; }
        public bool AulaCJ { get; set; }
        public string Turma { get; set; }
        public int? Bimestre { get; set; }
        public string ComponenteCurricular { get; set; }
        public DateTime DataAula { get; set; }
        public DateTime? DataPlanejamento { get; set; }
        public string UsuarioRf { get; set; }
        public string Usuario { get; set; }
        public string Planejamento { get; set; }
        public long? DevolutivaId { get; set; }
        public TipoAula TipoAula { get; set; }
    }
}
