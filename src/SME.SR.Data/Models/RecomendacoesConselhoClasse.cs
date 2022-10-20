using System.Collections;
using System.Collections.Generic;

namespace SME.SR.Data
{
    public class RecomendacoesConselhoClasse
    {
        public RecomendacoesConselhoClasse()
        {
            RecomendacoesPreDefinidasAluno = new List<ConselhoClasseRecomendacao>();
            RecomendacoesPreDefinidasFamilia = new List<ConselhoClasseRecomendacao>();
        }

        public long ConselhoClasseAlunoId { get; set; }
        public long ConselhoClasseId { get; set; }
        public string RecomendacoesAluno { get; set; }
        public string RecomendacoesFamilia { get; set; }
        public string AnotacoesPedagogicas { get; set; }
       public List<ConselhoClasseRecomendacao> RecomendacoesPreDefinidasAluno { get; set; }
       public List<ConselhoClasseRecomendacao> RecomendacoesPreDefinidasFamilia { get; set; }
    }
}
