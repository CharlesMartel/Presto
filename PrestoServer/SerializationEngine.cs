using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;
using System.Linq;
using System.Text;

namespace Presto {

    /// <summary>
    /// Handles serialization throughout the Presto Server
    /// </summary>
    public static class SerializationEngine {

        //an internal soap serializer
        private static BinaryFormatter serializer = new BinaryFormatter();
        //apparantly the serializer is not thread safe... found that out the hard way
        private static object locker = new object();

        /// <summary>
        /// Serializes the passed in object.
        /// </summary>
        /// <param id="obj">The object to be serialized.</param>
        /// <returns>The serialization stream of the object.</returns>
        public static MemoryStream Serialize(Object obj) {
            MemoryStream stream = new MemoryStream();
            lock (locker) {                
                serializer.Serialize(stream, obj);
            }
            stream.Dispose();
            return stream;            
        }

        /// <summary>
        /// Deserializes a byte array into
        /// </summary>
        /// <param id="bytes">The byte array of the object to be deserialized.</param>
        /// <returns></returns>
        public static Object Deserialize(byte[] bytes) {
            Object obj = null;
            MemoryStream stream = new MemoryStream(bytes);
            serializer.AssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple;
            serializer.Binder = new VersionConfigToNamespaceAssemblyObjectBinder();
            lock (locker) {                
                 obj = (Object)serializer.Deserialize(stream);
            }
            stream.Dispose();
            return obj;            
        }

        //I took this from http://social.msdn.microsoft.com/forums/en-US/netfxbcl/thread/fec7ea31-5241-4fbd-a9c7-9ae602e172d4/
        //It does a lookup for the appropriate assembly at the time when the object deserialization is binding to 
        // a type. it is useful because all of our assemblies are dynmically loaded and it would work otherwise...
        internal sealed class VersionConfigToNamespaceAssemblyObjectBinder : SerializationBinder {
            public override Type BindToType(string assemblyName, string typeName) {
                Type typeToDeserialize = null;
                List<Type> tmpTypes = new List<Type>();
                Type genType = null;
                try {
                    if (typeName.Contains("System.Collections.Generic") && typeName.Contains("[[")) {
                        string[] splitTyps = typeName.Split(new char[] { '[' });

                        foreach (string typ in splitTyps) {
                            if (typ.Contains("Version")) {
                                string asmTmp = typ.Substring(typ.IndexOf(',') + 1);
                                string asmName = asmTmp.Remove(asmTmp.IndexOf(']')).Trim();
                                string typName = typ.Remove(typ.IndexOf(','));
                                tmpTypes.Add(BindToType(asmName, typName));
                            }
                            else if (typ.Contains("Generic")) {
                                genType = BindToType(assemblyName, typ);
                            }
                        }
                        if (genType != null && tmpTypes.Count > 0) {
                            return genType.MakeGenericType(tmpTypes.ToArray());
                        }
                    }

                    string ToAssemblyName = assemblyName.Split(',')[0];
                    Assembly[] Assemblies = AppDomain.CurrentDomain.GetAssemblies();
                    foreach (Assembly assembly in Assemblies) {
                        if (assembly.FullName.Split(',')[0] == ToAssemblyName) {
                            typeToDeserialize = assembly.GetType(typeName);
                            break;
                        }
                    }
                }
                catch (System.Exception exception) {
                    throw exception;
                }
                return typeToDeserialize;
            }
        }
    }
}
