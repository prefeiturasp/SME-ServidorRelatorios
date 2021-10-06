using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Application
{
    public class ObterRelatorioBoletimEscolarDetalhadoEscolaAquiQuery : IRequest<RelatorioBoletimEscolarDetalhadoDto>
    {
        public string DreCodigo { get; set; }

        public string UeCodigo { get; set; }

        public int Semestre { get; set; }

        public string TurmaCodigo { get; set; }

        public int AnoLetivo { get; set; }

        public Modalidade Modalidade { get; set; }
        public int ModalidadeCodigo { get; set; }

        public string AlunoCodigo { get; set; }

        public Usuario Usuario { get; set; }

        public bool ConsideraHistorico { get; set; }
        public Guid CodigoArquivo { get; set; }
    }
}
