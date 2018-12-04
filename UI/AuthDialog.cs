using System.Threading.Tasks;
using System.Windows.Forms;

namespace EnroladorStandAlone
{
    public partial class AuthDialog : Form
    {
        private const int MAX_INTENTOS_FALLIDOS = 10;

        private static int intentosFallidos = 0;
        private Huellero huellero;
        public AuthDialog(Huellero huellero, string mensaje)
        {
            InitializeComponent();
            this.huellero = huellero;
            huellero.Rejected += Huellero_Rejected;
            huellero.Validated += Huellero_Validated;
            huellero.Habilitar(true);
            lblMensaje.Text = mensaje;
            RefrescarLabelIntentos();
        }

        private void AuthDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            huellero.Rejected -= Huellero_Rejected;
            huellero.Validated -= Huellero_Validated;
            huellero.Habilitar(false);
        }

        private async void Huellero_Validated(object sender, System.EventArgs e)
        {
            await huellero.Sonido(HuelleroSonidos.Correcto);
            intentosFallidos = 0;
            DialogResult = DialogResult.Yes;
        }

        private async void Huellero_Rejected(object sender, System.EventArgs e)
        {
            await huellero.Sonido(HuelleroSonidos.Incorrecto);
            intentosFallidos++;
            RefrescarLabelIntentos();
        }

        private void RefrescarLabelIntentos()
        {
            if (intentosFallidos >= MAX_INTENTOS_FALLIDOS - 3)
            {
                int rem = MAX_INTENTOS_FALLIDOS - intentosFallidos;
                if (rem <= 0)
                {
                    DialogResult = DialogResult.No;
                    return;
                }
                else
                {
                    lblIntentos.Visible = true;
                    lblIntentos.Text = string.Format("Queda{0} {1} intento{2}", rem > 1 ? "n" : "", rem, rem > 1 ? "s" : "");
                }
            }
        }

        private void btnCancelar_Click(object sender, System.EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
