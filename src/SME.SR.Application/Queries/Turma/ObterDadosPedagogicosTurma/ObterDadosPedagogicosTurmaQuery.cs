using MediatR;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Application
{
    public class ObterDadosPedagogicosTurmaQuery : IRequest<List<RelatorioAcompanhamentoRegistrosPedagogicosBimestreInfantilDto>>
    {
        public ObterDadosPedagogicosTurmaQuery() { }
        public ObterDadosPedagogicosTurmaQuery(int anoLetivo, long[] codigoTurmas, string professorNome, string professorCodigo, List<int> bimestres)
        {
            TurmasId = codigoTurmas;
            AnoLetivo = anoLetivo;
            ProfessorNome = professorNome;
            ProfessorCodigo = professorCodigo;
            Bimestres = bimestres;
        }

        public int AnoLetivo { get; set; }
        public long[] TurmasId { get; set; }
        public string ProfessorNome { get; set; }
        public string ProfessorCodigo { get; set; }
        public List<int> Bimestres { get; set; }
    }
}
