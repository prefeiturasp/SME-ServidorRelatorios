using System;

namespace SME.SR.Infra
{
    public struct TurmasDoAlunoDTO
    {
        public int CodigoAluno { get; set; }
        public int TipoTurno { get; set; }
        public int AnoLetivo { get; set; }
        public string NomeAluno { get; set; }
        public string NomeSocialAluno { get; set; }
        public int CodigoSituacaoMatricula { get; set; }
        public string SituacaoMatricula { get; set; }
        public DateTime DataSituacao { get; set; }
        public DateTime DataNascimento { get; set; }
        public string NumeroAlunoChamada { get; set; }
        public int CodigoTurma { get; set; }
        public string NomeResponsavel { get; set; }
        public string TipoResponsavel { get; set; }
        public string CelularResponsavel { get; set; }
        public DateTime DataAtualizacaoContato { get; set; }
        public int CodigoTipoTurma { get; set; }
        public string TurmaNome { get; set; }
    }
}