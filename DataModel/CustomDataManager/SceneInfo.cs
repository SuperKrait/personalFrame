using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.CustomDataManager
{
    public class SceneInfo
    {
        public string Id
        {
            get;
            set;
        }        

        public string Name
        {
            get;
            set;
        }

        public List<string> sceneType = new List<string>();

        public List<SceneFlagStruct> flag = new List<SceneFlagStruct>();

        public string ThumbnailPath
        {
            get;
            set;
        }

        public List<string> includeProductIds = new List<string>();

        public List<string> includePanoIds = new List<string>();

        public string DefaultPanoName
        {
            set;
            get;
        }
    }
}
