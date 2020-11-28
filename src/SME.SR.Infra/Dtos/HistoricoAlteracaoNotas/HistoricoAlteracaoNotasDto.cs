using System;

namespace SME.SR.Infra
{
    public class HistoricoAlteracaoNotasDto
    {
        public string CodigoAluno { get; set; }
        public TipoNota TipoNotaConceito { get; set; }
        public double NotaAnterior { get; set; }
        public double NotaAtribuida { get; set; }
        public string ConceitoAnteriorId { get; set; }
        public string ConceitoAtribuidoId { get; set; }
        public DateTime DataAlteracao { get; set; }
        public string UsuarioAlteracao { get; set; }
        public string RfAlteracao { get; set; }
    }
}
