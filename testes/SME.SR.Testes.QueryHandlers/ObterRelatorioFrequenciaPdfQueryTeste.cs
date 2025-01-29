using Xunit;
using SME.SR.Application.Queries.RelatorioFaltasFrequencia;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Tests.Application.Queries.RelatorioFaltasFrequencia
{
    public class ObterRelatorioFrequenciaPdfQueryTeste
    {
        [Fact]
        public void DeveInicializarAQueryComOFiltroDado()
        {
            var filtro = new FiltroRelatorioFrequenciasDto
            {
                AnoLetivo = 2025,
                CodigoDre = "DRE01",
                CodigoUe = "UE01",
                Modalidade = Modalidade.Fundamental,
                Semestre = 1,
                TipoRelatorio = TipoRelatorioFaltasFrequencia.Turma,
                AnosEscolares = new List<string> { "1", "2" },
                TurmasPrograma = true,
                CodigosTurma = new List<string> { "T01", "T02" },
                ComponentesCurriculares = new List<string> { "Matematica", "Portugues" },
                Bimestres = new List<int> { 1, 2 },
                Condicao = CondicoesRelatorioFaltasFrequencia.Igual,
                QuantidadeAusencia = 5,
                TipoQuantidadeAusencia = TipoQuantidadeAusencia.Percentual,
                TipoFormatoRelatorio = TipoFormatoRelatorio.Pdf,
                NomeUsuario = "Usuário Teste",
                CodigoRf = "RF123"
            };

            var query = new ObterRelatorioFrequenciaPdfQuery(filtro);

            Assert.NotNull(query);
            Assert.Equal(filtro, query.Filtro);
            Assert.Equal(2025, query.Filtro.AnoLetivo);
            Assert.Equal("DRE01", query.Filtro.CodigoDre);
            Assert.Equal("UE01", query.Filtro.CodigoUe);
            Assert.Equal(Modalidade.Fundamental, query.Filtro.Modalidade);
            Assert.Equal(1, query.Filtro.Semestre);
            Assert.Equal(TipoRelatorioFaltasFrequencia.Turma, query.Filtro.TipoRelatorio);
            Assert.Contains("1", query.Filtro.AnosEscolares);
            Assert.Contains("2", query.Filtro.AnosEscolares);
            Assert.True(query.Filtro.TurmasPrograma);
            Assert.Contains("T01", query.Filtro.CodigosTurma);
            Assert.Contains("T02", query.Filtro.CodigosTurma);
            Assert.Contains("Matematica", query.Filtro.ComponentesCurriculares);
            Assert.Contains("Portugues", query.Filtro.ComponentesCurriculares);
            Assert.Contains(1, query.Filtro.Bimestres);
            Assert.Contains(2, query.Filtro.Bimestres);
            Assert.Equal(5, query.Filtro.QuantidadeAusencia);
            Assert.Equal(TipoQuantidadeAusencia.Percentual, query.Filtro.TipoQuantidadeAusencia);
            Assert.Equal(TipoFormatoRelatorio.Pdf, query.Filtro.TipoFormatoRelatorio);
            Assert.Equal("Usuário Teste", query.Filtro.NomeUsuario);
            Assert.Equal("RF123", query.Filtro.CodigoRf);
        }
    }
}
