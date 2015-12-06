using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace LighTake.Infrastructure.Common.Configuration
{
    public enum RunModelEnum
    {
        Release = 1,
        Debug = 2,
        Default = 1
    }

    public class RunModel : ConfigurationSection
    {
        protected RunModel()
        {
        }

        [ConfigurationProperty("model")]
        public virtual Model Model
        {
            get
            {
                return (Model)this["model"] ?? new ReleaseModel();
            }
        }

        [ConfigurationProperty("debugModel")]
        public virtual DebugModel DebugModel
        {
            get { return (DebugModel)this["debugModel"]; }
        }

        public static RunModel GetInstance()
        {
            try
            {
                return (RunModel)ConfigurationManager.GetSection("runModel") ?? new ReleaseRunModel();
            }
            catch
            {
                return new ReleaseRunModel();
            }
        }
    }

    public class ReleaseRunModel : RunModel
    {
        public override Model Model
        {
            get
            {
                return new ReleaseModel();
            }
        }
    }

    [Serializable]
    public class Model : ConfigurationElement
    {
        [ConfigurationProperty("value")]
        public virtual RunModelEnum Value
        {
            get
            {
                string str = this["value"] == null ? string.Empty : this["value"].ToString().Trim().ToLowerInvariant();
                RunModelEnum result;
                switch (str)
                {
                    case "release":
                        result = RunModelEnum.Release;
                        break;
                    case "debug":
                        result = RunModelEnum.Debug;
                        break;
                    default:
                        result = RunModelEnum.Release;
                        break;
                };
                return result;
            }
        }
    }

    public class ReleaseModel : Model
    {
        public override RunModelEnum Value
        {
            get
            {
                return RunModelEnum.Release;
            }
        }
    }

    public class DebugModel : ConfigurationElement
    {
        [ConfigurationProperty("userName")]
        public string UserName
        {
            get
            {
                return this["userName"] == null ? string.Empty : this["userName"].ToString();
            }
        }
    }
}
