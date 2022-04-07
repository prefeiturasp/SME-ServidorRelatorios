using System.Collections.Generic;
using System.Linq;

namespace SME.SR.Infra
{
    public class RelatorioPaginadoSubColuna<T> : RelatorioPaginadoPorColuna<T> where T : class
    {
        private Dictionary<SubColuna, List<IColuna>> _coluna;

        public RelatorioPaginadoSubColuna(ParametroRelatorioPaginadoPorColuna<T> parametro, Dictionary<SubColuna, List<IColuna>> colunas): base(parametro)
        {
            this._coluna = colunas;
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

        private void CarreguePaginaPorQuebraDeLinha(Dictionary<SubColuna, List<IColuna>> listaDeColunas)
        {
            var dicionario = ObtenhaDicionarioPorQuantidade();
            var paginaAtual = this.ObtenhaIndicePaginaAtual();

            foreach (var indicePagina in dicionario.Keys)
            {
                var novaPagina = paginaAtual + indicePagina;

                this.AdicionePagina(this.ObtenhaPagina(novaPagina, indicePagina, dicionario[indicePagina], listaDeColunas));
            }
        }

        private Dictionary<int, Dictionary<SubColuna, List<IColuna>>> ObtenhaColunasPorPagina()
        {
            if (AtingiuLimiteDeColunas())
            {
                var dicionario = new Dictionary<int, Dictionary<SubColuna, List<IColuna>>>();
                dicionario.Add(1, this._coluna);

                return dicionario;
            }

            return ObtenhaColunasPaginada();
        }

        private bool AtingiuLimiteDeColunas()
        {
            return ObtenhaLarguraTotal() >= ObtenhaLarguraTotalColunas(this._coluna);
        }

        private int ObtenhaLarguraTotalColunas(Dictionary<SubColuna, List<IColuna>> colunas)
        {
            var total = 0;

            foreach(var lista in colunas.Values)
            {
                total += ObtenhaLarguraTotalColunas(lista);
            }

            return total;
        }

        private Pagina ObtenhaPagina(int indice, int ordenacao, List<T> valores, Dictionary<SubColuna, List<IColuna>> colunas)
        {
            return new PaginaSubColuna()
            {
                Indice = indice,
                Ordenacao = ordenacao,
                Valores = valores,
                Colunas = colunas
            };
        }

        private bool AtingiuLimiteDeColunas(
                            Dictionary<SubColuna, List<IColuna>> valorAdicionado,
                            List<IColuna> valorNovo)
        {
            var totalAdicionado = ObtenhaLarguraTotalColunas(valorAdicionado);
            var totalNovo = ObtenhaLarguraTotalColunas(valorNovo);

            return ObtenhaLarguraTotal() >= (totalAdicionado + totalNovo);
        }

        private Dictionary<int, Dictionary<SubColuna, List<IColuna>>> ObtenhaColunasPaginada()
        {
            var lista = new Dictionary<SubColuna, List<IColuna>>();
            var dicionario = new Dictionary<int, Dictionary<SubColuna, List<IColuna>>>();
            var pagina = 0;
            var valorDeChave = this._coluna.Where(coluna => coluna.Key.Chave).FirstOrDefault();

            lista.Add(valorDeChave.Key, valorDeChave.Value);

            foreach (var subColuna in this._coluna.Where(coluna => !coluna.Key.Chave))
            {
                if (AtingiuLimiteDeColunas(lista, subColuna.Value))
                {
                    lista.Add(subColuna.Key, subColuna.Value);
                } else
                {
                    pagina += 1;
                    dicionario.Add(pagina, lista);

                    lista = new Dictionary<SubColuna, List<IColuna>>();
                    lista.Add(valorDeChave.Key, valorDeChave.Value);
                    lista.Add(subColuna.Key, subColuna.Value);
                }
            }

            if (lista.Count > 1)
            {
                pagina += 1;
                dicionario.Add(pagina, lista);
            }

            return dicionario;
        }
    }
}
