﻿using System;

namespace SME.SR.Infra
{
    public class SondagemAutoralPorAlunoDto
    {
        public string OrdemId { get; set; }

        public string OrdemDescricao { get; set; }

        public long CodigoAluno { get; set; }

        public string NomeAluno { get; set; }

        public string PerguntaId { get; set; }

        public string PerguntaDescricao { get; set; }

        public string RespostaId { get; set; }

        public string RespostaDescricao { get; set; }
    }
}
