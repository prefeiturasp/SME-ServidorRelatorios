using System;

namespace SME.SR.Infra
{
    public class AulaPlanoAulaDto
    {
        public long AulaId { get; set; }
        public bool AulaCJ { get; set; }
        public string Turma { get; set; }
        public int? Bimestre { get; set; }
        public string ComponenteCurricular { get; set; }
        public long ComponenteCurricularId { get; set; }
        public DateTime DataAula { get; set; }
        public int QuantidadeAula { get; set; }
        public DateTime? DataPlanejamento { get; set; }
        public string UsuarioRf { get; set; }
        public string Usuario { get; set; }
        public string ObjetivosEspecificos { get; set; }
        public string LicaoCasa { get; set; }
        public string RecuperacaoContinua { get; set; }
        public string ObjetivosSalecionados { get; set; }
        public int QtdObjetivosSelecionados { get; set; }
        public int QtdSecoesPreenchidas { get; set; }
    }
}
