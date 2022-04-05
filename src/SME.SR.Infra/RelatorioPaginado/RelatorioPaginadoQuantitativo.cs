using SME.SR.Infra.RelatorioPaginado.Interfaces;
using System;

namespace SME.SR.Infra.RelatorioPaginado
{
    public class RelatorioPaginadoQuantitativo<T> : RelatorioPaginado<T> where T : class
    {

        private readonly ParametroRelatorioPaginadoQuantitativo<T> _parametro;
   
        public RelatorioPaginadoQuantitativo(ParametroRelatorioPaginadoQuantitativo<T> parametro)
        {
            this._parametro = parametro;
            this.ListaDeValores = parametro.Valores;
        }

        protected override void Construir()
        {
            var totalDePaginaPorColuna = ObtenhaTotalDePaginaPorColuna();

            for (var indicePagina = 1; indicePagina <= totalDePaginaPorColuna; indicePagina++)
            {
                if (AtingiuLimiteDeLinhas())
                {
                    this.AdicionePagina(this.ObtenhaPagina(indicePagina, indicePagina, this.ListaDeValores));
                }
                else
                {
                    this.CarreguePaginaPorQuebraDeLinha();
                }
            }
        }
        protected override int ObtenhaQuantidadeDeLinha()
        {
            return this._parametro.QuantidadeDeLinhas;
        }

        private void CarreguePaginaPorQuebraDeLinha()
        {
            var paginaAtual = this.ObtenhaIndicePaginaAtual();
            var dicionario = this.ObtenhaDicionarioPorQuantidade();

            foreach (var indicePagina in dicionario.Keys)
            {
                var novaPagina = paginaAtual + indicePagina;

                this.AdicionePagina(this.ObtenhaPagina(novaPagina, indicePagina, dicionario[indicePagina]));
            }
        }

        private bool AtingiuLimiteDeLinhas()
        {
            return this.ListaDeValores.Count <= this._parametro.QuantidadeDeLinhas;
        }

        private double ObtenhaTotalDePaginaPorColuna()
        {
            if (this._parametro.QuantidadeDeColunasPorLinha >= this._parametro.QuantidadeTotalDeColunas)
            {
                return 1;
            }

            var pagina = (double)(this._parametro.QuantidadeTotalDeColunas / this._parametro.QuantidadeDeColunasPorLinha);

            return Math.Ceiling(pagina);
        }
    }
}
