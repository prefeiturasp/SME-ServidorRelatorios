using System;

namespace SME.SR.Data
{
    public class ComponenteCurricularPorTurmaRegencia
    {
        public string CodigoTurma { get; set; }
        public long CodDisciplina { get; set; }
        public long? CodDisciplinaPai { get; set; }
        public string Disciplina { get; set; }
        public bool Regencia { get; set; }
        public bool Compartilhada { get; set; }
        public bool Frequencia { get; set; }
        public bool TerritorioSaber { get; set; }
        public bool LancaNota { get; set; }
        public bool BaseNacional { get; set; }
        public ComponenteCurricularGrupoMatriz GrupoMatriz { get; set; }

        public string ObterDisciplina()
        {
            var tamnhoDisciplina = Disciplina.Length;
            return tamnhoDisciplina > 37 ? Disciplina.Substring(0,37) : Disciplina;
        }
    }
}
