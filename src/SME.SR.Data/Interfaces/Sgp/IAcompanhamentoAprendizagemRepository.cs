﻿using SME.SR.Infra;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data.Interfaces
{
    public interface IAcompanhamentoAprendizagemRepository
    {
        Task<IEnumerable<AcompanhamentoAprendizagemTurmaDto>> ObterAcompanhamentoAprendizagemPorTurmaESemestre(long turmaId, string alunoCodigo, int semestre);
    }
}