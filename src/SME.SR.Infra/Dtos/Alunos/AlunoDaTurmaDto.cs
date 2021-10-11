namespace SME.SR.Infra
{
    public class AlunoDaTurmaDto
    {
        public int CodigoTurma { get; set; }
        public int CodigoAluno { get; set; }
        public string NomeAluno { get; set; }
        public string NomeSocialAluno { get; set; }
        public string NumeroAlunoChamada { get; set; }

        public string ObterNomeFinal()
        {
            if (string.IsNullOrEmpty(NomeSocialAluno))
                return NomeAluno;

            return NomeSocialAluno;            
        }
    }
}
