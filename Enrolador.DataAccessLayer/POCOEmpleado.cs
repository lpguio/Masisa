using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enrolador.DataAccessLayer {
    [Serializable]
    /// <summary>
    /// Representa una clase con los datos necesarios para el EMPLEADO
    /// </summary>
    public class POCOEmpleado {
        public Guid Oid { get; set; }
        public string RUT { get; set; }
        public int EnrollId { get; set; }
        public string Correo { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public bool TieneContraseña { get; set; }
        public string Contraseña { get; set; }
        public string NumeroTelefono { get; set; }
    }
}
