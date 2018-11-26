using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EnroladorStandAlone
{
    public partial class LogInDialog : Form
    {
        private CancellationTokenSource ctsMensaje;

        private Tuple<Guid, string, string> _loggedUser = null;
        public Tuple<Guid, string, string> LoggedUser { get { return _loggedUser; } }

        private Tuple<Guid, string, string> storedUser = null;

        private bool online = false;
        public bool Online { get { return online; } }

        public LogInDialog()
        {
            InitializeComponent();
            this.online = true;
        }

        public LogInDialog(Tuple<Guid, string, string> storedUser, bool cambiarUsuario)
        {
            InitializeComponent();
            txtUser.Text = storedUser.Item2;
            txtUser.Enabled = false;
            this.storedUser = storedUser;
            if (cambiarUsuario)
            {
                btnCambiarUsuario.Visible = true;
            }
        }

        private async void btnAceptar_Click(object sender, EventArgs e)
        {
            if (ctsMensaje != null)
            {
                ctsMensaje.Cancel();
                ctsMensaje = null;
                lblUsuarioError.Text = "";
                lblContraseñaError.Text = "";
            }

            bool error = false;

            string user = txtUser.Text;
            if (string.IsNullOrEmpty(user))
            {
                txtUser.Focus();
                lblUsuarioError.Text = "Ingrese un usuario";
                error = true;
            }
            string pass = txtPass.Text;
            if (!error && string.IsNullOrEmpty(pass))
            {
                txtPass.Focus();
                lblContraseñaError.Text = "Ingrese una contraseña";
                error = true;
            }

            if (!error)
            {
                if (online)
                {
                    try
                    {
                        var res = await new EnroladorWebServices.EnroladorWebServicesClient().LoginAsync(user, pass);

                        if (res.HasValue)
                        {
                            _loggedUser = new Tuple<Guid, string, string>(res.Value, user, PassVerifier.StorePassword(pass, res.Value));
                            DialogResult = DialogResult.OK;
                            return;
                        }
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Ocurrió un problema al intentar iniciar sesión. Compruebe la conexión con el servidor", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                else
                {
                    if (PassVerifier.AreEqual(storedUser.Item3, pass, storedUser.Item1))
                    {
                        _loggedUser = storedUser;
                        DialogResult = DialogResult.OK;
                        return;
                    }
                    else
                    {
                        txtPass.Text = "";
                        txtPass.Focus();
                        lblContraseñaError.Text = "Contraseña incorrecta";
                        error = true;
                    }
                }
            }

            if (!error)
            {
                txtUser.Text = "";
                txtPass.Text = "";
                txtUser.Focus();
                lblUsuarioError.Text = "Usuario o contraseña incorrectos";
            }

            try
            {
                await Task.Delay(5000, (ctsMensaje = new CancellationTokenSource()).Token);
                if (ctsMensaje != null && !ctsMensaje.IsCancellationRequested)
                {
                    lblUsuarioError.Text = "";
                    lblContraseñaError.Text = "";
                    ctsMensaje = null;
                }
            }
            catch (TaskCanceledException) { }
        }

        private void LogInDialog_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                btnAceptar_Click(this, EventArgs.Empty);
                e.Handled = true;
            }
        }

        private void btnCambiarUsuario_Click(object sender, EventArgs e)
        {
            if (online)
            {
                online = false;
                btnCambiarUsuario.Text = "Cambiar usuario";
                txtUser.Text = storedUser.Item2;
                txtUser.Enabled = false;
                txtPass.Focus();
            }
            else
            {
                if (MessageBox.Show("Se conectará al servidor para iniciar sesión con otro usuario. Asegurese de que exista conexión con el servidor", "Aviso", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    online = true;
                    btnCambiarUsuario.Text = "Volver";
                    txtUser.Text = "";
                    txtUser.Enabled = true;
                    txtUser.Focus();
                }
            }
        }
    }
}
