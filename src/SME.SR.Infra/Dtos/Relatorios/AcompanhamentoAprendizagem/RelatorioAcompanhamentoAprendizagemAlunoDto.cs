using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioAcompanhamentoAprendizagemAlunoDto
    {
        public RelatorioAcompanhamentoAprendizagemAlunoDto()
        {
            Fotos = new List<RelatorioAcompanhamentoAprendizagemAlunoFotoDto>();
            Frequencias = new List<RelatorioAcompanhamentoAprendizagemAlunoFrequenciaDto>();
            Ocorrencias = new List<RelatorioAcompanhamentoAprendizagemAlunoOcorrenciaDto>();
            PercursoTurmaImagens = new List<AcompanhamentoAprendizagemPercursoTurmaImagemDto>();
        }
        public string Nome { get; set; }
        public string DataNascimento { get; set; }
        public string CodigoEol { get; set; }
        public string Responsavel { get; set; }
        public string Telefone { get; set; }
        public string RegistroPercursoTurma { get; set; }
        public string Observacoes { get; set; }
        public string NomeEol { get; set; }
        public string PercursoIndividual { get; set; }

        public List<RelatorioAcompanhamentoAprendizagemAlunoFotoDto> Fotos { get; set; }
        public List<RelatorioAcompanhamentoAprendizagemAlunoFrequenciaDto> Frequencias { get; set; }
        public List<RelatorioAcompanhamentoAprendizagemAlunoOcorrenciaDto> Ocorrencias { get; set; }
        public List<AcompanhamentoAprendizagemPercursoTurmaImagemDto> PercursoTurmaImagens { get; set; }
        public string Situacao { get; set; }
    }
}