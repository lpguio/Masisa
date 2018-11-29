using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.Threading.Tasks;
using System.Threading;
using DevExpress.XtraBars;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using DevExpress.XtraGrid.Views.Grid;
using System.Data;
using System.Deployment.Application;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraGrid.Views.Base;

namespace EnroladorStandAlone
{
    public partial class Form1 : XtraForm
    {
        #region Fields
        private const int DIAS_RECOMENDAR_CONECTAR = 14;
        private bool actualizando = false;
        private object fileLock = new object();
        private List<Accion> accionesPorEnviar = new List<Accion>();
        private List<string> erroresDeEnvio = new List<string>();
        private BindingList<Tuple<string, string, string, string, EnrollForm.TipoAccion?>> bnlEmpleados = new BindingList<Tuple<string, string, string, string, EnrollForm.TipoAccion?>>();
        private Guid HWID;
        private DateTime ultimaConexión;
        private string loginFile;
        private Tuple<Guid, string, string> loggedUser; // Oid, UserName, StoredPassword
        public Tuple<Guid, string, string> LoggedUser { get { return loggedUser; } }
        private string huellaUserFile;
        private Dictionary<Guid, Tuple<TipoHuella, string>> huellaUserTable; // Oid, TipoHuella, Data
        private string cadenaFile;
        private Dictionary<Guid, Tuple<string, List<Guid>>> cadenaTable; // Oid => Nombre, lista de instalaciones
        private string instalacionFile;
        private Dictionary<Guid, Tuple<string, List<Guid>>> instalacionTable; // Oid => Nombre, lista de dispositivos
        private string dispositivoFile;
        private Dictionary<Guid, Tuple<string, string, int, TipoDispositivo, List<Guid>>> dispositivoTable; // Oid => Nombre, Host, Puerto, TipoDispositivo, lista de empleados que marcan
        private string empresaFile;
        private Dictionary<Guid, Tuple<string, List<Guid>, List<Guid>>> empresaTable; // Oid => Nombre, lista de cargos, lista de cuentas
        private string cargoFile;
        private Dictionary<Guid, string> cargoTable; // Oid => Nombre
        private string cuentaFile;
        private Dictionary<Guid, Tuple<string, DateTime?>> cuentaTable; // Oid => Nombre
        private string empleadoFile;
        private Dictionary<Guid, Tuple<int, string, bool, string, string, Tuple<List<Guid>, List<Guid>, List<Guid>>>> empleadoTable; // Oid => EnrollID, RUT, Contraseña, FirstName, LastName, (lista de huellas, lista de dispositivos, lista de contratos)
        private Dictionary<string, Guid> empleadoRUTIndex; // Indice sobre RUT en tabla Empleado
        private string huellaFile;
        private Dictionary<Guid, TipoHuella> huellaTable; // Oid => TipoHuella
        private string contratoFile;
        private Dictionary<Guid, Tuple<Guid, Guid, Guid, DateTime, DateTime?>> contratoTable; // Oid => Empresa, Cuenta, Cargo, InicioVigencia, FinVigencia
        private bool dataLoaded = false;
        public Huellero Huellero { get { return huellero; } }
        private Huellero huellero;
        private string programFolder; 
        #endregion

        #region Propiedades
        public Dictionary<Guid, Tuple<TipoHuella, string>> HuellaUserTable { get { return huellaUserTable; } }
        public Dictionary<Guid, Tuple<string, List<Guid>>> CadenaTable { get { return cadenaTable; } }
        public Dictionary<Guid, Tuple<string, List<Guid>>> InstalacionTable { get { return instalacionTable; } }
        public Dictionary<Guid, Tuple<string, string, int, TipoDispositivo, List<Guid>>> DispositivoTable { get { return dispositivoTable; } }
        public Dictionary<Guid, Tuple<string, List<Guid>, List<Guid>>> EmpresaTable { get { return empresaTable; } }
        public Dictionary<Guid, string> CargoTable { get { return cargoTable; } }
        public Dictionary<Guid, Tuple<string, DateTime?>> CuentaTable { get { return cuentaTable; } }
        public Dictionary<Guid, Tuple<int, string, bool, string, string, Tuple<List<Guid>, List<Guid>, List<Guid>>>> EmpleadoTable { get { return empleadoTable; } }
        public Dictionary<string, Guid> EmpleadoRUTIndex { get { return empleadoRUTIndex; } }
        public Dictionary<Guid, TipoHuella> HuellaTable { get { return huellaTable; } }
        public Dictionary<Guid, Tuple<Guid, Guid, Guid, DateTime, DateTime?>> ContratoTable { get { return contratoTable; } }
        #endregion

        #region Constructor
        public Form1()
        {
            InitializeComponent();

            programFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SmartBoxes", "Enrolador");
            try
            {
                Directory.CreateDirectory(programFolder);
            }
            catch (Exception)
            {

            }
        }
        #endregion
        
        #region FormEvents
        private async void Form1_Load(object sender, EventArgs e)
        {
            // Primero recupera las acciones pendientes de enviar que estan guardadas en el archivo
            Accion accion;
            string file = Path.Combine(programFolder, "acciones.dat");
            if (File.Exists(file))
            {
                using (Stream stream = File.Open(file, FileMode.Open, FileAccess.Read))
                {
                    BinaryFormatter bin = new BinaryFormatter();
                    while (stream.Position < stream.Length)
                    {
                        accion = (Accion)bin.Deserialize(stream);
                        accionesPorEnviar.Add(accion);
                    }
                }
            }
            await IniciarSistema();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!actualizando && dataLoaded && accionesPorEnviar.Count() > 0)
            {
                DialogResult res = MessageBox.Show("Hay acciones que no han sido enviadas. Enviar ahora?", "Acciones no enviadas", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);
                if (res != DialogResult.No)
                {
                    e.Cancel = true;
                    if (res == DialogResult.Yes)
                    {
                        recargar_ItemClick(this, null);
                    }
                    return;
                }
            }
            if (huellero != null)
            {
                huellero.Dispose();
                huellero = null;
            }
        }
        #endregion

