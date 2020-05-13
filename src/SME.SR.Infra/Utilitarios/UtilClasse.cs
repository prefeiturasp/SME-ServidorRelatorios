using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra.Utilitarios
{
    public static class UtilClasse
    {
        public static IDictionary<string, string> ClasseParaDicionario<T>(T classe)
        {
            Dictionary<string, string> retorno = new Dictionary<string, string>();

            foreach (var propriedade in classe.GetType().GetProperties())
            {
                var valor = propriedade.GetValue(classe);

                if (valor != null)
                    retorno.Add(propriedade.Name, valor.ToString());
            }

            return retorno;
        }
    }
}
