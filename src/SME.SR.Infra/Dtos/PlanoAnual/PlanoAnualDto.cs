using System;
using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class PlanoAnualDto
    {
        public long Id { get; set; }
        public string Ue { get; set; }
        public string TipoEscola { get; set; }
        public string Dre { get; set; }
        public Modalidade ModalidadeTurma { get; set; }
        public string Turma { get; set; }
        public string TurmaCodigo { get; set; }
        public string ComponenteCurricular { get; set; }
        public string Usuario { get; set; }
        public int Bimestre { get; set; }
        public string DescricaoPlanejamento { get; set; }
        public string CodigoObjetivo { get; set; }
        public string DescricaoObjetivo { get; set; }
        public IEnumerable<BimestreDescricaoObjetivosPlanoAnualDto> Objetivos { get; set; }
    }
}
