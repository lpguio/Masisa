using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using System.Windows.Forms;

namespace EnroladorStandAlone.Helper
{
    class SingleGlobalInstance
    {
        private static Mutex createMutex()
        {
            // get application GUID as defined in AssemblyInfo.cs
            string appGuid = ((GuidAttribute)Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(GuidAttribute), false).GetValue(0)).Value.ToString();

            // unique id for global mutex - Global prefix means it is global to the machine
            string mutexId = string.Format("Global\\{{{0}}}", appGuid);

            // Need a place to store a return value in Mutex() constructor call
            bool createdNew = false;

            // edited by Jeremy Wiebe to add example of setting up security for multi-user usage
            // edited by 'Marc' to work also on localized systems (don't use just "Everyone") 
            var allowEveryoneRule = new MutexAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), MutexRights.FullControl, AccessControlType.Allow);
            var securitySettings = new MutexSecurity();
            securitySettings.AddAccessRule(allowEveryoneRule);

            // edited by MasonGZhwiti to prevent race condition on security settings via VanNguyen
            return new Mutex(false, mutexId, out createdNew, securitySettings);
        }


        public static void Run(int timeOut, Form form)
        {
            using (var mutex = createMutex())
            {
                // edited by acidzombie24
                var hasHandle = false;
                try
                {
                    try
                    {
                        // note, you may want to time out here instead of waiting forever
                        // edited by acidzombie24
                        if (timeOut < 0)
                            hasHandle = mutex.WaitOne(Timeout.Infinite, false);
                        else
                            hasHandle = mutex.WaitOne(timeOut, false);

                        if (hasHandle == false)
                        {
                            throw new TimeoutException("Timeout waiting for exclusive access");
                        }

                    }
                    catch (AbandonedMutexException)
                    {
                        // Log the fact the mutex was abandoned in another process, it will still get aquired
                        hasHandle = true;
                    }
                    catch (TimeoutException)
                    {
                        MessageBox.Show("El sistema ya está ejecutandose. Si desea abirlo nuevamente debe cerrar la ventana actual.", "Sistema abierto", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        Application.Exit();
                    }

                    if (hasHandle)
                        Application.Run(form);
                }
                finally
                {
                    // edited by acidzombie24, added if statemnet
                    if (hasHandle)
                        mutex.ReleaseMutex();
                }
            }
        }
    }
}

