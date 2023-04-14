using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SME.SR.Infra
{
    public class GeradorDeTabelaExcelAnaliticoSondagemProducaoDeTexto : GeradorDeTabelaExcelAnaliticoSondagem
    {
        private const string NAO_PRODUZIU_ENTREGOU_BRANCO = "NaoProduziuEntregouEmBranco";
        private const string NAO_APRESENTOU_DIFICULDADES = "NaoApresentouDificuldades";
        private const string ESCRITA_NAO_ALFABETICA = "EscritaNaoAlfabetica";
        private const string DIFICULDADE_ASPECTOS_SEMANTICOS = "DificuldadesComAspectosSemantico";
        private const string DIFICULDADE_ASPECTOS_TEXTUAIS = "DificuldadesComAspectosTextuais";
        private const string DIFICULDADE_ASPECTOS_ORTOGRAFICOS = "DificuldadesComAspectosOrtograficos";
        private const string SEM_PREENCHIMENTO = "SemPreenchimento";

        public GeradorDeTabelaExcelAnaliticoSondagemProducaoDeTexto(IEnumerable<RelatorioSondagemAnaliticoPorDreDto> relatorioSondagemAnaliticoPorDreDtos) : base(relatorioSondagemAnaliticoPorDreDtos)
        {
        }

        protected override void CarregarLinhas(DataTable data, RelatorioSondagemAnaliticoPorDreDto sondagemAnalitica)
        {
            var producaoDeTextoDto = (RelatorioSondagemAnaliticoProducaoDeTextoDto)sondagemAnalitica;

            foreach (var resposta in producaoDeTextoDto.Respostas.OrderBy(x => x.Ue))
            {
                DataRow linha = data.NewRow();

                CarregaLinha(linha, resposta);

                linha[NAO_PRODUZIU_ENTREGOU_BRANCO] = resposta.NaoProduziuEntregouEmBranco;
                linha[NAO_APRESENTOU_DIFICULDADES] = resposta.NaoApresentouDificuldades;
                linha[ESCRITA_NAO_ALFABETICA] = resposta.EscritaNaoAlfabetica;
                linha[DIFICULDADE_ASPECTOS_SEMANTICOS] = resposta.DificuldadesComAspectosSemanticos;
                linha[DIFICULDADE_ASPECTOS_TEXTUAIS] = resposta.DificuldadesComAspectosTextuais;
                linha[DIFICULDADE_ASPECTOS_ORTOGRAFICOS] = resposta.DificuldadesComAspectosOrtograficosNotacionais;
                linha[SEM_PREENCHIMENTO] = resposta.SemPreenchimento;

                data.Rows.Add(linha);
            }
        }

        protected override void CarregarLinhaTitulo(DataRow linha)
        {
            linha[NAO_PRODUZIU_ENTREGOU_BRANCO] = "Não produziu/entregou em branco";
            linha[NAO_APRESENTOU_DIFICULDADES] = "Não apresentou dificuldades";
            linha[ESCRITA_NAO_ALFABETICA] = "Escrita não alfabética";
            linha[DIFICULDADE_ASPECTOS_SEMANTICOS] = "Dificuldades com aspectos semânticos";
            linha[DIFICULDADE_ASPECTOS_TEXTUAIS] = "Dificuldades com aspectos textuais";
            linha[DIFICULDADE_ASPECTOS_ORTOGRAFICOS] = "Dificuldades com aspectos ortográficos e notacionais";
            linha[SEM_PREENCHIMENTO] = "Sem preenchimento";
        }

        protected override DataColumn[] ObterColunas()
        {
            return new DataColumn[]
            {
                new DataColumn(NAO_PRODUZIU_ENTREGOU_BRANCO),
                new DataColumn(NAO_APRESENTOU_DIFICULDADES),
                new DataColumn(ESCRITA_NAO_ALFABETICA),
                new DataColumn(DIFICULDADE_ASPECTOS_SEMANTICOS),
                new DataColumn(DIFICULDADE_ASPECTOS_TEXTUAIS),
                new DataColumn(DIFICULDADE_ASPECTOS_ORTOGRAFICOS),
                new DataColumn(SEM_PREENCHIMENTO)
            };
        }
    }
}
