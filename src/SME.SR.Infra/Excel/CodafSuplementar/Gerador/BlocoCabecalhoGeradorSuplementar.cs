using ClosedXML.Excel;
using SME.SR.Infra.Dtos.Codaf;
using SME.SR.Infra.Excel.Codaf.Gerador.Interfaces;
using SME.SR.Infra.Excel.CodafSuplementar.Gerador.Interfaces;
using SME.SR.Infra.Extensions.Codaf;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SME.SR.Infra.Excel.CodafSuplementar.Gerador
{
    public class BlocoCabecalhoGeradorSuplementar : IBlocoCabecalhoGerador, IBlocoCabecalhoGeradorSuplementar
    {
        public int Processar(IXLWorksheet sheet, int linhaInicial, CabecalhoRelatorioCodafDto dados)
        {
            var linha = linhaInicial;

            // Linha de Opções (Checkboxes)
            RenderizarLinhaOpcoes(sheet, ref linha, dados);
            RenderizarDadosDaFormação(sheet, ref linha, dados);
            RenderizarDadosDom(sheet, ref linha, dados);
            RenderizarBlocoRetificacoes(sheet, ref linha, dados.Retificacoes);

            // Dados da Turma e Carga Horária
            RenderizarDadosAulas(sheet, ref linha, dados);
            RenderizarCargaHoraria(sheet, ref linha, dados);
            RenderizarDadosTurma(sheet, ref linha, dados);

            // Prévia dos Inscritos
            RenderizarPreviaInscritos(sheet, ref linha, dados.PreviaInscritosSme);
            RenderizarPreviaInscritos(sheet, ref linha, dados.PreviaInscritosSemRf);

            // Observações
            RenderizarObservacao(sheet, ref linha, dados);

            FinalizarEstiloCabecalho(sheet, linhaInicial, linha - 1);

            return linha;
        }
        private static void FinalizarEstiloCabecalho(IXLWorksheet sheet, int linhaInicial, int linhaFinal)
        {
            var range = sheet.Range(linhaInicial, 1, linhaFinal, 20); // A até T
            range.Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
            range.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            sheet.Rows(linhaInicial, linhaFinal).Height = 32;
        }

        private static void RenderizarLinhaOpcoes(IXLWorksheet sheet, ref int linha, CabecalhoRelatorioCodafDto dados)
        {
            // Linha 6 - Opções com Checkboxes
            sheet.Range(linha, 1, linha, 20).Style.Border.OutsideBorder = XLBorderStyleValues.Thick;

            // --- GRUPO 1: TIPO FORMAÇÃO ---
            RenderizarOpcaoCheckbox(sheet, linha, 1, "CURSO", dados.TipoFormacao == TipoFormacaoRelatorioCodaf.Curso);
            RenderizarSeparadorOu(sheet, linha, 3);
            RenderizarOpcaoCheckbox(sheet, linha, 4, "EVENTO", dados.TipoFormacao == TipoFormacaoRelatorioCodaf.Evento,
                colsLabel: 2, bordaDireita: XLBorderStyleValues.Thick);

            // --- GRUPO 2: MODALIDADE ---
            RenderizarOpcaoCheckbox(sheet, linha, 7, "A DISTÂNCIA", dados.Modalidade == ModalidadeRelatorioCodaf.Distancia,
                colsLabel: 2, bordaDireita: XLBorderStyleValues.Thin);
            RenderizarOpcaoCheckbox(sheet, linha, 10, "HÍBRIDO", dados.Modalidade == ModalidadeRelatorioCodaf.Hibrido,
                colsLabel: 1, bordaDireita: XLBorderStyleValues.Thin);
            RenderizarOpcaoCheckbox(sheet, linha, 12, "PRESENCIAL", dados.Modalidade == ModalidadeRelatorioCodaf.Presencial,
                colsLabel: 1, bordaDireita: XLBorderStyleValues.Thick);

            // --- GRUPO 3: CERTIFICAÇÃO ---
            RenderizarOpcaoCheckbox(sheet, linha, 14, "COM CERTIFICAÇÃO", dados.TipoCertificacao == TipoCertificacaoRelatorioCodaf.ComCertificacao,
                colsLabel: 2);
            RenderizarSeparadorOu(sheet, linha, 17);
            RenderizarOpcaoCheckbox(sheet, linha, 18, "SEM CERTIFICAÇÃO", dados.TipoCertificacao == TipoCertificacaoRelatorioCodaf.SemCertificacao,
                colsLabel: 2);
            linha++;

        }

        private static void RenderizarDadosDaFormação(IXLWorksheet sheet, ref int linha, CabecalhoRelatorioCodafDto dados)
        {
            // Layout: Label A:B | Valor C:T
            CriarLinhaPadrao(sheet, linha++, "ÁREA PROMOTORA:", dados.AreaPromotora, bordaDireitaGrossa: true, alinharEsquerda: true);
            CriarLinhaPadrao(sheet, linha++, "NOME DA FORMAÇÃO:", dados.NomeFormacao, bordaDireitaGrossa: true, alinharEsquerda: true);

            // Linha Mista
            CriarCampoChaveValor(sheet, linha, "A:B", "HOMOLOGAÇÃO:", "C:F", dados.NumeroHomologacao.ToString());
            CriarCampoChaveValor(sheet, linha++, "G:J", "CÓDIGO DO EVENTO (SIGPEC):", "K:T", dados.CodigoEventoSigpec.ToString(), bordaDireitaValor: true);
        }

        private static void RenderizarDadosDom(IXLWorksheet sheet, ref int linha, CabecalhoRelatorioCodafDto dados)
        {
            // Linha 10 - Comunicado, Data do Comunicado, Publicação do DOM
            CriarCampoChaveValor(sheet, linha, "A:B", "COMUNICADO N°:", "C:D", dados.NumeroComunicado.ToString());
            CriarCampoChaveValor(sheet, linha, "E", "DATA:", "F:H", dados.DataComunicado.ToString("dd/MM/yyyy"));
            CriarCampoChaveValor(sheet, linha, "I:K", "PUBLICAÇÃO DO D.O.C:", null, null); // Apenas label
            CriarCampoChaveValor(sheet, linha, "L", "DATA:", "M:N", dados.DataPublicacaoDom.ToString("dd/MM/yyyy"));
            CriarCampoChaveValor(sheet, linha++, "O", "PÁGINA:", "P", dados.PaginaDom.ToString());
        }

        private static void RenderizarBlocoRetificacoes(IXLWorksheet sheet, ref int linha, List<RetificacaoRelatorioCodafDto> retificacoes)
        {
            // Linha 11 em diante - Retificações
            var linhaInicial = linha;
            int totalItens = retificacoes != null && retificacoes.Any() ? retificacoes.Count : 1;
            int qtdLinhas = (int)Math.Ceiling(totalItens / 3.0); // 3 slots por linha
            int linhaFinal = linhaInicial + qtdLinhas - 1;
            linha += qtdLinhas;

            // Label Lateral Mesclada
            var rangeLateral = sheet.Range(linhaInicial, 1, linhaFinal, 2); // A:B
            rangeLateral.Merge();
            rangeLateral.Value = "RETIFICAÇÃO:";
            rangeLateral.EstilizarLabel(alinhamento: XLAlignmentHorizontalValues.Right);
            rangeLateral.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            for (var i = 0; i < qtdLinhas; i++)
            {
                var linhaAtual = linhaInicial + i;
                var r1 = retificacoes.ElementAtOrDefault(i * 3);
                var r2 = retificacoes.ElementAtOrDefault(i * 3 + 1);
                var r3 = retificacoes.ElementAtOrDefault(i * 3 + 2);

                RenderizarSlotRetificacao(sheet, linhaAtual, 1, r1);
                RenderizarSlotRetificacao(sheet, linhaAtual, 2, r2);
                RenderizarSlotRetificacao(sheet, linhaAtual, 3, r3);
            }
        }

        private static void RenderizarDadosAulas(IXLWorksheet sheet, ref int linha, CabecalhoRelatorioCodafDto dados)
        {
            var periodo = $"{dados.DataPeriodoRealizacaoInicio:dd/MM/yyyy} a {dados.DataPeriodoRealizacaoFim:dd/MM/yyyy}";
            CriarCampoChaveValor(sheet, linha, "A:B", "PERÍODO DE REALIZAÇÃO:", "C:G", periodo);

            string textoDatas = FormatarDatasAulas(dados.DataDasAulasSincronas);
            CriarCampoChaveValor(sheet, linha++, "H:L", "DATAS DAS AULAS SÍNCRONAS/ PRESENCIAIS:", "M:T", textoDatas);
        }

        private static void RenderizarCargaHoraria(IXLWorksheet sheet, ref int linha, CabecalhoRelatorioCodafDto dados)
        {
            // Linha "13" - Carga Horária
            CriarCampoChaveValor(sheet, linha, "A:B", "CARGA HORÁRIA TOTAL:", "C:E", $"{dados.CargaHorariaTotal}h", bordaDireitaValor: true);
            CriarCampoChaveValor(sheet, linha, "F:I", "CARGA HORÁRIA A DISTÂNCIA:", "J:L", dados.CargaHorariaDistancia.ToString(), bordaDireitaValor: true);
            CriarCampoChaveValor(sheet, linha++, "M:P", "CARGA PRESENCIAL:", "Q:T", dados.CargaHorariaPresencial.ToString());
        }

        private static void RenderizarDadosTurma(IXLWorksheet sheet, ref int linha, CabecalhoRelatorioCodafDto dados)
        {
            // Linha "14" - Dados da Turma
            CriarCampoChaveValor(sheet, linha, "A:B", "DRE:", "C:G", dados.NomeDre, bordaDireitaValor: true);
            CriarCampoChaveValor(sheet, linha, "H:K", "QUANTIDADE DE TURMAS:", "L", dados.QuantidadeTurmas.ToString(), bordaDireitaValor: true);
            CriarCampoChaveValor(sheet, linha, "M", "TURMA:", "N:O", dados.NomeTurma, bordaDireitaValor: true);
            CriarCampoChaveValor(sheet, linha++, "P:S", "NÚMERO DE VAGAS DA TURMA:", "T", dados.NumeroVagas.ToString());
        }

        private static void RenderizarPreviaInscritos(IXLWorksheet sheet, ref int linha, PreviaInscritosRelatorioCodafDto previaInscritos)
        {
            // Linha "15/16" - Prévia dos Inscritos
            var titulo = previaInscritos.TemRf ? "SME:" : "SEM R.F.:";
            CriarCampoChaveValor(sheet, linha, "A:D", titulo, null, null); // Apenas label
            CriarCampoChaveValor(sheet, linha, "E:H", "Nº DE INSCRITOS:", "I", previaInscritos.TotalInscritos.ToString(), bordaDireitaValor: true);
            CriarCampoChaveValor(sheet, linha, "J:L", "Nº DE APROVADOS:", "M", previaInscritos.TotalAprovados.ToString(), bordaDireitaValor: true);
            CriarCampoChaveValor(sheet, linha++, "N:P", "REPROVADOS/ DESISTENTES:", "Q:T", previaInscritos.TotalReprovados.ToString());
        }

        private static void RenderizarObservacao(IXLWorksheet sheet, ref int linha, CabecalhoRelatorioCodafDto dados)
        {
            // Linha "17" de Observações
            var textoObservacao = dados.Observacao;
            var textoDocumento = "Documento suplementar do arquivo gerado em " + dados.DataCodaf.ToShortDateString();

            // Renderiza a observação normalmente
            var rangeLabel = sheet.ObterRange("A:B", linha);
            rangeLabel.ConfigurarLabelComFundo("OBSERVAÇÕES:");
            rangeLabel.EstilizarLabel();
            rangeLabel.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            rangeLabel.Style.Border.OutsideBorderColor = XLColor.Black;
            rangeLabel.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            // Renderiza o valor da observação (esquerda)
            var rangeObservacao = sheet.ObterRange("C:O", linha);
            rangeObservacao.Value = textoObservacao;
            rangeObservacao.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            rangeObservacao.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            rangeObservacao.Style.Alignment.WrapText = true;
            rangeObservacao.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            rangeObservacao.Style.Border.RightBorder = XLBorderStyleValues.None;

            // Renderiza o texto do documento (direita)
            var rangeDocumento = sheet.ObterRange("P:T", linha);
            rangeDocumento.Value = textoDocumento;
            rangeDocumento.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            rangeDocumento.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            rangeDocumento.Style.Alignment.WrapText = true;
            rangeDocumento.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            rangeDocumento.Style.Border.RightBorder = XLBorderStyleValues.Thin;

            linha++;
        }

        private static void RenderizarOpcaoCheckbox(IXLWorksheet sheet, int linha, int colCheck, string label, bool marcado,
            int colsLabel = 1, XLBorderStyleValues bordaDireita = XLBorderStyleValues.None)
        {
            // Checkbox
            sheet.Cell(linha, colCheck).ConfigurarCheckbox(marcado);

            // Label
            var colInicioLabel = colCheck + 1;
            var colFimLabel = colInicioLabel + colsLabel - 1;
            var rangeLabel = sheet.Range(linha, colInicioLabel, linha, colFimLabel);

            if (colsLabel > 1) rangeLabel.Merge();

            rangeLabel.ConfigurarLabelComFundo(label);

            if (bordaDireita != XLBorderStyleValues.None)
                rangeLabel.Style.Border.RightBorder = bordaDireita;
        }

        private static void RenderizarSeparadorOu(IXLWorksheet sheet, int linha, int col)
        {
            var cell = sheet.Cell(linha, col);
            cell.ConfigurarLabelComFundo("OU");
        }

        private static void RenderizarSlotRetificacao(IXLWorksheet sheet, int linha, int slot, RetificacaoRelatorioCodafDto dto)
        {
            var (colLblData, colValData, colLblPag, colValPag) = slot switch
            {
                1 => ("C", "D:E", "F:G", "H"),
                2 => ("I", "J:K", "L", "M"),
                _ => ("N", "O:P", "Q:R", "S:T")
            };

            var dataTexto = dto?.Data.ToString("dd/MM/yyyy") ?? "";
            var pagTexto = dto?.NumeroPagina.ToString() ?? "";

            CriarCampoChaveValor(sheet, linha, colLblData, "DATA:", colValData, dataTexto);
            CriarCampoChaveValor(sheet, linha, colLblPag, "PÁGINA:", colValPag, pagTexto);
        }

        private static string FormatarDatasAulas(List<DateTime> datas)
        {
            if (datas == null || !datas.Any()) return "";

            var datasFormatadas = datas.Select(d => d.ToString("dd/MM")).ToList();
            if (datasFormatadas.Count == 1) return datasFormatadas[0];

            var todasMenosUltima = string.Join(", ", datasFormatadas.Take(datasFormatadas.Count - 1));
            var ultima = datasFormatadas.Last();
            return $"{todasMenosUltima} e {ultima}";
        }

        private static void CriarLinhaPadrao(IXLWorksheet sheet, int linha, string label, string valor,
            bool bordaDireitaGrossa = false, bool alinharEsquerda = false)
        {
            CriarCampoChaveValor(sheet, linha, "A:B", label, "C:T", valor, bordaDireitaValor: bordaDireitaGrossa);
            if (alinharEsquerda)
            {
                sheet.ObterRange("C:T", linha).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            }
        }

        private static void CriarCampoChaveValor(IXLWorksheet sheet, int linha,
            string colsLabel, string textoLabel,
            string colsValor, string textoValor,
            bool labelNegrito = false, bool bordaDireitaValor = false)
        {
            var rangeLabel = sheet.ObterRange(colsLabel, linha);
            rangeLabel.ConfigurarLabelComFundo(textoLabel);
            rangeLabel.EstilizarLabel(negrito: labelNegrito);
            rangeLabel.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            rangeLabel.Style.Border.OutsideBorderColor = XLColor.Black;

            if (!string.IsNullOrEmpty(colsValor))
            {
                var rangeValor = sheet.ObterRange(colsValor, linha);
                rangeValor.Value = textoValor;
                rangeValor.EstilizarValor(bordaDireitaValor ? XLBorderStyleValues.Thin : XLBorderStyleValues.None, centralizar: true);
                rangeValor.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                rangeValor.Style.Border.RightBorder = XLBorderStyleValues.Thin;
            }
        }
    }
}