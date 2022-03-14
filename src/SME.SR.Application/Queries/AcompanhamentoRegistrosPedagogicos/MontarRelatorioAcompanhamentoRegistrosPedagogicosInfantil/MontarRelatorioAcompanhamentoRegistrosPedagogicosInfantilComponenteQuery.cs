using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class MontarRelatorioAcompanhamentoRegistrosPedagogicosInfantilComponenteQuery : IRequest<RelatorioAcompanhamentoRegistrosPedagogicosInfantilComponenteDto>
    {
        public MontarRelatorioAcompanhamentoRegistrosPedagogicosInfantilComponenteQuery(Dre dre, Ue ue, IEnumerable<Turma> turmas,
            List<RelatorioAcompanhamentoRegistrosPedagogicosBimestreInfantilComponenteDto> dadosBimestre, int[] bimestres, string nomeUsuario, string rfUsuario,
            long[] componentesCurricularesIds)
        {
            Dre = dre;
            Ue = ue;
            Turmas = turmas;
            Bimestres = bimestres;
            DadosBimestre = dadosBimestre;
            UsuarioNome = nomeUsuario;
            UsuarioRF = rfUsuario;
            ComponentesCurricularesIds = componentesCurricularesIds;
        }

        public Dre Dre { get; set; }
        public Ue Ue { get; set; }
        public IEnumerable<Turma> Turmas { get; set; }
        public int[] Bimestres { get; set; }
        public List<RelatorioAcompanhamentoRegistrosPedagogicosBimestreInfantilComponenteDto> DadosBimestre { get; set; }
        public string UsuarioNome { get; set; }
        public string UsuarioRF { get; set; }
        public long[] ComponentesCurricularesIds { get; set; }
    }
}

