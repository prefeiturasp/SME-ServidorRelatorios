using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class ParecerConclusivo
    {
        public static ParecerConclusivo Promovido = new ParecerConclusivo("Promovido", "Promovido");
        public static ParecerConclusivo PromovidoConselho = new ParecerConclusivo("Promovido pelo conselho", "Prom Cons");
        public static ParecerConclusivo ContinuidadeEstudos = new ParecerConclusivo("Continuidade dos estudos", "Continuidade");
        public static ParecerConclusivo Retido = new ParecerConclusivo("Retido", "Retido");
        public static ParecerConclusivo RetidoFrequencia = new ParecerConclusivo("Retido por frequência", "Ret Freq");
        
        public readonly string Nome;
        public readonly string Sigla;

        public static string ObterSiglaParecer(string nome)
        {
            return Pareceres.Find(parecer => parecer.Nome == nome)?.Sigla;
        }

        private static List<ParecerConclusivo> Pareceres = new List<ParecerConclusivo>() { Promovido, PromovidoConselho, ContinuidadeEstudos, Retido, RetidoFrequencia };

        private ParecerConclusivo(string nome, string sigla)
        {
            Nome = nome;
            Sigla = sigla;
        }
    }
}
