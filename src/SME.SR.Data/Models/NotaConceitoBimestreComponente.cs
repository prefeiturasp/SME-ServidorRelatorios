using SME.SR.Infra;
using SME.SR.Infra.Utilitarios;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace SME.SR.Data
{
    public class NotaConceitoBimestreComponente
    {
        private readonly TipoNota? _tipoNota;

        public NotaConceitoBimestreComponente()
        {
        }

        public NotaConceitoBimestreComponente(TipoNota tipoNota)
        {
            _tipoNota = tipoNota;
        }

        public string AlunoCodigo { get; set; }
        public int? Bimestre { get; set; }
        public long ComponenteCurricularCodigo { get; set; }
        public long? ConceitoId { get; set; }
        public string Conceito { get; set; }
        public double? Nota { get; set; }
        public string Sintese { get; set; }        
        public string NotaConceito { get => ObterNotaConceito(); }
        public long ConselhoClasseAlunoId { get; set; }
        public int NotaId { get; set; }
        public bool Aprovado { get; set; }


        private string ObterNotaConceito()
        {
            if (PrecisaTratarNotaParaConceito())
                TrataNotaNumericoParaConceito();

            return ConceitoId.HasValue ? Conceito : Nota.HasValue ? Nota.Value.ToString("0.0", CultureInfo.InvariantCulture) : "";
        }

        private bool PrecisaTratarNotaParaConceito()
        {
            return _tipoNota.HasValue && _tipoNota.GetValueOrDefault() == TipoNota.Conceito && Nota.HasValue;
        }

        private void TrataNotaNumericoParaConceito()
        {
            ConceitoDto conceito = ObterConceitoPlenamenteSatisfatorio() ?? ObterConceitoSatisfatorio() ?? ObterConceitoNaoSatisfatorio();

            if (conceito != null)
            {
                ConceitoId = long.Parse(conceito.Valor);
                Conceito = conceito.Descricao;
            }
        }

        private ConceitoDto ObterConceitoPlenamenteSatisfatorio()
        {
            return Nota.Value >= 7 ? ObterConceito(TipoConceito.PlenamenteSatisfatorio) : null;
        }

        private ConceitoDto ObterConceitoSatisfatorio()
        {
            return Nota.Value >= 5 && Nota.Value < 7 ? ObterConceito(TipoConceito.Satisfatorio) : null;
        }

        private ConceitoDto ObterConceitoNaoSatisfatorio()
        {
            return Nota.Value < 5 ? ObterConceito(TipoConceito.NaoSatisfatorio) : null;
        }

        private ConceitoDto ObterConceito(TipoConceito tipoConceito)
        {
            return new ConceitoDto(((int)tipoConceito).ToString(), tipoConceito.GetAttribute<DisplayAttribute>().Name);
        }
    }
}
