using SME.SR.Infra;
using SME.SR.Infra.Utilitarios;
using System.ComponentModel.DataAnnotations;

namespace SME.SR.Data
{
    public class Turma
    {
        public string Codigo { get; set; }
        public int AnoLetivo { get; set; }
        public int Ano { get; set; }
        public string Nome { get; set; }
        public int Semestre { get; set; }
        public Dre Dre { get; set; }
        public Ue Ue { get; set; }
        public Modalidade ModalidadeCodigo { get; set; }
        public ModalidadeTipoCalendario ModalidadeTipoCalendario
        {
            get => ModalidadeCodigo == Modalidade.EJA ?
                ModalidadeTipoCalendario.EJA :
                ModalidadeTipoCalendario.FundamentalMedio;
        }

        public bool EhEja
        {
            get => ModalidadeCodigo == Modalidade.EJA;
        }

        public string NomeRelatorio => $"{ModalidadeCodigo.GetAttribute<DisplayAttribute>().ShortName} - {Nome}";
    }
}
