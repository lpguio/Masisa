using System;

namespace EnroladorStandAlone
{
    public class EnrolledEventArgs : EventArgs
    {
        protected bool _success;
        protected string _template;
        public bool Success { get { return _success; } }
        public string Template { get { return _template; } }
        public EnrolledEventArgs(bool success, string template)
        {
            _success = success;
            _template = template;
        }
    }
}
