using SME.SR.Infra.RelatorioPaginado.Interfaces;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SME.SR.Infra.RelatorioPaginado
{
    public abstract class RelatorioPaginado<T> : IRelatorioPaginado where T : class
    {
        protected List<T> ListaDeValores {  get; set; }

        protected List<Pagina> ListaDePaginas { get; private set; }

        public List<Pagina> Paginas()
        {
            ListaDePaginas = new List<Pagina>();

            this.Construir();

            return this.ListaDePaginas.OrderBy(pag => pag.Ordenacao).ToList();
        }

        protected abstract void Construir();

        protected abstract int ObtenhaQuantidadeDeLinha();

        protected int ObtenhaIndicePaginaAtual()
        {
            return this.ListaDePaginas.Any() ? this.ListaDePaginas.Max(x => x.Indice) : 0;
        }

        protected void AdicionePagina(Pagina pagina)
        {
            this.ListaDePaginas.Add(pagina);
        }

        protected Pagina ObtenhaPagina(int indice, int ordenacao, List<T> valores)
        {
            return ObtenhaPagina(indice, ordenacao, valores, null);
        }

        protected Pagina ObtenhaPagina(int indice, int ordenacao, List<T> valores, List<IColuna> colunas)
        {
            return new Pagina()
            {
                Indice = indice,
                Ordenacao = ordenacao,
                Valores = valores,
                Colunas = colunas
            };
        }

        protected Dictionary<int, List<T>> ObtenhaDicionarioPorQuantidade()
        {
            Dictionary<int, List<T>> dicionarioValores = new Dictionary<int, List<T>>();
            var totalDeLinhas = ObtenhaQuantidadeDeLinha();
            var totalDeRegistro = 0;
            var pagina = 1;

            dicionarioValores.Add(pagina, new List<T>());

            foreach (var valor in this.ListaDeValores)
            {
                totalDeRegistro++;
                dicionarioValores[pagina].Add(valor);

                if (totalDeRegistro >= totalDeLinhas)
                {
                    pagina++;
                    totalDeRegistro = 0;
                    dicionarioValores.Add(pagina, new List<T>());
                }
            }

            return dicionarioValores;
        }
    }
}
