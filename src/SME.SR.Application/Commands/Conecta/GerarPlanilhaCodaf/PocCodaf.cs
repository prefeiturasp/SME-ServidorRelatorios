using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ClosedXML.Excel;
using ClosedXML.Excel.Drawings;

namespace SME.SR.Application.Commands.Conecta.GerarPlanilhaCodaf
{
    public class PocCodaf
    {
        public void ExecutarPoc()
        {
            var nomeArquivo = $"RelatorioCodaf_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            using var stream = GerarArquivoParaExcel();
            using var fileStream = File.Create(nomeArquivo);
            stream.CopyTo(fileStream);
            stream.Close();
                        
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
            {
                FileName = nomeArquivo,
                UseShellExecute = true,
                Verb = "open"
            });
        }
        private MemoryStream GerarArquivoParaExcel()
        {
            var stream = new MemoryStream();
            using var workbook = new XLWorkbook();
            workbook.Style.Font.FontName = "Arial";
            workbook.Style.Font.FontSize = 11;
            var sheet = workbook.Worksheets.Add("Turma 1");
            DefinirLarguraCm(sheet.Column("B"), 3.57);
            DefinirLarguraCm(sheet.Column("K"), 4.09);
            DefinirLarguraCm(sheet.Column("L"), 2.29);
            DefinirLarguraCm(sheet.Column("M"), 2.90);
            DefinirLarguraCm(sheet.Column("N"), 2.18);
            DefinirLarguraCm(sheet.Column("O"), 2.75);
            DefinirLarguraCm(sheet.Column("S"), 2.35);
            DefinirLarguraCm(sheet.Column("T"), 2.35);
            sheet.Rows(2, 5).Height = 25;
            var rangeImagem = sheet.Range("A2:B5");
            rangeImagem.Merge();
            const double FatorAltura = 1.3333;
            const double FatorLargura = 7.5;

            var alturaRangePx = sheet.Rows(2, 5).Sum(r => r.Height) * FatorAltura;
            var larguraRangePx = sheet.Columns("A:B").Sum(c => c.Width) * FatorLargura;

            // --- 2. Adicionar a Imagem ---
            // O ClosedXML lê as dimensões originais da imagem ao adicionar
            var imagem = sheet.AddPicture(ObterBrasaoPrefeitura());

            // --- 3. Calcular Escala (Manter Proporção) ---
            var larguraOriginal = imagem.OriginalWidth;
            var alturaOriginal = imagem.OriginalHeight;

            // Descobre qual lado "bate" primeiro na borda (menor proporção vence)
            var ratioX = larguraRangePx / larguraOriginal;
            var ratioY = alturaRangePx / alturaOriginal;
            var escala = Math.Min(ratioX, ratioY);
            if (escala > 1) escala = 1;

            // Novas dimensões calculadas
            var novaLargura = (int)(larguraOriginal * escala);
            var novaAltura = (int)(alturaOriginal * escala);

            // --- 4. Calcular Centralização (Offsets) ---
            var offsetX = (int)((larguraRangePx - novaLargura) / 2);
            var offsetY = (int)((alturaRangePx - novaAltura) / 2);

            // --- 5. Aplicar ao ClosedXML ---
            // Move para A2 com os deslocamentos calculados (Padding)
            imagem.MoveTo(sheet.Cell("A2"), offsetX, offsetY);
            imagem.WithSize(novaLargura, novaAltura);

            // Garante que a imagem não distorça se mexerem nas células
            imagem.Placement = XLPicturePlacement.MoveAndSize;

            // Linha 3
            ConfigurarLinhaCabecalho(sheet, 3, "SECRETARIA MUNICIPAL DE EDUCAÇÃO - SME");

            // Linha 4
            ConfigurarLinhaCabecalho(sheet, 4, "CONTROLE DE DOCUMENTAÇÃO DAS AÇÕES FORMATIVAS - CODAF");

            // Linha 5
            ConfigurarLinhaCabecalho(sheet, 5, " RELATÓRIO DE CONCLUSÃO DE TURMA - MODELO 2025 - REDE DIRETA");

            // Linha 6
            ConstruirLinhaOpcoes(sheet);

            // Linha 7
            ConfigurarLabel(sheet, 7, "A:B", "ÁREA PROMOTORA:", false, XLBorderStyleValues.Thin, XLAlignmentHorizontalValues.Right);

            var rangeAreaPromotoraValor = sheet.Range("C7:T7");
            rangeAreaPromotoraValor.Merge();
            rangeAreaPromotoraValor.Value = "DF/EMFORPEF";
            rangeAreaPromotoraValor.Style.Font.Bold = true;
            rangeAreaPromotoraValor.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            rangeAreaPromotoraValor.Style.Border.RightBorder = XLBorderStyleValues.Thick;
            //var rangeLinha7 = sheet.Range("A7:T7");
            //rangeLinha7.Style.Border.BottomBorder = XLBorderStyleValues.Thin;

            // Linha 8
            ConfigurarLabel(sheet, 8, "A:B", "NOME DA FORMAÇÃO:", false, XLBorderStyleValues.Thin, XLAlignmentHorizontalValues.Right);

            var rangeNomeFormacaoValor = sheet.Range("C8:T8");
            rangeNomeFormacaoValor.Merge();
            rangeNomeFormacaoValor.Value = "ENTRE POSSIBILIDADES: DIÁLOGOS SOBRE GESTÃO EDUCACIONAL";
            rangeNomeFormacaoValor.Style.Font.Bold = true;
            rangeNomeFormacaoValor.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            rangeNomeFormacaoValor.Style.Border.RightBorder = XLBorderStyleValues.Thick;
            //var rangeLinha8 = sheet.Range("A8:T8");
            //rangeLinha8.Style.Border.BottomBorder = XLBorderStyleValues.Thin;

            // Linha 9
            ConfigurarLabel(sheet, 9, "A:B", "HOMOLOGAÇÃO:", false, XLBorderStyleValues.Thin, XLAlignmentHorizontalValues.Right);
            var rangeHomologacao = sheet.Range("C9:F9");
            rangeHomologacao.Merge();
            rangeHomologacao.Value = "25159";
            rangeHomologacao.Style.Font.Bold = true;
            rangeHomologacao.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rangeHomologacao.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            rangeHomologacao.Style.Border.RightBorder = XLBorderStyleValues.Thin;

            ConfigurarLabel(sheet, 9, "G:J", "CÓDIGO DO EVENTO (SIGPEC):", false, XLBorderStyleValues.Thin, XLAlignmentHorizontalValues.Right);
            var rangeCodigoEvento = sheet.Range("K9:T9");
            rangeCodigoEvento.Merge();
            //rangeCodigoEvento.Value = "EVT-2025-00001234";
            rangeCodigoEvento.Style.Font.Bold = true;
            //var rangeLinha9 = sheet.Range("A9:T9");
            //rangeLinha9.Style.Border.BottomBorder = XLBorderStyleValues.Thin;

            // Linha 10
            ConstruirLinhaDadosDom(sheet);

            //var rangeLinha10 = sheet.Range("A10:T10");
            //rangeLinha10.Style.Border.BottomBorder = XLBorderStyleValues.Thin;

            // Exemplo de Retificações
            var retificacoes = new List<RetificacaoDto>
            {
                new RetificacaoDto { DataFormatada = "05/01/2025", NumeroPagina = "150" },
                new RetificacaoDto { DataFormatada = "10/01/2025", NumeroPagina = "152" },
                new RetificacaoDto { DataFormatada = "15/01/2025", NumeroPagina = "155" },
                new RetificacaoDto { DataFormatada = "20/01/2025", NumeroPagina = "158" }
            };
            var proximaLinha = GerarBlocoRetificacoes(sheet, 11, retificacoes);

            ConstruirLinhaDadosAulas(sheet, proximaLinha++);
            ConstruirLinhaCargaHoraria(sheet, proximaLinha++);
            ConstruirLinhaDadosTurma(sheet, proximaLinha++);



            // Duas linhas que não são do cabeçalho, vão entrar aqui. São uma prévia dos inscritos.



            ConfigurarLabel(sheet, proximaLinha, "A:B", "OBSERVAÇÕES:", false, XLBorderStyleValues.Thin, XLAlignmentHorizontalValues.Right);
            var rangeObservacoes = sheet.Range($"C{proximaLinha}:T{proximaLinha}");
            rangeObservacoes.Merge();
            rangeObservacoes.Value = "";

            sheet.Rows(6, proximaLinha).Height = 32;

            var rangeLinhasCabecalho = sheet.Range($"A7:T{proximaLinha}");
            rangeLinhasCabecalho.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            rangeLinhasCabecalho.Style.Border.OutsideBorder = XLBorderStyleValues.Thick;

            // Depois do final do cabeçalho, vem outros blocos (Professores, Inscritos, etc)
            // Os inscritos serão divididos em blocos de aprovados e reprovados

            // Ao final de tudo, tem os blocos para assinaturas

            workbook.SaveAs(stream);
            stream.Position = 0;
            return stream;
        }

