using SME.SR.Infra.Utilitarios;
using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioFrequenciaTurmaAnoDto
    {
        public RelatorioFrequenciaTurmaAnoDto()
        {
            Bimestres = new List<RelatorioFrequenciaBimestreDto>();
        }

        public string Nome { get; set; }
        public Modalidade ModalidadeCodigo { get; set; }
        public string Ano { get; set; }

        public string NomeTurmaAno
        { 
            get => EhExibirTurma
                ?
                Nome
                :
                $"{ModalidadeCodigo.ShortName()}-{Nome}-{Ano}ºANO"; 
        }

        public bool EhExibirTurma { get; set; }
        public List<RelatorioFrequenciaBimestreDto> Bimestres { get; set; }
    }
}
