using System.Text.RegularExpressions;

namespace SME.SR.Infra
{
    public class AcompanhamentoAprendizagemAlunoDto
    {
        public string PercursoColetivoTurma { get; set; }
        public string Semestre { get; set; }
        public string AlunoCodigo { get; set; }
        public string Observacoes { get; set; }
        public string PercursoIndividual { get; set; }

        public string ObservacoesFormatado()
        {
            if (string.IsNullOrEmpty(Observacoes))
                return "";
            var str = Regex.Replace(Observacoes, @"-\d+px", "0px");
            return str;
        }

        public string PercursoColetivoTurmaFormatado()
        {
            if (string.IsNullOrEmpty(PercursoColetivoTurma))
                return "";
            var str = Regex.Replace(PercursoColetivoTurma, @"-\d+px", "0px");
            return str;
        }

        public string PercusoIndividualFormatado()
        {
            if (string.IsNullOrEmpty(PercursoIndividual))
                return "";
            var str = Regex.Replace(PercursoIndividual, @"-\d+px", "0px");
            return str;
        }
    }
}
