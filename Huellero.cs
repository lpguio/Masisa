
//#define DESACTIVAHUELLERO // Desactiva el huellero en modo DEBUG. Comentar para activar.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AxZKFPEngXControl;

namespace EnroladorStandAlone
{

    public class Huellero : IDisposable
    {
        private AxZKFPEngX ZKFPEng;
        private bool connected = false;
        private int FPCache;
        private bool lastValidated = false;
        private bool fingerTouched = false;
        private DateTime lastRejected = DateTime.Now;

        private bool validando = false;
        private bool enrolling = false;
        private int features = 0;

        private Dictionary<int, string> FPTable; // FPID -> Data

        public bool IsConnected { get { return connected; } }
        public Huellero()
        {
#if (DEBUG && DESACTIVAHUELLERO)
            System.Windows.Forms.MessageBox.Show("Huellero desactivado", "Modo de prueba", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);

#else
            ZKFPEng = new AxZKFPEngX();
            ZKFPEng.CreateControl();
            ZKFPEng.FPEngineVersion = "10";
#endif
        }

        #region Inicializacion
        private void Clean()
        {
#if (DEBUG && DESACTIVAHUELLERO)
            FPTable.Clear();
            FPTable = null;
#else
            ZKFPEng.FreeFPCacheDB(FPCache);
            FPTable.Clear();
            FPTable = null;
            ZKFPEng.EndEngine();
#endif
        }

        public bool Connect(Dictionary<Guid, Tuple<TipoHuella, string>> huellaTable)
        {
            if (!connected)
            {
                FPTable = new Dictionary<int, string>();

#if (!DEBUG || !DESACTIVAHUELLERO)
                int ret = ZKFPEng.InitEngine();
                if (ret != 0)
                {
                    ZKFPEng.EndEngine();
                    return false;
                }
                FPCache = ZKFPEng.CreateFPCacheDB();
#endif

                foreach (var huella in huellaTable.Values)
                {
                    int FPID = (int)huella.Item1 + 1;
                    if (FPTable.ContainsKey(FPID))
                    {
                        continue;
                    }
#if (DEBUG && DESACTIVAHUELLERO)
                    FPTable[FPID] = huella.Item2;
#else
                    if (ZKFPEng.AddRegTemplateStrToFPCacheDB(FPCache, FPID, huella.Item2) != 1)
                    {
                        Clean();
                        return false;
                    }
                    else
                    {
                        FPTable[FPID] = huella.Item2;
                    }
#endif
                }
#if (!DEBUG || !DESACTIVAHUELLERO)
                ZKFPEng.OnEnroll += ZKFPEng_OnEnroll;
                ZKFPEng.OnFeatureInfo += ZKFPEng_OnFeatureInfo;
                ZKFPEng.OnCapture += ZKFPEng_OnCapture;
                ZKFPEng.OnFingerLeaving += ZKFPEng_OnFingerLeaving;
                ZKFPEng.OnFingerTouching += ZKFPEng_OnFingerTouching;
#endif

                connected = true;
            }
            return true;
        }

        public void Disconnect()
        {
            if (connected)
            {
#if (!DEBUG || !DESACTIVAHUELLERO)
                ZKFPEng.OnEnroll -= ZKFPEng_OnEnroll;
                ZKFPEng.OnFeatureInfo -= ZKFPEng_OnFeatureInfo;
                ZKFPEng.OnCapture -= ZKFPEng_OnCapture;
                ZKFPEng.OnFingerLeaving -= ZKFPEng_OnFingerLeaving;
                ZKFPEng.OnFingerTouching -= ZKFPEng_OnFingerTouching;
#endif
                Clean();
                connected = false;
            }
        }

        public bool Refrescar(Dictionary<Guid, Tuple<TipoHuella, string>> huellaTable)
        {
            if (!connected)
            {
                return false;
            }

            Dictionary<int, string> FPTable_new = new Dictionary<int, string>();

            foreach (var huella in huellaTable.Values)
            {
                int FPID = (int)huella.Item1 + 1;

#if (!DEBUG || !DESACTIVAHUELLERO)
                if (FPTable.ContainsKey(FPID))
                {
                    string value = FPTable[FPID];
                    if (value.Equals(huella.Item2))
                    {
                        FPTable_new[FPID] = huella.Item2;
                        continue;
                    }
                    if (ZKFPEng.RemoveRegTemplateFromFPCacheDB(FPCache, FPID) != 1)
                    {
                        Disconnect();
                        return false;
                    }
                }

                if (ZKFPEng.AddRegTemplateStrToFPCacheDB(FPCache, FPID, huella.Item2) != 1)
                {
                    Disconnect();
                    return false;
                }
                else
                {
                    FPTable_new[FPID] = huella.Item2;
                }
#else
                FPTable_new[FPID] = huella.Item2;
#endif
            }

#if (!DEBUG || !DESACTIVAHUELLERO)
            foreach (int key in FPTable.Keys)
            {
                if (!FPTable_new.ContainsKey(key) && ZKFPEng.RemoveRegTemplateFromFPCacheDB(FPCache, key) != 1)
                {
                    Disconnect();
                    return false;
                }
            }
#endif
            FPTable = FPTable_new;
            return true;
        }
        #endregion

