using System.Collections.Generic;
using System.Data;

namespace SME.SR.Infra
{
    public class GeradorDeTabelaExcelAnaliticoSondagemCapacidadeDeLeitura : GeradorDeTabelaExcelAnaliticoSondagem
    {
        private const int COLUNA_ORDEM_NARRAR = 4;
        private const int COLUNA_ORDEM_RELATAR = 16;
        private const int COLUNA_ORDEM_ARGUMENTAR = 28;
        private const string ADEQUADA = "Adequada";
        private const string INADEQUADA = "Inadequada";
        private const string NAO_RESOLVEU = "NaoResolveu";
        private const string SEM_PREENCHIMENTO = "Sem preenchimento";

        public GeradorDeTabelaExcelAnaliticoSondagemCapacidadeDeLeitura(IEnumerable<RelatorioSondagemAnaliticoPorDreDto> relatorioSondagemAnaliticoPorDreDtos) : base(relatorioSondagemAnaliticoPorDreDtos)
        {
        }

        protected override void CarregarLinhas(DataTable data, RelatorioSondagemAnaliticoPorDreDto sondagemAnalitica)
        {
            var leitura = (RelatorioSondagemAnaliticoCapacidadeDeLeituraDto)sondagemAnalitica;

            foreach (var resposta in leitura.Respostas)
            {
                DataRow linha = data.NewRow();

                CarregaLinha(linha, resposta);
                CarregarLinhaResposta(resposta.OrdemDoNarrar, linha, COLUNA_ORDEM_NARRAR);
                CarregarLinhaResposta(resposta.OrdemDoRelatar, linha, COLUNA_ORDEM_RELATAR);
                CarregarLinhaResposta(resposta.OrdemDoArgumentar, linha, COLUNA_ORDEM_ARGUMENTAR);

                data.Rows.Add(linha);
            }
        }

        private void CarregarLinhaResposta(RespostaCapacidadeDeLeituraDto dto, DataRow linha, int coluna)
        {
            CarregarLinhaResposta(dto.Localizacao, linha, coluna);
            CarregarLinhaResposta(dto.Inferencia, linha, coluna + 4);
            CarregarLinhaResposta(dto.Reflexao, linha, coluna + 8);
        }

        private void CarregarLinhaResposta(ItemRespostaCapacidadeDeLeituraDto dto, DataRow linha, int coluna)
        {
            linha[ObterAdequada(coluna)] = dto.Adequada;
            linha[ObterInadequada(coluna)] = dto.Inadequada;
            linha[ObterNaoResolveu(coluna)] = dto.NaoResolveu;
            linha[ObterSemPreenchimento(coluna)] = dto.SemPreenchimento;
        }

        protected override void CarregarTitulo(DataTable data)
        {
            CarregarLinhaTituloOrdem(data);
            CarregarLinhaTituloSubOrdem(data);
            base.CarregarTitulo(data);
        }

        protected override void CarregarLinhaTitulo(DataRow linha)
        {
            CarregarLinhaTituloSubResposta(linha, COLUNA_ORDEM_NARRAR);
            CarregarLinhaTituloSubResposta(linha, COLUNA_ORDEM_RELATAR);
            CarregarLinhaTituloSubResposta(linha, COLUNA_ORDEM_ARGUMENTAR);
        }

        protected override DataColumn[] ObterColunas()
        {
            var colunas = new List<DataColumn>();

            colunas.AddRange(ObterColunasPergunta(COLUNA_ORDEM_NARRAR));
            colunas.AddRange(ObterColunasPergunta(COLUNA_ORDEM_RELATAR));
            colunas.AddRange(ObterColunasPergunta(COLUNA_ORDEM_ARGUMENTAR));

            return colunas.ToArray();
        }

        private void CarregarLinhaTituloOrdem(DataTable data)
        {
            DataRow linhaTitulo = data.NewRow();

            linhaTitulo[COLUNA_ORDEM_NARRAR] = "Ordem do narrar";
            linhaTitulo[COLUNA_ORDEM_RELATAR] = "Ordem do relatar";
            linhaTitulo[COLUNA_ORDEM_ARGUMENTAR] = "Ordem do argumentar";

            data.Rows.Add(linhaTitulo);
        }

        private void CarregarLinhaTituloSubOrdem(DataTable data)
        {
            DataRow linha = data.NewRow();

            CarregarLinhaTituloSubOrdem(linha, COLUNA_ORDEM_NARRAR);
            CarregarLinhaTituloSubOrdem(linha, COLUNA_ORDEM_RELATAR);
            CarregarLinhaTituloSubOrdem(linha, COLUNA_ORDEM_ARGUMENTAR);

            data.Rows.Add(linha);
        }

        private void CarregarLinhaTituloSubOrdem(DataRow linha, int coluna)
        {
            linha[coluna] = "Localização";
            linha[coluna + 4] = "Inferência";
            linha[coluna + 8] = "Reflexão";
        }

        private void CarregarLinhaTituloSubResposta(DataRow linha, int coluna)
        {
            CarregarLinhaTituloResposta(linha, coluna);
            CarregarLinhaTituloResposta(linha, coluna + 4);
            CarregarLinhaTituloResposta(linha, coluna + 8);
        }

        private void CarregarLinhaTituloResposta(DataRow linha, int coluna)
        {
            linha[ObterAdequada(coluna)] = "Adequada";
            linha[ObterInadequada(coluna)] = "Inadequada";
            linha[ObterNaoResolveu(coluna)] = "Não resolveu";
            linha[ObterSemPreenchimento(coluna)] = "Sem preenchimento";
        }

        private List<DataColumn> ObterColunasPergunta(int coluna)
        {
            var colunas = new List<DataColumn>();

            colunas.AddRange(ObterColunasSubPergunta(coluna));
            colunas.AddRange(ObterColunasSubPergunta(coluna + 4));
            colunas.AddRange(ObterColunasSubPergunta(coluna + 8));

            return colunas;
        }

        private List<DataColumn> ObterColunasSubPergunta(int coluna)
        {
            return new List<DataColumn>()
            {
                new DataColumn(ObterAdequada(coluna)),
                new DataColumn(ObterInadequada(coluna)),
                new DataColumn(ObterNaoResolveu(coluna)),
                new DataColumn(ObterSemPreenchimento(coluna))
            };
        }

        private string ObterAdequada(int coluna)
            => $"{coluna}_{ADEQUADA}";

        private string ObterInadequada(int coluna)
            => $"{coluna}_{INADEQUADA}";

        private string ObterNaoResolveu(int coluna)
            => $"{coluna}_{NAO_RESOLVEU}";

        private string ObterSemPreenchimento(int coluna)
            => $"{coluna}_{SEM_PREENCHIMENTO}";
    }
}
