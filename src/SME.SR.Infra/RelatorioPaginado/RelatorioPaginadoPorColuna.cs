using SME.SR.Infra.RelatorioPaginado.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace SME.SR.Infra.RelatorioPaginado
{
    public abstract class RelatorioPaginadoPorColuna<T> : RelatorioPaginado<T> where T : class
    {
        public RelatorioPaginadoPorColuna(ParametroRelatorioPaginadoPorColuna<T> parametro)
        {
            this.ListaDeValores = parametro.Valores;
            this.Parametro = parametro;
        }

        protected ParametroRelatorioPaginadoPorColuna<T> Parametro { get; set; }

        protected override int ObtenhaQuantidadeDeLinha()
        {
            var totalLinha = 0;
            var alturaLinha = 0;

            do
            {
                totalLinha += 1;
                alturaLinha += this.Parametro.AlturaDaLinha; 
            } while (alturaLinha <= this.Parametro.TipoDePapel.AlturaPx);

            return totalLinha;
        }
        protected bool AtingiuLimiteDeLinhas()
        {
            return ValorTotalAlturaPorPagina() <= this.Parametro.TipoDePapel.AlturaPx;
        }

        protected int ObtenhaLarguraTotalColunas(List<IColuna> colunas)
        {
            return colunas.Sum(coluna => coluna.Largura);
        }

        protected bool AtingiuLimiteDeColunas(List<IColuna> colunas, int largura)
        {
            var totalLargura = ObtenhaLarguraTotalColunas(colunas) + largura;

            return ObtenhaLarguraTotal() >= totalLargura;
        }

        protected int ObtenhaLarguraTotal()
        {
            if (this.Parametro.UnidadeDeTamanho == EnumUnidadeDeTamanho.PIXEON)
            {
                return this.Parametro.TipoDePapel.LarguraPx;
            }

            return 100;
        }
        private int ValorTotalAlturaPorPagina()
        {
            return this.ListaDeValores.Count * this.Parametro.AlturaDaLinha;
        }
    }
}
