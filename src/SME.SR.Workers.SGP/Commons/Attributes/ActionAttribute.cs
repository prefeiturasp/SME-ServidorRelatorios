using System;

namespace SME.SR.Workers.SGP.Commons.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ActionAttribute : Attribute
    {
        public ActionAttribute(string name, Type tipoCasoDeUso)
        {
            Name = name;
            TipoCasoDeUso = tipoCasoDeUso;
        }

        public string Name { get; }
        public Type TipoCasoDeUso { get; }
    }
}

