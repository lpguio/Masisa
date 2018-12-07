using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace EnroladorStandAlone
{
    public partial class LoadingDialog : Form, ILoadingDialog
    {
        #region Atributos
        string nombrePaso;
        int numeroPaso;
        #endregion

        #region Constructor
        public LoadingDialog() {
            InitializeComponent();
            lblTotal.Text = "";
            lblActual.Text = "";
            OrganizaElementos();
        }
        #endregion

        #region Metodos
        public void PrimerPaso(int totalPasos, int totalPrimero, string nombrePrimero) {
            pgbTotal.Minimum = 0;
            pgbTotal.Maximum = totalPasos * 10;
            pgbTotal.Value = 0;
            pgbTotal.Step = 1;
            pgbActual.Minimum = 0;
            pgbActual.Maximum = totalPrimero < 1 ? 1 : totalPrimero;
            pgbActual.Value = 0;
            pgbActual.Step = 1;
            nombrePaso = nombrePrimero;
            numeroPaso = 1;
            ActualizaLabels();
        }

        public void SiguientePaso(int totalActual, string nombrePaso) {
            numeroPaso++;
            pgbTotal.Value = (numeroPaso - 1) * 10;
            pgbActual.Value = 0;
            pgbActual.Maximum = totalActual < 1 ? 1 : totalActual;
            this.nombrePaso = nombrePaso;
            ActualizaLabels();
        }

        public void AvanzarActual() {
            //Hay que revisar que está dando problemas y no muestra lo que es
            pgbActual.PerformStep();
            var value = ((numeroPaso - 1) * 10) + (10 * pgbActual.Value / pgbActual.Maximum);
            pgbTotal.Value = value > 100 ? 100 : value;
            ActualizaLabels();
        }

        private void ActualizaLabels() {
            lblTotal.Text = string.Format("Paso {0} de {1}: {2}", numeroPaso, pgbTotal.Maximum / 10, nombrePaso);
            lblActual.Text = (100 * pgbActual.Value / pgbActual.Maximum).ToString() + "%";
            lblTotal.Location = new Point((panel.Width - lblTotal.Width) / 2, lblTotal.Location.Y);
            lblActual.Location = new Point((panel.Width - lblActual.Width) / 2, lblActual.Location.Y);
        }

        private void OrganizaElementos() {
            //const int altura = 20;
            //int separacion = (panel.Height - (lblTotal.Height + lblActual.Height + 2 * altura)) / 5;
            //pgbTotal.SetBounds(10, separacion, panel.Width - 20, altura);
            //lblTotal.Location = new Point((panel.Width - lblTotal.Width) / 2, pgbTotal.Location.Y + pgbTotal.Height + separacion);
            //pgbActual.SetBounds(10, lblTotal.Location.Y + lblTotal.Height + separacion, panel.Width - 20, altura);
            //lblActual.Location = new Point((panel.Width - lblActual.Width) / 2, pgbActual.Location.Y + pgbActual.Height + separacion);
        }
        #endregion
    }
}
