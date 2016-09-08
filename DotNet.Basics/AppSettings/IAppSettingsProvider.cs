using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNet.Basics.AppSettings
{
    public interface IAppSettingsProvider
    {
        IReadOnlyCollection<IAppSetting> GetAppSettings();
    }
}
