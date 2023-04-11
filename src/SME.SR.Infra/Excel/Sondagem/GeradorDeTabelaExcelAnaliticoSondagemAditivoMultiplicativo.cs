using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SME.SR.Infra
{
    public class GeradorDeTabelaExcelAnaliticoSondagemAditivoMultiplicativo : GeradorDeTabelaExcelAnaliticoSondagem
    {
        private List<(int, string)> perguntaPorOrdem;

        public GeradorDeTabelaExcelAnaliticoSondagemAditivoMultiplicativo(IEnumerable<RelatorioSondagemAnaliticoPorDreDto> relatorioSondagemAnaliticoPorDreDtos) : base(relatorioSondagemAnaliticoPorDreDtos)
        {
        }

        protected override void CarregarLinhas(DataTable data, RelatorioSondagemAnaliticoPorDreDto sondagemAnalitica)
        {
            var matematica = (RelatorioSondagemAnaliticoCampoAditivoMultiplicativoDto)sondagemAnalitica;

            foreach (var resposta in matematica.Respostas)
            {
                DataRow linha = data.NewRow();

                CarregaLinha(linha, resposta);

                foreach (var ordem in resposta.Ordens)
                {
                    CarregarLinhaResposta(ordem, linha, $"{ordem.Ordem}_{ordem.Descricao}");
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
            foreach (var pergunta in perguntaPorOrdem)
            {
                CarregarLinhaTituloResposta(linha, $"{pergunta.Item1}_{pergunta.Item2}_Ideia");
                CarregarLinhaTituloResposta(linha, $"{pergunta.Item1}_{pergunta.Item2}_Resultado");
            }
        }

        protected override DataColumn[] ObterColunas()
        {
            var colunas = new List<DataColumn>();

            perguntaPorOrdem = ObterPerguntas();

            foreach (var pergunta in perguntaPorOrdem)
            {
                colunas.AddRange(ObterColunasResposta($"{pergunta.Item1}_{pergunta.Item2}_Ideia"));
                colunas.AddRange(ObterColunasResposta($"{pergunta.Item1}_{pergunta.Item2}_Resultado"));
            }

            return colunas.ToArray();
        }

        private void CarregarLinhaTituloResposta(DataRow linha, string coluna)
        {
            linha[$"{coluna}_1"] = "Acertou";
            linha[$"{coluna}_2"] = "Errou";
            linha[$"{coluna}_3"] = "Não resolveu";
            linha[$"{coluna}_4"] = "Sem preenchimento";
        }

        private void CarregarLinhaTituloOrdem(DataTable data)
        {
            DataRow linhaTitulo = data.NewRow();

            foreach (var pergunta in perguntaPorOrdem)
            {
                linhaTitulo[$"{pergunta.Item1}_{pergunta.Item2}_Ideia_1"] = $"ORDEM {pergunta.Item1} - {pergunta.Item2.ToUpper()}";
            }

            data.Rows.Add(linhaTitulo);
        }

        private void CarregarLinhaTituloIdeiaResultado(DataTable data)
        {
            DataRow linhaTitulo = data.NewRow();

            foreach (var pergunta in perguntaPorOrdem)
            {
                linhaTitulo[$"{pergunta.Item1}_{pergunta.Item2}_Ideia_1"] = "Ideia";
                linhaTitulo[$"{pergunta.Item1}_{pergunta.Item2}_Resultado_1"] = "Resultado";
            }

            data.Rows.Add(linhaTitulo);
        }

        private List<DataColumn> ObterColunasResposta(string coluna)
        {
            return new List<DataColumn>()
            {
                new DataColumn($"{coluna}_1"),
                new DataColumn($"{coluna}_2"),
                new DataColumn($"{coluna}_3"),
                new DataColumn($"{coluna}_4"),
            };
        }

        private void CarregarLinhaResposta(RespostaOrdemMatematicaDto dto, DataRow linha, string coluna)
        {
            CarregarLinhaResposta(dto.Ideia, linha, $"{coluna}_Ideia");
            CarregarLinhaResposta(dto.Resultado, linha, $"{coluna}_Resultado");
        }

        private void CarregarLinhaResposta(RespostaMatematicaDto dto, DataRow linha, string coluna)
        {
            linha[$"{coluna}_1"] = dto.Acertou;
            linha[$"{coluna}_2"] = dto.Errou;
            linha[$"{coluna}_3"] = dto.NaoResolveu;
            linha[$"{coluna}_4"] = dto.SemPreenchimento;
        }

        private List<(int, string)> ObterPerguntas()
        {
            var perguntas = new List<(int, string)>();
            var relatorio = relatorioSondagemAnaliticoPorDreDtos.Cast<RelatorioSondagemAnaliticoCampoAditivoMultiplicativoDto>();
            var perguntasAno = relatorio.SelectMany(dre => dre.Respostas.GroupBy(a => a.Ano).SelectMany(ano => ano.FirstOrDefault().Ordens));

            foreach (var pergunta in perguntasAno)
            {
                if (!perguntas.Exists(p => p.Item1 == pergunta.Ordem && p.Item2 == pergunta.Descricao))
                    perguntas.Add((pergunta.Ordem, pergunta.Descricao));
            }

            return perguntas.OrderBy(p => p.Item1).ToList();
        }
    }
}
