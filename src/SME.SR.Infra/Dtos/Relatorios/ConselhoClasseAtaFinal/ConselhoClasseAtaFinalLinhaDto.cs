using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class ConselhoClasseAtaFinalLinhaDto
    {
        public ConselhoClasseAtaFinalLinhaDto()
        {
            Celulas = new List<ConselhoClasseAtaFinalCelulaDto>();
        }

        public long Id { get; set; }
        public string Nome { get; set; }
        public string Situacao { get; set; }
        public bool Inativo { get; set; }
        public List<ConselhoClasseAtaFinalCelulaDto> Celulas { get; set; }

        public void AdicionaCelula(long grupoMatriz, long componenteCurricular, string valor, int coluna, bool regencia = false)
            => Celulas.Add(new ConselhoClasseAtaFinalCelulaDto()
            {
                GrupoMatriz = grupoMatriz,
                ComponenteCurricular = componenteCurricular,
                Coluna = coluna,
                Valor = valor,
                Regencia = regencia
            });

    }
}
