using System;
using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class InformacaoEscolarAlunoDto
    {
        public InformacaoEscolarAlunoDto()
        {}

        public string CodigoAluno { get; set; }
        public int TipoNecessidadeEspecial { get; set; }
        public string DescricaoNecessidadeEspecial { get; set; }
        public int TipoRecurso { get; set; }
        public string DescricaoRecurso { get; set; }
        public string FrequenciaGlobal { get; set; }
    }
}