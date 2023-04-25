using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace SME.SR.Infra
{
    public abstract class RelatorioPaginado<T> : IRelatorioPaginado where T : class
    {
        protected List<T> ListaDeValores { get; set; }

        protected List<Pagina> ListaDePaginas { get; private set; }

        protected List<Func<T, object>> ListaDeAgrupamento { get; set; }

        protected int TotalDeAgrupamentoOculto { get; set; }

        public List<Pagina> Paginas()
        {
            ListaDePaginas = new List<Pagina>();

            this.Construir();

            return this.ListaDePaginas.OrderBy(pag => pag.Ordenacao).ToList();
        }

        public void AdicioneAgrupamento(Func<T, object> funcao)
        {
            this.ListaDeAgrupamento.Add(funcao);
        }

        protected abstract void Construir();

        protected abstract int ObtenhaQuantidadeDeLinha();

        protected abstract bool RelatorioPorQuantidadeDeLinha();

        protected abstract Dictionary<int, List<T>> ObtenhaDicionarioPorQuantidadeSemAgrupamentoCalculado(List<IColuna> listaDeColunas);

        protected int ObtenhaIndicePaginaAtual()
        {
            return this.ListaDePaginas.Any() ? this.ListaDePaginas.Max(x => x.Indice) : 0;
        }

        protected void AdicionePagina(Pagina pagina)
        {
            this.ListaDePaginas.Add(pagina);
        }

        protected Dictionary<int, List<T>> ObtenhaDicionarioPorQuantidade()
        {
            return ObtenhaDicionarioPorQuantidade(null);
        }

        protected Dictionary<int, List<T>> ObtenhaDicionarioPorQuantidade(List<IColuna> listaDeColunas)
        {
            if (ListaDeAgrupamento.Any())
            {
                return ObtenhaDicionarioPorQuantidadePorAgrupamento();
            }

            return ObtenhaDicionarioPorQuantidadeSemAgrupamento(listaDeColunas);
        }

        protected Dictionary<int, List<T>> ObtenhaDicionarioPorQuantidadeSemAgrupamento(List<IColuna> listaDeColunas)
        {
            if (RelatorioPorQuantidadeDeLinha())
            {
                return ObtenhaDicionarioPorQuantidadeSemAgrupamentoPorLinha();
            }

            return ObtenhaDicionarioPorQuantidadeSemAgrupamentoCalculado(listaDeColunas);
        }

        protected Dictionary<int, List<T>> ObtenhaDicionarioPorQuantidadeSemAgrupamentoPorLinha()
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

        protected Dictionary<int, List<T>> ObtenhaDicionarioPorQuantidadePorAgrupamento()
        {
            Dictionary<int, List<T>> dicionarioValores = new Dictionary<int, List<T>>();
            var totalDeLinhas = ObtenhaQuantidadeDeLinha();
            var totalDeRegistro = 0;
            var pagina = 1;
            var indiceAgrupamento = 0;

            dicionarioValores.Add(pagina, new List<T>());

            var listaDeAgrupamento = ObtenhaListaPorAgrupamento(this.ListaDeValores.GroupBy(ListaDeAgrupamento[indiceAgrupamento]), 1, 1);

            foreach(var lista in listaDeAgrupamento)
            {
                totalDeRegistro += lista.Item2;

                foreach (var valor in lista.Item1)
                {
                    totalDeRegistro++;
                    dicionarioValores[pagina].Add(valor);

                    if (totalDeRegistro >= totalDeLinhas)
                    {
                        pagina++;
                        totalDeRegistro = ListaDeAgrupamento.Count - TotalDeAgrupamentoOculto;
                        dicionarioValores.Add(pagina, new List<T>());
                    }
                }
            }

            return dicionarioValores;
        }

        private List<(List<T>, int)> ObtenhaListaPorAgrupamento(IEnumerable<IGrouping<object, T>> grupo, int indiceAgrupamento, int totalLinhaAgrupada)
        {
            var totalLinha = 0;
            var ListaRetorno = new List<(List<T>, int)>();
            var indiceAtual = indiceAgrupamento;

            totalLinha += totalLinhaAgrupada;

            foreach (var itemGrupo in grupo)
            {
                var funcao = ListaDeAgrupamento.Count > indiceAgrupamento ? ListaDeAgrupamento[indiceAtual] : null;

                totalLinha++;

                if (funcao != null)
                {
                    var listaGrupo = ObtenhaListaPorAgrupamento(itemGrupo.ToList().GroupBy(funcao), indiceAtual + 1, totalLinha);

                    ListaRetorno.AddRange(listaGrupo);
                } else
                {
                    ListaRetorno.Add((itemGrupo.ToList(), totalLinha));
                }

                totalLinha = 1;
            }

            return ListaRetorno;
        }
    }
}
