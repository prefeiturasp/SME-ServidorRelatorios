using System;
using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioDevolutivasDto
    {
        public string Dre { get; set; }
        public string Ue { get; set; }
        public string Turma { get; set; }
        public string Bimestre { get; set; }
        public string Usuario { get; set; }
        public string RF { get; set; }
        public string DataSolicitacao { get => DateTime.Now.ToString("dd/MM/yyyy"); }
        public bool ExibeConteudoDevolutivas { get; set; }
        public IEnumerable<TurmasDevolutivasDto> Turmas { get; set; }
    }
}
