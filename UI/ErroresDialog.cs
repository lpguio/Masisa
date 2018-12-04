using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EnroladorStandAlone
{
    public partial class ErroresDialog: Form
    {
        public ErroresDialog(List<string> erroresDeEnvio)
        {
            InitializeComponent();
            memErrores.Text = string.Format("Se generaron los siguientes errores durante el envío de acciones:{0}{1}", Environment.NewLine, string.Join(Environment.NewLine, erroresDeEnvio));
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }
    }
}
