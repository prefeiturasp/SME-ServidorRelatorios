using MediatR;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Application
{
    public class ObterRelatorioAcompanhamentoRegistrosPedagogicosQuery : IRequest<RelatorioAcompanhamentoRegistrosPedagogicosDto>
    {
        private string _dreCodigo;
        private string _ueCodigo;
        private List<int> _bimestres { get; set; }
        private List<string> _turmas { get; set; }
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
        public List<string> Turmas

        {
            get => TodosParaNullTurmas(_turmas);
            set { _turmas = value; }
        }
        public List<int> Bimestres
        {
            get => TodosParaNullBimestres(_bimestres);
            set { _bimestres = value; }
        }
        public string ProfessorCodigo { get; set; }
        public string UsuarioNome { get; set; }
        public string UsuarioRF { get; set; }
        public long[] ComponentesCurriculares { get; set; }

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
