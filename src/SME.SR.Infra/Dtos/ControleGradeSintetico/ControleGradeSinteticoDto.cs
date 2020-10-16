using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class ControleGradeSinteticoDto
    {
        public string Dre { get; set; }
        public string Ue { get; set; }
        public string TurmaDescricao { get; set; }
        public string BimestreDescricao { get; set; }
        public string ComponenteCurricularDescricao { get; set; }
        public string Usuario { get; set; }
        public string RF { get; set; }
        public DateTime Data => DateTime.Now;
        public IEnumerable<TurmaControleGradeSinteticoDto> Turmas { get; set; }
    }
}
