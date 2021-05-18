using SME.SR.Infra.Utilitarios;
using System.Collections.Generic;
using System.Linq;

namespace SME.SR.Infra
{
    public class AcompanhamentoAprendizagemAlunoDto
    {
        public AcompanhamentoAprendizagemAlunoDto()
        {
            Fotos = new List<AcompanhamentoAprendizagemAlunoFotoDto>();
        }

        public long Id { get; set; }
        public string AlunoCodigo { get; set; }
        public string ApanhadoGeral { get; set; }
        public string Observacoes { get; set; }
        public List<AcompanhamentoAprendizagemAlunoFotoDto> Fotos { get; set; }

        public void Add(AcompanhamentoAprendizagemAlunoFotoDto acompanhamentoAprendizagemAlunoFotoDto)
        {
            if (!Fotos.Any(a => a.Id == acompanhamentoAprendizagemAlunoFotoDto.Id))
                Fotos.Add(acompanhamentoAprendizagemAlunoFotoDto);

        }

        public string ObservacoesFormatado()
        {
            if (string.IsNullOrEmpty(Observacoes))
                return string.Empty;

            var registroFormatado = UtilRegex.RemoverTagsHtmlMidia(Observacoes);
            return UtilRegex.RemoverTagsHtml(registroFormatado);
        }
    }
}
