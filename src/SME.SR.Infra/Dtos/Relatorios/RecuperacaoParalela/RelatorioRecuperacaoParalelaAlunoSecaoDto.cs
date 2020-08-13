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
            SecaoValor = secaoValor;
        }

        public string SecaoNome { get; set; }
        public string SecaoValor { get; set; }

        public string[] SecaoValorArray { get { return SplitInParts(LimparString(SecaoValor)).ToArray(); } }

        public static string LimparString(string str)
        {
            str = str.ToLower().Replace("<br>", " ");
            str = str.ToLower().Replace("<p>", "");
            str = str.ToLower().Replace("</p>", "");
            return str;
        }

        public IEnumerable<string> SplitInParts(string s)
        {
            for (var i = 0; i < s.Length; i += 124)
                yield return s.Substring(i, Math.Min(124, s.Length - i));
        }


    }
}
