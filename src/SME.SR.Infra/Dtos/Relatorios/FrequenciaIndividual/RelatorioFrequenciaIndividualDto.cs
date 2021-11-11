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
        public string RF { get; set; }
        public string Data => DateTime.Now.ToString("dd/MM/yyyy");
        public bool ehInfantil{ get; set; }
        public bool ehTodosBimestre{ get; set; }

        public List<RelatorioFrequenciaIndividualAlunosDto> Alunos { get; set; }
    }
}
