using System.Collections.Generic;
using System.Data;

namespace SME.SR.Infra
{
    public class GeradorDeTabelaExcelAnaliticoSondagemEscrita : GeradorDeTabelaExcelAnaliticoSondagem
    {
        private const string SILABICO_SEM_VALOR = "SilabicoSemValor";
        private const string SILABICO_COM_VALOR = "SilabicoComValor";
        private const string SILABICO_ALFABETICO = "SilabicoAlfabetico";
        private const string ALFABETICO = "Alfabetico";
        private const string SEM_PREENCHIMENTO = "SemPreenchimento";

        public GeradorDeTabelaExcelAnaliticoSondagemEscrita(IEnumerable<RelatorioSondagemAnaliticoPorDreDto> relatorioSondagemAnaliticoPorDreDtos) : base(relatorioSondagemAnaliticoPorDreDtos)
        {
        }

        protected override void CarregarLinhas(DataTable data, RelatorioSondagemAnaliticoPorDreDto sondagemAnalitica)
        {
            var vozAlta = (RelatorioSondagemAnaliticoEscritaDto)sondagemAnalitica;

            foreach (var resposta in vozAlta.Respostas)
            {
                DataRow linha = data.NewRow();

                CarregaLinha(linha, resposta);

                linha[SILABICO_SEM_VALOR] = resposta.SilabicoSemValor;
                linha[SILABICO_COM_VALOR] = resposta.SilabicoComValor;
                linha[SILABICO_ALFABETICO] = resposta.SilabicoAlfabetico;
                linha[ALFABETICO] = resposta.Alfabetico;
                linha[SEM_PREENCHIMENTO] = resposta.SemPreenchimento;

                data.Rows.Add(linha);
            }
        }

        protected override void CarregarLinhaTitulo(DataRow linha)
        {
            linha[SILABICO_SEM_VALOR] = "Silábico sem valor";
            linha[SILABICO_COM_VALOR] = "Silábico com valor";
            linha[SILABICO_ALFABETICO] = "Silábico alfabético";
            linha[ALFABETICO] = "Alfabético";
            linha[SEM_PREENCHIMENTO] = "Sem preenchimento";
        }

        protected override DataColumn[] ObterColunas()
        {
            return new DataColumn[]
            {
                new DataColumn(SILABICO_SEM_VALOR),
                new DataColumn(SILABICO_COM_VALOR),
                new DataColumn(SILABICO_ALFABETICO),
                new DataColumn(ALFABETICO),
                new DataColumn(SEM_PREENCHIMENTO)
            };
        }
    }
}
