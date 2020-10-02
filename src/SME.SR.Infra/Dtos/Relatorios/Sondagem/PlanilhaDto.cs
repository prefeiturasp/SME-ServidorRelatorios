using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class PlanilhaDto
    {
        public AlunoDto Aluno { get; set; }
        public List<OrdemDto> Ordens { get; set; } = new List<OrdemDto>();
        public List<OrdemRespostasDto> OrdensRespostas { get; set; } = new List<OrdemRespostasDto>();

        public PlanilhaDto()
        {
            this.Ordens = new List<OrdemDto>();
            this.OrdensRespostas = new List<OrdemRespostasDto>();
        }
    }
}
