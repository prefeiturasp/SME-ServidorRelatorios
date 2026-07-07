using ClosedXML.Excel;
using SME.SR.Infra.Dtos.Codaf;
using SME.SR.Infra.Excel.CodafSuplementar.Gerador.Interfaces;
using SME.SR.Infra.Extensions.Codaf;
using System.IO;

namespace SME.SR.Application.Services.CodafSuplementar
{
    public class GeradorRelatorioCodafSuplementarService : IGeradorRelatorioCodafSuplementarService
    {
        private readonly IBlocoTituloGeradorSuplementar _blocoTitulo;
        private readonly IBlocoCabecalhoGeradorSuplementar _blocoCabecalho;
        private readonly IBlocoRegentesGeradorSuplementar _blocoRegentes;
        private readonly IBlocoAlunosGeradorSuplementar _blocoAlunos;
        private readonly IBlocoAssinaturaGeradorSuplementar _blocoAssinatura;
        public GeradorRelatorioCodafSuplementarService(IBlocoTituloGeradorSuplementar blocoTitulo,
            IBlocoCabecalhoGeradorSuplementar blocoCabecalho,
            IBlocoRegentesGeradorSuplementar blocoRegentes,
            IBlocoAlunosGeradorSuplementar blocoAlunos,
            IBlocoAssinaturaGeradorSuplementar blocoAssinatura)
        {
            _blocoTitulo = blocoTitulo;
            _blocoCabecalho = blocoCabecalho;
            _blocoRegentes = blocoRegentes;
            _blocoAlunos = blocoAlunos;
            _blocoAssinatura = blocoAssinatura;
        }
        public MemoryStream GerarRelatorio(RelatorioCodafDto dadosRelatorio)
        {
            var stream = new MemoryStream();
            using var workbook = new XLWorkbook();
            workbook.ConfigurarEstiloPadrao();

            foreach (var turma in dadosRelatorio.Turmas)
            {
                var nomeAba = turma.NomeTurma.Length > 31 ? turma.NomeTurma[..31] : turma.NomeTurma;
                var sheet = workbook.Worksheets.Add(nomeAba);
                ConfigurarDimensoes(sheet);

                var linhaAtual = 1;

                // 1º Bloco: Título (Brasão)
                linhaAtual = _blocoTitulo.Processar(sheet, linhaAtual, null);

                // 2º Bloco: Cabeçalho
                linhaAtual = _blocoCabecalho.Processar(sheet, linhaAtual, turma.Cabecalho);

                // 3º Bloco: Regentes
                linhaAtual = _blocoRegentes.Processar(sheet, linhaAtual, turma.RegentesDaTurma);

                // Linha Vazia
                var rangeDivisoria = sheet.Range(linhaAtual, 1, linhaAtual, 20);
                rangeDivisoria.Merge();
                rangeDivisoria.Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                rangeDivisoria.Style.Border.RightBorder = XLBorderStyleValues.Thick;
                linhaAtual++;

                // 4º Bloco: Alunos
                linhaAtual = _blocoAlunos.Processar(sheet, linhaAtual, turma.AlunosAprovadosMunicipal);
                linhaAtual = _blocoAlunos.Processar(sheet, linhaAtual, turma.AlunosAprovadosParceira);
                linhaAtual = _blocoAlunos.Processar(sheet, linhaAtual, turma.AlunosReprovadosMunicipal);
                linhaAtual = _blocoAlunos.Processar(sheet, linhaAtual, turma.AlunosReprovadosParceira);

                var rangeBordaInferior = sheet.Range(6, 1, linhaAtual - 1, 20);
                rangeBordaInferior.Style.Border.OutsideBorder = XLBorderStyleValues.Thick;

                // 5º Bloco: Assinaturas
                _blocoAssinatura.Processar(sheet, linhaAtual, null);
            }

            workbook.SaveAs(stream);
            stream.Position = 0;
            return stream;
        }
        private static void ConfigurarDimensoes(IXLWorksheet sheet)
        {
            // Cumprimento padrão é 1.73 cm (com fator de 4.85 é igual a 8.40)
            // Tamanho padrão é 15 - corresponde a 0,53 cm
            sheet.Column("B").DefinirLarguraCm(3.70);
            sheet.Column("K").DefinirLarguraCm(4.09);
            sheet.Column("L").DefinirLarguraCm(2.29);
            sheet.Column("M").DefinirLarguraCm(2.90);
            sheet.Column("N").DefinirLarguraCm(2.18);
            sheet.Column("O").DefinirLarguraCm(2.95);
            sheet.Column("P").DefinirLarguraCm(3.25);
            sheet.Column("S").DefinirLarguraCm(2.35);
            sheet.Column("T").DefinirLarguraCm(2.35);
            sheet.Rows(2, 5).Height = 17;
        }
    }
}