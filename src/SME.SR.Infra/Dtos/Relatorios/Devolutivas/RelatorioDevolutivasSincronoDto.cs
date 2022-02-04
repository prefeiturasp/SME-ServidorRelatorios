using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioDevolutivasSincronoDto
    {
        public string Dre { get; set; }
        public string Ue { get; set; }
        public string Turma { get; set; }
        public string Bimestre { get; set; }
        public string Usuario { get; set; }
        public string RF { get; set; }
        public string DataSolicitacao { get; set; }
        public bool ExibeConteudoDevolutivas { get; set; }
        public IEnumerable<TurmaDevolutivaSincronoDto> Turmas{ get; set; }
    }
}
