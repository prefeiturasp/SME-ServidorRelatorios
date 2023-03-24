using System.Collections.Generic;
using System.Data;

namespace SME.SR.Infra.Excel.Sondagem
{
    public class GeradorDeTabelaExcelAnaliticoSondagemLeitura : GeradorDeTabelaExcelAnaliticoSondagem
    {
        private const string NIVEL1 = "Nivel1";
        private const string NIVEL2 = "Nivel2";
        private const string NIVEL3 = "Nivel3";
        private const string NIVEL4 = "Nivel4";
        private const string SEM_PREENCHIMENTO = "SemPreenchimento";

        public GeradorDeTabelaExcelAnaliticoSondagemLeitura(IEnumerable<RelatorioSondagemAnaliticoPorDreDto> relatorioSondagemAnaliticoPorDreDtos) : base(relatorioSondagemAnaliticoPorDreDtos)
        {
        }

        protected override void CarregarLinhas(DataTable data, RelatorioSondagemAnaliticoPorDreDto sondagemAnalitica)
        {
            var leitura = (RelatorioSondagemAnaliticoLeituraDto)sondagemAnalitica;

            foreach(var resposta in leitura.Respostas)
            {
                DataRow linha = data.NewRow();

                CarregaLinha(linha, resposta);
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
                new DataColumn(NIVEL1),
                new DataColumn(NIVEL2),
                new DataColumn(NIVEL3),
                new DataColumn(NIVEL4),
                new DataColumn(SEM_PREENCHIMENTO)
            };
        }
    }
}
