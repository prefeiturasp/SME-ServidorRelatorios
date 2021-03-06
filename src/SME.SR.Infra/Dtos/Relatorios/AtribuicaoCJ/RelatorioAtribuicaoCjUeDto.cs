﻿using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioAtribuicaoCjUeDto
    {
        public RelatorioAtribuicaoCjUeDto(string nome, string codigo)
        {
            AtribuicoesCjPorTurma = new List<AtribuicaoCjPorTurmaDto>();
            AtribuicoesCjPorProfessor = new List<AtribuicaoCjPorProfessorDto>();
            AtribuicoesEsporadicas = new List<AtribuicaoEsporadicaDto>();
            Nome = nome;
            Codigo = codigo;
        }
        public string Codigo { get; set; }
        public string Nome { get; set; }
        public List<AtribuicaoCjPorTurmaDto> AtribuicoesCjPorTurma { get; set; }
        public List<AtribuicaoCjPorProfessorDto> AtribuicoesCjPorProfessor { get; set; }
        public List<AtribuicaoEsporadicaDto> AtribuicoesEsporadicas { get; set; }
    }
}