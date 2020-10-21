using System;

namespace SME.SR.Data
{
    public class AulaPrevistaBimestreQuantidade
    {
        public string TurmaNome { get; set; }

        public long ComponenteCurricularId { get; set; }

        public string ComponenteCurricularNome { get; set; }

        public int Bimestre { get; set; }

        public DateTime DataInicio { get; set; }

        public DateTime DataFim { get; set; }

        public int Previstas { get; set; }

        public int CriadasTitular { get; set; }

        public int CriadasCJ { get; set; }

        public int CumpridasTitular { get; set; }

        public int CumpridasCj { get; set; }

        public int Reposicoes { get; set; }

        public bool Divergencias { get; set; }

    }
}
