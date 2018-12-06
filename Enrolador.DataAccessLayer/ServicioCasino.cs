using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enrolador.DataAccessLayer
{
    [Serializable]
    /// <summary>
    /// Representa los servicios de un casino
    /// </summary>
    public class ServicioCasino
    {
        public Guid Oid { get; set; }
        public Guid Casino { get; set; }
        public string Nombre { get; set; }
        public Boolean Vigente { get; set; }
    }
}
