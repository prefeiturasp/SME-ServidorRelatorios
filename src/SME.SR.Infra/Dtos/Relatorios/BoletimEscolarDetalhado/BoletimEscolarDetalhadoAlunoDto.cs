using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class BoletimEscolarDetalhadoAlunoDto
    {
        public string TipoNota { get; set; } = "Nota";
        public BoletimEscolarDetalhadoCabecalhoDto Cabecalho { get; set; }
        public bool PossuiNotaFinalRegencia { get; set; }
        public bool PossuiNotaFinal { get; set; }

        public ComponenteCurricularRegenciaDto ComponenteCurricularRegencia { get; set; }

        public List<GrupoMatrizComponenteCurricularDto> Grupos { get; set; }

        public string ParecerConclusivo { get; set; }

        public string RecomendacoesEstudante { get; set; }

        public string RecomendacoesFamilia { get; set; }

        public bool ExibirRecomendacoes { get; set; }

        public BoletimEscolarDetalhadoAlunoDto()
        {
            Cabecalho = new BoletimEscolarDetalhadoCabecalhoDto();
            Grupos = new List<GrupoMatrizComponenteCurricularDto>();
        }
    }
}
