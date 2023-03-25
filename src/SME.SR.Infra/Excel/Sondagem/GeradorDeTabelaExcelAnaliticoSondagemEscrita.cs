using System.Collections.Generic;
using System.Data;

namespace SME.SR.Infra
{
    public class GeradorDeTabelaExcelAnaliticoSondagemEscrita : GeradorDeTabelaExcelAnaliticoSondagem
    {
        private const string PRE_SILABICO = "PreSilabico";
        private const string SILABICO_SEM_VALOR = "SilabicoSemValor";
        private const string SILABICO_COM_VALOR = "SilabicoComValor";
        private const string SILABICO_ALFABETICO = "SilabicoAlfabetico";
        private const string ALFABETICO = "Alfabetico";
        private const string NIVEL1 = "Nivel1";
        private const string NIVEL2 = "Nivel2";
        private const string NIVEL3 = "Nivel3";
        private const string NIVEL4 = "Nivel4";
        private const string SEM_PREENCHIMENTO = "SemPreenchimento";

        public GeradorDeTabelaExcelAnaliticoSondagemEscrita(IEnumerable<RelatorioSondagemAnaliticoPorDreDto> relatorioSondagemAnaliticoPorDreDtos) : base(relatorioSondagemAnaliticoPorDreDtos)
        {
        }

        protected override void CarregarLinhas(DataTable data, RelatorioSondagemAnaliticoPorDreDto sondagemAnalitica)
        {
            var escrita = (RelatorioSondagemAnaliticoEscritaDto)sondagemAnalitica;

            foreach (var resposta in escrita.Respostas)
            {
                DataRow linha = data.NewRow();

                CarregaLinha(linha, resposta);

                linha[PRE_SILABICO] = resposta.PreSilabico;
                linha[SILABICO_SEM_VALOR] = resposta.SilabicoSemValor;
                linha[SILABICO_COM_VALOR] = resposta.SilabicoComValor;
                linha[SILABICO_ALFABETICO] = resposta.SilabicoAlfabetico;
                linha[ALFABETICO] = resposta.Alfabetico;
                linha[NIVEL1] = resposta.Nivel1;
                linha[NIVEL2] = resposta.Nivel2;
                linha[NIVEL3] = resposta.Nivel3;
                linha[NIVEL4] = resposta.Nivel4;
                linha[SEM_PREENCHIMENTO] = resposta.SemPreenchimento;

                data.Rows.Add(linha);
            }
        }

        protected override void CarregarLinhaTitulo(DataRow linha)
        {
            linha[PRE_SILABICO] = "PS";
            linha[SILABICO_SEM_VALOR] = "SSV";
            linha[SILABICO_COM_VALOR] = "SCV";
            linha[SILABICO_ALFABETICO] = "SA";
            linha[ALFABETICO] = "A";
            linha[NIVEL1] = "Nivel1";
            linha[NIVEL2] = "Nivel2";
            linha[NIVEL3] = "Nivel3";
            linha[NIVEL4] = "Nivel4";
            linha[SEM_PREENCHIMENTO] = "Sem preenchimento";
        }

        protected override DataColumn[] ObterColunas()
        {
            return new DataColumn[]
            {
                new DataColumn(PRE_SILABICO),
                new DataColumn(SILABICO_SEM_VALOR),
                new DataColumn(SILABICO_COM_VALOR),
                new DataColumn(SILABICO_ALFABETICO),
                new DataColumn(ALFABETICO),
                new DataColumn(NIVEL1),
                new DataColumn(NIVEL2),
                new DataColumn(NIVEL3),
                new DataColumn(NIVEL4),
                new DataColumn(SEM_PREENCHIMENTO)
            };
        }
    }
}
