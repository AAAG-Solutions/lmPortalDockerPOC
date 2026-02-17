using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMWholesale.Common
{
	public class ScreenStack
	{
		private string startScreen = "";
		private Stack<string> subscreens = new Stack<string>();

		public string StartScreen
		{
			get
			{
				return startScreen;
			}
			set
			{
				subscreens.Clear();
			}
		}

		public string CurrentScreen
		{
			get
			{
				if (subscreens.Count == 0)
					return startScreen;
				else
					return subscreens.First();
			}
		}

		public void PushScreen(string screen)
		{
			subscreens.Push(screen);
		}

		public string PopScreen()
		{
			string retval = subscreens.Pop();
			return retval;
		}
	}
}