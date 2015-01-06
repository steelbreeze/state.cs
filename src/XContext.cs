/* State v5 finite state machine library
 * http://www.steelbreeze.net/state.cs
 * Copyright (c) 2014-5 Steelbreeze Limited
 * Licensed under MIT and GPL v3 licences
 */using System;
using System.Linq;
using System.Xml.Linq;

namespace Steelbreeze.Behavior.StateMachines {
	/// <summary>
	/// A sample of an object to extend as a base for a state machine context objects based on an XElement.
	/// </summary>
	public class XContext : XContextBase<XContext> { }
}
