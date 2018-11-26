using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnroladorStandAlone
{
    [Serializable]
    public class Historia
    {
        public Historia(DateTime fecha, string rut, string nombre, string apellido, string empresa, string cuenta, string cargo)
        {
            Fecha = fecha;
            RUT = rut;
            Nombre = nombre;
            Apellido = apellido;
            Empresa = empresa;
            Cuenta = cuenta;
            Cargo = cargo;
        }
        public DateTime Fecha { get; set; }
        public string RUT { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Empresa { get; set; }
        public string Cuenta { get; set; }
        public string Cargo { get; set; }
    }
}
