using SME.SR.Infra.Utilitarios;
using System.Collections.Generic;
using System.Linq;

namespace SME.SR.Infra
{
    public class AcompanhamentoAprendizagemAlunoDto
    {
        public AcompanhamentoAprendizagemAlunoDto()
        {
            Fotos = new List<ArquivoDto>();
        }

        public long Id { get; set; }
        public string AlunoCodigo { get; set; }
        public string ApanhadoGeral { get; set; }
        public string Observacoes { get; set; }
        public List<ArquivoDto> Fotos { get; set; }

        public void Add(ArquivoDto acompanhamentoAprendizagemAlunoFotoDto)
        {
            if (!Fotos.Any(a => a.Codigo == acompanhamentoAprendizagemAlunoFotoDto.Codigo))
                Fotos.Add(acompanhamentoAprendizagemAlunoFotoDto);
        }

        public string ObservacoesFormatado()
        {
            if (string.IsNullOrEmpty(Observacoes))
                return string.Empty;

            var registroFormatado = UtilRegex.RemoverTagsHtmlMultiMidia(Observacoes);
            return UtilRegex.RemoverTagsHtml(registroFormatado);
        }
    }
}
