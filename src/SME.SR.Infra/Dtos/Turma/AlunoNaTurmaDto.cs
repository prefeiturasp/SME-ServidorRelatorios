﻿using System;

namespace SME.SR.Infra
{
    public class AlunoNaTurmaDto
    {
        public int CodigoComponenteCurricular { get; set; }
        public int TipoResponsavel { get; set; }
        public string NomeResponsavel { get; set; }
        public Nullable<char> ParecerConclusivo { get; set; }
        public string TurmaRemanejamento { get; set; }
        public string TurmaTransferencia { get; set; }
        public string EscolaTransferencia { get; set; }
        public bool Remanejado { get; set; }
        public bool Transferencia_Interna { get; set; }
        public bool PossuiDeficiencia { get; set; }
        public int NumeroAlunoChamada { get; set; }
        public DateTime DataMatricula { get; set; }
        public DateTime DataSituacao { get; set; }
        public string SituacaoMatricula { get; set; }
        public int CodigoSituacaoMatricula { get; set; }
        public string NomeSocialAluno { get; set; }
        public DateTime DataNascimento { get; set; }
        public string NomeAluno { get; set; }
        public int CodigoTurma { get; set; }
        public int CodigoAluno { get; set; }
        public string TurmaEscola { get; set; }
        public int Ano { get; set; }
        public string CelularResponsavel { get; set; }
        public DateTime DataAtualizacaoContato { get; set; }
    }
}