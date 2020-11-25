using MediatR;
using Sentry;
using SME.SR.Infra;
using SME.SR.Infra.Utilitarios;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterRelatorioConselhoClasseAtaFinalExcelQueryHandler : IRequestHandler<ObterRelatorioConselhoClasseAtaFinalExcelQuery, IEnumerable<DataTable>>
    {
        public async Task<IEnumerable<DataTable>> Handle(ObterRelatorioConselhoClasseAtaFinalExcelQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var lstDatatables = new List<DataTable>();

                DataTable dt;

                var agrupamentoTurmas = request.ObjetoExportacao.GroupBy(g => g.Cabecalho);

                foreach (var turma in agrupamentoTurmas)
                {
                    dt = new DataTable();

                    var gruposMatriz = turma.SelectMany(e => e.GruposMatriz);

                    var gruposMatrizPorId = gruposMatriz.DistinctBy(d => d.Id);
                    var componentesCurriculares = gruposMatriz.SelectMany(e => e.ComponentesCurriculares).GroupBy(gm => gm.IdGrupoMatriz);

                    MontarColunas(gruposMatrizPorId, componentesCurriculares, dt);

                    MontarComponentes(gruposMatrizPorId, componentesCurriculares, dt);

                    var linhas = turma.SelectMany(l => l.Linhas);

                    MontarLinhas(linhas, dt);

                    lstDatatables.Add(dt);
                }

                return await Task.FromResult(lstDatatables);
            }
            catch (System.Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw ex;
            }

        }

        private void MontarLinhas(IEnumerable<ConselhoClasseAtaFinalLinhaDto> linhas, DataTable dt)
        {
            var agrupamentoPorAluno = linhas.GroupBy(g => new { g.Id, g.Nome, g.Inativo, g.Situacao });

            foreach (var linha in agrupamentoPorAluno)
            {
                DataRow row = dt.NewRow();

                var celulas = linha.SelectMany(c => c.Celulas);

                row["NumeroChamada"] = linha.Key.Id;

                if (linha.Key.Inativo)
                    row["NomeAluno"] = $"{linha.Key.Nome} ({linha.Key.Situacao})";
                else
                    row["NomeAluno"] = linha.Key.Nome;

                foreach (var celula in celulas)
                {
                    row[$"Grupo{celula.GrupoMatriz}_Componente{celula.ComponenteCurricular}_Coluna{celula.Coluna}"] = celula.Valor;
                }

                dt.Rows.Add(row);
            }
        }

        private void MontarColunas(IEnumerable<ConselhoClasseAtaFinalGrupoDto> gruposMatriz, IEnumerable<IGrouping<long, ConselhoClasseAtaFinalComponenteDto>> componentesCurriculares, DataTable dt)
        {
            dt.Columns.Add("NumeroChamada");
            dt.Columns.Add("NomeAluno");

            foreach (var grupo in gruposMatriz)
            {
                foreach (var componente in componentesCurriculares.FirstOrDefault(c => c.Key == grupo.Id).DistinctBy(c => c.Id))
                {
                    foreach (var coluna in componente.Colunas)
                    {
                        dt.Columns.Add($"Grupo{grupo.Id}_Componente{componente.Id}_Coluna{coluna.Id}");
                    }
                }
            }

            dt.Columns.Add("Grupo99_Componente99_Coluna1");
            dt.Columns.Add("Grupo99_Componente99_Coluna2");
            dt.Columns.Add("Grupo99_Componente99_Coluna3");
            dt.Columns.Add("Grupo99_Componente99_Coluna4");
        }

        private void MontarComponentes(IEnumerable<ConselhoClasseAtaFinalGrupoDto> gruposMatriz, IEnumerable<IGrouping<long, ConselhoClasseAtaFinalComponenteDto>> componentes, DataTable dataTable)
        {
            int colunaGrupo = 2;
            int colunaComponente = 2;
            int colunaDetalhe = 2;
            DataRow linhaGrupos = dataTable.NewRow();
            DataRow linhaComponentes = dataTable.NewRow();
            DataRow linhaDetalhes = dataTable.NewRow();

            foreach (var grupo in gruposMatriz)
            {
                linhaGrupos[colunaGrupo] = grupo.Nome;

                colunaGrupo += grupo.QuantidadeColunas;

                foreach (var componente in componentes.FirstOrDefault(c => c.Key == grupo.Id).DistinctBy(c => c.Id))
                {
                    linhaComponentes[colunaComponente] = componente.Nome;

                    colunaComponente += componente.Colunas.Count();

                    foreach (var coluna in componente.Colunas)
                    {
                        linhaDetalhes[colunaDetalhe] = coluna.Nome;
                        colunaDetalhe++;
                    }
                }
            }

            linhaGrupos["Grupo99_Componente99_Coluna1"] = "Anual";

            linhaDetalhes["NumeroChamada"] = "Nº";
            linhaDetalhes["NomeAluno"] = "Nome";
            linhaDetalhes["Grupo99_Componente99_Coluna1"] = "F";
            linhaDetalhes["Grupo99_Componente99_Coluna2"] = "CA";
            linhaDetalhes["Grupo99_Componente99_Coluna3"] = "%";
            linhaDetalhes["Grupo99_Componente99_Coluna4"] = "Parecer Conclusivo";

            dataTable.Rows.Add(linhaGrupos);
            dataTable.Rows.Add(linhaComponentes);
            dataTable.Rows.Add(linhaDetalhes);
        }
    }

}
