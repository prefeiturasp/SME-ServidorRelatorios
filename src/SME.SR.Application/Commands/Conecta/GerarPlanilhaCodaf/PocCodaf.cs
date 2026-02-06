using ClosedXML.Excel;
using ClosedXML.Excel.Drawings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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



            ConfigurarLabel(sheet, proximaLinha, "A:D", "SME:", false, XLBorderStyleValues.Thin, XLAlignmentHorizontalValues.Right);
            ConfigurarLabel(sheet, proximaLinha, "E:H", "Nº DE INSCRITOS:", false, XLBorderStyleValues.Thin, XLAlignmentHorizontalValues.Right);
            ConfigurarLabel(sheet, proximaLinha, "J:L", "Nº DE APROVADOS:", false, XLBorderStyleValues.Thin, XLAlignmentHorizontalValues.Right);
            ConfigurarLabel(sheet, proximaLinha, "N:P", "Nº DE REPROVADOS:", false, XLBorderStyleValues.Thin, XLAlignmentHorizontalValues.Right);
            var rangeInscritosSme = sheet.Cell($"I{proximaLinha}");
            rangeInscritosSme.Value = "34";
            rangeInscritosSme.Style.Font.Bold = true;
            rangeInscritosSme.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rangeInscritosSme.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            var rangeAprovadosSme = sheet.Cell($"M{proximaLinha}");
            rangeAprovadosSme.Value = "20";
            rangeAprovadosSme.Style.Font.Bold = true;
            rangeAprovadosSme.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rangeAprovadosSme.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            var rangeReprovadosSme = sheet.Cell($"Q{proximaLinha}");
            rangeReprovadosSme.Value = "14";
            rangeReprovadosSme.Style.Font.Bold = true;
            rangeReprovadosSme.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rangeReprovadosSme.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ConfigurarLabel(sheet, ++proximaLinha, "A:D", "SEM R.F:", false, XLBorderStyleValues.Thin, XLAlignmentHorizontalValues.Right);
            ConfigurarLabel(sheet, proximaLinha, "E:H", "Nº DE INSCRITOS:", false, XLBorderStyleValues.Thin, XLAlignmentHorizontalValues.Right);
            ConfigurarLabel(sheet, proximaLinha, "J:L", "Nº DE APROVADOS:", false, XLBorderStyleValues.Thin, XLAlignmentHorizontalValues.Right);
            ConfigurarLabel(sheet, proximaLinha, "N:P", "Nº DE REPROVADOS:", false, XLBorderStyleValues.Thin, XLAlignmentHorizontalValues.Right);
            var rangeInscritosSemRf = sheet.Cell($"I{proximaLinha}");
            rangeInscritosSemRf.Value = "";
            rangeInscritosSemRf.Style.Font.Bold = true;
            rangeInscritosSemRf.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rangeInscritosSemRf.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            var rangeAprovadosSemRf = sheet.Cell($"M{proximaLinha}");
            rangeAprovadosSemRf.Value = "";
            rangeAprovadosSemRf.Style.Font.Bold = true;
            rangeAprovadosSemRf.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rangeAprovadosSemRf.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            var rangeReprovadosSemRf = sheet.Cell($"Q{proximaLinha}");
            rangeReprovadosSemRf.Value = "";
            rangeReprovadosSemRf.Style.Font.Bold = true;
            rangeReprovadosSemRf.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rangeReprovadosSemRf.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ConfigurarLabel(sheet, ++proximaLinha, "A:B", "OBSERVAÇÕES:", false, XLBorderStyleValues.Thin, XLAlignmentHorizontalValues.Right);
            var rangeObservacoes = sheet.Range($"C{proximaLinha}:T{proximaLinha}");
            rangeObservacoes.Merge();
            rangeObservacoes.Value = "";

            sheet.Rows(6, proximaLinha).Height = 32;

            var rangeLinhasCabecalho = sheet.Range($"A7:T{proximaLinha++}");
            rangeLinhasCabecalho.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            rangeLinhasCabecalho.Style.Border.OutsideBorder = XLBorderStyleValues.Thick;

            // ======== INÍCIO DA IMPLEMENTAÇÃO NOVA NA POC - REGENTES E PARTICIPANTES ========
            ConstruirBlocoRegentesDaTurma(sheet, ref proximaLinha);

            var rangeLinhaVazia = sheet.Range($"A{proximaLinha}:T{proximaLinha++}");
            rangeLinhaVazia.Merge();
            rangeLinhaVazia.Style.Border.LeftBorder = XLBorderStyleValues.Thick;
            rangeLinhaVazia.Style.Border.RightBorder = XLBorderStyleValues.Thick;

            ConstruirBlocoParticipantes(sheet, ref proximaLinha, participantes);
            RenderizarBlocoAssinaturas(sheet, ref proximaLinha);
            // ======== FIM DA IMPLEMENTAÇÃO NOVA NA POC - REGENTES E PARTICIPANTES ========

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
        private readonly XLColor _corFundoTitulo = XLColor.FromHtml("#BFBFBF");
        private readonly XLColor _corFundoSubTitulo = XLColor.FromHtml("#E6E6E6");
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

        public class DadosRegente
        {
            public string Nome { get; set; }
            public string Rf { get; set; }
            public string NumeroRegistro { get; set; }
        }
        private void ConstruirBlocoRegentesDaTurma(IXLWorksheet sheet, ref int linha)
        {
            var rangeTitulo = sheet.Range($"A{linha}:T{linha}");
            rangeTitulo.Merge();
            rangeTitulo.Value = "REGENTES DA TURMA COM RF";
            rangeTitulo.Style.Font.Bold = true;
            rangeTitulo.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rangeTitulo.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            rangeTitulo.Style.Fill.BackgroundColor = _corFundoSubTitulo;
            rangeTitulo.Style.Border.OutsideBorder = XLBorderStyleValues.Thick;

            var regentesTurma = new List<DadosRegente>
            {
                new DadosRegente
                {
                    Nome = "LUCIANA XAVIER FERREIRA",
                    Rf = "812.130.3",
                    NumeroRegistro = "23280"
                },
                new DadosRegente
                {
                    Nome = "MARINEUSA MEDEIROS DA SILVA",
                    Rf = "695.581.9",
                    NumeroRegistro = "23281"
                },
                new DadosRegente
                {
                    Nome = "RICARDO DE SOUZA",
                    Rf = "721.363.8",
                    NumeroRegistro = "23282"
                },
                new DadosRegente
                {
                    Nome = "ROGÉRIO GONÇALVES DA SILVA",
                    Rf = "752.813.2",
                    NumeroRegistro = "23283"
                },
                new DadosRegente
                {
                    Nome = "REGINA CÉLIA  FORTUNA BROTI GAVASSA",
                    Rf = "",
                    NumeroRegistro = "*****"
                }
            };


            foreach (var regente in regentesTurma)
            {
                ConfigurarLabel(sheet, ++linha, "A", "NOME:", false, XLBorderStyleValues.Thin, XLAlignmentHorizontalValues.Right);
                var rangeNomeRegente = sheet.Range($"B{linha}:K{linha}");
                rangeNomeRegente.Merge();
                rangeNomeRegente.Value = regente.Nome;
                rangeNomeRegente.Style.Font.Bold = true;
                rangeNomeRegente.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                rangeNomeRegente.Style.Border.RightBorder = XLBorderStyleValues.Thin;

                ConfigurarLabel(sheet, linha, "L", "R.F.:", false, XLBorderStyleValues.Thin, XLAlignmentHorizontalValues.Right);
                var rangeRfRegente = sheet.Range($"M{linha}:O{linha}");
                rangeRfRegente.Merge();
                rangeRfRegente.Value = regente.Rf;
                rangeRfRegente.Style.Font.Bold = true;
                rangeRfRegente.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                rangeRfRegente.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                rangeRfRegente.Style.Border.RightBorder = XLBorderStyleValues.Thin;

                ConfigurarLabel(sheet, linha, "P:R", "Nº DE REGISTRO:", false, XLBorderStyleValues.Thin, XLAlignmentHorizontalValues.Right);
                var rangeRegistroRegente = sheet.Range($"S{linha}:T{linha}");
                rangeRegistroRegente.Merge();
                rangeRegistroRegente.Value = regente.NumeroRegistro;
                rangeRegistroRegente.Style.Font.Bold = true;
                rangeRegistroRegente.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                rangeRegistroRegente.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                rangeRegistroRegente.Style.Border.RightBorder = XLBorderStyleValues.Thick;

                var rangeLinhaAtual = sheet.Range($"A{linha}:T{linha}");
                rangeLinhaAtual.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            }
            linha++;
        }

        public class DadosParticipante
        {
            public string Nome { get; set; }
            public string Documento { get; set; }
            public decimal Frequencia { get; set; }
            public bool Obrigatoria { get; set; }
            public string ConceitoFinal { get; set; }
            public string NumeroCertificado { get; set; }
            public bool Aprovado { get; set; }
        }
        private void ConstruirBlocoParticipantes(IXLWorksheet sheet, ref int linha, List<DadosParticipante> participantes)
        {
            var indice = 0;
            var participantesAprovados = participantes.Where(p => p.Aprovado).ToList();
            var participantesReprovados = participantes.Where(p => !p.Aprovado).ToList();

            var aprovadosComCpf = participantesAprovados.Where(p => p.Documento.Length >= 11).ToList();
            var reprovadosComCpf = participantesReprovados.Where(p => p.Documento.Length >= 11).ToList();

            var aprovadosSemCpf = participantesAprovados.Where(p => p.Documento.Length < 11).ToList();
            var reprovadosSemCpf = participantesReprovados.Where(p => p.Documento.Length < 11).ToList();

            var rangeTituloAprovados = sheet.Range($"A{linha}:T{linha}");
            rangeTituloAprovados.Merge();
            rangeTituloAprovados.Value = "PARTICIPANTES APROVADOS";
            rangeTituloAprovados.Style.Font.Bold = true;
            rangeTituloAprovados.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rangeTituloAprovados.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            rangeTituloAprovados.Style.Fill.BackgroundColor = _corFundoTitulo;
            rangeTituloAprovados.Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
            rangeTituloAprovados.Style.Font.FontSize = 14;
            sheet.Row(linha).Height = 30;
            linha++;

            RenderizarTabelaParticipantes(sheet, ref linha, ref indice, aprovadosSemCpf);
            RenderizarTabelaParticipantes(sheet, ref linha, ref indice, aprovadosComCpf);

            var rangeTituloReprovados = sheet.Range($"A{linha}:T{linha}");
            rangeTituloReprovados.Merge();
            rangeTituloReprovados.Value = "PARTICIPANTES DESISTENTES E REPROVADOS";
            rangeTituloReprovados.Style.Font.Bold = true;
            rangeTituloReprovados.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rangeTituloReprovados.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            rangeTituloReprovados.Style.Fill.BackgroundColor = _corFundoTitulo;
            rangeTituloReprovados.Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
            rangeTituloReprovados.Style.Font.FontSize = 14;
            sheet.Row(linha).Height = 30;
            linha++;

            RenderizarTabelaParticipantes(sheet, ref linha, ref indice, reprovadosSemCpf);
            RenderizarTabelaParticipantes(sheet, ref linha, ref indice, reprovadosComCpf);
        }
        private void RenderizarTabelaParticipantes(IXLWorksheet sheet, ref int linha, ref int indice, List<DadosParticipante> participantes)
        {
            var temCpf = participantes.Any(p => p.Documento.Length >= 11);
            var tituloColunaDocumento = temCpf ? "CPF" : "REGISTRO FUNCIONAL";
            var tituloColunaNome = temCpf ? "RELAÇÃO DE PARTICIPANTES DA REDE PARCEIRA" : "RELAÇÃO DE PARTICIPANTES EXCLUSIVAMENTE DA REDE MUNICIPAL DE ENSINO";

            // Cabeçalho da Tabela
            ConfigurarLabel(sheet, linha, "A", "Nº", true, XLBorderStyleValues.Thick, XLAlignmentHorizontalValues.Center);
            ConfigurarLabel(sheet, linha, "B:C", tituloColunaDocumento, true, XLBorderStyleValues.Thick, XLAlignmentHorizontalValues.Center);
            ConfigurarLabel(sheet, linha, "D:N", tituloColunaNome, true, XLBorderStyleValues.Thick, XLAlignmentHorizontalValues.Center);
            ConfigurarLabel(sheet, linha, "O", "FREQUÊNCIA (%)", true, XLBorderStyleValues.Thick, XLAlignmentHorizontalValues.Center);
            ConfigurarLabel(sheet, linha, "P", "ATIVIDADE OBRIGATÓRIA S/N", true, XLBorderStyleValues.Thick, XLAlignmentHorizontalValues.Center);
            ConfigurarLabel(sheet, linha, "Q:R", "CONCEITO FINAL", true, XLBorderStyleValues.Thick, XLAlignmentHorizontalValues.Center);
            ConfigurarLabel(sheet, linha, "S:T", "NÚMERO DE REGISTRO DO CERTIFICADO", true, XLBorderStyleValues.Thick, XLAlignmentHorizontalValues.Center);
            // O texto deve quebrar linha se for muito grande
            sheet.Row(linha).Height = 45;
            sheet.Row(linha++).Style.Alignment.WrapText = true;

            // Linhas de Dados
            foreach (var participante in participantes)
            {
                ConfigurarLabel(sheet, linha, "A", (++indice).ToString(), false, XLBorderStyleValues.Thin, XLAlignmentHorizontalValues.Center);
                var rangeDocumento = sheet.Range($"B{linha}:C{linha}");
                rangeDocumento.Merge();
                rangeDocumento.Value = participante.Documento;
                rangeDocumento.Style.Font.Bold = true;
                rangeDocumento.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                rangeDocumento.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                rangeDocumento.Style.Border.RightBorder = XLBorderStyleValues.Thin;
                var rangeNome = sheet.Range($"D{linha}:N{linha}");
                rangeNome.Merge();
                rangeNome.Value = participante.Nome;
                rangeNome.Style.Font.Bold = true;
                rangeNome.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                rangeNome.Style.Border.RightBorder = XLBorderStyleValues.Thin;
                var rangeFrequencia = sheet.Range($"O{linha}");
                rangeFrequencia.Value = participante.Frequencia.ToString("F2") + "%";
                rangeFrequencia.Style.Font.Bold = true;
                rangeFrequencia.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                rangeFrequencia.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                rangeFrequencia.Style.Border.RightBorder = XLBorderStyleValues.Thin;

                var rangeAtividadeObrigatoria = sheet.Range($"P{linha}");
                rangeAtividadeObrigatoria.Value = participante.Obrigatoria ? "S" : "N";
                rangeAtividadeObrigatoria.Style.Font.Bold = true;
                rangeAtividadeObrigatoria.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                rangeAtividadeObrigatoria.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                rangeAtividadeObrigatoria.Style.Border.RightBorder = XLBorderStyleValues.Thin;

                var rangeConceitoFinal = sheet.Range($"Q{linha}:R{linha}");
                rangeConceitoFinal.Merge();
                rangeConceitoFinal.Value = participante.ConceitoFinal;
                rangeConceitoFinal.Style.Font.Bold = true;
                rangeConceitoFinal.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                rangeConceitoFinal.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                rangeConceitoFinal.Style.Border.RightBorder = XLBorderStyleValues.Thin;

                var rangeNumeroCertificado = sheet.Range($"S{linha}:T{linha}");
                rangeNumeroCertificado.Merge();
                rangeNumeroCertificado.Value = participante.NumeroCertificado;
                rangeNumeroCertificado.Style.Font.Bold = true;
                rangeNumeroCertificado.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                rangeNumeroCertificado.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                rangeNumeroCertificado.Style.Border.RightBorder = XLBorderStyleValues.Thin;
                // Borda Inferior da Linha
                var rangeLinhaAtual = sheet.Range($"A{linha}:T{linha}");
                rangeLinhaAtual.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                linha++;
            }
        }
        private void RenderizarBlocoAssinaturas(IXLWorksheet sheet, ref int linha)
        {
            linha++;
            RenderizarAssinatura(sheet, ref linha, 2, "Responsável da Área Promotora pela documentação");
            RenderizarAssinatura(sheet, ref linha, 12, "Responsável da Área Promotora por conferir a documentação");

            sheet.Row(linha + 3).Height = 70;
        }
        private void RenderizarAssinatura(IXLWorksheet sheet, ref int linha, int colunaComeco, string titulo)
        {
            var colunaFinal = colunaComeco + 8;
            var rangeTitulo = sheet.Range(linha, colunaComeco, linha, colunaFinal);
            rangeTitulo.Merge();
            rangeTitulo.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            rangeTitulo.Value = titulo;
            rangeTitulo.Style.Font.Bold = true;
            rangeTitulo.Style.Font.FontSize = 9;
            rangeTitulo.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rangeTitulo.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            var rangeNome = sheet.Range(linha + 1, colunaComeco, linha + 1, colunaFinal);
            rangeNome.Merge();
            rangeNome.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            rangeNome.Value = "NOME/RF DO RESPONSÁVEL:";
            rangeNome.Style.Font.Bold = true;
            rangeNome.Style.Font.FontSize = 9;
            rangeNome.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

            var rangeAssinatura = sheet.Range(linha + 2, colunaComeco, linha + 3, colunaFinal);
            rangeAssinatura.Merge();
            rangeAssinatura.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            rangeAssinatura.Value = "ASSINATURA / CARIMBO:";
            rangeAssinatura.Style.Font.Bold = false;
            rangeAssinatura.Style.Font.FontSize = 9;
            rangeAssinatura.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            rangeTitulo.Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;

            var rangeBlocoAssinatura = sheet.Range(linha, colunaComeco, linha + 3, colunaFinal);
            rangeBlocoAssinatura.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        }

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

        private readonly List<DadosParticipante> participantes = new List<DadosParticipante>
            {
                // Participantes Aprovados
                new DadosParticipante
                {
                    Nome = "Maria Cristina Barboza Ribeiro Chaves",
                    Documento = "7807571",
                    Frequencia = 100,
                    Obrigatoria = true,
                    ConceitoFinal = "S",
                    NumeroCertificado = "23284",
                    Aprovado = true
                },
                new DadosParticipante
                {
                    Nome = "Maria Isabel De La Torre Santos",
                    Documento = "6716423",
                    Frequencia = 100,
                    Obrigatoria = true,
                    ConceitoFinal = "S",
                    NumeroCertificado = "23285",
                    Aprovado = true
                },
                new DadosParticipante
                {
                    Nome = "Maria Luisa do Nascimento Quandt",
                    Documento = "7459653",
                    Frequencia = 100,
                    Obrigatoria = true,
                    ConceitoFinal = "S",
                    NumeroCertificado = "23286",
                    Aprovado = true
                },
                new DadosParticipante
                {
                    Nome = "PATRICIA KITAZAWA DE SOUZA SANTOS",
                    Documento = "8097143",
                    Frequencia = 100,
                    Obrigatoria = true,
                    ConceitoFinal = "S",
                    NumeroCertificado = "23287",
                    Aprovado = true
                },
                new DadosParticipante
                {
                    Nome = "Rita de Cássia Ferreira de Lemos",
                    Documento = "6299669588",
                    Frequencia = 100,
                    Obrigatoria = true,
                    ConceitoFinal = "S",
                    NumeroCertificado = "23288",
                    Aprovado = true
                },
                new DadosParticipante
                {
                    Nome = "Tania Regina Aparecida dos Santos Vogel",
                    Documento = "83702900519",
                    Frequencia = 100,
                    Obrigatoria = true,
                    ConceitoFinal = "S",
                    NumeroCertificado = "23289",
                    Aprovado = true
                },
                new DadosParticipante
                {
                    Nome = "Tatiane Marli Oliveira Ramalho dos Santos",
                    Documento = "81951029588",
                    Frequencia = 100,
                    Obrigatoria = true,
                    ConceitoFinal = "S",
                    NumeroCertificado = "23290",
                    Aprovado = true
                },
                new DadosParticipante
                {
                    Nome = "Yolanda Maria Aparecida Castro",
                    Documento = "72116435888",
                    Frequencia = 100,
                    Obrigatoria = true,
                    ConceitoFinal = "S",
                    NumeroCertificado = "23291",
                    Aprovado = true
                },

                // Participantes Desistentes e Reprovados
                new DadosParticipante
                {
                    Nome = "Maria Amélia Quadrado",
                    Documento = "5412355",
                    Frequencia = 0,
                    Obrigatoria = false,
                    ConceitoFinal = "NS",
                    NumeroCertificado = "***",
                    Aprovado = false
                },
                new DadosParticipante
                {
                    Nome = "Maria Aparecida Reginato Viana",
                    Documento = "8542154",
                    Frequencia = 85,
                    Obrigatoria = false,
                    ConceitoFinal = "NS",
                    NumeroCertificado = "***",
                    Aprovado = false
                },
                new DadosParticipante
                {
                    Nome = "Maria de Fátima Borges de Oliveira",
                    Documento = "9854211",
                    Frequencia = 0,
                    Obrigatoria = false,
                    ConceitoFinal = "NS",
                    NumeroCertificado = "***",
                    Aprovado = false
                },
                new DadosParticipante
                {
                    Nome = "Marília Clotildes Silva Magalhães",
                    Documento = "6541852",
                    Frequencia = 0,
                    Obrigatoria = false,
                    ConceitoFinal = "NS",
                    NumeroCertificado = "***",
                    Aprovado = false
                },
                new DadosParticipante
                {
                    Nome = "Marina Matias de Menezes",
                    Documento = "1234567",
                    Frequencia = 25,
                    Obrigatoria = false,
                    ConceitoFinal = "NS",
                    NumeroCertificado = "***",
                    Aprovado = false
                },
                new DadosParticipante
                {
                    Nome = "Marisa de Almeida Pedroso Diz",
                    Documento = "7654321",
                    Frequencia = 85,
                    Obrigatoria = false,
                    ConceitoFinal = "NS",
                    NumeroCertificado = "***",
                    Aprovado = false
                },
                new DadosParticipante
                {
                    Nome = "Marta Ferreira Marques",
                    Documento = "2345678",
                    Frequencia = 0,
                    Obrigatoria = false,
                    ConceitoFinal = "NS",
                    NumeroCertificado = "***",
                    Aprovado = false
                },
                new DadosParticipante
                {
                    Nome = "Michele Renata Morelli Acquafreda",
                    Documento = "8765432",//
                    Frequencia = 85,
                    Obrigatoria = false,
                    ConceitoFinal = "NS",
                    NumeroCertificado = "***",
                    Aprovado = false
                },
                new DadosParticipante
                {
                    Nome = "Michelle Weinberger Coutinho Silva",
                    Documento = "81951029590",
                    Frequencia = 85,
                    Obrigatoria = false,
                    ConceitoFinal = "NS",
                    NumeroCertificado = "",
                    Aprovado = false
                },
                // Início da Relação de Participantes da Rede Parceira
                new DadosParticipante
                {
                    Nome = "Nilma da Silva Oliveira",
                    Documento = "82835675988",
                    Frequencia = 0,
                    Obrigatoria = false,
                    ConceitoFinal = "NS",
                    NumeroCertificado = "***",
                    Aprovado = false
                },
                new DadosParticipante
                {
                    Nome = "Patrícia Gomes de Miranda",
                    Documento = "79206285984",
                    Frequencia = 50,
                    Obrigatoria = false,
                    ConceitoFinal = "NS",
                    NumeroCertificado = "***",
                    Aprovado = false
                },
                new DadosParticipante
                {
                    Nome = "Regiane Veschi",
                    Documento = "82835675989",
                    Frequencia = 0,
                    Obrigatoria = false,
                    ConceitoFinal = "NS",
                    NumeroCertificado = "",
                    Aprovado = false
                },
                new DadosParticipante
                {
                    Nome = "RENATO HIDEO CAETANO DA SILVA",
                    Documento = "79206285985",
                    Frequencia = 0,
                    Obrigatoria = false,
                    ConceitoFinal = "NS",
                    NumeroCertificado = "",
                    Aprovado = false
                },
                new DadosParticipante
                {
                    Nome = "Rita de Cassia Pereira Xavier de Araujo",
                    Documento = "82835675990",
                    Frequencia = 50,
                    Obrigatoria = false,
                    ConceitoFinal = "NS",
                    NumeroCertificado = "",
                    Aprovado = false
                },
                new DadosParticipante
                {
                    Nome = "Rosane Nunes Rodrigues",
                    Documento = "79206285986",
                    Frequencia = 0,
                    Obrigatoria = false,
                    ConceitoFinal = "NS",
                    NumeroCertificado = "",
                    Aprovado = false
                },
                new DadosParticipante
                {
                    Nome = "Rosangela Tomini Barbosa",
                    Documento = "82835675991",
                    Frequencia = 85,
                    Obrigatoria = false,
                    ConceitoFinal = "NS",
                    NumeroCertificado = "",
                    Aprovado = false
                },
                new DadosParticipante
                {
                    Nome = "ROSILENE DOS SANTOS PEREIRA",
                    Documento = "79206285987",
                    Frequencia = 85,
                    Obrigatoria = false,
                    ConceitoFinal = "NS",
                    NumeroCertificado = "",
                    Aprovado = false
                },
                new DadosParticipante
                {
                    Nome = "Sandra Cristina Lima da Silva",
                    Documento = "82835675992",
                    Frequencia = 0,
                    Obrigatoria = false,
                    ConceitoFinal = "NS",
                    NumeroCertificado = "",
                    Aprovado = false
                },
                new DadosParticipante
                {
                    Nome = "Sandra de Araújo Muniz Lopes",
                    Documento = "79206285988",
                    Frequencia = 85,
                    Obrigatoria = false,
                    ConceitoFinal = "NS",
                    NumeroCertificado = "",
                    Aprovado = false
                },
                new DadosParticipante
                {
                    Nome = "Simeia de Matos Oliveira",
                    Documento = "82835675993",
                    Frequencia = 25,
                    Obrigatoria = false,
                    ConceitoFinal = "NS",
                    NumeroCertificado = "",
                    Aprovado = false
                },
                new DadosParticipante
                {
                    Nome = "Tatiana Domingues Macarrão",
                    Documento = "79206285989",
                    Frequencia = 50,
                    Obrigatoria = false,
                    ConceitoFinal = "NS",
                    NumeroCertificado = "",
                    Aprovado = false
                },
                new DadosParticipante
                {
                    Nome = "Tatiana Monteiro Raquel",
                    Documento = "82835675994",
                    Frequencia = 85,
                    Obrigatoria = false,
                    ConceitoFinal = "NS",
                    NumeroCertificado = "",
                    Aprovado = false
                },
                new DadosParticipante
                {
                    Nome = "THUANE DO NASCIMENTO AMORIM NOGUEIRA",
                    Documento = "79206285990",
                    Frequencia = 25,
                    Obrigatoria = false,
                    ConceitoFinal = "NS",
                    NumeroCertificado = "",
                    Aprovado = false
                },
                new DadosParticipante
                {
                    Nome = "Valéria Azambuja Pereira",
                    Documento = "82835675995",
                    Frequencia = 25,
                    Obrigatoria = false,
                    ConceitoFinal = "NS",
                    NumeroCertificado = "",
                    Aprovado = false
                },
                new DadosParticipante
                {
                    Nome = "Vanda Michele Costa de Souza Lima",
                    Documento = "79206285991",
                    Frequencia = 25,
                    Obrigatoria = false,
                    ConceitoFinal = "NS",
                    NumeroCertificado = "",
                    Aprovado = false
                },
                new DadosParticipante
                {
                    Nome = "Vinicius Agnellos Silva",
                    Documento = "82835675996",
                    Frequencia = 0,
                    Obrigatoria = false,
                    ConceitoFinal = "NS",
                    NumeroCertificado = "",
                    Aprovado = false
                }
            };
    }
}
