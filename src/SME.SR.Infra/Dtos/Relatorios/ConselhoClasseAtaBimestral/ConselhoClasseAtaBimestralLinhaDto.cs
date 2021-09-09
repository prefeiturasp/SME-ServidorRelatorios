using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class ConselhoClasseAtaBimestralLinhaDto
    {
        public ConselhoClasseAtaBimestralLinhaDto()
        {
            Celulas = new List<ConselhoClasseAtaBimestralCelulaDto>();
        }

        public long Id { get; set; }
        public string Nome { get; set; }
        public string ConselhoClasse { get; set; }
        public string Situacao { get; set; }
        public bool Inativo { get; set; }
        public List<ConselhoClasseAtaBimestralCelulaDto> Celulas { get; set; }

        public void AdicionaCelula(long grupoMatriz, long componenteCurricular, string valor, int coluna)
            => Celulas.Add(new ConselhoClasseAtaBimestralCelulaDto()
            {
                GrupoMatriz = grupoMatriz,
                ComponenteCurricular = componenteCurricular,
                Coluna = coluna,
                Valor = valor
            });
    }
}
