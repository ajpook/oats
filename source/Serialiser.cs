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
using System.Linq;

namespace Oats
{
    public class SerialiserInfo
    {
        public Type SerialiserType { get; set; }
        public Guid SerialiserUUID { get; set; }
        public Type TargetType { get; set; }
    }

    public class SerialiserUUIDAttribute : Attribute
    {
        readonly Guid uuid;

        public Guid UUID { get { return uuid; } }

        public SerialiserUUIDAttribute (String uuidString)
        {
            this.uuid = Guid.Parse (uuidString);
        }

        public SerialiserUUIDAttribute (Guid uuid)
        {
            this.uuid = uuid;
        }
    }

	public abstract class Serialiser
    {
        readonly SerialiserInfo info;

        public Type TargetType { get { return info.TargetType; } }

        public Guid UUID { get { return info.SerialiserUUID; } }

        public SerialiserInfo Info { get { return info; } }

        protected Serialiser(Type targetType)
		{
            var t = GetType ();
            this.info = new SerialiserInfo {
                SerialiserType = t,
                SerialiserUUID = GetUUID (t),
                TargetType = targetType
            };
		}

		public abstract Object ReadObject (ISerialisationChannel sc);

		public abstract void WriteObject (ISerialisationChannel sc, Object obj);

        public static Guid GetUUID (Type t)
        {
            var attribute = t
                .GetCustomAttributes (typeof(SerialiserUUIDAttribute), true)
                .FirstOrDefault() as SerialiserUUIDAttribute;

            if (attribute != null)
            {
                return attribute.UUID;
            }
            else
            {
                throw new Exception ("Serialiser must have SerialiserUUID Attribute");
            }
        }
	}

	public abstract class Serialiser<T>
		: Serialiser
	{
        protected Serialiser (): base(typeof(T)) {}

		public override Object ReadObject (ISerialisationChannel sc)
		{
			return this.Read (sc);
		}

		public override void WriteObject (ISerialisationChannel sc, Object obj)
		{
			this.Write (sc, (T) obj);
		}

		public abstract T Read (ISerialisationChannel sc);

		public abstract void Write (ISerialisationChannel sc, T obj);
	}
}

