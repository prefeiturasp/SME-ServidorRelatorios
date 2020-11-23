namespace SME.SR.Infra
{
    public class UeConclusaoPorAlunoAno
    {
        public long AlunoCodigo { get; set; }

        public string UeCodigo { get; set; }

        public string UeNome { get; set; }

        public string TurmaAno { get; set; }

        public string UeMunicipio
        {
            get
            {
                return "São Paulo";
            }
        }
 
        public string UeUF
        {
            get
            {
                return "SP";
            }
        }
    }
}
