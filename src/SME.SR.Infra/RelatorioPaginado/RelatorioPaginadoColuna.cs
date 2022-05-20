using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioPaginadoColuna<T> : RelatorioPaginadoPorColuna<T> where T : class
    {
        private List<IColuna> _coluna;
        public RelatorioPaginadoColuna(ParametroRelatorioPaginadoPorColuna<T> parametro, List<IColuna> colunas): base(parametro)
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

        private Dictionary<int, List<IColuna>> ObtenhaColunasPorPagina()
        {
            if (AtingiuLimiteDeColunas())
            {
                var dicionario = new Dictionary<int, List<IColuna>>();
                dicionario.Add(1, this._coluna);

                return dicionario;
            }

            return ObtenhaColunasPaginada();
        }
        private bool AtingiuLimiteDeColunas()
        {
            return ObtenhaLarguraTotal() >= ObtenhaLarguraTotalColunas(this._coluna);
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

        private Dictionary<int, List<IColuna>> ObtenhaColunasPaginada()
        {
            var listaDeChave = this._coluna.FindAll(colunas => colunas.Chave);
            var listaColunaSemChave = this._coluna.FindAll(colunas => !colunas.Chave);
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

        private Pagina ObtenhaPagina(int indice, int ordenacao, List<T> valores, List<IColuna> colunas)
        {
            return new PaginaComColuna()
            {
                Indice = indice,
                Ordenacao = ordenacao,
                Valores = valores,
                Colunas = colunas
            };
        }
    }
}