        private void ConfigurarLinhaCabecalho(IXLWorksheet sheet, int linha, string texto)
        {
            // Define o range dinamicamente: C{linha}:R{linha}
            // C é coluna 3, R é coluna 18. Usar números é mais robusto em loops, 
            // mas string interpolada fica muito legível aqui.
            var range = sheet.Range($"C{linha}:R{linha}");

            range.Merge();
            range.Value = texto;

            // Estilização "Chique"
            range.Style.Font.Bold = true;

            // Geralmente textos mesclados assim são títulos, então centralizamos
            range.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            range.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        }

        private Stream ObterBrasaoPrefeitura()
        {
            var assembly = typeof(GerarPlanilhaCodafCommandHandler).Assembly;
            return assembly.GetManifestResourceStream("SME.SR.Application.Commands.Conecta.GerarPlanilhaCodaf.brasao_prefeitura_titulo_educacao.png");
        }

        private readonly XLColor _corFundoLabel = XLColor.FromHtml("#F2F2F2");
        private void ConstruirLinhaDadosTurma(IXLWorksheet sheet, int linha)
        {
            ConfigurarLabel(sheet, linha, "A:B", "DRE:", false, XLBorderStyleValues.Thin, XLAlignmentHorizontalValues.Right);
            var rangeNomeDre = sheet.Range($"C{linha}:G{linha}");
            rangeNomeDre.Merge();
            rangeNomeDre.Value = "";
            rangeNomeDre.Style.Font.Bold = true;
            rangeNomeDre.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rangeNomeDre.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ConfigurarLabel(sheet, linha, "H:K", "QUANTIDADE DE TURMAS:", false, XLBorderStyleValues.Thin, XLAlignmentHorizontalValues.Right);
            var rangeQuantidadeTurmas = sheet.Range($"L{linha}");
            rangeQuantidadeTurmas.Value = "5";
            rangeQuantidadeTurmas.Style.Font.Bold = true;
            rangeQuantidadeTurmas.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rangeQuantidadeTurmas.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ConfigurarLabel(sheet, linha, "M", "TURMA:", false, XLBorderStyleValues.Thin, XLAlignmentHorizontalValues.Right);
            var rangeNomeTurma = sheet.Range($"N{linha}:O{linha}");
            rangeNomeTurma.Merge();
            rangeNomeTurma.Value = "TURMA 1";
            rangeNomeTurma.Style.Font.Bold = true;
            rangeNomeTurma.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rangeNomeTurma.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ConfigurarLabel(sheet, linha, "P:S", "NÚMERO DE VAGAS DA TURMA:", false, XLBorderStyleValues.Thin, XLAlignmentHorizontalValues.Right);
            var rangeNumeroVagas = sheet.Range($"T{linha}");
            rangeNumeroVagas.Value = "50";
            rangeNumeroVagas.Style.Font.Bold = true;
            rangeNumeroVagas.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rangeNumeroVagas.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        }
        private void ConstruirLinhaCargaHoraria(IXLWorksheet sheet, int linha)
        {
            ConfigurarLabel(sheet, linha, "A:B", "CARGA HORÁRIA TOTAL:", false, XLBorderStyleValues.Thin, XLAlignmentHorizontalValues.Right);
            var rangeCargaHoraria = sheet.Range($"C{linha}:E{linha}");
            rangeCargaHoraria.Merge();
            rangeCargaHoraria.Value = "40h";
            rangeCargaHoraria.Style.Font.Bold = true;
            rangeCargaHoraria.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rangeCargaHoraria.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            rangeCargaHoraria.Style.Border.RightBorder = XLBorderStyleValues.Thin;

            ConfigurarLabel(sheet, linha, "F:I", "CARGA HORÁRIA A DISTÂNCIA:", false, XLBorderStyleValues.Thin, XLAlignmentHorizontalValues.Right);
            var rangeCargaHorariaDistancia = sheet.Range($"J{linha}:L{linha}");
            rangeCargaHorariaDistancia.Merge();
            rangeCargaHorariaDistancia.Value = "20h";
            rangeCargaHorariaDistancia.Style.Font.Bold = true;
            rangeCargaHorariaDistancia.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rangeCargaHorariaDistancia.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            rangeCargaHorariaDistancia.Style.Border.RightBorder = XLBorderStyleValues.Thin;

            ConfigurarLabel(sheet, linha, "M:P", "CARGA HORÁRIA SÍNCRONA/PRESENCIAL:", false, XLBorderStyleValues.Thin, XLAlignmentHorizontalValues.Right);
            var rangeCargaHorariaPresencial = sheet.Range($"Q{linha}:T{linha}");
            rangeCargaHorariaPresencial.Merge();
            rangeCargaHorariaPresencial.Value = "20h";
            rangeCargaHorariaPresencial.Style.Font.Bold = true;
            rangeCargaHorariaPresencial.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rangeCargaHorariaPresencial.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            rangeCargaHorariaPresencial.Style.Border.RightBorder = XLBorderStyleValues.Thin;
        }

