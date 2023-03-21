using System.Collections.Generic;
using System.Data;

namespace SME.SR.Infra
{
    public class GeradorDeTabelaExcelAnaliticoSondagemLeituraDeVozAlta : GeradorDeTabelaExcelAnaliticoSondagem
    {
        private const string NAO_CONSEGUIU_NAO_QUIS_LER = "NaoConseguiuOuNaoQuisLer";
        private const string LEU_COM_MUITA_DIFICULDADE = "LeuComMuitaDificuldade";
        private const string LEU_COM_ALGUMA_FLUENCIA = "LeuComAlgumaFluencia";
        private const string LEU_COM_FLUENCIA = "LeuComFluencia";
        private const string SEM_PREENCHIMENTO = "SemPreenchimento";
        public GeradorDeTabelaExcelAnaliticoSondagemLeituraDeVozAlta(IEnumerable<RelatorioSondagemAnaliticoPorDreDto> relatorioSondagemAnaliticoPorDreDtos) : base(relatorioSondagemAnaliticoPorDreDtos)
        {
        }

        protected override void CarregarLinhas(DataTable data, RelatorioSondagemAnaliticoPorDreDto sondagemAnalitica)
        {
            var vozAlta = (RelatorioSondagemAnaliticoLeituraDeVozAltaDto)sondagemAnalitica;

            foreach (var resposta in vozAlta.Respostas)
            {
                DataRow linha = data.NewRow();

                CarregaLinha(linha, resposta);

                linha[NAO_CONSEGUIU_NAO_QUIS_LER] = resposta.NaoConseguiuOuNaoQuisLer;
                linha[LEU_COM_MUITA_DIFICULDADE] = resposta.LeuComMuitaDificuldade;
                linha[LEU_COM_ALGUMA_FLUENCIA] = resposta.LeuComAlgumaFluência;
                linha[LEU_COM_FLUENCIA] = resposta.LeuComFluencia;
                linha[SEM_PREENCHIMENTO] = resposta.SemPreenchimento;

                data.Rows.Add(linha);
            }
        }

        protected override void CarregarLinhaTitulo(DataRow linha)
        {
            linha[NAO_CONSEGUIU_NAO_QUIS_LER] = "Não conseguiu ou não quis ler";
            linha[LEU_COM_MUITA_DIFICULDADE] = "Leu com muita dificuldade";
            linha[LEU_COM_ALGUMA_FLUENCIA] = "Leu com alguma fluência";
            linha[LEU_COM_FLUENCIA] = "Leu com fluência";
            linha[SEM_PREENCHIMENTO] = "Sem preenchimento";
        }

        protected override DataColumn[] ObterColunas()
        {
            return new DataColumn[]
{
                new DataColumn(NAO_CONSEGUIU_NAO_QUIS_LER),
                new DataColumn(LEU_COM_MUITA_DIFICULDADE),
                new DataColumn(LEU_COM_ALGUMA_FLUENCIA),
                new DataColumn(LEU_COM_FLUENCIA),
                new DataColumn(SEM_PREENCHIMENTO)
            };
        }
    }
}
