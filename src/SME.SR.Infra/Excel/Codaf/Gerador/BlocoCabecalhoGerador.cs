using ClosedXML.Excel;
using SME.SR.Infra.Dtos.Codaf;
using SME.SR.Infra.Extensions.Codaf;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SME.SR.Infra.Excel.Codaf.Gerador
{
    public class BlocoCabecalhoGerador : IBlocoCabecalhoGerador
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

            // Fechamento visual do cabeçalho
            var linhaFinalCabecalho = linha-1;
            var rangeCabecalho = sheet.Range($"A6:T{linhaFinalCabecalho}");
            rangeCabecalho.Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
            rangeCabecalho.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            sheet.Rows(6, linhaFinalCabecalho).Height = 32;

            return linha;
        }

        private static void RenderizarLinhaOpcoes(IXLWorksheet sheet, ref int linha, CabecalhoRelatorioCodafDto dados)
        {
            // Linha 6 - Opções com Checkboxes
            sheet.Range(linha, 1, linha, 20).Style.Border.OutsideBorder = XLBorderStyleValues.Thick;

            // Curso
            sheet.Cell(linha, 1).ConfigurarCheckbox(dados.EhCurso); // A
            sheet.Range(linha, 2, linha, 2).ConfigurarLabelComFundo("CURSO"); // B

            sheet.Cell(linha, 3).Value = "OU"; // C
            sheet.Cell(linha, 3).EstilizarValor();

            // Evento
            sheet.Cell(linha, 4).ConfigurarCheckbox(dados.EhEvento); // D
            var rangeEvento = sheet.Range(linha, 5, linha, 6); // E:F
            rangeEvento.Merge();
            rangeEvento.ConfigurarLabelComFundo("EVENTO");
            rangeEvento.Style.Border.RightBorder = XLBorderStyleValues.Thick;

            // A distância
            sheet.Cell(linha, 7).ConfigurarCheckbox(dados.EhDistancia); // G
            var rangeDistancia = sheet.Range(linha, 8, linha, 9); // H:I
            rangeDistancia.Merge();
            rangeDistancia.ConfigurarLabelComFundo("A DISTÂNCIA");
            rangeDistancia.Style.Border.RightBorder = XLBorderStyleValues.Thin;

            // Híbrido
            sheet.Cell(linha, 10).ConfigurarCheckbox(dados.EhHibrido); // J
            var cellHibrido = sheet.Cell(linha, 11); // K
            cellHibrido.ConfigurarLabelComFundo("HÍBRIDO");
            cellHibrido.Style.Border.RightBorder = XLBorderStyleValues.Thin;

            // Presencial
            sheet.Cell(linha, 12).ConfigurarCheckbox(dados.EhPresencial); // L
            var cellPresencial = sheet.Cell(linha, 13); // M
            cellPresencial.ConfigurarLabelComFundo("PRESENCIAL");
            cellPresencial.Style.Border.RightBorder = XLBorderStyleValues.Thick;

            // Com Certificação
            sheet.Cell(linha, 14).ConfigurarCheckbox(dados.ComCertificacao); // N
            var rangeComCertificacao = sheet.Range(linha, 15, linha, 16); // O:P
            rangeComCertificacao.Merge();
            rangeComCertificacao.ConfigurarLabelComFundo("COM CERTIFICAÇÃO");

            sheet.Cell(linha, 17).Value = "OU"; // Q
            sheet.Cell(linha, 17).EstilizarValor();

            // Sem Certificação
            sheet.Cell(linha, 18).ConfigurarCheckbox(dados.SemCertificacao); // R
            var rangeSemCertificacao = sheet.Range(linha, 19, linha, 20); // S:T
            rangeSemCertificacao.Merge();
            rangeSemCertificacao.ConfigurarLabelComFundo("SEM CERTIFICAÇÃO");
            linha++;

        }

        private static void RenderizarDadosDaFormação(IXLWorksheet sheet, ref int linha, CabecalhoRelatorioCodafDto dados)
        {
            // Linha 7 - Área Promotora
            CriarCampoChaveValor(sheet, linha++, "A:B", "ÁREA PROMOTORA:", "C:T", dados.AreaPromotora, bordaDireitaValor: true);
            var rangeLinha7 = sheet.Range(ObterRangeString("C:T", linha-1));
            rangeLinha7.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

            // Linha 8 - Nome Formação
            CriarCampoChaveValor(sheet, linha++, "A:B", "NOME DA FORMAÇÃO:", "C:T", dados.NomeFormacao, bordaDireitaValor: true);
            var rangeLinha8 = sheet.Range(ObterRangeString("C:T", linha - 1));
            rangeLinha8.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

            // Linha 9 - Homologação e Código
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
            int totalItens = retificacoes.Any() ? retificacoes.Count : 1;
            int qtdLinhas = (int)Math.Ceiling(totalItens / 3.0); // 3 slots por linha
            int linhaFinal = linhaInicial + qtdLinhas - 1;
            linha = linhaFinal + 1;

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
            // Linha "12" - Dados das Aulas
            var dataInicio = dados.DataPeriodoRealizacaoInicio.ToString("dd/MM");
            var dataFim = dados.DataPeriodoRealizacaoFim.ToString("dd/MM/yyyy");
            CriarCampoChaveValor(sheet, linha, "A:B", "PERÍODO DE REALIZAÇÃO:", "C:G", $"{dataInicio} a {dataFim}");
            
            var lista = dados.DataDasAulasSincronas
                 ?.Select(d => d.ToString("dd/MM"))
                 .ToList() ?? new List<string>();
            var datasAulas = lista.Count switch
            {
                0 => "",
                1 => lista[0],
                _ => $"{string.Join(", ", lista.Take(lista.Count - 1))} e {lista[^1]}"
            };
            CriarCampoChaveValor(sheet, linha++, "H:L", "DATAS DAS AULAS SÍNCRONAS/ PRESENCIAIS:", "M:T", datasAulas);
        }

        private static void RenderizarCargaHoraria(IXLWorksheet sheet, ref int linha, CabecalhoRelatorioCodafDto dados)
        {
            // Linha "13" - Carga Horária
            CriarCampoChaveValor(sheet, linha, "A:B", "CARGA HORÁRIA TOTAL:", "C:E", dados.CargaHorariaTotal.ToString()+'h', bordaDireitaValor: true);
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
            CriarCampoChaveValor(sheet, linha++, "A:B", "OBSERVAÇÕES:", "C:T", dados.Observacao);
        }

        private static void RenderizarSlotRetificacao(IXLWorksheet sheet, int linha, int slot, RetificacaoRelatorioCodafDto dto)
        {
            string colLabelData = slot == 1 ? "C" : (slot == 2 ? "I" : "N");
            string colValData = slot == 1 ? "D:E" : (slot == 2 ? "J:K" : "O:P");
            string colLabelPag = slot == 1 ? "F:G" : (slot == 2 ? "L" : "Q:R");
            string colValPag = slot == 1 ? "H" : (slot == 2 ? "M" : "S:T");

            CriarCampoChaveValor(sheet, linha, colLabelData, "DATA:", colValData, dto?.Data.ToString("dd/MM/yyyy") ?? "");
            CriarCampoChaveValor(sheet, linha, colLabelPag, "PÁGINA:", colValPag, dto?.NumeroPagina.ToString() ?? "");
        }
        private static void CriarCampoChaveValor(IXLWorksheet sheet, int linha,
            string colsLabel, string textoLabel,
            string colsValor, string textoValor,
            bool labelNegrito = false, bool bordaDireitaValor = false)
        {
            var rangeLabel = sheet.Range(ObterRangeString(colsLabel, linha));
            if (colsLabel.Contains(':')) rangeLabel.Merge();
            rangeLabel.Value = textoLabel;
            rangeLabel.EstilizarLabel(negrito: labelNegrito);
            rangeLabel.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            rangeLabel.Style.Border.OutsideBorderColor = XLColor.Black;

            if (!string.IsNullOrEmpty(colsValor))
            {
                var rangeValor = sheet.Range(ObterRangeString(colsValor, linha));
                if (colsValor.Contains(':')) rangeValor.Merge();
                rangeValor.Value = textoValor;
                rangeValor.EstilizarValor(bordaDireita: bordaDireitaValor);
                rangeValor.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                rangeValor.Style.Border.OutsideBorderColor = XLColor.Black;
            }
        }
        private static string ObterRangeString(string cols, int linha)
        {
            if (cols.Contains(':'))
            {
                var partes = cols.Split(':');
                return $"{partes[0]}{linha}:{partes[1]}{linha}";
            }
            return $"{cols}{linha}";
        }
    }
}