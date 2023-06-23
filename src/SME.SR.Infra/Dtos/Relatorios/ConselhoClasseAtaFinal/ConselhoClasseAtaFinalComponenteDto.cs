using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class ConselhoClasseAtaFinalComponenteDto
    {
        public ConselhoClasseAtaFinalComponenteDto()
        {
            Colunas = new List<ConselhoClasseAtaFinalColunaDto>();
        }

        public long IdGrupoMatriz { get; set; }
        public long Id { get; set; }
        public string Nome { get; set; }
        public bool Regencia {  get; set; }
        public List<ConselhoClasseAtaFinalColunaDto> Colunas { get; set; }

        public void AdicionarColuna(string nome)
        {
            Colunas.Add(new ConselhoClasseAtaFinalColunaDto()
            {
                Id = Colunas.Count + 1,
                Nome = nome
            });
        }
    }
}
