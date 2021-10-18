using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Application
{
    public class MontarRelatorioAcompanhamentoRegistrosPedagogicosQuery : IRequest<RelatorioAcompanhamentoRegistrosPedagogicosDto>
    {
        public MontarRelatorioAcompanhamentoRegistrosPedagogicosQuery(Dre dre, Ue ue, IEnumerable<Turma> turmas, List<RelatorioAcompanhamentoRegistrosPedagogicosBimestreDto> dadosBimestre, int[] bimestres, string nomeUsuario, string rfUsuario)
        {
            Dre = dre;
            Ue = ue;
            Turmas = turmas;
            Bimestres = bimestres;
            DadosBimestre = dadosBimestre;
            UsuarioNome = nomeUsuario;
            UsuarioRF = rfUsuario;
        }

        public Dre Dre { get; set; }
        public Ue Ue { get; set; }
        public IEnumerable<Turma> Turmas { get; set; }
        public int[] Bimestres { get; set; }
        public List<RelatorioAcompanhamentoRegistrosPedagogicosBimestreDto> DadosBimestre { get; set; }
        public string UsuarioNome { get; set; }
        public string UsuarioRF { get; set; }
    }
}
