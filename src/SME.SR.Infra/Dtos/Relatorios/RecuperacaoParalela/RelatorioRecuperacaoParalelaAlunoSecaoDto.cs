using SME.SR.Infra.Extensions;
using SME.SR.Infra.Utilitarios;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SME.SR.Infra
{
    public class RelatorioRecuperacaoParalelaAlunoSecaoDto
    {
        public RelatorioRecuperacaoParalelaAlunoSecaoDto(string secaoNome, string secaoValor)
        {
            SecaoNome = secaoNome;
            SecaoValor = UtilRegex.RemoverTagsHtmlMidia(secaoValor);
            SecaoValor = UtilRegex.RemoverTagsHtml(SecaoValor);            
        }

        public string SecaoNome { get; set; }
        public string SecaoValor { get; set; }

        public string[] SecaoValorArray { get { return SplitInParts(SecaoValor).ToArray(); } }

        public IEnumerable<string> SplitInParts(string s)
        {
            for (var i = 0; i < s.Length; i += 124)
                yield return s.Substring(i, Math.Min(124, s.Length - i));
        }


    }
}
