using System;
using System.Collections.Generic;
using System.Linq;

namespace SME.SR.Infra
{
    public class RelatorioAtribuicaoCjDto
    {
        public string DreNome { get; set; }
        public string UeNome { get; set; }
        public string Modalidade { get; set; }
        public string Semestre { get; set; }
        public string Turma { get; set; }
        public string Professor { get; set; }
        public string RfProfessor { get; set; }
        public string Usuario { get; set; }
        public string RfUsuario { get; set; }
        public string DataImpressao { get { return DateTime.Today.ToString("dd/MM/yyyy"); } }
        public List<AtribuicaoCjPorTurmaDto> AtribuicoesCjPorTurma { get; set; }
        public List<AtribuicaoCjPorProfessorDto> AtribuicoesCjPorProfessor { get; set; }
        public List<AtribuicaoEsporadicaDto> AtribuicoesEsporadicas { get; set; }
        public bool ExibirDre { get; set; }     
        public List<RelatorioAtribuicaoCjDreDto> Dres { get; set; }


        public RelatorioAtribuicaoCjDto()
        {
            AtribuicoesCjPorTurma = new List<AtribuicaoCjPorTurmaDto>();
            AtribuicoesCjPorProfessor = new List<AtribuicaoCjPorProfessorDto>();
            AtribuicoesEsporadicas = new List<AtribuicaoEsporadicaDto>();
            Dres = new List<RelatorioAtribuicaoCjDreDto>();
        }

        public bool RelatorioVazio(TipoVisualizacaoRelatorioAtribuicaoCJ tipoVisualizacao ) {

            if (tipoVisualizacao == TipoVisualizacaoRelatorioAtribuicaoCJ.Professor)
                return !AtribuicoesCjPorProfessor.Any() && !AtribuicoesEsporadicas.Any() && !Dres.Any();
            else
                return !AtribuicoesCjPorTurma.Any() && !AtribuicoesEsporadicas.Any() && !Dres.Any(); 
        }
    }
}
