using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bronk
{
    public interface ITimeline
	{
		TimelineType Type { get;}
		void CopyFrom(ITimeline timeline);

	}
}
