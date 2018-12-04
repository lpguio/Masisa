using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enrolador.DataAccessLayer
{
    [Serializable]
    /// <summary>
    /// Representa los  turnos de servicios de un casino
    /// </summary>
    public class TurnoServicio
    {
        public Guid Oid { get; set; }
        public Guid Servicio { get; set; }
        public string Nombre { get; set; }
        public Boolean Vigente { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }
    }
}
