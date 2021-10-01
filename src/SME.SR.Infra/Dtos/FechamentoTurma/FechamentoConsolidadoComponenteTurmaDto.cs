using System;

namespace SME.SR.Infra
{
    public class FechamentoConsolidadoComponenteTurmaDto
    {
        public DateTime DataAtualizacao { get; set; }

        public SituacaoFechamento Status { get; set; }

        public SituacaoFechamento StatusRelatorio { get => ObterSituacaoFechamento(); }

        public long ComponenteCurricularCodigo { get; set; }

        public string ProfessorRf { get; set; }

        public string ProfessorNome { get; set; }

        public string TurmaCodigo { get; set; }

        public int Bimestre { get; set; }

        private SituacaoFechamento ObterSituacaoFechamento()
        {
            if (Status != Infra.SituacaoFechamento.EmProcessamento)
                return Status;

            return Infra.SituacaoFechamento.NaoIniciado;
        }
    }
}
