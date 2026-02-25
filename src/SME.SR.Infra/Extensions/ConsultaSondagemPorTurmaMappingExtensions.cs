using SME.SR.Infra.Dtos.SondagemTurmaEscritaEF;
using System;
using System.Linq;

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
            return new EscritaEfTurmaSondagemCabecalhoExcelDto
            {
                AnoLetivo = anoLetivo,
                Semestre = source.Semestre,
                Turma = turma,
                Ue = ue,
                Dre = dre,
                Modalidade = modalidade,
                Proeficiencia = "Escrita",
                DataImpressao = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                NomeUsuarioSolicitacao = nomeUsuarioSolicitacao,
                CorpoRelatorio = source.Estudantes?
                    .Select((estudante, index) => estudante.MapToEscritaEfTurmaSondagemCorpoExcelDto(index + 1))
                    .ToList()
            };
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

            return new EscritaEfTurmaSondagemCorpoExcelDto
            {
                Numero = estudante.NumeroAlunoChamada,
                Nome = estudante.Nome,
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
        }

        private static string ObterDescricaoOpcaoResposta(ColunaDto coluna)
        {
            if (coluna?.Resposta?.OpcaoRespostaId == null || coluna.Resposta.OpcaoRespostaId == 0)
                return string.Empty;

            var opcao = coluna.OpcaoResposta?.FirstOrDefault(o => o.Id == coluna.Resposta.OpcaoRespostaId);

            return opcao?.DescricaoOpcaoResposta ?? string.Empty;
        }
    }

}
