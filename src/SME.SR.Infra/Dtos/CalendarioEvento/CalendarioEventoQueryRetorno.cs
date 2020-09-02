﻿using System;

namespace SME.SR.Infra
{
    public class CalendarioEventoQueryRetorno
    {
        public long Id { get; set; }
        public DateTime DataInicio { get; set; }
        public string InicioFimDesc { get; set; }
        public string Nome { get { return $"{_nome} { InicioFimDesc.Replace("inicio", "início") }"; } set { _nome = value; } }
        public string TipoEvento { get; set; }
        private string _nome { get; set; }
    }
}
