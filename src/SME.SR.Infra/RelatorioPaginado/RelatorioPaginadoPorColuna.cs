using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace SME.SR.Infra
{
    public abstract class RelatorioPaginadoPorColuna<T> : RelatorioPaginado<T> where T : class
    {
        private const int ALTURA_ADICIONAL = 230;
        public RelatorioPaginadoPorColuna(ParametroRelatorioPaginadoPorColuna<T> parametro)
        {
            this.ListaDeValores = parametro.Valores;
            this.Parametro = parametro;
            this.ListaDeAgrupamento = new List<Func<T, object>>();
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

        protected override Dictionary<int, List<T>> ObtenhaDicionarioPorQuantidadeSemAgrupamentoCalculado(List<IColuna> listaDeColunas)
        {
            Dictionary<int, List<T>> dicionarioValores = new Dictionary<int, List<T>>();
            var pagina = 1;
            int totalAlturaDaPagina = 0;
            int alturaConteudo = ObtenhaAlturaConteudoPagina();

            dicionarioValores.Add(pagina, new List<T>());

            foreach (var valor in this.ListaDeValores)
            {
                totalAlturaDaPagina = ObtenhaTotalAlturaPagina(listaDeColunas, totalAlturaDaPagina, valor);
                dicionarioValores[pagina].Add(valor);

                if (totalAlturaDaPagina >= alturaConteudo)
                {
                    pagina++;
                    totalAlturaDaPagina = 0;
                    dicionarioValores.Add(pagina, new List<T>());
                }
            }

            if (!dicionarioValores.LastOrDefault().Value.Any())
            {
                dicionarioValores.Remove(dicionarioValores.LastOrDefault().Key);
            }

            return dicionarioValores;
        }

        protected virtual string ObtenhaValorDaColunaCustom(IColuna coluna, T valor)
        {
            return string.Empty;
        }

        private int ObtenhaAlturaPorLinha(string texto, int largura)
        {
            var novaLargura = ObtenhaLarguraPx(largura);
            StringFormat formato = new StringFormat(StringFormatFlags.LineLimit);
            Graphics grafico = Graphics.FromImage(new Bitmap(1, 1));

            return (int)grafico.MeasureString(texto, Parametro.Fonte, novaLargura, formato).Height;
        }

        private int ObtenhaTotalAlturaPagina(List<IColuna> listaDeColunas, float totalLinhasPagina, T valor)
        {
            float maiorAlturaDaLinha = 0;

            foreach (var coluna in listaDeColunas)
            {
                var resultado = ObtenhaValorColuna(coluna, valor);
                var alturaColunaAtual = ObtenhaAlturaPorLinha(resultado, coluna.Largura);

                if (alturaColunaAtual > maiorAlturaDaLinha)
                    maiorAlturaDaLinha = alturaColunaAtual;
            }

            return (int)(maiorAlturaDaLinha + totalLinhasPagina);
        }

        private string ObtenhaValorColuna(IColuna coluna, T valor)
        {
            if (coluna.ContemPropriedade())
            {
                return coluna.ObtenhaValorDaPropriedade(valor);
            }

            return ObtenhaValorDaColunaCustom(coluna, valor);
        }

        private int ObtenhaLarguraPx(int largura)
        {
            if (this.Parametro.UnidadeDeTamanho == EnumUnidadeDeTamanho.PERCENTUAL)
            {
                double percentual = (double)largura / 100.0;
                return (int)(percentual * Parametro.TipoDePapel.LarguraPx);
            }

            return largura;
        }

        protected bool AtingiuLimiteDeLinhas()
        {
            return RelatorioPorQuantidadeDeLinha() && ValorTotalAlturaPorPagina() <= this.Parametro.TipoDePapel.AlturaPx;
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

        protected override bool RelatorioPorQuantidadeDeLinha()
        {
            return Parametro.Fonte == null;
        }

        protected int ObtenhaAlturaConteudoPagina()
        {
            return Parametro.TipoDePapel.AlturaPx + ALTURA_ADICIONAL;
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
