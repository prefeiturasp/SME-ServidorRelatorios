using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioPaginadoFrequenciaMensal : RelatorioPaginadoColuna<FrequenciaMensalDto>
    {
        private FrequenciaMensalCabecalhoDto cabecalho;
        public RelatorioPaginadoFrequenciaMensal(
                            ParametroRelatorioPaginadoPorColuna<FrequenciaMensalDto> parametro, 
                            List<IColuna> colunas,
                            FrequenciaMensalCabecalhoDto cabecalho) : base(parametro, colunas)
        {
            this.cabecalho = cabecalho;
        }

        protected override Pagina ObtenhaPagina(int indice, int ordenacao, List<FrequenciaMensalDto> valores, List<IColuna> colunas)
        {
            return new PaginaFrenquenciaMensal(this.cabecalho.NomeDre, this.cabecalho.NomeUe)
            {
                Indice = indice,
                Ordenacao = ordenacao,
                Valores = valores,
                Colunas = colunas
            };
        }
    }
}
