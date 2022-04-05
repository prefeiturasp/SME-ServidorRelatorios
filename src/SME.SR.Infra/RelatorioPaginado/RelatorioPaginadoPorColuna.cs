using SME.SR.Infra.RelatorioPaginado.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace SME.SR.Infra.RelatorioPaginado
{
    public class RelatorioPaginadoPorColuna<T> : RelatorioPaginado<T> where T : class
    {
        private readonly ParametroRelatorioPaginadoPorColuna<T> _parametro;

        public RelatorioPaginadoPorColuna(ParametroRelatorioPaginadoPorColuna<T> parametro)
        {
            this.ListaDeValores = parametro.Valores;
            this._parametro = parametro;
        }

        protected override void Construir()
        {
            var dicionarioColunas = ObtenhaColunasPorPagina();

            foreach (var indicePagina in dicionarioColunas.Keys)
            {
                if (AtingiuLimiteDeLinhas())
                {
                    this.AdicionePagina(this.ObtenhaPagina(1, indicePagina, this.ListaDeValores, dicionarioColunas[indicePagina]));
                }
                else
                {
                    CarreguePaginaPorQuebraDeLinha(dicionarioColunas[indicePagina]);
                }
            }
        }

        private void CarreguePaginaPorQuebraDeLinha(List<IColuna> listaDeColunas)
        {
            var dicionario = ObtenhaDicionarioPorQuantidade();
            var paginaAtual = this.ObtenhaIndicePaginaAtual();

            foreach (var indicePagina in dicionario.Keys)
            {
                var novaPagina = paginaAtual + indicePagina;

                this.AdicionePagina(this.ObtenhaPagina(novaPagina, indicePagina, dicionario[indicePagina], listaDeColunas));
            }
        }

        protected override int ObtenhaQuantidadeDeLinha()
        {
            var totalLinha = 0;
            var alturaLinha = 0;

            do
            {
                totalLinha += 1;
                alturaLinha += this._parametro.AlturaDaLinha; 
            } while (alturaLinha <= this._parametro.TipoDePapel.AlturaPx);

            return totalLinha;
        }

        private bool AtingiuLimiteDeLinhas()
        {
            return ValorTotalAlturaPorPagina() <= this._parametro.TipoDePapel.AlturaPx;
        }

        private int ValorTotalAlturaPorPagina()
        {
            return this.ListaDeValores.Count * this._parametro.AlturaDaLinha;
        }

        private bool AtingiuLimiteDeColunas()
        {
            return this._parametro.TipoDePapel.LarguraPx >= ObtenhaLarguraTotalColunas(this._parametro.Colunas);
        }

        private int ObtenhaLarguraTotalColunas(List<IColuna> colunas)
        {
            return colunas.Sum(coluna => coluna.Largura);
        }

        private bool AtingiuLimiteDeColunas(List<IColuna> colunas, int largura)
        {
            var totalLargura = ObtenhaLarguraTotalColunas(colunas) + largura;

            return this._parametro.TipoDePapel.LarguraPx >= totalLargura;
        }
        private Dictionary<int, List<IColuna>> ObtenhaColunasPorPagina()
        {
            if (AtingiuLimiteDeColunas())
            {
                var dicionario = new Dictionary<int, List<IColuna>>();
                dicionario.Add(1, this._parametro.Colunas);

                return dicionario;
            }

            return ObtenhaColunasPaginada();
        }

        private Dictionary<int, List<IColuna>> ObtenhaColunasPaginada()
        {
            var listaDeChave = this._parametro.Colunas.FindAll(colunas => colunas.Chave);
            var listaColunaSemChave = this._parametro.Colunas.FindAll(colunas => !colunas.Chave);
            var lista = new List<IColuna>();
            var dicionario = new Dictionary<int, List<IColuna>>();
            var pagina = 0;

            lista.AddRange(listaDeChave);

            foreach (var coluna in listaColunaSemChave)
            {
                if (AtingiuLimiteDeColunas(lista, coluna.Largura))
                {
                    lista.Add(coluna);
                }
                else
                {
                    pagina += 1;
                    dicionario.Add(pagina, lista);

                    lista = new List<IColuna>();
                    lista.AddRange(listaDeChave);
                    lista.Add(coluna);
                }
            }

            if (lista.Exists(coluna => !coluna.Chave))
            {
                pagina += 1;
                dicionario.Add(pagina, lista);
            }

            return dicionario;
        }
    }
}
