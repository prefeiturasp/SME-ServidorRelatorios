using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SME.SR.Infra
{
    public class BuscaAtivaSimplesDto
    {
        public long Id { get; set; }    
        public string DreCodigo { get; set; }
        public string DreAbreviacao { get; set; }
        public string UeCodigo { get; set; }
        public string UeNome { get; set; }
        public TipoEscola TipoEscola { get; set; }
        public string TurmaCodigo { get; set; }
        public string TurmaNome { get; set; }
        public int TurmaAno { get; set; }
        public int TurmaTipoTurno { get; set; }
        public int AnoLetivo { get; set; }
        public Modalidade Modalidade { get; set; }
        public string AlunoCodigo { get; set; }
        public string AlunoNome { get; set; }
        public DateTime DataRegistroAcao { get; set; }
        public string ProcedimentoRealizado { get; set; }
        public string ConseguiuContatoResponsavel { get; set; }
        public string ObsGeralAoContatarOuNaoResponsavel { get; set; }
        public string QuestoesObsDuranteVisita { get; set; }
        public string JustificativaMotivoFalta { get; set; }
        public string JustificativaMotivoFaltaOpcaoOutros { get; set; }
    }
}
