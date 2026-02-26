using System.Collections.Generic;

namespace SME.SR.Infra.Dtos.NovoSondagem
{
    public class EstudanteQuestionarioDto
    {
        public string NumeroAlunoChamada { get; set; } = string.Empty;
        public bool LinguaPortuguesaSegundaLingua { get; set; }
        public int Codigo { get; set; }
        public string Raca { get; set; } = string.Empty;
        public string Genero { get; set; } = string.Empty;
        public string NomeRelatorio { get; set; } = string.Empty;
        public bool Pap { get; set; }
        public bool Aee { get; set; }
        public bool PossuiDeficiencia { get; set; }
        public IEnumerable<ColunaQuestionarioDto>? Coluna { get; set; }
    }
}