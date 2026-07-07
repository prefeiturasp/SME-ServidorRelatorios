using ClosedXML.Excel;
using SME.SR.Infra.Dtos.Codaf;
using SME.SR.Infra.Excel.Codaf.Gerador.Interfaces;
using SME.SR.Infra.Excel.CodafSuplementar.Gerador.Interfaces;
using SME.SR.Infra.Extensions;
using SME.SR.Infra.Extensions.Codaf;

namespace SME.SR.Infra.Excel.CodafSuplementar.Gerador
{
    public class BlocoAlunosGeradorSuplementar : IBlocoAlunosGerador, IBlocoAlunosGeradorSuplementar
    {
        private readonly XLColor _corFundoTitulo = XLColor.FromHtml("#BFBFBF");

        public int Processar(IXLWorksheet sheet, int linhaInicial, GrupoAlunosRelatorioCodafDto grupo)
        {
            var linha = linhaInicial;

            // 1. Título do Grupo (Ex: PARTICIPANTES APROVADOS)
            RenderizarTituloBloco(sheet, ref linha, grupo.TituloBloco);

            // 2. Cabeçalho da Tabela
            RenderizarCabecalhoTabela(sheet, ref linha, grupo.EhRedeParceira);

            // 3. Linhas dos Alunos
            foreach (var aluno in grupo.Alunos)
            {
                RenderizarLinhaAluno(sheet, linha, aluno);
                linha++;
            }

            return linha;
        }

        private void RenderizarTituloBloco(IXLWorksheet sheet, ref int linha, string titulo)
        {
            if (string.IsNullOrWhiteSpace(titulo)) return;

            var range = sheet.Range(linha, 1, linha, 20); // A:T
            range.Merge();
            range.Value = titulo;
            range.Style.Font.Bold = true;
            range.Style.Font.FontSize = 14;
            range.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            range.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            range.Style.Fill.BackgroundColor = _corFundoTitulo;
            range.Style.Border.OutsideBorder = XLBorderStyleValues.Thick;

            sheet.Row(linha).Height = 30;
            linha++;
        }

        private static void RenderizarCabecalhoTabela(IXLWorksheet sheet, ref int linha, bool ehRedeParceira)
        {
            var tituloDoc = ehRedeParceira ? "CPF" : "REGISTRO FUNCIONAL";
            var tituloNome = ehRedeParceira
                ? "RELAÇÃO DE PARTICIPANTES DA REDE PARCEIRA"
                : "RELAÇÃO DE PARTICIPANTES EXCLUSIVAMENTE DA REDE MUNICIPAL DE ENSINO";

            CriarHeader(sheet, linha, "A", "Nº");
            CriarHeader(sheet, linha, "B:C", tituloDoc);
            CriarHeader(sheet, linha, "D:N", tituloNome);
            CriarHeader(sheet, linha, "O", "FREQUÊNCIA (%)");
            CriarHeader(sheet, linha, "P", "ATIVIDADE OBRIGATÓRIA S/N");
            CriarHeader(sheet, linha, "Q:R", "CONCEITO FINAL");
            CriarHeader(sheet, linha, "S:T", "NÚMERO DE REGISTRO DO CERTIFICADO");

            sheet.Row(linha).Height = 45;
            sheet.Row(linha).Style.Alignment.WrapText = true;
            linha++;
        }

        private static void RenderizarLinhaAluno(IXLWorksheet sheet, int linha, AlunoRelatorioCodafDto aluno)
        {
            ConfigurarCelulaDados(sheet, linha, "A", aluno.NumeroSequencial.ToString());
            ConfigurarCelulaDados(sheet, linha, "B:C", aluno.DocumentoAluno.FormatarDocumento());
            ConfigurarCelulaDados(sheet, linha, "D:N", aluno.NomeAluno, alinharEsquerda: true);
            ConfigurarCelulaDados(sheet, linha, "O", $"{aluno.PercentualFrequencia:F2}%");
            ConfigurarCelulaDados(sheet, linha, "P", aluno.AtividadeObrigatoria ? "S" : "N");
            ConfigurarCelulaDados(sheet, linha, "Q:R", aluno.ConceitoFinal);
            ConfigurarCelulaDados(sheet, linha, "S:T", aluno.CodigoCertificado.FormatarValorOuMascarar());

            // Borda inferior da linha
            sheet.Range(linha, 1, linha, 20).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
        }

        private static void CriarHeader(IXLWorksheet sheet, int linha, string colunas, string texto)
        {
            var range = sheet.ObterRange(colunas, linha);

            range.Value = texto;
            range.Style.Font.Bold = true;
            range.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            range.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            range.Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
        }

        private static void ConfigurarCelulaDados(IXLWorksheet sheet, int linha, string colunas, string valor, bool alinharEsquerda = false)
        {
            var range = sheet.ObterRange(colunas, linha);

            range.Value = valor;
            range.Style.Font.Bold = true;
            range.Style.Border.RightBorder = XLBorderStyleValues.Thin;
            range.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            range.Style.Alignment.Horizontal = alinharEsquerda ? XLAlignmentHorizontalValues.Left : XLAlignmentHorizontalValues.Center;
        }
    }
}