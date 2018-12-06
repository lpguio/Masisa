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
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraGrid.Views.Base;

namespace EnroladorStandAlone.UI
{
    public partial class FrmCasino : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        #region Propiedades
        string RUT { get; set; }
        Form1 GlobalForm { get; set; } 
        Guid Empleado
        {
            get
            {
                return GlobalForm.EmpleadoRUTIndex[RUT];
            }
        }
        List<Accion> Acciones { get; set; }
        List<POCOCasino> Casinos { get; set; }
        List<EmpleadoTurnoServicioCasino> ListEmpleadoTurnoServicioCasino { get; set; }
        List<ServicioCasino> ListServicioCasino { get; set; }
        List<TurnoServicio> ListTurnoServicio { get; set; }
        private List<Accion> AccionesPorEnviar { get; set; }
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

        public static List<Accion> ProcesarCasinos(List<Accion> accionesPorEnviar, string rUT, Form1 globalForm, List<POCOCasino> casinos, List<EmpleadoTurnoServicioCasino> listEmpleadoTurnoServicioCasino, List<ServicioCasino> listServicioCasino, List<TurnoServicio> listTurnoServicio)
        {
            var frm = new FrmCasino() { AccionesPorEnviar = accionesPorEnviar, RUT = rUT, GlobalForm = globalForm, Casinos = casinos, ListEmpleadoTurnoServicioCasino = listEmpleadoTurnoServicioCasino, ListServicioCasino = listServicioCasino, ListTurnoServicio = listTurnoServicio };
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

        private void slueCasinos_EditValueChanged(object sender, EventArgs e)
        {
            bdsServiciosCasinos.DataSource = null;
            bdsTurnosServicios.DataSource = null;
            if (slueCasinos.EditValue == null) return;
            var list = ListServicioCasino.Where(p => p.Casino == (Guid)slueCasinos.EditValue).OrderBy(p => p.Nombre).ToList();
            bdsServiciosCasinos.DataSource = list;
        }

        private void FrmCasino_Load(object sender, EventArgs e)
        {
            bdsCasinos.DataSource = Casinos;
            RefrescarGrid();
        }

        private void slueServicios_EditValueChanged(object sender, EventArgs e)
        {
            bdsTurnosServicios.DataSource = null;
            if (slueServicios.EditValue == null) return;
            var ServicioCasino = bdsServiciosCasinos.Current as ServicioCasino;
            bdsTurnosServicios.DataSource = ListTurnoServicio.Where(p => p.Servicio == (Guid)slueServicios.EditValue).OrderBy(p => p.Nombre).ToList();
        }

        private void bbiNew_ItemClick(object sender, ItemClickEventArgs e)
        {
            if ((slueTurnos.EditValue == null) || (string.IsNullOrEmpty(slueTurnos.EditValue.ToString()))) return;
            var OidTurno = (Guid)slueTurnos.EditValue;
            var elemento = ListEmpleadoTurnoServicioCasino.FirstOrDefault(p => (p.Empleado == Empleado) && (p.TurnoServicio == OidTurno));
            if (elemento != null) return;
            var empleadoTurnoServicioCasino = new EmpleadoTurnoServicioCasino()
            {
                Empleado = Empleado,
                TurnoServicio = OidTurno
            };
            ListEmpleadoTurnoServicioCasino.Add(empleadoTurnoServicioCasino);

            Accion accion = new AccionEmpleadoTurnoServicioCasino(empleadoTurnoServicioCasino, GlobalForm);

            AccionesPorEnviar.Add(accion);

            GlobalForm.GuardarAcciones(AccionesPorEnviar);

            RefrescarGrid();
        }

        private void RefrescarGrid()
        {
            bdsTurnosServiciosEmpleado.DataSource = ListEmpleadoTurnoServicioCasino.Where(p => p.Empleado == Empleado).ToList();
        }

        private void gridView_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if (e.ListSourceRowIndex == DevExpress.XtraGrid.GridControl.InvalidRowHandle) return;
            var view = sender as ColumnView;
            var idx = (Guid)view.GetListSourceRowCellValue(e.ListSourceRowIndex, colIdx);
            switch (e.Column.Name)
            {
                case "colTurno":
                    e.DisplayText = ListTurnoServicio.FirstOrDefault(p => p.Oid == idx).Nombre;
                break;
                case "colServicio":
                    e.DisplayText = ListServicioCasino.FirstOrDefault(p => p.Oid == ListTurnoServicio.FirstOrDefault(q => q.Oid == idx).Servicio).Nombre;
                break;
                default:
                    var oidServicio = ListTurnoServicio.FirstOrDefault(q => q.Oid == idx).Servicio;
                    var oidEmpresa = ListServicioCasino.FirstOrDefault(p => p.Oid == oidServicio).Casino;
                    switch (e.Column.Name)
                    {
                        case "colInicio":
                            var timespan = ListTurnoServicio.FirstOrDefault(p => p.Oid == idx).HoraInicio;
                            e.DisplayText = string.Format("{0:00}:{1:00}", timespan.Hours, timespan.Minutes);
                            break;
                        case "colFin":
                            var timespan1 = ListTurnoServicio.FirstOrDefault(p => p.Oid == idx).HoraFin;
                            e.DisplayText = string.Format("{0:00}:{1:00}", timespan1.Hours, timespan1.Minutes);
                        break;
                        default:
                            e.DisplayText = GlobalForm.InstalacionTable[oidEmpresa].Item1;
                        break;
                    }
                break;
            }
        }

        private void bbiDelete_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (XtraMessageBox.Show("Desea eliminar el acceso seleccionado?", "Eliminar Acceso", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                var empleadoTurnoServicioCasino = (EmpleadoTurnoServicioCasino)gridView.GetRow(gridView.GetSelectedRows()[0]);
                ListEmpleadoTurnoServicioCasino.Remove(empleadoTurnoServicioCasino);

                Accion accion = new AccionEliminarEmpleadoTurnoServicio(empleadoTurnoServicioCasino, GlobalForm);

                AccionesPorEnviar.Add(accion);

                GlobalForm.GuardarAcciones(AccionesPorEnviar);

                RefrescarGrid();
            }
        }
    }
}