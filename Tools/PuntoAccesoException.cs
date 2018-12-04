using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnroladorStandAlone
{
    class PuntoAccesoException : Exception
    {
        private PuntoAccesoErrores error;
        public PuntoAccesoException(PuntoAccesoErrores error, Exception excepcion)
            : base(error.ToString(), excepcion)
        {
            this.error = error;
        }
        public PuntoAccesoErrores Error { get { return error; } }
    }
}
