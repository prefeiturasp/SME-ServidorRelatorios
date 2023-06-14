using SME.SR.Infra.Utilitarios;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace SME.SR.Infra
{
    public class RetornoNotaConceitoBimestreComponenteDto
    {
        private TipoNota? _tipoNota;
        public string AlunoCodigo { get; set; }
        public int? Bimestre { get; set; }
        public long ComponenteCurricularCodigo { get; set; }
        public long? ConselhoClasseAlunoId { get; set; }
        public long? ConceitoId { get; set; }
        public string Conceito { get; set; }
        public long? SinteseId { get; set; }
        public string Sintese { get; set; }
        public double? Nota { get; set; }
        public string NotaConceito { get => ObterNotaConceito(); }
        public bool EhNotaConceitoFechamento { get; set; }
        public bool PossuiTurmaAssociada { get; set; }
        public string DreNome { get; set; }
        public string DreCodigo { get; set; }
        public string DreAbreviacao { get; set; }
        public string UeCodigo { get; set; }
        public string UeNome { get; set; }
        public string Ano { get; set; }
        public string TurmaCodigo { get; set; }
        public string TurmaNome { get; set; }
        public TipoEscola TipoEscola { get; set; }
        public string NotaConceitoFinal => $"{NotaConceito} {(!ConselhoClasseAlunoId.HasValue ? "*" : string.Empty)}";
        public string UeNomeComTipoEscola { get { return $"{TipoEscola.ShortName()} - {UeNome}"; } }
        public bool ExcluirNota { get; set; }        
        public string NotaConceitoEmAprovacao { get; set; }
        public string NotaConceitoPosConselhoEmAprovacao { get; set; }
        public long? ConselhoClasseNotaId { get; set; }

        public void CarregaTipoNota(TipoNota? tipoNota)
        {
            _tipoNota = tipoNota;
        }

        private string ObterNotaConceito()
        {
            if (PrecisaTratarNotaParaConceito())
                TrataNotaNumericoParaConceito();

            return ConceitoId.HasValue ? Conceito : Nota.HasValue ? Nota.Value.ToString("0.0", CultureInfo.InvariantCulture) : "";
        }

        private bool PrecisaTratarNotaParaConceito()
        {
            return _tipoNota.HasValue && _tipoNota.GetValueOrDefault() == TipoNota.Conceito && Nota.HasValue && !ConselhoClasseAlunoId.HasValue;
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
            return Nota.Value > 7 ? ObterConceito(TipoConceito.PlenamenteSatisfatorio) : null;
        }

        private ConceitoDto ObterConceitoSatisfatorio()
        {
            return Nota.Value >= 5 && Nota.Value <= 7 ? ObterConceito(TipoConceito.Satisfatorio) : null;
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