        private void ConstruirLinhaDadosAulas(IXLWorksheet sheet, int linha)
        {
            ConfigurarLabel(sheet, linha, "A:B", "PERÍODO DE REALIZAÇÃO:", false, XLBorderStyleValues.Thin, XLAlignmentHorizontalValues.Right);
            var rangePeriodoRealizacao = sheet.Range($"C{linha}:G{linha}");
            rangePeriodoRealizacao.Merge();
            rangePeriodoRealizacao.Value = "01/01 A 31/12/2025";
            rangePeriodoRealizacao.Style.Font.Bold = true;
            rangePeriodoRealizacao.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rangePeriodoRealizacao.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            rangePeriodoRealizacao.Style.Border.RightBorder = XLBorderStyleValues.Thin;

            ConfigurarLabel(sheet, linha, "H:L", "DATAS DAS AULAS SÍNCRONAS/ PRESENCIAIS:", false, XLBorderStyleValues.Thin, XLAlignmentHorizontalValues.Right);
            var rangeDatasAulas = sheet.Range($"M{linha}:T{linha}");
            rangeDatasAulas.Merge();
            rangeDatasAulas.Value = "01/02, 15/03, 10/04 e 20/05";
            rangeDatasAulas.Style.Font.Bold = true;
            rangeDatasAulas.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rangeDatasAulas.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            rangeDatasAulas.Style.Border.RightBorder = XLBorderStyleValues.Thin;
        }
        private void ConstruirLinhaDadosDom(IXLWorksheet sheet)
        {
            var linha = 10;
            ConfigurarLabel(sheet, linha, "A:B", "COMUNICADO N°:", false, XLBorderStyleValues.Thin, XLAlignmentHorizontalValues.Right);
            var rangeComunicado = sheet.Range($"C{linha}:D{linha}");
            rangeComunicado.Merge();
            rangeComunicado.Value = "289";
            rangeComunicado.Style.Font.Bold = true;
            rangeComunicado.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            rangeComunicado.Style.Border.RightBorder = XLBorderStyleValues.Thin;


            ConfigurarLabel(sheet, linha, "E", "DATA:", false, XLBorderStyleValues.Thin, XLAlignmentHorizontalValues.Right);
            var rangeDataInicio = sheet.Range($"F{linha}:H{linha}");
            rangeDataInicio.Merge();
            rangeDataInicio.Value = "1/1/2025";
            rangeDataInicio.Style.Font.Bold = true;
            rangeDataInicio.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            rangeDataInicio.Style.Border.RightBorder = XLBorderStyleValues.Thin;

            ConfigurarLabel(sheet, linha, "I:K", "PUBLICAÇÃO DO D.O.C", false, XLBorderStyleValues.Thin, XLAlignmentHorizontalValues.Right);
            ConfigurarLabel(sheet, linha, "L", "DATA:", false, XLBorderStyleValues.Thin, XLAlignmentHorizontalValues.Right);
            var rangeDataDoc = sheet.Range($"M{linha}:N{linha}");
            rangeDataDoc.Merge();
            rangeDataDoc.Value = "2/1/2025";
            rangeDataDoc.Style.Font.Bold = true;
            rangeDataDoc.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            rangeDataDoc.Style.Border.RightBorder = XLBorderStyleValues.Thin;

            ConfigurarLabel(sheet, linha, "O", "PÁGINA:", false, XLBorderStyleValues.Thin, XLAlignmentHorizontalValues.Right);
            var rangePaginaDoc = sheet.Range($"P{linha}");
            rangePaginaDoc.Value = "147";
            rangePaginaDoc.Style.Font.Bold = true;
            rangePaginaDoc.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            rangePaginaDoc.Style.Border.RightBorder = XLBorderStyleValues.Thin;
        }
        private void ConstruirLinhaOpcoes(IXLWorksheet sheet)
        {
            var linha = 6;

            // --- 1. Configuração da Borda Externa Grossa (A6 até T6) ---
            var rangeTotal = sheet.Range(linha, 1, linha, 20); // A até T
            rangeTotal.Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
            //rangeTotal.Style.Border.OutsideBorderColor = XLColor.Black;

            // --- 2. Preenchimento célula a célula (Seguindo sua especificação) ---

            // B6 - Check | C6 - Label Normal
            ConfigurarCheck(sheet, linha, "A", true);
            ConfigurarLabel(sheet, linha, "B", "CURSO", negrito: false);

            // D6 - Label Solto (Negrito)
            ConfigurarLabel(sheet, linha, "C", "OU", negrito: true);

            // E6 - Check | F6:G6 - Label Normal (Borda Direita Grossa)
            ConfigurarCheck(sheet, linha, "D", false);
            ConfigurarLabel(sheet, linha, "E:F", "EVENTO", negrito: false, bordaDireita: XLBorderStyleValues.Thick);

            // H6 - Check | I6:J6 - Label Normal (Borda Direita Fina)
            ConfigurarCheck(sheet, linha, "G", true);
            ConfigurarLabel(sheet, linha, "H:I", "A DISTÂNCIA", negrito: false, bordaDireita: XLBorderStyleValues.Thin);

            // K6 - Check | L6 - Label Normal (Borda Direita Fina)
            ConfigurarCheck(sheet, linha, "J", false);
            ConfigurarLabel(sheet, linha, "K", "HÍBRIDO", negrito: false, bordaDireita: XLBorderStyleValues.Thin);

            // M6 - Check | N6 - Label Normal (Borda Direita Grossa)
            ConfigurarCheck(sheet, linha, "L", false);
            ConfigurarLabel(sheet, linha, "M", "PRESENCIAL", negrito: false, bordaDireita: XLBorderStyleValues.Thick);

            // O6 - Check | P6:Q6 - Label Normal
            ConfigurarCheck(sheet, linha, "N", true);
            ConfigurarLabel(sheet, linha, "O:P", "COM CERTIFICAÇÃO", negrito: false);

            // R6 - Label Solto (Negrito)
            ConfigurarLabel(sheet, linha, "Q", "OU", negrito: true);

            // S6 - Check | T6:U6 - Label Normal
            // Assumi Opcao1Ativa aqui apenas para exemplo, use a flag correta
            ConfigurarCheck(sheet, linha, "R", false);
            ConfigurarLabel(sheet, linha, "S:T", "SEM CERTIFICAÇÃO", negrito: false);
        }

