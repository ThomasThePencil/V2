using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace V2.Core
{
	/// <summary>
	/// Represents the three "appetite tunings" utilized by the Rod of Objectum.
	/// </summary>
	public enum ObjectAppetiteTuning
	{
		/// <summary>
		/// Objects under this appetite style will not eat anything unless explicitly force-fed.
		/// </summary>
		Reserved,
		/// <summary>
		/// Objects under this appetite style will occasionally eat nearby hostile mobs on their own.
		/// </summary>
		Friendly,
		/// <summary>
		/// Objects under this appetite style will eat anything and everything that comes in range on their own.
		/// </summary>
		Indiscriminate,
	}
}
