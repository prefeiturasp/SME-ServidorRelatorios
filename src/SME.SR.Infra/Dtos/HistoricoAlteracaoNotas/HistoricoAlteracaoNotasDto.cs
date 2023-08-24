using SME.SR.Infra.Utilitarios;
using System;

namespace SME.SR.Infra
{
    public class HistoricoAlteracaoNotasDto
    {
        private TipoNota? _tipoNota;
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

        public void CarregaTipoNota(TipoNota? tipoNota)
        {
            _tipoNota = tipoNota;
        }

        public string ObterNotaConceitoAnterior(TipoNota tipoNotaTurma)
        {
            return ObterNotaConceito(tipoNotaTurma, ConceitoAnteriorId, NotaAnterior);
        }

        public string ObterNotaConceitoAtribuido(TipoNota tipoNotaTurma)
        {
            return ObterNotaConceito(tipoNotaTurma, ConceitoAtribuidoId, NotaAtribuida);
        }

        private string ObterNotaConceito(TipoNota tipoNotaTurma, TipoConceito? conceito, double? nota)
        {
            if (PrecisaTratarNotaParaConceito(nota))
                return ObterNotaConvertidoParaConceito(nota);

            var tipoNota = _tipoNota ?? tipoNotaTurma;

            return tipoNota == Infra.TipoNota.Nota ? RetornarNota(nota) : RetornarConceito(conceito);
        }

        private string RetornarNota(double? nota)
        {
            if (nota.HasValue)
                return nota.ToString();
            return string.Empty;
        }

        private string RetornarConceito(TipoConceito? conceito)
        {
            if (conceito.HasValue)
                return conceito.Name();
            return string.Empty;
        }

        private bool PrecisaTratarNotaParaConceito(double? nota)
        {
            return _tipoNota.HasValue && _tipoNota.GetValueOrDefault() == Infra.TipoNota.Conceito && nota.HasValue && TipoNota == TipoAlteracaoNota.Fechamento;
        }

        private string ObterNotaConvertidoParaConceito(double? nota)
        {
            TipoConceito? tipoConceito = ObterConceitoPlenamenteSatisfatorio(nota) ?? ObterConceitoSatisfatorio(nota) ?? ObterConceitoNaoSatisfatorio(nota);

            return RetornarConceito(tipoConceito);
        }

        private TipoConceito? ObterConceitoPlenamenteSatisfatorio(double? nota)
        {
            return nota.Value > 7 ? (TipoConceito?)TipoConceito.PlenamenteSatisfatorio : null;
        }

        private TipoConceito? ObterConceitoSatisfatorio(double? nota)
        {
            return nota.Value >= 5 && nota.Value <= 7 ? (TipoConceito?)TipoConceito.Satisfatorio : null;
        }

        private TipoConceito? ObterConceitoNaoSatisfatorio(double? nota)
        {
            return nota.Value < 5 ? (TipoConceito?)TipoConceito.NaoSatisfatorio : null;
        }
    }
    
}
