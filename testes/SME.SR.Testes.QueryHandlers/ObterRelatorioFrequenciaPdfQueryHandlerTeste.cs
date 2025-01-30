using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit;
using SME.SR.Application.Queries.RelatorioFaltasFrequencia;
using SME.SR.Infra;

namespace SME.SR.Testes.QueryHandlers
{
    public class TesteCalcularCondicaoFalta
    {
        private static bool InvocarMetodoPrivate(int totalAusencias, int totalCompensacoes, int totalPresenca, int totalAulas, string frequenciaFormatada, CondicoesRelatorioFaltasFrequencia condicao)
        {
            var tipo = typeof(ObterRelatorioFrequenciaPdfQueryHandler);

            var metodo = tipo.GetMethod("CalcularCondicaoFalta", BindingFlags.NonPublic | BindingFlags.Static);

            var aluno = new RelatorioFrequenciaAlunoDto
            {
                TotalAusencias = totalAusencias,
                TotalCompensacoes = totalCompensacoes,
                TotalPresenca = totalPresenca,
                TotalAulas = totalAulas,
                FrequenciaFormatada = frequenciaFormatada
            };

            var tipoRelatorio = TipoRelatorioFaltasFrequencia.Ano;
            double quantidadeAusencia = 5;
            var tipoQuantidadeAusencia = TipoQuantidadeAusencia.Fixa;

            var operacao = new Dictionary<CondicoesRelatorioFaltasFrequencia, Func<double, double, bool>>()
            {
                { CondicoesRelatorioFaltasFrequencia.Maior, (valor, valorFiltro) => valor > valorFiltro },
                { CondicoesRelatorioFaltasFrequencia.Menor, (valor, valorFiltro) => valor < valorFiltro },
                { CondicoesRelatorioFaltasFrequencia.Igual, (valor, valorFiltro) => Math.Abs(valor - valorFiltro) < 0.01 }
            };

            return (bool)metodo.Invoke(null, new object[] { aluno, tipoRelatorio, condicao, quantidadeAusencia, tipoQuantidadeAusencia, operacao });
        }

        [Fact]
        public void Deve_Retornar_True_Quando_Faltas_Atingem_Limite()
        {
            bool resultado = InvocarMetodoPrivate(15, 5, 10, 20, "85", CondicoesRelatorioFaltasFrequencia.Maior);
            Assert.True(resultado);
        }

        [Fact]
        public void Deve_Retornar_False_Quando_Faltas_Nao_Atingem_Limite()
        {
            bool resultado = InvocarMetodoPrivate(8, 3, 10, 20, "85", CondicoesRelatorioFaltasFrequencia.Maior);
            Assert.False(resultado);
        }

        [Fact]
        public void Deve_Retornar_False_Quando_Frequencia_100()
        {
            bool resultado = InvocarMetodoPrivate(0, 0, 20, 20, "100", CondicoesRelatorioFaltasFrequencia.Maior);
            Assert.False(resultado);
        }

        [Fact]
        public void Deve_Retornar_False_Quando_Tipo_De_Ausencia_Variavel()
        {
            bool resultado = InvocarMetodoPrivate(10, 5, 10, 20, "85", CondicoesRelatorioFaltasFrequencia.Maior);
            Assert.False(resultado);
        }
    }
}
