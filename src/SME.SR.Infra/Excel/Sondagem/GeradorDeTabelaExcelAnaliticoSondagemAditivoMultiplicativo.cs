using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SME.SR.Infra
{
    public class GeradorDeTabelaExcelAnaliticoSondagemAditivoMultiplicativo : GeradorDeTabelaExcelAnaliticoSondagem
    {
        private const int COLUNA_ORDEM = 4;

        public GeradorDeTabelaExcelAnaliticoSondagemAditivoMultiplicativo(IEnumerable<RelatorioSondagemAnaliticoPorDreDto> relatorioSondagemAnaliticoPorDreDtos) : base(relatorioSondagemAnaliticoPorDreDtos)
        {
        }

        protected override void CarregarLinhas(DataTable data, RelatorioSondagemAnaliticoPorDreDto sondagemAnalitica)
        {
            var matematica = (RelatorioSondagemAnaliticoCampoAditivoMultiplicativoDto)sondagemAnalitica;
            var totalColuna = COLUNA_ORDEM;

            foreach (var resposta in matematica.Respostas)
            {
                DataRow linha = data.NewRow();

                CarregaLinha(linha, resposta);

                foreach (var ordem in resposta.Ordens)
                {
                    CarregarLinhaResposta(ordem, linha, totalColuna);
                    totalColuna += 8;
                }

                data.Rows.Add(linha);
            }
        }

        protected override void CarregarTitulo(DataTable data)
        {
            CarregarLinhaTituloOrdem(data);
            CarregarLinhaTituloIdeiaResultado(data);
            base.CarregarTitulo(data);
        }

        protected override void CarregarLinhaTitulo(DataRow linha)
        {
            var totalColuna = COLUNA_ORDEM;
            var matematica = (RelatorioSondagemAnaliticoCampoAditivoMultiplicativoDto)relatorioSondagemAnaliticoPorDreDtos.FirstOrDefault();

            foreach (var resposta in matematica.Respostas)
            {
                foreach (var ordem in resposta.Ordens)
                {
                    CarregarLinhaTituloResposta(linha, totalColuna);
                    totalColuna += COLUNA_ORDEM;
                    CarregarLinhaTituloResposta(linha, totalColuna);
                    totalColuna += COLUNA_ORDEM;
                }
            }
        }

        private void CarregarLinhaTituloResposta(DataRow linha, int coluna)
        {
            linha[coluna] = "Acertou";
            linha[coluna + 1] = "Errou";
            linha[coluna + 2] = "Não resolveu";
            linha[coluna + 3] = "Sem preenchimento";
        }

        private void CarregarLinhaTituloOrdem(DataTable data)
        {
            var totalColuna = COLUNA_ORDEM;
            DataRow linhaTitulo = data.NewRow();

            var matematica = (RelatorioSondagemAnaliticoCampoAditivoMultiplicativoDto)relatorioSondagemAnaliticoPorDreDtos.FirstOrDefault();

            foreach (var resposta in matematica.Respostas)
            {
                foreach(var ordem in resposta.Ordens)
                {
                    linhaTitulo[totalColuna] = $"ORDEM {ordem.Ordem} - {ordem.Descricao.ToUpper()}";
                    totalColuna += 8;
                }
            }

            data.Rows.Add(linhaTitulo);
        }

        private void CarregarLinhaTituloIdeiaResultado(DataTable data)
        {
            var totalColuna = COLUNA_ORDEM;
            DataRow linhaTitulo = data.NewRow();

            var matematica = (RelatorioSondagemAnaliticoCampoAditivoMultiplicativoDto)relatorioSondagemAnaliticoPorDreDtos.FirstOrDefault();

            foreach (var resposta in matematica.Respostas)
            {
                foreach (var ordem in resposta.Ordens)
                {
                    linhaTitulo[totalColuna] = "Ideia";
                    totalColuna += COLUNA_ORDEM;
                    linhaTitulo[totalColuna] = "Resultado";
                    totalColuna += COLUNA_ORDEM;
                }
            }

            data.Rows.Add(linhaTitulo);
        }

        protected override DataColumn[] ObterColunas()
        {
            var colunas = new List<DataColumn>();
            var totalColuna = COLUNA_ORDEM;
            var matematica = (RelatorioSondagemAnaliticoCampoAditivoMultiplicativoDto)relatorioSondagemAnaliticoPorDreDtos.FirstOrDefault();

            foreach (var resposta in matematica.Respostas)
            {
                foreach (var ordem in resposta.Ordens)
                {
                    colunas.AddRange(ObterColunasResposta(totalColuna));
                    totalColuna += COLUNA_ORDEM;
                    colunas.AddRange(ObterColunasResposta(totalColuna));
                    totalColuna += COLUNA_ORDEM;
                }
            }

            return colunas.ToArray();   
        }

        private List<DataColumn> ObterColunasResposta(int coluna)
        {
            return new List<DataColumn>()
            {
                new DataColumn(coluna.ToString()),
                new DataColumn((coluna + 1).ToString()),
                new DataColumn((coluna + 2).ToString()),
                new DataColumn((coluna + 3).ToString()),
            };
        }

        private void CarregarLinhaResposta(RespostaOrdemMatematicaDto dto, DataRow linha, int coluna)
        {
            CarregarLinhaResposta(dto.Ideia, linha, coluna);
            CarregarLinhaResposta(dto.Resultado, linha, coluna + 4);
        }

        private void CarregarLinhaResposta(RespostaMatematicaDto dto, DataRow linha, int coluna)
        {
            linha[coluna] = dto.Acertou;
            linha[coluna + 1] = dto.Errou;
            linha[coluna + 2] = dto.NaoResolveu;
            linha[coluna + 3] = dto.SemPreenchimento;
        }
    }
}
