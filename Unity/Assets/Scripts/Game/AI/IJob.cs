using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bronk
{
    public delegate void JobCallback(IJob job);

    public interface IJob
	{
        //event JobCallback Completed;
        //event JobCallback AbortedByAnt;

        void complete();
        void abortByAnt();
        void abortByUser();
	}
}
