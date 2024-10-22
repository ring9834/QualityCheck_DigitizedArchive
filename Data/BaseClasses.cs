using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prj_FileManageNCheckApp
{
    public class BaseClasses
    {

    }

    public enum ArchiveClass
    { 
        Package,
        Box        
    }

    public enum GJStatus
    { 
        Doing = 1,
        Finish = 2,
        HaveError = 3
    }
}
