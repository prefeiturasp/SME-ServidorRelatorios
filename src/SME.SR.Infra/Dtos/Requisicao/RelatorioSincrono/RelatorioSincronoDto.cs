using static SME.SR.Infra.Enumeradores;

namespace SME.SR.Infra.Dtos
{
    public class RelatorioSincronoDto : ExecutarRelatorioSincronoDto
    {        
        public string CaminhoCompleto
        {
            get
            {
                return string.Concat(CaminhoRelatorio, '.', Formato.ToString());
            }
        }
                
        public FormatoEnum Formato { get; set; }

        public string CaminhoRelatorio { get; set; }

    }
}
