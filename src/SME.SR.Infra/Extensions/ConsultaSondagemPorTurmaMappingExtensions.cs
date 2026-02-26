using SME.SR.Infra.Dtos.SondagemTurmaEscritaEF;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace SME.SR.Infra.Extensions
{
    public static class ConsultaSondagemPorTurmaMappingExtensions
    {
        public static EscritaEfTurmaSondagemCabecalhoExcelDto MapToEscritaEfTurmaSondagemCabecalhoExcelDto(
            this ConsultaSondagemPorTurmaDto source,
            int anoLetivo,
            string turma,
            string ue,
            string dre,
            string modalidade,
            string nomeUsuarioSolicitacao)
        {

            var dto = new EscritaEfTurmaSondagemCabecalhoExcelDto
            {
                AnoLetivo = anoLetivo,
                Semestre = source.Semestre,
                Turma = turma,
                Ue = ue,
                Dre = dre,
                Modalidade = modalidade,
                Proeficiencia = source.TituloTabelaRespostas,
                DataImpressao = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                NomeUsuarioSolicitacao = nomeUsuarioSolicitacao,
                CorpoRelatorio = source.Estudantes != null ? source.Estudantes?
                    .Select((estudante, index) => estudante.MapToEscritaEfTurmaSondagemCorpoExcelDto(index + 1))
                    .ToList() : new List<EscritaEfTurmaSondagemCorpoExcelDto>()
            };

            return dto;
        }

        private static EscritaEfTurmaSondagemCorpoExcelDto MapToEscritaEfTurmaSondagemCorpoExcelDto(
            this EstudanteDto estudante,
            int numero)
        {
            var colunaInicial = estudante.Coluna?.FirstOrDefault(c => c.IdCiclo == 1);
            var coluna1Bimestre = estudante.Coluna?.FirstOrDefault(c => c.IdCiclo == 2);
            var coluna2Bimestre = estudante.Coluna?.FirstOrDefault(c => c.IdCiclo == 3);
            var coluna3Bimestre = estudante.Coluna?.FirstOrDefault(c => c.IdCiclo == 4);
            var coluna4Bimestre = estudante.Coluna?.FirstOrDefault(c => c.IdCiclo == 5);

            var opcaoRespostaAtiva = colunaInicial?.OpcaoResposta?.FirstOrDefault(o =>
                o.Id == colunaInicial.Resposta?.OpcaoRespostaId);

            var dto = new EscritaEfTurmaSondagemCorpoExcelDto
            {
                Numero = estudante.NumeroAlunoChamada,
                Nome = estudante.NomeRelatorio,
                Raca = estudante.Raca,
                Genero = estudante.Genero,
                LpComoLinguaPrincipal = estudante.LinguaPortuguesaSegundaLingua ? "Sim" : "Não",
                SondagemInicial = ObterDescricaoOpcaoResposta(colunaInicial),
                PrimeiroBimestre = ObterDescricaoOpcaoResposta(coluna1Bimestre),
                SegundoBimestre = ObterDescricaoOpcaoResposta(coluna2Bimestre),
                TerceiroBimestre = ObterDescricaoOpcaoResposta(coluna3Bimestre),
                QuartoBimestre = ObterDescricaoOpcaoResposta(coluna4Bimestre),
                Cor = opcaoRespostaAtiva?.CorFundo
            };

            return dto;
        }
        private static string ObterDescricaoOpcaoResposta(ColunaDto coluna)
        {
            if (coluna?.Resposta?.OpcaoRespostaId == null || coluna.Resposta.OpcaoRespostaId == 0)
                return "Vazio";

            var opcao = coluna.OpcaoResposta?.FirstOrDefault(o => o.Id == coluna.Resposta.OpcaoRespostaId);

            return opcao?.DescricaoOpcaoResposta ?? "Vazio";
        }
    }

}
