using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class ConselhoClasseAtaBimestralComponenteDto
    {
        public ConselhoClasseAtaBimestralComponenteDto()
        {
            Colunas = new List<ConselhoClasseAtaBimestralColunaDto>();
        }

        public long IdGrupoMatriz { get; set; }
        public long Id { get; set; }
        public long IdComponenteCurricularTerritorio { get; set; }

        public string Nome { get; set; }
        public List<ConselhoClasseAtaBimestralColunaDto> Colunas { get; set; }

        public void AdicionarColuna(string nome)
        {
            Colunas.Add(new ConselhoClasseAtaBimestralColunaDto()
            {
                Id = Colunas.Count + 1,
                Nome = nome
            });
        }
    }
}
