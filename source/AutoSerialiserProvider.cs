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
using System.Linq;

namespace Oats
{
	public class AutoSerialiserProvider
		: ISerialiserProvider
	{
        readonly SerialiserCollection serialiserCollection = new SerialiserCollection ();

        readonly Dictionary <String, Boolean> autoAddAttempts = new Dictionary <String, Boolean> ();

        readonly Dictionary <Guid, String> basicSerialiserTypesU = new Dictionary <Guid, String> ();
        readonly Dictionary <String, SerialiserInfo> basicSerialiserTypesT = new Dictionary <String, SerialiserInfo> ();

        readonly Dictionary <Guid, String> genericSerialiserTypesU = new Dictionary <Guid, String> ();
        readonly Dictionary <String, SerialiserInfo> genericSerialiserTypesT = new Dictionary <String, SerialiserInfo> ();

		public AutoSerialiserProvider ()
		{
			var searchResult = SerialiserFinder.Search ();

			foreach (var serialiserInfo in searchResult.SerialiserInfos)
			{
                if (serialiserInfo.SerialiserType.IsGenericType)
                {
                    genericSerialiserTypesU.Add (serialiserInfo.SerialiserUUID, serialiserInfo.TargetType.GetIdentifier ());
                    genericSerialiserTypesT.Add (serialiserInfo.TargetType.GetIdentifier (), serialiserInfo);
				}
				else
                {
                    basicSerialiserTypesU.Add (serialiserInfo.SerialiserUUID, serialiserInfo.TargetType.GetIdentifier ());
                    basicSerialiserTypesT.Add (serialiserInfo.TargetType.GetIdentifier (), serialiserInfo);
				}
			}
		}

        public SerialiserInfo GetSerialiserInfo (Guid serialiserUUID)
        {
            if (basicSerialiserTypesU.ContainsKey (serialiserUUID)) return basicSerialiserTypesT[basicSerialiserTypesU [serialiserUUID]];
            if (genericSerialiserTypesU.ContainsKey (serialiserUUID)) return genericSerialiserTypesT[genericSerialiserTypesU [serialiserUUID]];
            throw new SerialisationException ("");
        }

        public SerialiserInfo GetSerialiserInfo (Type targetType)
        {
            if (basicSerialiserTypesT.ContainsKey (targetType.GetIdentifier ())) return basicSerialiserTypesT [targetType.GetIdentifier ()];
            if (genericSerialiserTypesT.ContainsKey (targetType.GetIdentifier ())) return genericSerialiserTypesT [targetType.GetIdentifier ()];
            throw new SerialisationException ("");
        }

		public Serialiser<TTarget> GetSerialiser<TTarget>()
		{
			var serialiser = GetSerialiser (typeof(TTarget));
			var typedSerialiser = serialiser as Serialiser<TTarget>;
			return typedSerialiser;
		}

        public Serialiser GetSerialiser (Guid uuid)
        {
            if (basicSerialiserTypesU.ContainsKey (uuid)) return GetSerialiser (Type.GetType (basicSerialiserTypesU [uuid]));
            if (genericSerialiserTypesU.ContainsKey (uuid)) return GetSerialiser (Type.GetType (genericSerialiserTypesU [uuid]));

            throw new SerialisationException ("");
        }

		public Serialiser GetSerialiser (Type targetType)
		{
            Console.WriteLine ("Get Serialiser: " + targetType);

            if (!autoAddAttempts.ContainsKey (targetType.GetIdentifier ()))
			{
				Boolean ok = TryAdd (targetType);

                if (!ok)
                {
                    throw new SerialisationException ("");
                }

                autoAddAttempts [targetType.GetIdentifier ()] = true;
			}

            if (autoAddAttempts [targetType.GetIdentifier ()])
			{
				return serialiserCollection.GetSerialiser (targetType);
			}

			throw new SerialisationException ("");
		}

		void Add (Type targetType)
		{
			Type serialiserType = null;

			if (targetType.IsEnum)
			{
				serialiserType = typeof(EnumSerialiser<>).MakeGenericType (targetType);
			}
			else if (targetType.IsArray)
			{
				Type t = targetType.GetElementType ();
				serialiserType = typeof(ArraySerialiser<>).MakeGenericType (t);
			}
			else if (targetType.IsGenericType)
			{
				Type gt = targetType.GetGenericTypeDefinition ();

                if (!genericSerialiserTypesT.ContainsKey (gt.GetIdentifier ()))
                {
                    throw new Exception ();
                }

                Type unboundSerialiserType = genericSerialiserTypesT [gt.GetIdentifier ()].SerialiserType;

				Type t = targetType.GetGenericArguments () [0];

				serialiserType = unboundSerialiserType.MakeGenericType (t);
			}
			else
			{
                serialiserType = basicSerialiserTypesT [targetType.GetIdentifier ()].SerialiserType;
			}

			Serialiser serialiserInstance = null;

			if (serialiserType != null)
			{
				serialiserInstance = SerialiserActivator.CreateReflective (serialiserType);
			}

			if (serialiserInstance != null)
			{
				serialiserCollection.AddSerialiser (serialiserInstance);
				// Console.WriteLine ("SerialiserDatabase: Automatically registered -> " + serialiserInstance.GetType().ToString ());
			}
		}


		Boolean TryAdd (Type targetType)
		{
			try
			{
				Add (targetType);
				return true;
			}
			catch (Exception ex)
			{
                Console.WriteLine (ex.GetType () + " " + ex.Message);
				return false;
			}
		}
	}
}

