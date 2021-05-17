using System;
using System.IO;

namespace SME.SR.Infra
{
    public class AcompanhamentoAprendizagemAlunoFotoDto
    {
        public string TipoArquivo { get; set; }
        public Guid Codigo { get; set; }
        public string NomeOriginal { get; set; }
        public string Extensao { get => NomeOriginal != String.Empty ? NomeOriginal.Split(".")[NomeOriginal.Split(".").Length - 1] : ""; }
    }
}
