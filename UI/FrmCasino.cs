using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraBars;
using System.ComponentModel.DataAnnotations;
using Enrolador.DataAccessLayer;

namespace EnroladorStandAlone.UI
{
    public partial class FrmCasino : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        #region Propiedades
        List<Accion> Acciones { get; set; }
        List<POCOCasino> Casinos { get; set; }
        List<EmpleadoTurnoServicioCasino> ListEmpleadoTurnoServicioCasino { get; set; }
        List<ServicioCasino> ListServicioCasino { get; set; }
        List<TurnoServicio> ListTurnoServicio { get; set; }

        #endregion

        #region Constructor
        public FrmCasino()
        {
            InitializeComponent();
            //gridControl.DataSource = dataSource;
            //bsiRecordsCount.Caption = "RECORDS : " + dataSource.Count;
        }
        #endregion


        #region Metodos

        public static List<Accion> ProcesarCasinos(List<POCOCasino> casinos, List<EmpleadoTurnoServicioCasino> listEmpleadoTurnoServicioCasino, List<ServicioCasino> listServicioCasino, List<TurnoServicio> listTurnoServicio)
        {
            var frm = new FrmCasino() { Casinos = casinos, ListEmpleadoTurnoServicioCasino = listEmpleadoTurnoServicioCasino, ListServicioCasino = listServicioCasino, ListTurnoServicio = listTurnoServicio };
            if (frm.ShowDialog() == DialogResult.Yes)
            {

            }
            return new List<Accion>();
        }

        #endregion
        void bbiPrintPreview_ItemClick(object sender, ItemClickEventArgs e)
        {
            gridControl.ShowRibbonPrintPreview();
        }

        private void bdsCasinos_CurrentItemChanged(object sender, EventArgs e)
        {
            bdsServiciosCasinos.DataSource = null;
            if (bdsCasinos.Current == null) return;
            var Casino = bdsCasinos.Current as POCOCasino;
            bdsServiciosCasinos.DataSource = ListServicioCasino.Where(p => p.Casino == Casino.Oid).ToList();
        }

        private void bdsServiciosCasinos_CurrentChanged(object sender, EventArgs e)
        {
            bdsTurnosServicios.DataSource = null;
            if (bdsServiciosCasinos.Current == null) return;
            var ServicioCasino = bdsServiciosCasinos.Current as ServicioCasino;
            bdsTurnosServicios.DataSource = ListTurnoServicio.Where(p => p.Servicio == ServicioCasino.Oid).ToList();
        }

        private void FrmCasino_Load(object sender, EventArgs e)
        {
            bdsCasinos.DataSource = Casinos;
        }
    }
}