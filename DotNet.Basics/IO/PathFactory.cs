using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNet.Basics.IO
{
    public class PathFactory
    {
        public Path Create(string protocol, string[] pathTokens)
        {


            throw new NotImplementedException();

        }
        public Path Create(string protocol, string[] pathTokens, bool isFolder, PathDelimiter delimeter)
        {
            if (isFolder)
                return new DirPath(protocol, pathTokens, delimeter);
            else
                return new FilePath(protocol, pathTokens, delimeter);
        }

        public Path Create(string path, DetectOptions detectOptions)
        {
            throw new NotImplementedException();
        }

    }
}
