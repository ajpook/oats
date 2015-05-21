// ┌────────────────────────────────────────────────────────────────────────┐ \\
// │ ________          __                                                   │ \\
// │ \_____  \ _____ _/  |_  ______                                         │ \\
// │  /   |   \\__  \\   __\/  ___/                                         │ \\
// │ /    |    \/ __ \|  |  \___ \                                          │ \\
// │ \_______  (____  /__| /____  >                                         │ \\
// │         \/     \/          \/                                          │ \\
// │                                                                        │ \\
// │ An awesome C# serialisation library.                                   │ \\
// │                                                                        │ \\
// ├────────────────────────────────────────────────────────────────────────┤ \\
// │ Copyright © 2012 - 2015 ~ Blimey3D (http://www.blimey3d.com)           │ \\
// ├────────────────────────────────────────────────────────────────────────┤ \\
// │ Authors:                                                               │ \\
// │ ~ Ash Pook (http://www.ajpook.com)                                     │ \\
// ├────────────────────────────────────────────────────────────────────────┤ \\
// │ Permission is hereby granted, free of charge, to any person obtaining  │ \\
// │ a copy of this software and associated documentation files (the        │ \\
// │ "Software"), to deal in the Software without restriction, including    │ \\
// │ without limitation the rights to use, copy, modify, merge, publish,    │ \\
// │ distribute, sublicense, and/or sellcopies of the Software, and to      │ \\
// │ permit persons to whom the Software is furnished to do so, subject to  │ \\
// │ the following conditions:                                              │ \\
// │                                                                        │ \\
// │ The above copyright notice and this permission notice shall be         │ \\
// │ included in all copies or substantial portions of the Software.        │ \\
// │                                                                        │ \\
// │ THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,        │ \\
// │ EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF     │ \\
// │ MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. │ \\
// │ IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY   │ \\
// │ CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,   │ \\
// │ TORT OR OTHERWISE, ARISING FROM,OUT OF OR IN CONNECTION WITH THE       │ \\
// │ SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.                 │ \\
// └────────────────────────────────────────────────────────────────────────┘ \\

using System;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;

namespace Oats
{

	/// <summary>
	/// This class provides a mechanism for finding implementations
	/// of Oats.Serialiser<> at runtime.
	/// </summary>
	public static class SerialiserFinder
	{
		public class SearchResult
		{
            public SerialiserInfo[] SerialiserInfos { get; set; }
		}

		/// <summary>
		/// Searches loaded assemblies for Types that implement
		/// Serialiser <T> and builds a cache.
		/// </summary>
		public static SearchResult Search ()
		{
            List <SerialiserInfo> serialiserInfos = new List<SerialiserInfo> ();

			Assembly[] assemblies = AppDomain.CurrentDomain
				.GetAssemblies ()
				.Where (a => a.GetName ().Name.ToLower () != "mscorlib")
				.Where (a => !a.GetName ().Name.ToLower ().StartsWith ("microsoft"))
				.Where (a => !a.GetName ().Name.ToLower ().StartsWith ("mono"))
				.Where (a => !a.GetName ().Name.ToLower ().StartsWith ("system"))
				.Where (a => !a.GetName ().Name.ToLower ().StartsWith ("nunit"))
				.ToArray ();

			foreach (var assembly in assemblies)
			{
				SearchResult sr = Search (assembly);
                serialiserInfos.AddRange (sr.SerialiserInfos);
			}

			var result = new SearchResult ()
			{
                SerialiserInfos = serialiserInfos.ToArray ()
			};

			return result;
		}

		public static SearchResult Search (Assembly assembly)
		{
			var assemblyTypes = assembly.GetTypes ();

            var serialiserInfos = assemblyTypes
				.Where (x => !x.IsAbstract)
				.Where (x => (x.BaseType != null) && x.BaseType.BaseType == typeof (Serialiser))
                .Select (t =>
                    new SerialiserInfo {
                        SerialiserType = t,
                        SerialiserUUID = Serialiser.GetUUID (t),
                        TargetType = t.BaseType.GetGenericArguments ()[0] })
				.ToList ();

			var result = new SearchResult ()
			{
                SerialiserInfos = serialiserInfos.ToArray ()
			};

			return result;
		}
	}
}

