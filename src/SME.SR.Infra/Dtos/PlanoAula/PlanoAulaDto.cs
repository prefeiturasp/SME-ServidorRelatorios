using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class PlanoAulaDto
    {
        public long Id { get; set; }
        public string DesenvolvimentoAula { get; set; }
        public string Descricao { get; set; }
        public Modalidade ModalidadeTurma { get; set; }
        public string Recuperacao { get; set; }
        public string LicaoCasa { get; set; }
        public string TipoEscola { get; set; }
        public string Dre { get; set; }
        public string Ue { get; set; }
        public string Turma { get; set; }
        public string TurmaCodigo { get; set; }
        public string ComponenteCurricular { get; set; }
        public long ComponenteCurricularId { get; set; }
        public string Usuario { get; set; }
        public string RF { get; set; }
        public DateTime Data => DateTime.Now;
        public DateTime DataPlanoAula { get; set; }

        public IEnumerable<ObjetivoAprendizagemDto> Objetivos { get; set; }
    }
}
