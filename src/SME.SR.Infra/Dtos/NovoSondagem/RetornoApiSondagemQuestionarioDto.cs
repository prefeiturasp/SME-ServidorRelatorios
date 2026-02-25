using System.Collections.Generic;

namespace SME.SR.Infra.Dtos.NovoSondagem
{
    public class RetornoApiSondagemQuestionarioDto
    {
        public string TituloTabelaRespostas { get; set; } = string.Empty;
        public string Semestre { get; set; } = string.Empty;
        public IEnumerable<EstudanteQuestionarioDto> Estudantes { get; set; }
    }
}