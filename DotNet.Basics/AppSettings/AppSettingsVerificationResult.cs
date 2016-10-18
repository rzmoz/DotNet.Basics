using System.Collections.Generic;
using System.Linq;

namespace DotNet.Basics.AppSettings
{
    public class AppSettingsVerificationResult
    {
        public AppSettingsVerificationResult(IEnumerable<string> missingKeys = null)
        {
            MissingKeys = missingKeys?.ToList() ?? new List<string>();
            AllGood = MissingKeys.Count == 0;
        }
        public IReadOnlyCollection<string> MissingKeys { get; }
        public bool AllGood { get; }
    }
}
