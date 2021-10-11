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
        public MontarRelatorioAcompanhamentoRegistrosPedagogicosQuery(Dre dre, Ue ue, IEnumerable<Turma> turmas, IEnumerable<ComponenteCurricularPorTurma> componenteCurriculares, int[] bimestres, string nomeUsuario, string rfUsuario)
        {
            Dre = dre;
            Ue = ue;
            Turmas = turmas;
            Bimestres = bimestres;
            ComponentesCurriculares = componenteCurriculares;
            UsuarioNome = nomeUsuario;
            UsuarioRF = rfUsuario;
        }

        public Dre Dre { get; set; }
        public Ue Ue { get; set; }
        public IEnumerable<Turma> Turmas { get; set; }
        public int[] Bimestres { get; set; }
        public IEnumerable<ComponenteCurricularPorTurma> ComponentesCurriculares { get; set; }
        public string UsuarioNome { get; set; }
        public string UsuarioRF { get; set; }
    }
}