        // --- Helpers Privados (O Segredo do Clean Code) ---

        private void ConfigurarCheck(IXLWorksheet sheet, int linha, string coluna, bool marcado)
        {
            var cell = sheet.Cell($"{coluna}{linha}");
            cell.Value = marcado ? "( X )" : "(   )";

            // Estilização Fixa do Check
            cell.Style.Font.Bold = true;
            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            // Checks geralmente não têm o fundo cinza, mas se tiverem, descomente abaixo:
            // cell.Style.Fill.BackgroundColor = _corFundoLabel; 
        }

        private void ConfigurarLabel(IXLWorksheet sheet, int linha, string colunas, string texto,
            bool negrito, XLBorderStyleValues bordaDireita = XLBorderStyleValues.None,
            XLAlignmentHorizontalValues alinhamentoHorizontal = XLAlignmentHorizontalValues.Center)
        {
            // Resolve se é range (ex: "F:G") ou célula única (ex: "C")
            var rangeString = colunas.Contains(':') ? $"{colunas.Split(':')[0]}{linha}:{colunas.Split(':')[1]}{linha}" : $"{colunas}{linha}";
            var range = sheet.Range(rangeString);

            // Se for range de mais de uma célula, mescla
            if (colunas.Contains(':'))
            {
                range.Merge();
            }

            range.Value = texto.ToUpper(); // Regra: Uppercase
            range.Style.Fill.BackgroundColor = _corFundoLabel; // Regra: Fundo #F2F2F2

            // Alinhamento
            range.Style.Alignment.Horizontal = alinhamentoHorizontal;
            range.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            // Fonte
            range.Style.Font.Bold = negrito;

            // Borda Direita Específica
            if (bordaDireita != XLBorderStyleValues.None)
            {
                range.Style.Border.RightBorder = bordaDireita;
                range.Style.Border.RightBorderColor = XLColor.Black;
            }
        }
        public int GerarBlocoRetificacoes(IXLWorksheet sheet, int linhaInicial, List<RetificacaoDto> retificacoes)
        {
            // Se não houver retificações, decidimos se imprime uma linha vazia ou nada.
            // Aqui assumirei que se tiver 0, imprimimos ao menos 1 linha vazia para manter o layout,
            // ou você pode retornar linhaInicial se preferir esconder.
            int totalItens = retificacoes.Count == 0 ? 1 : retificacoes.Count;

            // Cálculo de quantas linhas de Excel serão necessárias (3 retificações por linha)
            int qtdLinhasExcel = (int)Math.Ceiling(totalItens / 3.0);
            int linhaFinal = linhaInicial + qtdLinhasExcel - 1;

            // --- 1. Label Lateral (Esquerda - A e B) ---
            var rangeLabelLateral = sheet.Range(linhaInicial, 1, linhaFinal, 2); // Colunas A(2) e B(3)
            rangeLabelLateral.Merge();
            rangeLabelLateral.Value = "RETIFICAÇÃO:";
            rangeLabelLateral.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            rangeLabelLateral.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            rangeLabelLateral.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            rangeLabelLateral.Style.Font.Bold = true;

            // --- 2. Loop das Linhas ---
            for (int i = 0; i < qtdLinhasExcel; i++)
            {
                int linhaAtual = linhaInicial + i;

                // Calcula os índices da lista para esta linha (ex: 0, 1, 2 ou 3, 4, 5)
                var idx1 = i * 3;
                var idx2 = i * 3 + 1;
                var idx3 = i * 3 + 2;

                // Obtém os dados (ou null se acabou a lista)
                var ret1 = idx1 < retificacoes.Count ? retificacoes[idx1] : null;
                var ret2 = idx2 < retificacoes.Count ? retificacoes[idx2] : null;
                var ret3 = idx3 < retificacoes.Count ? retificacoes[idx3] : null;

                // Renderiza os 3 slots (passando null quando vazio para desenhar só a estrutura)
                RenderizarSlot1(sheet, linhaAtual, ret1);
                RenderizarSlot2(sheet, linhaAtual, ret2);
                RenderizarSlot3(sheet, linhaAtual, ret3);
            }

            // Retorna a próxima linha disponível para continuar o relatório
            return linhaFinal + 1;
        }