        #region Metodos
        public async Task Sonido(HuelleroSonidos tipo)
        {
            if (connected)
            {
                await Task.Delay(1);

#if (!DEBUG || !DESACTIVAHUELLERO)
                switch (tipo)
                {
                    case HuelleroSonidos.Solicitud:
                        ZKFPEng.ControlSensor(11, 1);
                        ZKFPEng.ControlSensor(12, 1);
                        /*ZKFPEng.ControlSensor(13, 1);
                        await Task.Delay(500);
                        ZKFPEng.ControlSensor(13, 0);*/
                        ZKFPEng.ControlSensor(12, 0);
                        ZKFPEng.ControlSensor(11, 0);
                        break;
                    case HuelleroSonidos.Correcto:
                        ZKFPEng.ControlSensor(11, 1);
                        /*ZKFPEng.ControlSensor(13, 1);
                        ZKFPEng.ControlSensor(13, 0);*/
                        ZKFPEng.ControlSensor(11, 0);
                        break;
                    case HuelleroSonidos.Incorrecto:
                    default:
                        ZKFPEng.ControlSensor(12, 1);
                        /*ZKFPEng.ControlSensor(13, 1);
                        ZKFPEng.ControlSensor(13, 0);
                        ZKFPEng.ControlSensor(13, 1);
                        ZKFPEng.ControlSensor(13, 0);
                        ZKFPEng.ControlSensor(13, 1);
                        ZKFPEng.ControlSensor(13, 0);
                        ZKFPEng.ControlSensor(13, 1);
                        ZKFPEng.ControlSensor(13, 0);*/
                        ZKFPEng.ControlSensor(12, 0);
                        break;
                }
#endif
            }
        }

        public void CancelarEnrolamiento()
        {
#if (!DEBUG || !DESACTIVAHUELLERO)
            ZKFPEng.CancelEnroll();
#endif
            enrolling = false;
        }

        public void Habilitar(bool habilitar)
        {
            validando = habilitar;
            if (habilitar)
            {
                CancelarEnrolamiento();
            }
        }

        public void Enroll()
        {
            // TODO: Cambiar exceptions
            if (!connected)
                throw new Exception("No hay conexión con el huellero.");

            if (enrolling)
                throw new Exception("Ya se está realizando un enrolamiento");

            enrolling = true;
            features = 0;

#if (!DEBUG || !DESACTIVAHUELLERO)
            try
            {
                ZKFPEng.BeginEnroll();
            }
            catch
            {
                throw new Exception("No fue posible iniciar el modo de enrolamiento.");
            }
#endif
        }
        #endregion

        #region Eventos Internos
        void ZKFPEng_OnEnroll(object sender, IZKFPEngXEvents_OnEnrollEvent e)
        {
            bool ret = e.actionResult;
            string userTemplate = "";

            enrolling = false;
            if (ret)
            {
                userTemplate = ZKFPEng.GetTemplateAsStringEx("10");
                int score = 0, processedFPNumber = 0;
                if (ZKFPEng.IsOneToOneTemplateStr(userTemplate) || (ZKFPEng.IdentificationInFPCacheDB(FPCache, e.aTemplate, ref score, ref processedFPNumber) != -1 && FPTable.ContainsKey(processedFPNumber)))
                {
                    userTemplate = string.Empty;
                    ret = false;
                }
            }
            OnEnrolled(new EnrolledEventArgs(ret, userTemplate));
        }

        void ZKFPEng_OnFeatureInfo(object sender, IZKFPEngXEvents_OnFeatureInfoEvent e)
        {
            if (enrolling)
            {
                features++;
                OnFingerFeature(new FingerFeatureEventArgs(features));
            }
        }

        void ZKFPEng_OnFingerTouching(object sender, EventArgs e)
        {
            fingerTouched = true;
            lastValidated = false;
        }

        void ZKFPEng_OnFingerLeaving(object sender, EventArgs e)
        {
            if (fingerTouched && !enrolling && validando && !lastValidated && (DateTime.Now - lastRejected).TotalSeconds > 1)
            {
                OnRejected();
            }
        }

        void ZKFPEng_OnCapture(object sender, IZKFPEngXEvents_OnCaptureEvent e)
        {
            if (e.actionResult && validando)
            {
                int score = 0, processedFPNumber = 0;
                if (ZKFPEng.IdentificationInFPCacheDB(FPCache, e.aTemplate, ref score, ref processedFPNumber) != -1 && FPTable.ContainsKey(processedFPNumber))
                {
                    lastValidated = true;
                    OnValidated();
                }
            }
        }
        #endregion

        #region Eventos
        public delegate void RejectedEventHandler(object sender, EventArgs e);
        public event RejectedEventHandler Rejected;
        protected virtual void OnRejected()
        {
            lastRejected = DateTime.Now;
            RejectedEventHandler handler = Rejected;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        public delegate void ValidatedEventHandler(object sender, EventArgs e);
        public event ValidatedEventHandler Validated;
        protected virtual void OnValidated()
        {
            ValidatedEventHandler handler = Validated;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        public delegate void EnrolledEventHandler(object sender, EnrolledEventArgs e);
        public event EnrolledEventHandler Enrolled;
        protected virtual void OnEnrolled(EnrolledEventArgs e)
        {
            EnrolledEventHandler handler = Enrolled;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public delegate void FingerFeatureEventHandler(object sender, FingerFeatureEventArgs e);
        public event FingerFeatureEventHandler FingerFeature;
        protected virtual void OnFingerFeature(FingerFeatureEventArgs e)
        {
            FingerFeatureEventHandler handler = FingerFeature;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        #endregion

        public void Dispose()
        {
            if (connected)
            {
                Disconnect();
            }
            ZKFPEng = null;
        }
    }
}
