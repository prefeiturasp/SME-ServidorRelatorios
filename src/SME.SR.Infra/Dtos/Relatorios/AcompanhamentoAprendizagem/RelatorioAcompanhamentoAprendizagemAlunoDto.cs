using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioAcompanhamentoAprendizagemAlunoDto
    {
        public RelatorioAcompanhamentoAprendizagemAlunoDto()
        {
            Frequencias = new List<RelatorioAcompanhamentoAprendizagemAlunoFrequenciaDto>();
            Ocorrencias = new List<RelatorioAcompanhamentoAprendizagemAlunoOcorrenciaDto>();
        }
        public string Nome { get; set; }
        public string DataNascimento { get; set; }
        public string CodigoEol { get; set; }
        public string Responsavel { get; set; }
        public string Telefone { get; set; }
        public string Situacao { get; set; }
        public string Observacoes { get; set; }
        public string NomeEol { get; set; }
        public string PercursoIndividual { get; set; }
        public string PercursoColetivoTurma { get; set; }
        public Modalidade ModalidadeTurma { get; set; }

        public List<RelatorioAcompanhamentoAprendizagemAlunoFrequenciaDto> Frequencias { get; set; }
        public List<RelatorioAcompanhamentoAprendizagemAlunoOcorrenciaDto> Ocorrencias { get; set; }
    }
}