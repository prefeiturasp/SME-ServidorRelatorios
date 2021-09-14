using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterRelatorioAcompanhamentoFechamentoConselhoClasseConsolidadoQuery : IRequest<RelatorioAcompanhamentoFechamentoConsolidadoPorUeDto>
    {
        private string _dreCodigo;
        private string _ueCodigo;
        private List<int> _bimestres { get; set; }
        private List<string> _turmasCodigo { get; set; }
        public int AnoLetivo { get; set; }

        public string DreCodigo
        {
            get => TodosParaNull(_dreCodigo);
            set { _dreCodigo = value; }
        }
        public string UeCodigo
        {
            get => TodosParaNull(_ueCodigo);
            set { _ueCodigo = value; }
        }
        public Modalidade Modalidade { get; set; }
        public int Semestre { get; set; }
        public List<string> TurmasCodigo

        {
            get => TodosParaNullTurmas(_turmasCodigo);
            set { _turmasCodigo = value; }
        }
        public List<int> Bimestres
        {
            get => TodosParaNullBimestres(_bimestres);
            set { _bimestres = value; }
        }
        public SituacaoFechamento? SituacaoFechamento { get; set; }
        public SituacaoConselhoClasse? SituacaoConselhoClasse { get; set; }
        public bool ListarPendencias { get; set; }
        public Usuario Usuario { get; set; }

        private string TodosParaNull(string filtro)
        {

            if (filtro == "-99")
                return null;
            return filtro;
        }


        private List<int> TodosParaNullBimestres(List<int> filtro)
        {

            if (filtro != null && (filtro.Contains(-99)))
                return null;
            return filtro;
        }

        private List<string> TodosParaNullTurmas(List<string> filtro)
        {
            if (filtro != null && (filtro.Contains("-99")))
                return null;
            return filtro;
        }
    }
}
