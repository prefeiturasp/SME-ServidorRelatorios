using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SME.SR.Infra
{
    public class GeradorDeTabelaExcelAnaliticoSondagemNumeroIAD : GeradorDeTabelaExcelAnaliticoSondagem
    {
        public GeradorDeTabelaExcelAnaliticoSondagemNumeroIAD(IEnumerable<RelatorioSondagemAnaliticoPorDreDto> relatorioSondagemAnaliticoPorDreDtos) : base(relatorioSondagemAnaliticoPorDreDtos)
        {
        }

        protected override void CarregarLinhas(DataTable data, RelatorioSondagemAnaliticoPorDreDto sondagemAnalitica)
        {
            var matematica = (RelatorioSondagemAnaliticoNumeroIadDto)sondagemAnalitica;

            foreach (var resposta in matematica.Respostas)
            {
                DataRow linha = data.NewRow();

                CarregaLinha(linha, resposta);

                foreach (var coluna in resposta.Respostas)
                {
                    linha[coluna.IdPerguntaResposta] = coluna.Valor;
                }

                data.Rows.Add(linha);
            }
        }

        protected override void CarregarLinhaTitulo(DataRow linha)
        {
            var relatorio = (RelatorioSondagemAnaliticoNumeroIadDto)relatorioSondagemAnaliticoPorDreDtos.FirstOrDefault();

            foreach (var cabelho in relatorio.ColunasDoCabecalho)
            {
                foreach (var subCabelho in cabelho.SubCabecalhos)
                {
                    linha[subCabelho.IdPerguntaResposta] = subCabelho.Descricao;
                }
            }
        }

        protected override void CarregarTitulo(DataTable data)
        {
            CarregarLinhaTituloPergunta(data);
            base.CarregarTitulo(data);
        }

        protected override DataColumn[] ObterColunas()
        {
            var colunas = new List<DataColumn>();
            var relatorio = (RelatorioSondagemAnaliticoNumeroIadDto)relatorioSondagemAnaliticoPorDreDtos.FirstOrDefault();

            foreach(var cabelho in relatorio.ColunasDoCabecalho)
            {
                foreach(var subCabelho in cabelho.SubCabecalhos)
                {
                    colunas.Add(new DataColumn(subCabelho.IdPerguntaResposta));
                }
            }

            return colunas.ToArray();
        }

        protected void CarregarLinhaTituloPergunta(DataTable data)
        {
            var relatorio = (RelatorioSondagemAnaliticoNumeroIadDto)relatorioSondagemAnaliticoPorDreDtos.FirstOrDefault();

            DataRow linhaTitulo = data.NewRow();

            foreach (var cabelho in relatorio.ColunasDoCabecalho)
            {
                var subCabelho = cabelho.SubCabecalhos.FirstOrDefault();
                linhaTitulo[subCabelho.IdPerguntaResposta] = cabelho.Descricao;
            }
                
            data.Rows.Add(linhaTitulo);
        }

        protected override RelatorioSondagemAnaliticoExcelDto ObterExcelDto(RelatorioSondagemAnaliticoPorDreDto dto)
        {
            var dtoExcel = base.ObterExcelDto(dto);

            dtoExcel.MergeColunas = ObterMergeColunas();

            return dtoExcel;
        }

        private List<MergeColunaDto> ObterMergeColunas()
        {
            var colunas = new List<MergeColunaDto>();
            var relatorio = (RelatorioSondagemAnaliticoNumeroIadDto)relatorioSondagemAnaliticoPorDreDtos.FirstOrDefault();
            var indiceColuna = TOTAL_COLUNA;

            foreach (var cabelho in relatorio.ColunasDoCabecalho)
            {
                var merge = new MergeColunaDto();
                merge.ColunaInicio = indiceColuna + 1;
                indiceColuna += cabelho.SubCabecalhos.Count();
                merge.ColunaFim = indiceColuna;

                colunas.Add(merge);
            }

            return colunas;
        }
    }
}
