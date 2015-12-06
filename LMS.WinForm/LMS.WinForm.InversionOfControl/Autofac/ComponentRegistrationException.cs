using System;
using System.Runtime.Serialization;

namespace LMS.WinForm.InversionOfControl.Autofac
{
    [Serializable]
    public class ComponentRegistrationException : Exception
    {
        public ComponentRegistrationException(string serviceName)
            : base(String.Format("Component {0} could not be found but is registered in the Lightake/engine/components section", serviceName))
        {
        }

        protected ComponentRegistrationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
