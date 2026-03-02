using System.Collections.Generic;

namespace SME.SR.Infra.Dtos.NovoSondagem
{
    /// <summary>
    /// Mapeia exatamente o JSON retornado pela API externa de sondagem.
    /// NÃO confundir com os DTOs internos do relatório (QuestionarioSondagemRelatorioDto, EstudanteQuestionarioDto).
    /// </summary>
    public class RetornoApiSondagemQuestionarioDto
    {
        public string TituloTabelaRespostas { get; set; } = string.Empty;
        public string Semestre { get; set; } = string.Empty;
        public IEnumerable<EstudanteApiSondagemDto>? Estudantes { get; set; }
        public IEnumerable<LegendaApiSondagemDto>? Legenda { get; set; }
    }

    /// <summary>
    /// Estudante conforme retornado pela API — tem tanto "nome" quanto "nomeRelatorio".
    /// O UseCase usa NomeRelatorio para alimentar EstudanteQuestionarioDto.Nome da view.
    /// </summary>
    public class EstudanteApiSondagemDto
    {
        public string NumeroAlunoChamada { get; set; } = string.Empty;
        public bool LinguaPortuguesaSegundaLingua { get; set; }
        public int Codigo { get; set; }
        public string Raca { get; set; } = string.Empty;
        public string Genero { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public string NomeRelatorio { get; set; } = string.Empty;  // nome com código, ex: "AGATHA... (8356111)"
        public bool Pap { get; set; }
        public bool Aee { get; set; }
        public bool PossuiDeficiencia { get; set; }
        public IEnumerable<ColunaQuestionarioDto>? Coluna { get; set; }
    }

    /// <summary>
    /// Legenda retornada pela API — lista de opções de resposta disponíveis.
    /// </summary>
    public class LegendaApiSondagemDto
    {
        public int Id { get; set; }
        public int Ordem { get; set; }
        public string DescricaoOpcaoResposta { get; set; } = string.Empty;
        public string? Legenda { get; set; }
        public string? CorFundo { get; set; }
        public string? CorTexto { get; set; }
    }
}