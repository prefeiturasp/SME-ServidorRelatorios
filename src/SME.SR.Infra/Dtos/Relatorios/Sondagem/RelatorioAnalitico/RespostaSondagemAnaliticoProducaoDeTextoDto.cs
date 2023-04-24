using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class RespostaSondagemAnaliticoProducaoDeTextoDto : RelatorioSondagemAnaliticoDto
    {
        public int NaoProduziuEntregouEmBranco { get; set; }
        public int NaoApresentouDificuldades { get; set; }
        public int EscritaNaoAlfabetica { get; set; }
        public int DificuldadesComAspectosSemanticos { get; set; }
        public int DificuldadesComAspectosTextuais { get; set; }
        public int DificuldadesComAspectosOrtograficosNotacionais { get; set; }
        public int SemPreenchimento { get; set; }
    }
}
