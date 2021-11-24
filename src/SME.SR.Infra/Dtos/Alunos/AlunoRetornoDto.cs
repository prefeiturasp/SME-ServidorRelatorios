using SME.SR.Infra.Utilitarios;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SME.SR.Infra
{
    public class AlunoRetornoDto
    {
        public int TurmaCodigo { get; set; }
        public int AlunoCodigo { get; set; }
        public string NomeAluno { get; set; }
        public string NomeSocialAluno { get; set; }
        public DateTime DataNascimento { get; set; }
        public SituacaoMatriculaAluno CodigoSituacaoMatricula { get; set; }
        public string SituacaoMatricula { get; set; }
        public string NumeroAlunoChamada { get; set; }
        public DateTime DataSituacao { get; set; }
        public string ResponsavelNome { get; set; }
        public string ResponsavelDDD { get; set; }
        public string ResponsavelCelular { get; set; }
        public DateTime DataAtualizacaoContato { get; set; }
        public TipoResponsavel TipoResponsavel { get; set; }
        public bool Ativo => new[] { SituacaoMatriculaAluno.Ativo,
                       SituacaoMatriculaAluno.PendenteRematricula,
                       SituacaoMatriculaAluno.Rematriculado,
                       SituacaoMatriculaAluno.SemContinuidade,
                       SituacaoMatriculaAluno.Concluido }.Contains(this.CodigoSituacaoMatricula);
        public string NomeRelatorio =>
             $"Nº{Convert.ToInt32(NumeroAlunoChamada)} - {(NomeSocialAluno ?? NomeAluno)}".ToUpper();

        public string SituacaoRelatorio =>
            $"{CodigoSituacaoMatricula.GetAttribute<DisplayAttribute>().Name} em {DataSituacao:dd/MM/yyyy}".ToUpper();

        public string ResponsavelFormatado()
        {
            if (string.IsNullOrEmpty(ResponsavelNome))
                return string.Empty;

            return $"{ResponsavelNome} ({TipoResponsavel.GetAttribute<DisplayAttribute>().Name})".ToUpper();
        }

        public string ResponsavelCelularFormatado()
        {
            if (string.IsNullOrEmpty(ResponsavelDDD) || string.IsNullOrEmpty(ResponsavelCelular))
                return string.Empty;
            if(ResponsavelCelular.Length < 9)
                return $"({ResponsavelDDD.Trim()}) {ResponsavelCelular.Trim()} (Atualizado - {DataAtualizacaoContato:dd/MM/yyyy})";

            return $"({ResponsavelDDD.Trim()}) {ResponsavelCelular.Substring(0, 5).Trim()}-{ResponsavelCelular.Substring(5, 4).Trim()} (Atualizado - {DataAtualizacaoContato:dd/MM/yyyy})";
        }

        public string DataNascimentoFormatado()
        {
            return $"{DataNascimento:dd/MM/yyyy}";
        }
    }
}
