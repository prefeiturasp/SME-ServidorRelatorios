using System;
using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class DetalheBuscaAtivaDto
    {
        public DetalheBuscaAtivaDto()
        {
            Questoes = new List<ItemQuestaoDetalheBuscaAtivaDto>();
        }
        public string Aluno { get; set; }
        public string Turma { get; set; }
        public DateTime DataRegistroAcao { get; set; }
        public List<ItemQuestaoDetalheBuscaAtivaDto> Questoes { get; set; }
    }

    public class ItemQuestaoDetalheBuscaAtivaDto
    {
        public string Questao { get; set; }
        public string Resposta { get; set; }

        public ItemQuestaoDetalheBuscaAtivaDto(string questao, string resposta)
        {
            Questao = questao;
            Resposta = resposta;
        }
    } 
}
