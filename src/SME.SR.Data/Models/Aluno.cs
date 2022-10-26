using SME.SR.Infra;
using SME.SR.Infra.Utilitarios;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SME.SR.Data
{
    public class Aluno
    {
        public string ObterNomeFinal()
        {
            if (string.IsNullOrEmpty(NomeSocialAluno))
                return NomeAluno;
            else return NomeSocialAluno;
        }

        public int CodigoTurma { get; set; }
        public int CodigoAluno { get; set; }
        public string NomeAluno { get; set; }
        public string NomeSocialAluno { get; set; }
        public DateTime DataNascimento { get; set; }
        public SituacaoMatriculaAluno CodigoSituacaoMatricula { get; set; }
        public string SituacaoMatricula { get; set; }
        public DateTime DataSituacao { get; set; }
        public string NumeroAlunoChamada { get; set; }
        public bool PossuiDeficiencia { get; set; }

        public string SituacaoRelatorio =>
            $"{SituacaoMatricula} em {DataSituacao:dd/MM/yyyy}".ToUpper();

        public string NomeRelatorio =>
             $"{Convert.ToInt32(NumeroAlunoChamada)} - {(NomeSocialAluno ?? NomeAluno)} " +
             $"{(SituacaoEspecial ? $"({CodigoSituacaoMatricula.GetAttribute<DisplayAttribute>().Name})" : "")}";

        public string NomeSemNumRelatorio =>
             $"{(NomeSocialAluno ?? NomeAluno)} " +
             $"{(SituacaoEspecial ? $"({CodigoSituacaoMatricula.GetAttribute<DisplayAttribute>().Name})" : "")}";

        public bool EstaAtivo(DateTime dataBase) => SituacoesAtiva.Contains(CodigoSituacaoMatricula) || dataBase.Date <= DataSituacao.Date;

        public bool Ativo => SituacoesAtiva.Contains(CodigoSituacaoMatricula);

        private SituacaoMatriculaAluno[] SituacoesEspeciais => new[] { SituacaoMatriculaAluno.Transferido,
                        SituacaoMatriculaAluno.RemanejadoSaida,
                        SituacaoMatriculaAluno.ReclassificadoSaida,
                        SituacaoMatriculaAluno.Desistente };


        private SituacaoMatriculaAluno[] SituacoesAtiva => new[] { SituacaoMatriculaAluno.Ativo,
                        SituacaoMatriculaAluno.Rematriculado,
                        SituacaoMatriculaAluno.PendenteRematricula,
                        SituacaoMatriculaAluno.SemContinuidade,
                        SituacaoMatriculaAluno.Concluido};

        private SituacaoMatriculaAluno[] SituacoesAtivaSondagem => new[] { SituacaoMatriculaAluno.Ativo,
                        SituacaoMatriculaAluno.Rematriculado,
                        SituacaoMatriculaAluno.PendenteRematricula,
                        SituacaoMatriculaAluno.SemContinuidade,
            SituacaoMatriculaAluno.Concluido};

        public string ObterNomeParaRelatorioSondagem()
        {
            var nome = ObterNomeFinal();
            if (!SituacoesAtivaSondagem.Contains(CodigoSituacaoMatricula))
                return $"{nome} ({CodigoSituacaoMatricula.GetAttribute<DisplayAttribute>().Name} em {DataSituacao.ToString("dd/MM/yyyy")})";
            else return nome;
        }

        private bool SituacaoEspecial => !SituacoesAtiva.Contains(CodigoSituacaoMatricula) || SituacoesEspeciais.Contains(CodigoSituacaoMatricula);
    }
}
