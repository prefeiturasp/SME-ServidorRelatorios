
using System;
using System.Collections.Generic;
using System.Linq;

namespace SME.SR.Infra
{
    public class RelatorioFechamentoPendenciasPendenciaDto
    {
        public RelatorioFechamentoPendenciasPendenciaDto()
        {
            
        }        
        public string DescricaoPendencia { get; set; }
        public string DetalhamentoPendencia { get; set; }
        public string Situacao { get; set; }
        public string NomeUsuario { get; set; }
        public string CodigoUsuarioRf { get; set; }
        public string NomeUsuarioAprovacao { get; set; }
        public string CodigoUsuarioAprovacaoRf { get; set; }

        public string[] DetalhamentoPendenciaArray { get { return SplitInParts(DetalhamentoPendencia).ToArray(); }   }


        public IEnumerable<string> SplitInParts(string s)
        {
            for (var i = 0; i < s.Length; i += 124)
                yield return s.Substring(i, Math.Min(124, s.Length - i));
        }

    }
}