        // --- Helpers de Renderização (Private para encapsular a complexidade das colunas) ---

        private void RenderizarSlot1(IXLWorksheet sheet, int linha, RetificacaoDto dados)
        {
            // Col D: Label Data
            ConfigurarCelula(sheet, linha, "C", "DATA:", false, true, XLAlignmentHorizontalValues.Right); // true = Label Fixa

            // Col E:F: Valor Data
            ConfigurarCelula(sheet, linha, "D:E", dados?.DataFormatada ?? "", true, false);

            // Col G:H: Label PÁGINA (Align Right)
            ConfigurarCelula(sheet, linha, "F:G", "PÁGINA:", true, true, XLAlignmentHorizontalValues.Right);

            // Col I: Valor Página
            ConfigurarCelula(sheet, linha, "H", dados?.NumeroPagina ?? "", false, false);
        }

        private void RenderizarSlot2(IXLWorksheet sheet, int linha, RetificacaoDto dados)
        {
            // Col J: Label Data
            ConfigurarCelula(sheet, linha, "I", "DATA:", false, true, XLAlignmentHorizontalValues.Right);

            // Col K:L: Valor Data
            ConfigurarCelula(sheet, linha, "J:K", dados?.DataFormatada ?? "", true, false);

            // Col M: Label PÁGINA (Single Col, Right)
            ConfigurarCelula(sheet, linha, "L", "PÁGINA:", false, true, XLAlignmentHorizontalValues.Right);

            // Col N: Valor Página
            ConfigurarCelula(sheet, linha, "M", dados?.NumeroPagina ?? "", false, false);
        }

