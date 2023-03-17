using System;

namespace SME.SR.Infra
{
    public class HistoricoAlteracaoNotasDto
    {
        public string NomeTurma { get; set; }
        public string CodigoAluno { get; set; }
        public string NomeAluno { get; set; }
        public string NumeroChamada { get; set; }
        public long DisciplinaId { get; set; }
        public string ComponenteCurricularNome { get; set; }
        public long Bimestre { get; set; }
        public TipoNota TipoNotaConceito { get; set; }
        public TipoAlteracaoNota TipoNota { get; set; }
        public double? NotaAnterior { get; set; }
        public double? NotaAtribuida { get; set; }
        public TipoConceito? ConceitoAnteriorId { get; set; }
        public TipoConceito? ConceitoAtribuidoId { get; set; }
        public DateTime DataAlteracao { get; set; }
        public string UsuarioAlteracao { get; set; }
        public string RfAlteracao { get; set; }
        public WorkflowAprovacaoNivelStatus Situacao { get; set; }
        public string UsuarioAprovacao { get; set; }
        public string RfAprovacao { get; set; }
        public bool EmAprovacao { get; set; }
    }
}
