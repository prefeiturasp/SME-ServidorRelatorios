using System;
using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioFrequenciaIndividualDto
    {
        public RelatorioFrequenciaIndividualDto()
        {
            Alunos = new List<RelatorioFrequenciaIndividualAlunosDto>();
        }
        public string DreNome { get; set; }
        public string UeNome { get; set; }
        public string Usuario { get; set; }
        public string TurmaNome { get; set; }
        public string ComponenteNome { get; set; }
        public string RF { get; set; }
        public string Data => DateTime.Now.ToString("dd/MM/yyyy");
        public bool ehInfantil { get; set; }
        public bool ehTodosBimestre { get; set; }
        public int QtdeCaracteres => DreNome.Length + UeNome.Length + Usuario.Length + TurmaNome.Length + ComponenteNome.Length + RF.Length + Data.Length;

        public List<RelatorioFrequenciaIndividualAlunosDto> Alunos { get; set; }
        public bool ImprimirFrequenciaDiaria { get; set; }
        public long[] CodigosComponentesConsiderados { get; set; }
    }
}
