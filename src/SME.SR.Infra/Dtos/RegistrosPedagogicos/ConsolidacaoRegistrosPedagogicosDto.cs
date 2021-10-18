using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class ConsolidacaoRegistrosPedagogicosDto
    {
        public long PeriodoEscolarId { get; set; }
        public string Bimestre { get; set; }
        public long TurmaId { get; set; }
        public string TurmaNome { get; set; }
        public string TurmaModalidade { get; set; }
        public int AnoLetivo { get; set; }
        public long ComponenteCurricularId { get; set; }
        public string ComponenteCurricularNome { get; set; }
        public int QuantidadeAulas { get; set; }
        public int FrequenciasPendentes { get; set; }
        public DateTime DataUltimaFrequencia { get; set; }
        public DateTime DataUltimoPlanoAula { get; set; }
        public DateTime DataUltimoDiarioBordo { get; set; }
        public int DiarioBordoPendentes { get; set; }
        public int PlanoAulaPendentes { get; set; }
        public string NomeProfessor { get; set; }
        public string RFProfessor { get; set; }

        public ConsolidacaoRegistrosPedagogicosDto()
        {

        }

        public ConsolidacaoRegistrosPedagogicosDto(long periodoEscolarId, string bimestre, long turmaId, string turmaNome, string turmaModalidade, int anoLetivo, long componenteCurricularId, string componenteCurricularNome, int quantidadeAulas, int frequenciasPendentes, DateTime dataUltimaFrequencia, DateTime dataUltimoPlanoAula, DateTime dataUltimoDiarioBordo, int diarioBordoPendentes, int planoAulaPendentes, string nomeProfessor, string rFProfessor)
        {
            PeriodoEscolarId = periodoEscolarId;
            Bimestre = bimestre;
            TurmaId = turmaId;
            TurmaNome = turmaNome;
            TurmaModalidade = turmaModalidade;
            AnoLetivo = anoLetivo;
            ComponenteCurricularId = componenteCurricularId;
            ComponenteCurricularNome = componenteCurricularNome;
            QuantidadeAulas = quantidadeAulas;
            FrequenciasPendentes = frequenciasPendentes;
            DataUltimaFrequencia = dataUltimaFrequencia;
            DataUltimoPlanoAula = dataUltimoPlanoAula;
            DataUltimoDiarioBordo = dataUltimoDiarioBordo;
            DiarioBordoPendentes = diarioBordoPendentes;
            PlanoAulaPendentes = planoAulaPendentes;
            NomeProfessor = nomeProfessor;
            RFProfessor = rFProfessor;
        }
    }
}
