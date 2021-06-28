using System;

namespace SME.SR.Infra
{
    public class ConselhoClasseConsolidadoTurmaAlunoDto
    {
        public DateTime DataAtualizacao { get; set; }

        public SituacaoConselhoClasse Status { get; set; }

        public string AlunoCodigo { get; set; }

        public long? ParecerConclusivoId { get; set; }

        public long TurmaId { get; set; }

        public int Bimestre { get; set; }
    }
}
