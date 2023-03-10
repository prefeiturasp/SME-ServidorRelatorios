using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class QuestaoEncaminhamentoNAAPADetalhadoDto
    {
        public string Questao { get; set; }
        public long QuestaoId { get; set; }
        public TipoQuestao TipoQuestao { get; set; }
        public int Ordem { get; set; }
        public string OrdemMascara { get; set; }
        public string Resposta { get; set; }
        public string NomeComponente { get; set; }
        public IEnumerable<EnderecoDto> Enderecos { get; set; }
        public IEnumerable<ContatoResponsaveisDto> ContatoResponsaveis { get; set; }
        public IEnumerable<AtividadeContraTurnoDto> AtividadeContraTurnos { get; set; }
        public IEnumerable<TurmaProgramaDto> TurmasPrograma { get; set; }
    }
}
