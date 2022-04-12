using System.Collections.Generic;
using System.Linq;

namespace SME.SR.Infra
{
    public class RelatorioPaginadoGrafico : IRelatorioPaginado
    {
        private ParametroRelatorioPaginadoGrafico _parametro;

        public RelatorioPaginadoGrafico(ParametroRelatorioPaginadoGrafico parametro)
        {
            this._parametro = parametro;
        }

        public List<Pagina> Paginas()
        {
            var lista = new List<Pagina>();
            var pagina = 1;
            var contadorLinha = 1;
            var listaDeGraficoPagina = new List<List<GraficoBarrasVerticalDto>>();
            var agrupadorPorLinha = ObtenhaAgrupadorPorLinha();

            foreach (var items in agrupadorPorLinha)
            {
                listaDeGraficoPagina.Add(items.ToList());

                if (contadorLinha >= this._parametro.TotalDeLinhas)
                {
                    lista.Add(new PaginaComGrafico()
                    {
                        Graficos = listaDeGraficoPagina,
                        Indice = pagina
                    });
                    pagina++;
                    contadorLinha = 0;
                    listaDeGraficoPagina = new List<List<GraficoBarrasVerticalDto>>();
                }

                contadorLinha++;
            }

            if (listaDeGraficoPagina.Count > 0)
            {
                lista.Add(new PaginaComGrafico()
                {
                    Graficos = listaDeGraficoPagina,
                    Indice = pagina
                });
            }

            return lista;
        }

        private IEnumerable<IGrouping<int, GraficoBarrasVerticalDto>> ObtenhaAgrupadorPorLinha()
        {
           return this._parametro.Graficos.Select((grafico, indice) => new { grafico, indice = indice / this._parametro.TotalDeGraficosPorLinha })
                                    .GroupBy(grafico => grafico.indice, grafico => grafico.grafico);
        }
    }
}
