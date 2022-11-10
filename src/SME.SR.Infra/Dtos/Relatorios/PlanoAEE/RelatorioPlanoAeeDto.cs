namespace SME.SR.Infra
{
    public class RelatorioPlanoAeeDto
    {
        public RelatorioPlanoAeeDto()
        {
            Cabecalho = new CabecalhoPlanoAeeDto();
            Cadastro = new CadastroPlanoAeeDto();
            Parecer = new ParecerPlanoAeeDto();
        }        
        
        public CabecalhoPlanoAeeDto Cabecalho { get; set; }
        public CadastroPlanoAeeDto Cadastro { get; set; }
        public ParecerPlanoAeeDto Parecer { get; set; }
    }
}