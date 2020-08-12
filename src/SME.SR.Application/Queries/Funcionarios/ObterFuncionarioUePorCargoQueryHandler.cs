using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application.Queries.Funcionarios
{
    public class ObterFuncionarioUePorCargoQueryHandler : IRequestHandler<ObterFuncionarioUePorCargoQuery, IEnumerable<FuncionarioDto>>
    {
        private readonly IFuncionarioRepository _funcionarioRepository;

        public ObterFuncionarioUePorCargoQueryHandler(IFuncionarioRepository conceitoValoresRepository)
        {
            this._funcionarioRepository = conceitoValoresRepository ?? throw new ArgumentNullException(nameof(conceitoValoresRepository));
        }

        public async Task<IEnumerable<FuncionarioDto>> Handle(ObterFuncionarioUePorCargoQuery request, CancellationToken cancellationToken)
        {
            var funcionarios = await _funcionarioRepository.ObterFuncionariosPorCargoUe(request.CodigoCargo, request.CodigoUe);
            if (funcionarios == null && !funcionarios.Any())
                throw new NegocioException("Não foi possível localizar o funcionarios da ue com este cargo");

            return MapearParaDto(funcionarios);
        }

        private IEnumerable<FuncionarioDto> MapearParaDto(IEnumerable<Funcionario> funcionarios)
        {
            foreach (var funcionario in funcionarios)
            {
                yield return new FuncionarioDto()
                {
                    Cargo = funcionario.Cargo,
                    CodigoRF = funcionario.CodigoRF,
                    Documento = funcionario.Documento,
                    DataFim = funcionario.DataFim,
                    DataInicio = funcionario.DataInicio,
                    NomeServidor = funcionario.NomeServidor
                };
            }
        }
    }
}