        private void RenderizarSlot3(IXLWorksheet sheet, int linha, RetificacaoDto dados)
        {
            // Col O: Label Data
            ConfigurarCelula(sheet, linha, "N", "DATA:", false, true, XLAlignmentHorizontalValues.Right);

            // Col P:Q: Valor Data
            ConfigurarCelula(sheet, linha, "O:P", dados?.DataFormatada ?? "", true, false);

            // Col R:S: Label PÁGINA (Merged, Right)
            ConfigurarCelula(sheet, linha, "Q:R", "PÁGINA:", true, true, XLAlignmentHorizontalValues.Right);

            // Col T:U: Valor Página (Merged)
            ConfigurarCelula(sheet, linha, "S:T", dados?.NumeroPagina ?? "", true, false);
        }

        /// <summary>
        /// Método genérico para padronizar bordas e merge
        /// </summary>
        private void ConfigurarCelula(IXLWorksheet sheet, int linha, string colunas, string valor,
            bool merge, bool ehLabel, XLAlignmentHorizontalValues alinhamento = XLAlignmentHorizontalValues.Center)
        {
            var rangeStr = colunas.Contains(':') ? $"{colunas.Split(':')[0]}{linha}:{colunas.Split(':')[1]}{linha}" : $"{colunas}{linha}";
            var range = sheet.Range(rangeStr);

            if (merge && colunas.Contains(':')) range.Merge();

            range.Value = valor;

            // Estilização Padrão
            range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            range.Style.Border.OutsideBorderColor = XLColor.Black;
            range.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            range.Style.Alignment.Horizontal = alinhamento;

            // Se for Label (Data ou PÁGINA), geralmente negrito ou visual diferente?
            // Mantive simples conforme pedido, mas aqui você pode por:
            if (ehLabel) range.Style.Font.Bold = true;
        }
        public class RetificacaoDto
        {
            public string DataFormatada { get; set; } // Já vem dd/MM/yyyy
            public string NumeroPagina { get; set; }
        }

        private const double FatorCmParaExcel = 4.85;

        public static void DefinirLarguraCm(IXLColumn coluna, double cm)
        {
            coluna.Width = cm * FatorCmParaExcel;
        }
    }
}
