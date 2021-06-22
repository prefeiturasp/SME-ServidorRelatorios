using SME.SR.Infra.Utilitarios;
using System.Globalization;

namespace SME.SR.Infra
{
    public class RetornoNotaConceitoBimestreComponenteDto
    {
        public string AlunoCodigo { get; set; }
        public int? Bimestre { get; set; }
        public long ComponenteCurricularCodigo { get; set; }
        public long? ConselhoClasseAlunoId { get; set; }
        public long? ConceitoId { get; set; }
        public string Conceito { get; set; }
        public long? SinteseId { get; set; }
        public string Sintese { get; set; }
        public double? Nota { get; set; }
        public string NotaConceito { get => ConceitoId.HasValue ? Conceito : Nota.HasValue ? Nota.Value.ToString("0.0", CultureInfo.InvariantCulture) : ""; }
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
        public string NotaConceitoFinal => $"{NotaConceito} {(EhNotaConceitoFechamento ? "*" : string.Empty)}";
        public string UeNomeComTipoEscola { get { return $"{TipoEscola.ShortName()} - {UeNome}"; } }
        public bool ExcluirNota { get; set; }
    }
}
