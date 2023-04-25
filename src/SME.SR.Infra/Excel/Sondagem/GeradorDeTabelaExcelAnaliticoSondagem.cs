using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SME.SR.Infra
{
    public abstract class GeradorDeTabelaExcelAnaliticoSondagem
    {
        protected const int TOTAL_COLUNA = 4;

        protected IEnumerable<RelatorioSondagemAnaliticoPorDreDto> relatorioSondagemAnaliticoPorDreDtos; 

        public GeradorDeTabelaExcelAnaliticoSondagem(IEnumerable<RelatorioSondagemAnaliticoPorDreDto> relatorioSondagemAnaliticoPorDreDtos)
        {
            this.relatorioSondagemAnaliticoPorDreDtos = relatorioSondagemAnaliticoPorDreDtos;
        }

        protected abstract DataColumn[] ObterColunas();
        protected abstract void CarregarLinhaTitulo(DataRow linha);
        protected abstract void CarregarLinhas(DataTable data, RelatorioSondagemAnaliticoPorDreDto sondagemAnalitica);

        public IEnumerable<RelatorioSondagemAnaliticoExcelDto> ObterTabelaExcel()
        {
            var relatoriosExcel = new List<RelatorioSondagemAnaliticoExcelDto>();

            foreach (var dto in this.relatorioSondagemAnaliticoPorDreDtos)
            {
                var dtoExcel = ObterExcelDto(dto);

                dtoExcel.TabelaDeDado.Columns.AddRange(ObterColunas());

                CarregarTitulo(dtoExcel.TabelaDeDado);

                CarregarLinhas(dtoExcel.TabelaDeDado, dto);

                relatoriosExcel.Add(dtoExcel);
            }

            return relatoriosExcel;
        }

        protected void CarregaLinha(DataRow linha, RelatorioSondagemAnaliticoDto dto)
        {
            linha["UnidadeEscolar"] = dto.Ue;
            linha["Ano"] = dto.Ano;
            linha["TotalDeTurmas"] = dto.TotalDeTurma;
            linha["TotalDeAlunos"] = dto.TotalDeAlunos;
        }

        protected virtual void CarregarTitulo(DataTable data)
        {
            DataRow linhaTitulo = data.NewRow();

            linhaTitulo["UnidadeEscolar"] = "Unidade Escolar";
            linhaTitulo["Ano"] = "Ano";
            linhaTitulo["TotalDeTurmas"] = "Total de turmas";
            linhaTitulo["TotalDeAlunos"] = "Total de alunos";

            CarregarLinhaTitulo(linhaTitulo);

            data.Rows.Add(linhaTitulo);
        }

        protected virtual RelatorioSondagemAnaliticoExcelDto ObterExcelDto(RelatorioSondagemAnaliticoPorDreDto dto)
        {
            return new RelatorioSondagemAnaliticoExcelDto()
            {
                Dre = dto.Dre,
                DreSigla = dto.DreSigla,
                AnoLetivo = dto.AnoLetivo,
                Periodo = dto.Periodo,
                DescricaoTipoSondagem = dto.DescricaoTipoSondagem,
                TabelaDeDado = ObterData()
            };
        }

        private DataTable ObterData() {
            var data = new DataTable();

            data.Columns.Add("UnidadeEscolar");
            data.Columns.Add("Ano");
            data.Columns.Add("TotalDeTurmas");
            data.Columns.Add("TotalDeAlunos");

            return data;
        } 
    }
}
