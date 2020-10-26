using System;

namespace SME.SR.Infra
{
    public class SondagemAutoralPorAlunoDto
    {
        public long OrdemId { get; set; }

        public string OrdemDescricao { get; set; }

        public long CodigoAluno { get; set; }

        public string NomeAluno { get; set; }

        public Guid PerguntaId { get; set; }

        public string PerguntaDescricao { get; set; }

        public Guid RespostaId { get; set; }

        public string RespostaDescricao { get; set; }
    }
}