        #region Iniciar sistema
        private async Task IniciarSistema()
        {
            if (BuscarActualizaciones())
            {
                return;
            }

            HWID = HardwareID.GetBaseHardwareFingerPrint();
            //lpg
            HWID = new Guid("f6a74090-e0d1-daf9-e8c4-7d7497f4c9ad");
            string baseFile = Path.Combine(programFolder, HWID.ToString());
            loginFile = baseFile + "-0.dat";
            huellaUserFile = baseFile + "-1.dat";
            cadenaFile = baseFile + "-2.dat";
            instalacionFile = baseFile + "-3.dat";
            dispositivoFile = baseFile + "-4.dat";
            empresaFile = baseFile + "-5.dat";
            cargoFile = baseFile + "-6.dat";
            cuentaFile = baseFile + "-7.dat";
            empleadoFile = baseFile + "-8.dat";
            huellaFile = baseFile + "-9.dat";
            contratoFile = baseFile + "-10.dat";

            bool online = false;

            if (!IdentificarOffline())
            {
                // Equipo sin autorización almacenada. Conectar parña autorizar equipo
                if (MessageBox.Show("No existe autorización almacenada para este equipo. Se conectará al servidor para solicitar autorización. Asegurese de que exista conexión con el servidor", "Aviso", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.Cancel)
                {
                    Application.Exit();
                    return;
                }
                online = true;
                while (!(await Identificar()))
                {
                    if (MessageBox.Show("No se ha podido obtener autorización para este equipo. Compruebe la conexión con el servidor", "Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Hand) == DialogResult.Cancel)
                    {
                        Application.Exit();
                        return;
                    }
                }
            }

            if (online)
            {
                // Login online directo si ya nos habíamos conectado en el paso anterior
                if (!(await LogIn()))
                {
                    Application.Exit();
                    return;
                }
            }
            else
            {
                // Login offline puede convertirse en online
                Tuple<bool, bool> res = await LogInOffline();
                if (res.Item1)
                {
                    online = res.Item2;
                }
                else
                {
                    Application.Exit();
                    return;
                }
            }

            // Subir huellas del usuario al huellero
            huellero = new Huellero();
            //lpg
            //while (!(await Task<bool>.Factory.StartNew(() =>
            //{
            //    return huellero.Connect(huellaUserTable);
            //})))
            //{
            //    if (MessageBox.Show("No se ha podido conectar con el huellero. Compruebe la conexión del dispositivo", "Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Hand) == DialogResult.Cancel)
            //    {
            //        Application.Exit();
            //        return;
            //    }
            //}

            if (online || !LoadDataOffline())
            {
                //Pedir autorizacion para cargar data online si falla la carga offline

                if (await PedirAutorizacion("Por favor autorice con su huella la descarga de datos a este equipo", this) != ResultadoAutorizacion.Aceptado)
                {
                    MessageBox.Show("Cerrando aplicación. No se autorizó la descarga de datos", "No autorizado", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                    return;
                }


                online = true;
                while (!(await LoadData()))
                {
                    if (MessageBox.Show("No se han podido descargar los datos. Compruebe la conexión con el servidor", "Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Hand) == DialogResult.Cancel)
                    {
                        Application.Exit();
                        return;
                    }
                }
            }

            int diasDesdeUltimaConexion = (int)DateTime.Now.Subtract(ultimaConexión).TotalDays;
            if (!online && diasDesdeUltimaConexion > DIAS_RECOMENDAR_CONECTAR)
            {
                if (MessageBox.Show(string.Format("Han pasado {0} días desde la última sincronización. Desea sincronizar ahora?", diasDesdeUltimaConexion), "Sincronizar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    await Sincronizar();
                    return;
                }
            }

            LlenaGridEmpleados();

            HabilitarSistema(true);
        }

        private void LlenaGridEmpleados()
        {
            bnlEmpleados = new BindingList<Tuple<string, string, string, string, EnrollForm.TipoAccion?>>();
            List<Tuple<string, string, string, string, EnrollForm.TipoAccion?>> nuevos = new List<Tuple<string, string, string, string, EnrollForm.TipoAccion?>>();
            List<Tuple<string, string, string, string, EnrollForm.TipoAccion?>> modificados = new List<Tuple<string, string, string, string, EnrollForm.TipoAccion?>>();
            List<Tuple<string, string, string, string, EnrollForm.TipoAccion?>> eliminados = new List<Tuple<string, string, string, string, EnrollForm.TipoAccion?>>();
            List<Tuple<string, string, string, string, EnrollForm.TipoAccion?>> otros = new List<Tuple<string, string, string, string, EnrollForm.TipoAccion?>>();
            foreach (var emp in empleadoTable.OrderBy((obj) => { return obj.Value.Item1; }))
            {
                if (emp.Value.Item6.Item3.Count() > 0)
                {
                    bool found = false;
                    string identificacion;
                    if (emp.Value.Item3)
                    {
                        identificacion = "Clave";
                    }
                    else
                    {
                        identificacion = string.Format("{0} huella{1}", emp.Value.Item6.Item1.Count, emp.Value.Item6.Item1.Count == 1 ? "" : "s");
                    }
                    foreach (Accion accion in accionesPorEnviar)
                    {
                        if (accion is AccionCrearEmpleado && ((AccionCrearEmpleado)accion).Oid.Equals(emp.Key))
                        {
                            nuevos.Add(new Tuple<string, string, string, string, EnrollForm.TipoAccion?>(emp.Value.Item2, emp.Value.Item4, emp.Value.Item5, identificacion, EnrollForm.TipoAccion.Nueva));
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        foreach (Accion accion in accionesPorEnviar)
                        {
                            if (accion is AccionCaducarContrato)
                            {
                                AccionCaducarContrato accionCaducarContrato = (AccionCaducarContrato)accion;
                                if (accionCaducarContrato.Empleado.Equals(emp.Key) && accionCaducarContrato.FinVigencia.HasValue)
                                {
                                    eliminados.Add(new Tuple<string, string, string, string, EnrollForm.TipoAccion?>(emp.Value.Item2, emp.Value.Item4, emp.Value.Item5, identificacion, EnrollForm.TipoAccion.Eliminada));
                                    found = true;
                                    break;
                                }
                            }
                        }
                    }
                    if (!found)
                    {
                        foreach (Accion accion in accionesPorEnviar)
                        {
                            if (accion is AccionActualizarHuella)
                            {
                                AccionActualizarHuella accion2 = (AccionActualizarHuella)accion;
                                if (accion2.Empleado.Equals(emp.Key))
                                {
                                    found = true;
                                }
                            }
                            else if (accion is AccionCaducarContrato)
                            {
                                AccionCaducarContrato accion2 = (AccionCaducarContrato)accion;
                                if (accion2.Empleado.Equals(emp.Key))
                                {
                                    found = true;
                                }
                            }
                            else if (accion is AccionCrearAsignacion)
                            {
                                AccionCrearAsignacion accion2 = (AccionCrearAsignacion)accion;
                                if (accion2.Empleado.Equals(emp.Key))
                                {
                                    found = true;
                                }
                            }
                            else if (accion is AccionCrearContrato)
                            {
                                AccionCrearContrato accion2 = (AccionCrearContrato)accion;
                                if (accion2.Empleado.Equals(emp.Key))
                                {
                                    found = true;
                                }
                            }
                            else if (accion is AccionCrearHuella)
                            {
                                AccionCrearHuella accion2 = (AccionCrearHuella)accion;
                                if (accion2.Empleado.Equals(emp.Key))
                                {
                                    found = true;
                                }
                            }
                            else if (accion is AccionModificarContraseña)
                            {
                                AccionModificarContraseña accion2 = (AccionModificarContraseña)accion;
                                if (accion2.Oid.Equals(emp.Key))
                                {
                                    found = true;
                                }
                            }
                            if (found)
                            {
                                modificados.Add(new Tuple<string, string, string, string, EnrollForm.TipoAccion?>(emp.Value.Item2, emp.Value.Item4, emp.Value.Item5, identificacion, EnrollForm.TipoAccion.Modificada));
                                break;
                            }
                        }
                    }
                    if (!found)
                    {
                        otros.Add(new Tuple<string, string, string, string, EnrollForm.TipoAccion?>(emp.Value.Item2, emp.Value.Item4, emp.Value.Item5, identificacion, null));
                    }
                }
            }
            foreach (var emp in nuevos) { bnlEmpleados.Add(emp); }
            foreach (var emp in modificados) { bnlEmpleados.Add(emp); }
            foreach (var emp in eliminados) { bnlEmpleados.Add(emp); }
            foreach (var emp in otros) { bnlEmpleados.Add(emp); }
            gcHistoria.DataSource = bnlEmpleados;
        }

        private void HabilitarSistema(bool habilitar)
        {
            enrolar.Enabled = habilitar;
            recargar.Enabled = habilitar;
        }
        #endregion

        #region Pedir autorizacion
        public async Task<ResultadoAutorizacion> PedirAutorizacion(string mensaje, IWin32Window owner)
        {
            return ResultadoAutorizacion.Aceptado;

            return await Task<ResultadoAutorizacion>.Factory.StartNew((obj) =>
            {
                using (AuthDialog authDialog = new AuthDialog(huellero, mensaje))
                {
                    DialogResult res = authDialog.ShowDialog(owner);
                    if (res == DialogResult.Yes)
                    {
                        return ResultadoAutorizacion.Aceptado;
                    }
                    else if (res == DialogResult.No)
                    {
                        return ResultadoAutorizacion.Rechazado;
                    }
                    else
                    {
                        return ResultadoAutorizacion.Cancelado;
                    }
                }
            }, null, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
        }
        #endregion

        #region Sincronización
        private async void recargar_ItemClick(object sender, ItemClickEventArgs e)
        {
            await Sincronizar();
        }

        private async Task Sincronizar()
        {
            try
            {
                HabilitarSistema(false);
                // Pedir autorizacion para sincronizar
                //ResultadoAutorizacion res = await PedirAutorizacion("Por favor autorice con su huella el envío de acciones y la descarga\r\nde datos a este equipo", this);
                //if (res == ResultadoAutorizacion.Rechazado)
                //{
                //    MessageBox.Show("Cerrando aplicación. No se autorizó la sincronización", "No autorizado", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //    Application.Exit();
                //    return;
                //}
                //else if (res == ResultadoAutorizacion.Cancelado)
                //{
                //    MessageBox.Show("Sincronizacion cancelada. No se autorizó la sincronización", "No autorizado", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //    return;
                //}

                while (!(await Identificar()))
                {
                    if (MessageBox.Show("No se ha podido reautorizar al equipo. Compruebe la conexión con el servidor", "Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Hand) == DialogResult.Cancel)
                    {
                        return;
                    }
                }

                while (!(await RelogIn()))
                {
                    if (MessageBox.Show("No se han podido refrescar los datos del usuario. Compruebe la conexión con el servidor", "Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Hand) == DialogResult.Cancel)
                    {
                        return;
                    }
                }

                erroresDeEnvio.Clear();
                while (!(await EnviarAcciones()))
                {
                    if (MessageBox.Show("No se han podido enviar las acciones. Compruebe la conexión con el servidor", "Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Hand) == DialogResult.Cancel)
                    {
                        return;
                    }
                }

                while (!(await ReloadData()))
                {
                    if (MessageBox.Show("No se han podido refrescar los datos. Compruebe la conexión con el servidor", "Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Hand) == DialogResult.Cancel)
                    {
                        return;
                    }
                }

                if (erroresDeEnvio.Count > 0)
                {
                    await MostrarErrores();
                }

                LlenaGridEmpleados();
            }
            finally
            {
                HabilitarSistema(true);
            }
        }

        private async Task<bool> EnviarAcciones()
        {
            List<Accion> lista = accionesPorEnviar.OrderBy<Accion, DateTime>((accion) => { return accion.Fecha; }).ToList<Accion>();
            if (lista.Count > 0)
            {
                try
                {
                    List<Tuple<Accion, Exception>> pendientes = new List<Tuple<Accion, Exception>>(lista.Count);

                    //Bloquea archivo
                    Monitor.Enter(fileLock);
                    try
                    {
                        // Enviar las acciones
                        foreach (Accion accion in lista)
                        {
                            try
                            {
                                await accion.Enviar();
                            }
                            catch (Exception ex)
                            {
                                pendientes.Add(new Tuple<Accion, Exception>(accion, ex));
                            }
                            accionesPorEnviar.Remove(accion);
                        }

                        // Eliminar archivo de respaldo
                        try
                        {
                            File.Delete(Path.Combine(programFolder, "acciones.dat"));
                        }
                        catch (Exception) { }
                    }
                    finally
                    {
                        Monitor.Exit(fileLock);
                    }

                    // Guardar pendientes
                    if (pendientes.Count > 0)
                    {
                        foreach (var error in pendientes)
                        {
                            AgregaError(error);
                        }
                    }
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return true;
        }

        private async Task MostrarErrores()
        {
            await Task.Factory.StartNew((obj) =>
            {
                using (ErroresDialog erroresDialog = new ErroresDialog(erroresDeEnvio))
                {
                    erroresDialog.ShowDialog(this);
                }
            }, null, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
        }
        #endregion

        #region Identificar
        private async Task<bool> Identificar()
        {
            try
            {
                string file = Path.Combine(programFolder, string.Format("{0}.dat", HWID));

                if (await new EnroladorWebServices.EnroladorWebServicesClient().IdentificarAsync(HWID))
                {
                    using (Stream stream = File.Open(file, FileMode.Create, FileAccess.Write))
                    {
                        BinaryFormatter bin = new BinaryFormatter();
                        ultimaConexión = DateTime.Now;
                        bin.Serialize(stream, ultimaConexión);
                        stream.Close();
                    }
                }
                else
                {
                    File.Delete(file);
                    return false;
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool IdentificarOffline()
        {
            try
            {
                string file = Path.Combine(programFolder, string.Format("{0}.dat", HWID));
                if (!File.Exists(file))
                {
                    return false;
                }
                using (Stream stream = File.Open(file, FileMode.Open, FileAccess.Read))
                {
                    BinaryFormatter bin = new BinaryFormatter();
                    ultimaConexión = (DateTime)bin.Deserialize(stream);
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion

        #region LogIn
        private async Task<bool> LogIn()
        {
            try
            {
                var returnedUser = await Task<Tuple<Guid, string, string>>.Factory.StartNew((obj) =>
                    {
                        using (LogInDialog logInDialog = new LogInDialog())
                        {
                            if (logInDialog.ShowDialog(this) == DialogResult.Cancel)
                            {
                                return null;
                            }
                            else
                            {
                                return logInDialog.LoggedUser;
                            }
                        }
                    }, null, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());

                if (returnedUser == null)
                {
                    return false;
                }

                loggedUser = returnedUser;
                huellaUserTable = await LeeHuellaUser();

                using (Stream stream = File.Open(loginFile, FileMode.Create, FileAccess.Write))
                {
                    BinaryFormatter bin = new BinaryFormatter();
                    bin.Serialize(stream, loggedUser);
                    stream.Close();
                }
                using (Stream stream = File.Open(huellaUserFile, FileMode.Create, FileAccess.Write))
                {
                    BinaryFormatter bin = new BinaryFormatter();
                    bin.Serialize(stream, huellaUserTable);
                    stream.Close();
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private async Task<Tuple<bool, bool>> LogInOffline() // exito, online
        {
            try
            {
                if (!File.Exists(loginFile) || !File.Exists(huellaUserFile))
                {
                    if (MessageBox.Show("No existe información almacenada para iniciar sesión. Se conectará al servidor para iniciar sesión. Asegurese de que exista conexión con el servidor", "Aviso", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.Cancel)
                    {
                        return new Tuple<bool, bool>(false, false);
                    }
                    return new Tuple<bool, bool>(await LogIn(), true);
                }

                Tuple<Guid, string, string> storedUser;
                Dictionary<Guid, Tuple<TipoHuella, string>> storedHuellaUserTable;
                using (Stream stream = File.Open(loginFile, FileMode.Open, FileAccess.Read))
                {
                    BinaryFormatter bin = new BinaryFormatter();
                    storedUser = (Tuple<Guid, string, string>)bin.Deserialize(stream);
                }
                using (Stream stream = File.Open(huellaUserFile, FileMode.Open, FileAccess.Read))
                {
                    BinaryFormatter bin = new BinaryFormatter();
                    storedHuellaUserTable = (Dictionary<Guid, Tuple<TipoHuella, string>>)bin.Deserialize(stream);
                }
                if (storedUser == null || storedHuellaUserTable == null)
                {
                    if (MessageBox.Show("No existe información almacenada para iniciar sesión. Se conectará al servidor para iniciar sesión. Asegurese de que exista conexión con el servidor", "Aviso", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.Cancel)
                    {
                        return new Tuple<bool, bool>(false, false);
                    }
                    return new Tuple<bool, bool>(await LogIn(), true);
                }

                var res = await Task<Tuple<Tuple<Guid, string, string>, bool>>.Factory.StartNew((obj) =>
                 {
                     using (LogInDialog logInDialog = new LogInDialog(storedUser, accionesPorEnviar.Count() == 0))
                     {
                         if (logInDialog.ShowDialog(this) == DialogResult.Cancel)
                         {
                             return null;
                         }
                         else
                         {
                             return new Tuple<Tuple<Guid, string, string>, bool>(logInDialog.LoggedUser, logInDialog.Online);
                         }
                     }
                 }, null, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());

                if (res == null)
                {
                    return new Tuple<bool, bool>(false, false);
                }

                if (res.Item2)
                {
                    loggedUser = res.Item1;
                    huellaUserTable = await LeeHuellaUser();
                    using (Stream stream = File.Open(loginFile, FileMode.Create, FileAccess.Write))
                    {
                        BinaryFormatter bin = new BinaryFormatter();
                        bin.Serialize(stream, loggedUser);
                        stream.Close();
                    }
                    using (Stream stream = File.Open(huellaUserFile, FileMode.Create, FileAccess.Write))
                    {
                        BinaryFormatter bin = new BinaryFormatter();
                        bin.Serialize(stream, huellaUserTable);
                        stream.Close();
                    }
                }
                else
                {
                    loggedUser = storedUser;
                    huellaUserTable = storedHuellaUserTable;
                }
                return new Tuple<bool, bool>(true, res.Item2);
            }
            catch (Exception)
            {
                return new Tuple<bool, bool>(false, false);
            }
        }

        private async Task<bool> RelogIn()
        {
            try
            {
                Tuple<Guid, string, string> returnedUser = await RevalidarUser();

                if (returnedUser == null)
                {
                    return false;
                }

                loggedUser = returnedUser;
                huellaUserTable = await LeeHuellaUser();

                using (Stream stream = File.Open(loginFile, FileMode.Create, FileAccess.Write))
                {
                    BinaryFormatter bin = new BinaryFormatter();
                    bin.Serialize(stream, loggedUser);
                    stream.Close();
                }
                using (Stream stream = File.Open(huellaUserFile, FileMode.Create, FileAccess.Write))
                {
                    BinaryFormatter bin = new BinaryFormatter();
                    bin.Serialize(stream, huellaUserTable);
                    stream.Close();
                }
                //lpg
                return true;
                return huellero.Refrescar(huellaUserTable);
            }
            catch (Exception)
            {
                return false;
            }
        }

        private async Task<Tuple<Guid, string, string>> RevalidarUser()
        {
            try
            {
                Tuple<string, string> res = await new EnroladorWebServices.EnroladorWebServicesClient().RevalidarAsync(loggedUser.Item1);
                if (res != null)
                {
                    return new Tuple<Guid, string, string>(loggedUser.Item1, res.Item1, res.Item2);
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private async Task<Dictionary<Guid, Tuple<TipoHuella, string>>> LeeHuellaUser()
        {
            Dictionary<Guid, Tuple<TipoHuella, string>> newHuellaUserTable = new Dictionary<Guid, Tuple<TipoHuella, string>>();
            try
            {
                Tuple<Guid, int, string>[] res = await new EnroladorWebServices.EnroladorWebServicesClient().LeeHuellaUserAsync(loggedUser.Item1);

                foreach (var huella in res)
                {
                    newHuellaUserTable[huella.Item1] = new Tuple<TipoHuella, string>((TipoHuella)huella.Item2, huella.Item3);
                }
            }
            catch (Exception) { }

            return newHuellaUserTable;
        }
        #endregion

        #region LoadData y ReloadData
        private async Task<bool> LoadData()
        {
            if (!dataLoaded)
            {
                LoadingDialog loadingDialog = null;
                try
                {
                    loadingDialog = new LoadingDialog();
                    loadingDialog.Show(this);

                    cadenaTable = await LeeCadena(loadingDialog);
                    instalacionTable = await LeeInstalacion(loadingDialog, cadenaTable);
                    dispositivoTable = await LeeDispositivo(loadingDialog, instalacionTable);
                    empresaTable = await LeeEmpresa(loadingDialog);
                    cargoTable = await LeeCargo(loadingDialog, empresaTable);
                    cuentaTable = await LeeCuenta(loadingDialog, empresaTable);
                    var tablasEmpleado = await LeeEmpleado(loadingDialog);
                    empleadoTable = tablasEmpleado.Item1;
                    empleadoRUTIndex = tablasEmpleado.Item2;
                    huellaTable = await LeeHuella(loadingDialog, empleadoTable);
                    await LeeEmpleadosDispositivos(loadingDialog, dispositivoTable, empleadoTable);
                    contratoTable = await LeeContrato(loadingDialog, empleadoTable, empresaTable, cuentaTable, cargoTable);

                    if (!SaveLoadedData())
                    {
                        return false;
                    }
                    ReaplicarAcciones();

                    dataLoaded = true;
                }
                catch (Exception Ex)
                {
                    return false;
                }
                finally
                {
                    if (loadingDialog != null)
                    {
                        loadingDialog.Close();
                    }
                }
            }
            return true;
        }

        private bool LoadDataOffline()
        {
            try
            {
                if (!File.Exists(cadenaFile) || !File.Exists(instalacionFile) || !File.Exists(dispositivoFile) || !File.Exists(empresaFile) || !File.Exists(cargoFile) || !File.Exists(cuentaFile) || !File.Exists(empleadoFile) || !File.Exists(huellaFile) || !File.Exists(contratoFile))
                {
                    return false;
                }
                using (Stream stream = File.Open(cadenaFile, FileMode.Open, FileAccess.Read))
                {
                    BinaryFormatter bin = new BinaryFormatter();
                    cadenaTable = (Dictionary<Guid, Tuple<string, List<Guid>>>)bin.Deserialize(stream);
                    if (cadenaTable == null)
                    {
                        return false;
                    }
                }
                using (Stream stream = File.Open(instalacionFile, FileMode.Open, FileAccess.Read))
                {
                    BinaryFormatter bin = new BinaryFormatter();
                    instalacionTable = (Dictionary<Guid, Tuple<string, List<Guid>>>)bin.Deserialize(stream);
                    if (instalacionTable == null)
                    {
                        return false;
                    }

                }
                using (Stream stream = File.Open(dispositivoFile, FileMode.Open, FileAccess.Read))
                {
                    BinaryFormatter bin = new BinaryFormatter();
                    dispositivoTable = (Dictionary<Guid, Tuple<string, string, int, TipoDispositivo, List<Guid>>>)bin.Deserialize(stream);
                    if (dispositivoTable == null)
                    {
                        return false;
                    }
                }
                using (Stream stream = File.Open(empresaFile, FileMode.Open, FileAccess.Read))
                {
                    BinaryFormatter bin = new BinaryFormatter();
                    empresaTable = (Dictionary<Guid, Tuple<string, List<Guid>, List<Guid>>>)bin.Deserialize(stream);
                    if (empresaTable == null)
                    {
                        return false;
                    }
                }
                using (Stream stream = File.Open(cargoFile, FileMode.Open, FileAccess.Read))
                {
                    BinaryFormatter bin = new BinaryFormatter();
                    cargoTable = (Dictionary<Guid, string>)bin.Deserialize(stream);
                    if (cargoTable == null)
                    {
                        return false;
                    }
                }
                using (Stream stream = File.Open(cuentaFile, FileMode.Open, FileAccess.Read))
                {
                    BinaryFormatter bin = new BinaryFormatter();
                    cuentaTable = (Dictionary<Guid, Tuple<string, DateTime?>>)bin.Deserialize(stream);
                    if (cuentaTable == null)
                    {
                        return false;
                    }
                }
                using (Stream stream = File.Open(empleadoFile, FileMode.Open, FileAccess.Read))
                {
                    BinaryFormatter bin = new BinaryFormatter();
                    empleadoTable = (Dictionary<Guid, Tuple<int, string, bool, string, string, Tuple<List<Guid>, List<Guid>, List<Guid>>>>)bin.Deserialize(stream);
                    if (empleadoTable == null)
                    {
                        return false;
                    }
                    //Construir indice;
                    empleadoRUTIndex = new Dictionary<string, Guid>();
                    foreach (var emp in empleadoTable)
                    {
                        empleadoRUTIndex[emp.Value.Item2] = emp.Key;
                    }
                }
                using (Stream stream = File.Open(huellaFile, FileMode.Open, FileAccess.Read))
                {
                    BinaryFormatter bin = new BinaryFormatter();
                    huellaTable = (Dictionary<Guid, TipoHuella>)bin.Deserialize(stream);
                    if (huellaTable == null)
                    {
                        return false;
                    }
                }
                using (Stream stream = File.Open(contratoFile, FileMode.Open, FileAccess.Read))
                {
                    BinaryFormatter bin = new BinaryFormatter();
                    contratoTable = (Dictionary<Guid, Tuple<Guid, Guid, Guid, DateTime, DateTime?>>)bin.Deserialize(stream);
                    if (contratoTable == null)
                    {
                        return false;
                    }
                }

                ReaplicarAcciones();

                dataLoaded = true;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void ReaplicarAcciones()
        {
            List<Accion> lista = accionesPorEnviar.OrderBy<Accion, DateTime>((accion) => { return accion.Fecha; }).ToList<Accion>();
            foreach (Accion accion in lista)
            {
                try
                {
                    accion.Aplicar(this);
                }
                catch (Exception) { }
            }
        }

        private async Task<bool> ReloadData()
        {
            Dictionary<Guid, Tuple<string, List<Guid>>> newCadenaTable;
            Dictionary<Guid, Tuple<string, List<Guid>>> newInstalacionTable;
            Dictionary<Guid, Tuple<string, string, int, TipoDispositivo, List<Guid>>> newDispositivoTable;
            Dictionary<Guid, Tuple<string, List<Guid>, List<Guid>>> newEmpresaTable;
            Dictionary<Guid, string> newCargoTable;
            Dictionary<Guid, Tuple<string, DateTime?>> newCuentaTable;
            Dictionary<Guid, Tuple<int, string, bool, string, string, Tuple<List<Guid>, List<Guid>, List<Guid>>>> newEmpleadoTable;
            Dictionary<string, Guid> newEmpleadoRUTIndex;
            Dictionary<Guid, TipoHuella> newHuellaTable;
            Dictionary<Guid, Tuple<Guid, Guid, Guid, DateTime, DateTime?>> newContratoTable;

            LoadingDialog loadingDialog = null;
            try
            {
                loadingDialog = new LoadingDialog();
                loadingDialog.Show(this);

                newCadenaTable = await LeeCadena(loadingDialog);
                newInstalacionTable = await LeeInstalacion(loadingDialog, newCadenaTable);
                newDispositivoTable = await LeeDispositivo(loadingDialog, newInstalacionTable);
                newEmpresaTable = await LeeEmpresa(loadingDialog);
                newCargoTable = await LeeCargo(loadingDialog, newEmpresaTable);
                newCuentaTable = await LeeCuenta(loadingDialog, newEmpresaTable);
                var tablasEmpleado = await LeeEmpleado(loadingDialog);
                newEmpleadoTable = tablasEmpleado.Item1;
                newEmpleadoRUTIndex = tablasEmpleado.Item2;
                newHuellaTable = await LeeHuella(loadingDialog, newEmpleadoTable);
                await LeeEmpleadosDispositivos(loadingDialog, newDispositivoTable, newEmpleadoTable);
                newContratoTable = await LeeContrato(loadingDialog, newEmpleadoTable, newEmpresaTable, newCuentaTable, newCargoTable);

                cadenaTable = newCadenaTable;
                instalacionTable = newInstalacionTable;
                dispositivoTable = newDispositivoTable;
                empresaTable = newEmpresaTable;
                cargoTable = newCargoTable;
                cuentaTable = newCuentaTable;
                empleadoTable = newEmpleadoTable;
                empleadoRUTIndex = newEmpleadoRUTIndex;
                huellaTable = newHuellaTable;
                contratoTable = newContratoTable;

                return SaveLoadedData();
            }
            catch
            {
                return false;
            }
            finally
            {
                if (loadingDialog != null)
                {
                    loadingDialog.Close();
                }
            }
        }

        private async Task<Dictionary<Guid, Tuple<string, List<Guid>>>> LeeCadena(ILoadingDialog loading)
        {
            Dictionary<Guid, Tuple<string, List<Guid>>> newCadenaTable;
            var res = await new EnroladorWebServices.EnroladorWebServicesClient().LeeCadenaAsync(loggedUser.Item1);
            loading.PrimerPaso(12, res.Length, "Cargando Cadenas");
            newCadenaTable = new Dictionary<Guid, Tuple<string, List<Guid>>>(res.Length);
            foreach (var cadena in res)
            {
                Guid Oid = cadena.Item1;
                string Nombre = cadena.Item2;
                newCadenaTable[Oid] = new Tuple<string, List<Guid>>(Nombre, new List<Guid>());
                loading.AvanzarActual();
            }

            return newCadenaTable;
        }

        private async Task<Dictionary<Guid, Tuple<string, List<Guid>>>> LeeInstalacion(ILoadingDialog loading, Dictionary<Guid, Tuple<string, List<Guid>>> newCadenaTable)
        {
            Dictionary<Guid, Tuple<string, List<Guid>>> newInstalacionTable;
            var res = await new EnroladorWebServices.EnroladorWebServicesClient().LeeInstalacionAsync(loggedUser.Item1);
            loading.SiguientePaso(res.Length, "Cargando Instalaciones");
            newInstalacionTable = new Dictionary<Guid, Tuple<string, List<Guid>>>(res.Length);
            foreach (var instalacion in res)
            {
                Guid Oid = instalacion.Item1;
                string Nombre = instalacion.Item2;
                Guid Cadena = instalacion.Item3;
                if (newCadenaTable.ContainsKey(Cadena))
                {
                    newCadenaTable[Cadena].Item2.Add(Oid);
                    newInstalacionTable[Oid] = new Tuple<string, List<Guid>>(Nombre, new List<Guid>());
                }
                loading.AvanzarActual();
            }

            return newInstalacionTable;
        }

        private async Task<Dictionary<Guid, Tuple<string, string, int, TipoDispositivo, List<Guid>>>> LeeDispositivo(ILoadingDialog loading, Dictionary<Guid, Tuple<string, List<Guid>>> newInstalacionTable)
        {
            Dictionary<Guid, Tuple<string, string, int, TipoDispositivo, List<Guid>>> newDispositivoTable;
            var res = await new EnroladorWebServices.EnroladorWebServicesClient().LeeDispositivoAsync(loggedUser.Item1);
            loading.SiguientePaso(res.Length, "Cargando Dispositivos");
            newDispositivoTable = new Dictionary<Guid, Tuple<string, string, int, TipoDispositivo, List<Guid>>>(res.Length);
            foreach (var dispositivo in res)
            {
                Guid Oid = dispositivo.Item1;
                string Nombre = dispositivo.Item2;
                string Host = dispositivo.Item3;
                int Puerto = dispositivo.Item4;
                Guid Instalacion = dispositivo.Item5;

                TipoDispositivo TipoDispositivo = TipoDispositivo.Reloj;

                if (TipoDispositivo != TipoDispositivo.Invalido && newInstalacionTable.ContainsKey(Instalacion))
                {
                    newInstalacionTable[Instalacion].Item2.Add(Oid);
                    newDispositivoTable[Oid] = new Tuple<string, string, int, TipoDispositivo, List<Guid>>(Nombre, Host, Puerto, TipoDispositivo, new List<Guid>());
                }
                loading.AvanzarActual();
            }

            return newDispositivoTable;
        }

        private async Task<Dictionary<Guid, Tuple<string, List<Guid>, List<Guid>>>> LeeEmpresa(ILoadingDialog loading)
        {
            Dictionary<Guid, Tuple<string, List<Guid>, List<Guid>>> newEmpresaTable;
            var res = await new EnroladorWebServices.EnroladorWebServicesClient().LeeEmpresaAsync(loggedUser.Item1);
            loading.SiguientePaso(res.Length, "Cargando Empresas");
            newEmpresaTable = new Dictionary<Guid, Tuple<string, List<Guid>, List<Guid>>>(res.Length);
            foreach (var empresa in res)
            {
                Guid Oid = empresa.Item1;
                string Nombre = empresa.Item2;
                newEmpresaTable[Oid] = new Tuple<string, List<Guid>, List<Guid>>(Nombre, new List<Guid>(), new List<Guid>());
                loading.AvanzarActual();
            }

            return newEmpresaTable;
        }

        private async Task<Dictionary<Guid, string>> LeeCargo(ILoadingDialog loading, Dictionary<Guid, Tuple<string, List<Guid>, List<Guid>>> newEmpresaTable)
        {
            Dictionary<Guid, string> newCargoTable;
            var res = await new EnroladorWebServices.EnroladorWebServicesClient().LeeCargoAsync(loggedUser.Item1);
            loading.SiguientePaso(res.Length, "Cargando Cargos");
            newCargoTable = new Dictionary<Guid, string>(res.Length);
            foreach (var cargo in res)
            {
                Guid Oid = cargo.Item1;
                string Nombre = cargo.Item2;
                Guid Empresa = cargo.Item3;
                if (newEmpresaTable.ContainsKey(Empresa))
                {
                    newEmpresaTable[Empresa].Item2.Add(Oid);
                    newCargoTable[Oid] = Nombre;
                }
                loading.AvanzarActual();
            }

            return newCargoTable;
        }

        private async Task<Dictionary<Guid, Tuple<string, DateTime?>>> LeeCuenta(ILoadingDialog loading, Dictionary<Guid, Tuple<string, List<Guid>, List<Guid>>> newEmpresaTable)
        {
            Dictionary<Guid, Tuple<string, DateTime?>> newCuentaTable;
            var res = await new EnroladorWebServices.EnroladorWebServicesClient().LeeCuentaAsync(loggedUser.Item1);
            loading.SiguientePaso(res.Length, "Cargando Cuentas");
            newCuentaTable = new Dictionary<Guid, Tuple<string, DateTime?>>(res.Length);
            foreach (var cuenta in res)
            {
                Guid Oid = cuenta.Item1;
                string Nombre = cuenta.Item2;
                Guid Empresa = cuenta.Item3;
                DateTime? FechaUltimoCierre = cuenta.Item4;
                if (newEmpresaTable.ContainsKey(Empresa))
                {
                    newEmpresaTable[Empresa].Item3.Add(Oid);
                    var atupl = new Tuple<string, DateTime?>(Nombre, FechaUltimoCierre);
                    newCuentaTable[Oid] = atupl;
                }
                loading.AvanzarActual();
            }

            return newCuentaTable;
        }

        private async Task<Tuple<Dictionary<Guid, Tuple<int, string, bool, string, string, Tuple<List<Guid>, List<Guid>, List<Guid>>>>, Dictionary<string, Guid>>> LeeEmpleado(ILoadingDialog loading)
        {
            Dictionary<Guid, Tuple<int, string, bool, string, string, Tuple<List<Guid>, List<Guid>, List<Guid>>>> newEmpleadoTable;
            Dictionary<string, Guid> newEmpleadoRUTIndex;
            var res = await new EnroladorWebServices.EnroladorWebServicesClient().LeeEmpleadoAsync();
            loading.SiguientePaso(res.Length, "Cargando Empleados");
            newEmpleadoTable = new Dictionary<Guid, Tuple<int, string, bool, string, string, Tuple<List<Guid>, List<Guid>, List<Guid>>>>(res.Length);
            newEmpleadoRUTIndex = new Dictionary<string, Guid>(res.Length);
            foreach (var empleado in res)
            {
                Guid Oid = empleado.Item1;
                int EnrollID = empleado.Item2;
                string RUT = empleado.Item3;
                if (!ValidadorRUT.Validar(RUT, out RUT))
                {
                    loading.AvanzarActual();
                    continue;
                }
                bool Contraseña = empleado.Item4;
                string FirstName = empleado.Item5;
                string LastName = empleado.Item6;
                newEmpleadoTable[Oid] = new Tuple<int, string, bool, string, string, Tuple<List<Guid>, List<Guid>, List<Guid>>>(EnrollID, RUT, Contraseña, FirstName, LastName, new Tuple<List<Guid>, List<Guid>, List<Guid>>(new List<Guid>(), new List<Guid>(), new List<Guid>()));
                newEmpleadoRUTIndex[RUT] = Oid;
                loading.AvanzarActual();
            }

            return new Tuple<Dictionary<Guid, Tuple<int, string, bool, string, string, Tuple<List<Guid>, List<Guid>, List<Guid>>>>, Dictionary<string, Guid>>(newEmpleadoTable, newEmpleadoRUTIndex);
        }

        private async Task<Dictionary<Guid, TipoHuella>> LeeHuella(ILoadingDialog loading, Dictionary<Guid, Tuple<int, string, bool, string, string, Tuple<List<Guid>, List<Guid>, List<Guid>>>> newEmpleadoTable)
        {
            Dictionary<Guid, TipoHuella> newHuellaTable;
            var res = await new EnroladorWebServices.EnroladorWebServicesClient().LeeHuellaAsync();
            loading.SiguientePaso(res.Length, "Cargando Huellas");
            newHuellaTable = new Dictionary<Guid, TipoHuella>(res.Length);
            foreach (var huella in res)
            {
                Guid Oid = huella.Item1;
                int TipoHuella = huella.Item2;
                Guid Empleado = huella.Item3;
                if (newEmpleadoTable.ContainsKey(Empleado) && Enum.IsDefined(typeof(TipoHuella), TipoHuella))
                {
                    newEmpleadoTable[Empleado].Item6.Item1.Add(Oid);
                    newHuellaTable[Oid] = (TipoHuella)TipoHuella;
                }
                loading.AvanzarActual();
            }

            return newHuellaTable;
        }

        private async Task LeeEmpleadosDispositivos(ILoadingDialog loading, Dictionary<Guid, Tuple<string, string, int, TipoDispositivo, List<Guid>>> newDispositivoTable, Dictionary<Guid, Tuple<int, string, bool, string, string, Tuple<List<Guid>, List<Guid>, List<Guid>>>> newEmpleadoTable)
        {
            var res = await new EnroladorWebServices.EnroladorWebServicesClient().LeeEmpleadosDispositivosAsync();
            loading.SiguientePaso(res.Length, "Cargando Asignaciones");
            foreach (var asignacion in res)
            {
                Guid Dispositivo = asignacion.Item1;
                Guid Empleado = asignacion.Item2;
                if (newDispositivoTable.ContainsKey(Dispositivo) && newEmpleadoTable.ContainsKey(Empleado))
                {
                    newDispositivoTable[Dispositivo].Item5.Add(Empleado);
                    newEmpleadoTable[Empleado].Item6.Item2.Add(Dispositivo);
                }
                loading.AvanzarActual();
            }
        }

        private async Task<Dictionary<Guid, Tuple<Guid, Guid, Guid, DateTime, DateTime?>>> LeeContrato(ILoadingDialog loading, Dictionary<Guid, Tuple<int, string, bool, string, string, Tuple<List<Guid>, List<Guid>, List<Guid>>>> newEmpleadoTable, Dictionary<Guid, Tuple<string, List<Guid>, List<Guid>>> newEmpresaTable, Dictionary<Guid, Tuple<string, DateTime?>> newCuentaTable, Dictionary<Guid, string> newCargoTable)
        {
            Dictionary<Guid, Tuple<Guid, Guid, Guid, DateTime, DateTime?>> newContratoTable;
            var res = await new EnroladorWebServices.EnroladorWebServicesClient().LeeContratoAsync(loggedUser.Item1);
            loading.SiguientePaso(res.Length, "Cargando Contratos");
            newContratoTable = new Dictionary<Guid, Tuple<Guid, Guid, Guid, DateTime, DateTime?>>(res.Length);
            foreach (var contrato in res)
            {
                Guid Oid = contrato.Item1;
                Guid Empresa = contrato.Item2;
                Guid Cuenta = contrato.Item3;
                Guid Cargo = contrato.Item4;
                DateTime InicioVigencia = (contrato.Item5 == null) ? DateTime.MinValue : contrato.Item5;
                DateTime? FinVigencia = contrato.Item6;
                Guid Empleado = contrato.Item7;
                if (Oid.ToString().ToUpper().Equals("2D4554D6-6A05-4B08-BA50-874C1C2989F9"))
                {
                    loading.SiguientePaso(res.Length, "Cargando Contratos Entré");
                }
                if (newEmpleadoTable.ContainsKey(Empleado) && newEmpresaTable.ContainsKey(Empresa) && newCuentaTable.ContainsKey(Cuenta) && newCargoTable.ContainsKey(Cargo))
                {
                    newEmpleadoTable[Empleado].Item6.Item3.Add(Oid);
                    newContratoTable[Oid] = new Tuple<Guid, Guid, Guid, DateTime, DateTime?>(Empresa, Cuenta, Cargo, InicioVigencia, FinVigencia);
                }
                loading.AvanzarActual();
            }

            return newContratoTable;
        }

        private bool SaveLoadedData()
        {
            try
            {
                using (Stream stream = File.Open(cadenaFile, FileMode.Create, FileAccess.Write))
                {
                    BinaryFormatter bin = new BinaryFormatter();
                    bin.Serialize(stream, cadenaTable);
                    stream.Close();
                }

                using (Stream stream = File.Open(instalacionFile, FileMode.Create, FileAccess.Write))
                {
                    BinaryFormatter bin = new BinaryFormatter();
                    bin.Serialize(stream, instalacionTable);
                    stream.Close();
                }

                using (Stream stream = File.Open(dispositivoFile, FileMode.Create, FileAccess.Write))
                {
                    BinaryFormatter bin = new BinaryFormatter();
                    bin.Serialize(stream, dispositivoTable);
                    stream.Close();
                }

                using (Stream stream = File.Open(empresaFile, FileMode.Create, FileAccess.Write))
                {
                    BinaryFormatter bin = new BinaryFormatter();
                    bin.Serialize(stream, empresaTable);
                    stream.Close();
                }

                using (Stream stream = File.Open(cargoFile, FileMode.Create, FileAccess.Write))
                {
                    BinaryFormatter bin = new BinaryFormatter();
                    bin.Serialize(stream, cargoTable);
                    stream.Close();
                }

                using (Stream stream = File.Open(cuentaFile, FileMode.Create, FileAccess.Write))
                {
                    BinaryFormatter bin = new BinaryFormatter();
                    bin.Serialize(stream, cuentaTable);
                    stream.Close();
                }

                using (Stream stream = File.Open(empleadoFile, FileMode.Create, FileAccess.Write))
                {
                    BinaryFormatter bin = new BinaryFormatter();
                    bin.Serialize(stream, empleadoTable);
                    stream.Close();
                }

                using (Stream stream = File.Open(huellaFile, FileMode.Create, FileAccess.Write))
                {
                    BinaryFormatter bin = new BinaryFormatter();
                    bin.Serialize(stream, huellaTable);
                    stream.Close();
                }

                using (Stream stream = File.Open(contratoFile, FileMode.Create, FileAccess.Write))
                {
                    BinaryFormatter bin = new BinaryFormatter();
                    bin.Serialize(stream, contratoTable);
                    stream.Close();
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion

        #region Acciones
        public void GuardarAcciones(List<Accion> acciones)
        {
            accionesPorEnviar = acciones;
            string tmp = Path.Combine(programFolder, "acciones.tmp");
            string old = Path.Combine(programFolder, "acciones.old");
            string dat = Path.Combine(programFolder, "acciones.dat");

            Monitor.Enter(fileLock);
            try
            {
                using (Stream stream = File.Open(tmp, FileMode.Create, FileAccess.Write))
                {
                    foreach (Accion accion in acciones)
                    {
                        BinaryFormatter bin = new BinaryFormatter();
                        bin.Serialize(stream, accion);
                    }
                    stream.Close();
                }
                if (File.Exists(dat))
                {
                    File.Replace(tmp, dat, old);
                }
                else
                {
                    File.Move(tmp, dat);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al agregar accion al archivo de acciones temporales: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Monitor.Exit(fileLock);
            }
        }

        private void AgregaError(Tuple<Accion, Exception> error)
        {
            Monitor.Enter(fileLock);
            try
            {
                using (Stream stream = File.Open(Path.Combine(programFolder, "pendientes.dat"), FileMode.Append, FileAccess.Write))
                {
                    BinaryFormatter bin = new BinaryFormatter();
                    bin.Serialize(stream, error.Item1);
                    stream.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al agregar accion al archivo de acciones pendientes: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Monitor.Exit(fileLock);
                erroresDeEnvio.Add(string.Format("- {0}:\r\n{1}", error.Item1.Descripcion, error.Item2.Message));
            }
        }
        #endregion

        #region Grilla

        private void gvHistoria_RowStyle(object sender, RowStyleEventArgs e)
        {
            if (e.RowHandle == 0)
            {
                e.Appearance.Font = new Font(e.Appearance.Font.FontFamily, 15, FontStyle.Regular);
            }
        }
        #endregion

        #region Enrolar
        private void enrolar_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                HabilitarSistema(false);
                using (EnrollForm enrollForm = new EnrollForm(this, accionesPorEnviar, null))
                {
                    if (enrollForm.ShowDialog(this) == DialogResult.OK)
                    {
                        GuardarAcciones(enrollForm.Acciones);
                        LlenaGridEmpleados();
                    }
                    else
                    {
                        ReaplicarAcciones();
                    }
                }
            }
            finally
            {
                HabilitarSistema(true);
            }
        }
        #endregion

        #region Otros
        private bool BuscarActualizaciones()
        {
            UpdateCheckInfo info = null;

            if (ApplicationDeployment.IsNetworkDeployed)
            {
                ApplicationDeployment ad = ApplicationDeployment.CurrentDeployment;

                try
                {
                    info = ad.CheckForDetailedUpdate();

                }
                catch (DeploymentDownloadException dde)
                {
                    MessageBox.Show("No fue posible descargar la actualización. \n\nRevise su conexión o intente más tarde. Mensaje: " + dde.Message);
                    return false;
                }
                catch (InvalidDeploymentException ide)
                {
                    MessageBox.Show("No fue posible buscar actualizaciones. Mensaje: " + ide.Message);
                    return false;
                }
                catch (InvalidOperationException ioe)
                {
                    MessageBox.Show("No es posible actualizar esta aplicación. Mensaje: " + ioe.Message);
                    return false;
                }
                catch (Exception e)
                {
                    MessageBox.Show("No es posible actualizar esta aplicación. Mensaje: " + e.Message);
                    return false;
                }


                if (info.UpdateAvailable)
                {
                    Boolean doUpdate = true;

                    if (!info.IsUpdateRequired)
                    {
                        //MostrarMensaje("Actualización Disponible", TipoMensaje.Correcto);
                        DialogResult dr = MessageBox.Show("Hay una nueva versión disponible. Presione OK para actualizar.", "Actualización disponible", MessageBoxButtons.OKCancel);
                        if (!(DialogResult.OK == dr))
                        {
                            doUpdate = false;
                        }
                    }
                    else
                    {
                        //MostrarMensaje("Actualización Obligatoria", TipoMensaje.Advertencia);
                        MessageBox.Show("Hay una nueva versión disponible. Esta actualización es obligatoria." +
                            "La aplicación se actualizará y se reiniciará automáticamente.",
                            "Actualización obligatoria", MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }

                    if (doUpdate)
                    {
                        try
                        {
                            ad.Update();
                            actualizando = true;
                            //MostrarMensaje("Actualización completada.", TipoMensaje.Correcto);
                            MessageBox.Show("Se ha actualizado la aplicación. Ahora se reiniciará.", "Actualización completada");
                            Application.Restart();
                            return true;
                        }
                        catch (DeploymentDownloadException dde)
                        {
                            MessageBox.Show("No fue posible actualizar.\n\nIntente más tarde.\n\nError: " + dde);
                            actualizando = false;
                            return false;
                        }
                        catch (TrustNotGrantedException tnge)
                        {
                            MessageBox.Show("No fue posible actualizar. Error: " + tnge.Message);
                            actualizando = false;
                            return false;
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show("No fue posible actualizar. Error: " + e.Message);
                            actualizando = false;
                            return false;
                        }
                    }
                }
            }
            return false;
        }
        private void gvHistoria_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            if (e.RowHandle >= 0)
            {
                EnrollForm.TipoAccion? tipo = (EnrollForm.TipoAccion?)gvHistoria.GetRowCellValue(e.RowHandle, "Item5");
                if (tipo.HasValue)
                {
                    switch (tipo.Value)
                    {
                        case EnrollForm.TipoAccion.Nueva:
                            e.Appearance.BackColor = Color.Green;
                            break;
                        case EnrollForm.TipoAccion.Modificada:
                            e.Appearance.BackColor = Color.Yellow;
                            break;
                        case EnrollForm.TipoAccion.Eliminada:
                            e.Appearance.BackColor = Color.Red;
                            break;
                    }
                }
                else
                {
                    e.Appearance.BackColor = Color.White;
                }
            }
        }

        private void gvHistoria_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                Point pt = gvHistoria.GridControl.PointToClient(Control.MousePosition);
                GridHitInfo info = gvHistoria.CalcHitInfo(pt);
                if ((info.InRow || info.InDataRow || info.InRowCell) && info.RowHandle >= 0)
                {
                    string RUT = (string)gvHistoria.GetRowCellValue(info.RowHandle, "Item1");
                    if (!string.IsNullOrEmpty(RUT))
                    {
                        try
                        {
                            HabilitarSistema(false);
                            using (EnrollForm enrollForm = new EnrollForm(this, accionesPorEnviar, RUT))
                            {
                                if (enrollForm.ShowDialog(this) == DialogResult.OK)
                                {
                                    GuardarAcciones(enrollForm.Acciones);
                                    LlenaGridEmpleados();
                                }
                                else
                                {
                                    ReaplicarAcciones();
                                }
                            }
                        }
                        finally
                        {
                            HabilitarSistema(true);
                        }
                    }
                }
            }
            catch (Exception) { }
        }
        #endregion

        private void gcHistoria_Click(object sender, EventArgs e)
        {

        }

        private void ContratosActivos()
        {
            //BindingList<Tuple<string, string, string, DateTime, DateTime?, Guid?>> bnlContratos = new BindingList<Tuple<string, string, string, DateTime, DateTime?, Guid?>>();

            //gdcContratos.BeginUpdate();
            //bnlContratos.Clear();
            //if (parent.EmpleadoRUTIndex.ContainsKey(RUT) && parent.EmpleadoTable.ContainsKey(parent.EmpleadoRUTIndex[RUT]))
            //{
            //    foreach (Guid contrato in parent.EmpleadoTable[parent.EmpleadoRUTIndex[RUT]].Item6.Item3)
            //    {
            //        if (parent.ContratoTable.ContainsKey(contrato))
            //        {
            //            var contratoItem = parent.ContratoTable[contrato];
            //            Guid empresa = contratoItem.Item1;
            //            Guid cuenta = contratoItem.Item2;
            //            Guid cargo = contratoItem.Item3;
            //            DateTime inicio = contratoItem.Item4;
            //            DateTime? fin = contratoItem.Item5;
            //            if (parent.EmpresaTable.ContainsKey(empresa) && parent.CuentaTable.ContainsKey(cuenta) && parent.CargoTable.ContainsKey(cargo))
            //            {
            //                TipoAccion? tipoAccion = null;
            //                foreach (Accion accion in acciones)
            //                {
            //                    if (accion is AccionCrearContrato && ((AccionCrearContrato)accion).Oid.Equals(contrato))
            //                    {
            //                        tipoAccion = TipoAccion.Nueva;
            //                        break;
            //                    }
            //                    if (accion is AccionCaducarContrato)
            //                    {
            //                        AccionCaducarContrato accion2 = (AccionCaducarContrato)accion;
            //                        if (accion2.Oid.Equals(contrato))
            //                        {
            //                            tipoAccion = accion2.FinVigencia.HasValue ? TipoAccion.Eliminada : TipoAccion.Modificada;
            //                            break;
            //                        }
            //                    }
            //                }
            //                bnlContratos.Add(new Tuple<string, string, string, DateTime, DateTime?, Guid, TipoAccion?>(parent.EmpresaTable[empresa].Item1, parent.CuentaTable[cuenta], parent.CargoTable[cargo], inicio, fin, contrato, tipoAccion));
            //            }
            //        }
            //    }
            //}
            //gdcContratos.EndUpdate();
        }

        private void gvHistoria_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            //gvHistoria.selectr
            //e.
        }

        private void gvHistoria_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if ((e.Column.Name != "colContratosActivos") || (e.ListSourceRowIndex == DevExpress.XtraGrid.GridControl.InvalidRowHandle)) return;
            var view = sender as ColumnView;
            var rut = (string)view.GetListSourceRowCellValue(e.ListSourceRowIndex, RUT);
            var listaContratos = new List<string>();
            if (EmpleadoRUTIndex.ContainsKey(rut) && EmpleadoTable.ContainsKey(EmpleadoRUTIndex[rut]))
            {
                foreach (Guid contrato in EmpleadoTable[EmpleadoRUTIndex[rut]].Item6.Item3)
                {
                    if (ContratoTable.ContainsKey(contrato))
                    {
                        var contratoItem = ContratoTable[contrato];
                        Guid empresa = contratoItem.Item1;
                        Guid cuenta = contratoItem.Item2;
                        Guid cargo = contratoItem.Item3;
                        DateTime inicio = contratoItem.Item4;
                        DateTime? fin = contratoItem.Item5;

                        if ((fin != null) && (Convert.ToDateTime(fin).Date <= DateTime.Now)) continue; //Contrato Vencido

                        //Si estan todos los nomencladores
                        if (EmpresaTable.ContainsKey(empresa) && CuentaTable.ContainsKey(cuenta) && CargoTable.ContainsKey(cargo))
                        {
                            var inicioContrato = ((inicio == null) || (inicio == DateTime.MinValue) || (inicio == new DateTime(1900,1,1))) ? "Contrato Permanente" : Convert.ToDateTime(inicio).Date.ToShortDateString();

                            var finContrato = ((fin == null) || (fin == DateTime.MinValue)) ? "Contrato Permanente" : Convert.ToDateTime(fin).Date.ToShortDateString();

                            var rangoContrato = (inicioContrato == finContrato) ? "Contrato Permanente" :
                                                    "(" +
                                                        inicio.Date.ToShortDateString() + "-" +
                                                        finContrato + 
                                                    ")";
                            var descripcionContrato = EmpresaTable[empresa].Item1 + "-" +
                                                    CuentaTable[cuenta].ToString() + "-" +
                                                    CargoTable[cargo].ToString() + "-" + 
                                                    rangoContrato + Environment.NewLine;
                            listaContratos.Add(descripcionContrato);
                        }
                    }
                }

                e.DisplayText = string.Concat(listaContratos);
            }
        }
    }
}