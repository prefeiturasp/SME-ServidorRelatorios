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
                row["NomeAluno"] = linha.Key.Nome;
                foreach (var celula in celulas)
                {
                    row[$"Grupo{celula.GrupoMatriz}{(celula.Regencia ? "Regencia" : "Normal")}_Componente{celula.ComponenteCurricular}_Coluna{celula.Coluna}"] = celula.Valor;
                }
                row["Inativo"] = linha.Key.Inativo;
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
                        dt.Columns.Add($"Grupo{grupo.Id}{(componente.Regencia ? "Regencia" : "Normal")}_Componente{componente.Id}_Coluna{coluna.Id}");
                    }
                }
            }

            dt.Columns.Add("Grupo99Normal_Componente99_Coluna1");
            dt.Columns.Add("Grupo99Normal_Componente99_Coluna2");
            dt.Columns.Add("Grupo99Normal_Componente99_Coluna3");
            dt.Columns.Add("Grupo99Normal_Componente99_Coluna4");
            dt.Columns.Add("Inativo");
        }

        private void MontarComponentes(IEnumerable<ConselhoClasseAtaFinalGrupoDto> gruposMatriz, IEnumerable<IGrouping<long, ConselhoClasseAtaFinalComponenteDto>> componentes, DataTable dataTable)
        {
            int colunaGrupo = 2;
            int colunaDetalhe = 2;
            DataRow linhaGrupos = dataTable.NewRow();
            DataRow linhaComponentes = dataTable.NewRow();
            DataRow linhaDetalhes = dataTable.NewRow();

            foreach (var grupo in gruposMatriz)
            {
                var componentesDoGrupo = componentes.FirstOrDefault(c => c.Key == grupo.Id).DistinctBy(c => c.Id);
                if (grupo.Regencia)
                {
                    linhaGrupos[colunaGrupo] = "Regência de Classe";
                    MontarLinhasGruposComponentes(componentesDoGrupo.Where(c => c.Regencia), linhaComponentes, ref colunaGrupo, linhaDetalhes, ref colunaDetalhe);
                    MontarLinhasGruposComponentes(componentesDoGrupo.Where(c => !c.Regencia), linhaGrupos, ref colunaGrupo, linhaDetalhes, ref colunaDetalhe);
                }
                else
                    MontarLinhasGruposComponentes(componentesDoGrupo, linhaGrupos, ref colunaGrupo, linhaDetalhes, ref colunaDetalhe);
            }

            linhaGrupos["Grupo99Normal_Componente99_Coluna1"] = "Anual";

            linhaDetalhes["NumeroChamada"] = "Nº";
            linhaDetalhes["NomeAluno"] = "Nome";
            linhaDetalhes["Grupo99Normal_Componente99_Coluna1"] = "F";
            linhaDetalhes["Grupo99Normal_Componente99_Coluna2"] = "CA";
            linhaDetalhes["Grupo99Normal_Componente99_Coluna3"] = "%";
            linhaDetalhes["Grupo99Normal_Componente99_Coluna4"] = "Parecer Conclusivo";
            linhaDetalhes["Inativo"] = "Inativo";

            dataTable.Rows.Add(linhaGrupos);
            dataTable.Rows.Add(linhaComponentes);
            dataTable.Rows.Add(linhaDetalhes);
        }

        private void MontarLinhasGruposComponentes(IEnumerable<ConselhoClasseAtaFinalComponenteDto> componentes, DataRow linhaComponentes, ref int colunaGrupoComponente, DataRow linhaDetalhes, ref int colunaDetalhe)
        {
            foreach (var componente in componentes)
            {
                linhaComponentes[colunaGrupoComponente] = componente.Nome;
                colunaGrupoComponente += componente.Colunas.Count();

                foreach (var coluna in componente.Colunas)
                {
                    linhaDetalhes[colunaDetalhe] = coluna.Nome;
                    colunaDetalhe++;
                }
            }
        }
    }
}
