using MediatR;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Application
{
    public class ObterRelatorioAcompanhamentoRegistrosPedagogicosInfantilQuery : IRequest<RelatorioAcompanhamentoRegistrosPedagogicosInfantilDto>
    {
        private string _dreCodigo;
        private string _ueCodigo;
        private List<int> _bimestres { get; set; }
        private long[] _turmasId { get; set; }
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
        public long[] TurmasId

        {
            get => TodosParaNullTurmas(_turmasId);
            set { _turmasId = value; }
        }
        public List<int> Bimestres
        {
            get => TodosParaNullBimestres(_bimestres);
            set { _bimestres = value; }
        }
        public string ProfessorCodigo { get; set; }
        public string UsuarioNome { get; set; }
        public string UsuarioRF { get; set; }


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

        private long[] TodosParaNullTurmas(long[] filtro)
        {
            if (filtro != null && filtro[0] == -99)
                return null;
            return filtro;
        }
    }
}
