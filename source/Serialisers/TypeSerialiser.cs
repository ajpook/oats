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

namespace Oats
{
    // Originally when serialising a `Type` object this Oats serialiser 
    // simply read/wrote the Type's fully qualified .NET name, this don't 
    // really mean much on other platforms, so to make Oats platform 
    // agnostic this serialiser now reads/writes the UUID of the serialiser
    // register for actually serialising that type.
    // This does come with the drawback that Type objects can only be
    // serialised by this serialiser if the type they represent also has
    // a registered serialiser.
    [SerialiserUUID ("d4e166e4-d493-42a9-97ff-c3fac77e8135")]
	public class TypeSerialiser
		: Serialiser<Type>
	{
		public override Type Read (ISerialisationChannel sc)
		{
            var uuidBytes = new Byte [16];

            for (uint i = 0; i < 16; ++i)
                uuidBytes [i] = sc.Read <Byte> ();

            var serialiserUUID = new Guid (uuidBytes);
            Type type = sc.SerialiserInfo.GetSerialiserInfo (serialiserUUID).TargetType;

            if (type == null)
                throw new Exception ("Unknown type: " + serialiserUUID);

			return type;
		}

		public override void Write (ISerialisationChannel sc, Type type)
		{
            var serialiserUUID = sc.SerialiserInfo.GetSerialiserInfo (type).SerialiserUUID;
            var uuidBytes = serialiserUUID.ToByteArray ();

            for (uint i = 0; i < 16; ++i)
                sc.Write <Byte> (uuidBytes [i]);
		}
	}
}

