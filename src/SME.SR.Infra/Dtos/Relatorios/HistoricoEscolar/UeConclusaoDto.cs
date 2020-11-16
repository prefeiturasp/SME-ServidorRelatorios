namespace SME.SR.Infra
{
    public  class UeConclusaoDto
    {
        public int Ano { get; set; }
        public string UeNome { get; set; }
        public string UeMunicipio { get { return "São Paulo"; }  }
        public string UeUf { get { return "SP"; } }
    }
}
