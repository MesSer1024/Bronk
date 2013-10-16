using UnityEngine;
using System.Collections;

namespace Bronk
{
	public interface IMessageListener {
		void onMessage (IMessage message);
	}
}