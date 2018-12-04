using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enrolador.DataAccessLayer
{
    [Serializable]
    /// <summary>
    /// Representa los turnos de servicio que tiene acceso el empleado
    /// </summary>
    public class EmpleadoTurnoServicioCasino
    {
        public Guid Empleado { get; set; }
        public Guid TurnoServicio { get; set; }
    }
}
