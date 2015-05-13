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
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Collections;

namespace Oats
{
	public class SerialiserCollection
		: ISerialiserProvider 
	{
        readonly Dictionary <Type, Guid> type2uuid = new Dictionary<Type, Guid> ();
        readonly Dictionary <Guid, Serialiser> collection = new Dictionary<Guid, Serialiser> ();

		public void AddSerialiser<TTarget>(Serialiser<TTarget> serialiser)
		{
			AddSerialiser (serialiser);
		}

		public void AddSerialiser(Serialiser serialiser)
		{
			Type targetype = serialiser.GetType ().BaseType.GetGenericArguments() [0];

			if (type2uuid.ContainsKey (targetype))
			{
				throw new SerialisationException ("Already have serialiser for type: " + targetype);
			}

            type2uuid [targetype] = serialiser.UUID;
			collection [serialiser.UUID] = serialiser;
            Console.WriteLine (serialiser.UUID + " ~ " + targetype.FullName);
		}

#region ISerialiserProvider

        public Serialiser GetSerialiser (Guid uuid)
        {
            if (!collection.ContainsKey (uuid))
            {
                throw new Exception (
                    "Collection does not contain a Serialiser with uuid:" + uuid + ".");
            }

            return collection [uuid];
        }

		public Serialiser<TTarget> GetSerialiser<TTarget>()
		{
			var serialiser = GetSerialiser (typeof(TTarget));
			var typedSerialiser = serialiser as Serialiser<TTarget>;
			return typedSerialiser;
		}

		public Serialiser GetSerialiser (Type targetype)
		{
			if (!type2uuid.ContainsKey (targetype))
			{
				throw new Exception (
					"Collection does not contain a Serialiser for type " + targetype + ".");
			}

			return collection [type2uuid[targetype]];
		}

#endregion
	}
}

