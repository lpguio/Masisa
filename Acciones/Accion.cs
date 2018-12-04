using System;
using System.Threading.Tasks;

namespace EnroladorStandAlone
{
    [Serializable()]
    public abstract class Accion
    {
        protected Guid oid;
        protected Guid responsable;
        protected string descripcion;
        protected DateTime fecha;

        public Guid Oid { get { return oid; } }
        public Guid Responsable { get { return responsable; } }
        public string Descripcion { get { return descripcion; } }
        public DateTime Fecha { get { return fecha; } }

        public Accion(Guid responsable, DateTime fecha, Guid oid)
        {
            this.responsable = responsable;
            this.fecha = fecha;
            this.oid = oid;
        }

        public virtual Accion Clonar()
        {
            return null;
        }

        public virtual Task Enviar()
        {
            return null;
        }

        public virtual void Cancelar(Form1 parent)
        {
            return;
        }

        public virtual void Aplicar(Form1 parent)
        {
            return;
        }
    }
}
