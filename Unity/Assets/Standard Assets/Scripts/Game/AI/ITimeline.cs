using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bronk
{
    public interface ITimeline
	{
        float StartTime { get; }
        float EndTime { get; }
	}
}
