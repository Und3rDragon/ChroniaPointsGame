using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using ChroniaPointsGame.Utils;

namespace ChroniaPointsGame.Cores.Graphical;

public class FntData
{
    public Dictionary<int, MTexture> textures;
    public Dictionary<int, Vc2> offsets;

    public FntData() { }
    
    public FntData(string path)
    {
        path.CreateFntFontTextures(out textures, out offsets);
    }
    
    public class SessionData : FntData
    {
        public SessionData(string path, bool overwrite = false)
        {
            if (!ChroniaPointsGameModule.Session.cachedFntData.ContainsKey(path) || overwrite)
            {
                path.CreateFntFontTextures(out textures, out offsets);
            }

            if (!ChroniaPointsGameModule.Session.cachedFntData.ContainsKey(path))
            {
                ChroniaPointsGameModule.Session.cachedFntData.Add(path, this);
            }

            if (overwrite)
            {
                ChroniaPointsGameModule.Session.cachedFntData.Enter(path, this);
            }
        }
    }
}
