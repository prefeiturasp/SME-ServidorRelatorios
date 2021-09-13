using SME.SR.Infra.Utilitarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SME.SR.Infra
{
    public class ConselhoClasseConsolidadoTurmaDto
    {
        public string TurmaCodigo { get; set; }
        public string NomeUe { get; set; }
        public string NomeTurma { get; set; }

        public int Bimestre { get; set; }

        public long ModalidadeCodigo { get; set; }
        public string SomatoriaStatus { get; set; }
        public string NomeModalidade { get; set; }
        public string NomeTurmaFormatado { get => ObterNomeTurmaFormatado(); }

        private string ObterNomeTurmaFormatado()
        {
            var modalidade = Enum.GetValues(typeof(Modalidade))
                                .Cast<Modalidade>()
                                .Where(d => ((long)d) == ModalidadeCodigo)
                                .Select(d => new { name = d.Name(), shortName = d.ShortName() })
                                .FirstOrDefault();
            NomeModalidade = modalidade.name;
            return modalidade.shortName + " - " + NomeTurma;
        }
    }
}
