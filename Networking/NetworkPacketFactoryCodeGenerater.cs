using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace MHGameWork.TheWizards.Networking
{
    /// <summary>
    /// WARNING: this class is a stub, there is not yet a design for this functionality, so this class might be removed
    /// </summary>
    public class NetworkPacketFactoryCodeGenerater
    {
        private Dictionary<Type, INetworkPacketFactory> factoriesMap;

        private INetworkPacketFactoryFactory factoryFactory;
        private string generatedAssemblyFilename;

        private Assembly loadedCachedAssembly;

        public NetworkPacketFactoryCodeGenerater(string _generatedAssemblyFilename)
        {
            System.IO.FileInfo info = new FileInfo(_generatedAssemblyFilename);
            if (info.Directory == null) throw new InvalidOperationException();
            info.Directory.Create();

            generatedAssemblyFilename = _generatedAssemblyFilename;
            factoriesMap = new Dictionary<Type, INetworkPacketFactory>();
        }

        public INetworkPacketFactory<T> GetFactory<T>() where T : INetworkPacket
        {
            INetworkPacketFactory ret;
            if (!factoriesMap.TryGetValue(typeof(T), out ret))
            {
                ret = getGeneratedFactory<T>();
                if (ret != null)
                {
                    factoriesMap.Add(typeof(T), ret);
                    return ret as INetworkPacketFactory<T>;
                }
                HolderFactory<T> holder = new HolderFactory<T>(this);
                factoriesMap.Add(typeof(T), holder);
                ret = holder;
            }

            return ret as INetworkPacketFactory<T>;
        }

        private INetworkPacketFactory<T> getGeneratedFactory<T>() where T : INetworkPacket
        {
            if (factoryFactory == null) return null;
            return factoryFactory.GetFactory<T>();
        }

        /// <summary>
        /// This generates code for all registered factories and builds an assembly.
        /// </summary>
        public void BuildFactoriesAssembly()
        {
            if (loadedCachedAssembly != null) throw new InvalidOperationException("Assembly has already been loaded!");
            List<String> code = new List<string>();

            CompilerParameters cp = new CompilerParameters();
            cp.GenerateExecutable = false;
            cp.GenerateInMemory = false;
            cp.IncludeDebugInformation = true;
            cp.TreatWarningsAsErrors = true;
            cp.OutputAssembly = generatedAssemblyFilename;
            //cp.CompilerOptions = "/optimize";

            code.Add(generateFactoryFactoryCode());
            cp.ReferencedAssemblies.Add(typeof(INetworkPacketFactoryFactory).Assembly.Location);
            cp.ReferencedAssemblies.Add(typeof(INetworkPacketFactory).Assembly.Location);



            foreach (Type type in factoriesMap.Keys)
            {
                if (!type.IsPublic && !type.IsNestedPublic)
                {
                    throw new InvalidOperationException("Cannot create a factory for a non-public packet! (" + type.FullName + ")");
                }

                code.Add("\n\n" + generateFactoryCode(type));
                cp.ReferencedAssemblies.Add(type.Assembly.Location);

            }

            var sources = new List<string>();

            int num = 0;
            foreach (var codeFile in code)
            {
                var sourceFile = generatedAssemblyFilename.Replace(".dll", num + ".cs");
                sources.Add(sourceFile);
                File.WriteAllText(sourceFile, codeFile);
                num++;
            }

            Assembly assembly = AssemblyBuilder.CompileExecutableFromFile(cp, sources.ToArray());

            if (assembly == null) throw new InvalidOperationException();

            openGeneratedFactoriesAssembly(generatedAssemblyFilename);
        }

        public void LoadCachedAssembly()
        {
            openGeneratedFactoriesAssembly(generatedAssemblyFilename);
        }

        private void openGeneratedFactoriesAssembly(string filename)
        {
            if (!System.IO.File.Exists(filename)) return;
            Assembly assembly = Assembly.LoadFile(filename);
            loadedCachedAssembly = assembly;
            factoryFactory = assembly.CreateInstance("MHGameWork.TheWizards.Networking.Generated.NetworkPacketFactoryFactory") as INetworkPacketFactoryFactory;
            if (factoryFactory == null) return;
        }

        private string generateFactoryFactoryCode()
        {

            string ret = "";
            ret += "using System;";
            ret += "using System.Collections.Generic;";
            ret += "namespace MHGameWork.TheWizards.Networking.Generated\n";
            ret += "{\n";
            ret += "public class NetworkPacketFactoryFactory : INetworkPacketFactoryFactory";
            ret += "{\n";
            ret += "private Dictionary<Type, INetworkPacketFactory> factoriesMap;\n";

            ret += "public NetworkPacketFactoryFactory()\n";
            ret += "{\n";
            ret += "factoriesMap = new Dictionary<Type, INetworkPacketFactory>();\n";

            foreach (Type type in factoriesMap.Keys)
            {
                string fullTypeName;
                if (type.IsNestedPublic)
                {
                    fullTypeName = type.ReflectedType.FullName + "." + type.Name;
                }
                else
                {
                    fullTypeName = type.FullName;
                }

                ret += "factoriesMap.Add( typeof( " + fullTypeName + " ), new " + type.Name + "Factory() );\n";
            }

            ret += "}\n";

            ret += "public INetworkPacketFactory<T> GetFactory<T>() where T : INetworkPacket\n";
            ret += "{\n";
            ret += "INetworkPacketFactory ret;\n";
            ret += "if ( !factoriesMap.TryGetValue( typeof( T ), out ret ) )\n";
            ret += "return null;\n";

            ret += "return ret as INetworkPacketFactory<T>;\n";
            ret += "}\n";
            ret += "}\n";
            ret += "}\n";

            return ret;
        }

        private string generateFactoryCode(Type packetType)
        {
            string ret = "";

            if (packetType.IsNestedPublic)
            {
                ret += "using " + packetType.Name + " = " + packetType.ReflectedType.FullName + "." + packetType.Name + ";";
            }
            else
            {
                ret += "using " + packetType.Namespace + ";";
            }
            ret += "namespace MHGameWork.TheWizards.Networking.Generated\n";
            ret += "{\n";
            ret += "public class " + packetType.Name + "Factory : INetworkPacketFactory<" + packetType.Name + ">\n";
            ret += "{\n";
            ret += "private CodeGeneraterWriter genWriter;\n";
            ret += "private CodeGeneraterReader genReader;\n";
            ret += "public " + packetType.Name + "Factory()";
            ret += "{\n";
            ret += "genWriter = new CodeGeneraterWriter();\n";
            ret += "genReader = new CodeGeneraterReader();\n";
            ret += "}\n";
            ret += "public " + packetType.Name + " FromStream(System.IO.BinaryReader reader)\n";
            ret += "{\n";
            ret += "genReader.Reader = reader;\n";
            ret += "" + packetType.Name + " p = new " + packetType.Name + "();\n";


            foreach (FieldInfo info in packetType.GetFields())
            {
                if (info.GetCustomAttributes(typeof(NetworkPacketIgnoreAttribute), false).Length != 0) continue;
                ret += generateLoadField("p", info) + "\n";

            }
            foreach (PropertyInfo info in packetType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!info.CanRead) continue;
                if (!info.CanWrite) continue;
                if (info.GetCustomAttributes(typeof(NetworkPacketIgnoreAttribute), false).Length != 0) continue;
                ret += generateLoadProperty("p", info) + "\n";

            }


            ret += "return p;\n";
            ret += "}\n";

            ret += "public void ToStream(System.IO.BinaryWriter writer, " + packetType.Name + " packet)\n";
            ret += "{\n";
            ret += "genWriter.Writer = writer;\n";

            foreach (FieldInfo info in packetType.GetFields())
            {
                if (!info.IsPublic) continue;
                if (info.GetCustomAttributes(typeof(NetworkPacketIgnoreAttribute), false).Length != 0) continue;
                ret += generateSaveField("packet", info) + "\n";

            }
            foreach (PropertyInfo info in packetType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!info.CanRead) continue;
                if (!info.CanWrite) continue;
                if (info.GetCustomAttributes(typeof(NetworkPacketIgnoreAttribute), false).Length != 0) continue;
                ret += generateSaveProperty("packet", info) + "\n";

            }


            ret += "}\n";
            ret += "}\n";
            ret += "}\n";

            return ret;


        }

        private string generateLoadVariable(string variableName, Type variableType)
        {
            BinaryReader br;

            if (variableType == typeof(int))
            {
                return variableName + " = reader.ReadInt32();";
            }
            if (variableType == typeof(bool))
            {
                return variableName + " = reader.ReadBoolean();";
            }
            if (variableType == typeof(string))
            {
                return variableName + " = reader.ReadString();";
            }
            if (variableType == typeof(float))
            {
                return variableName + " = reader.ReadSingle();";
            }
            if (variableType == typeof(byte[]))
            {
                var ret = "{";
                ret += "var count = reader.ReadInt32();\n";
                ret += "if (count != -1 ) {\n";
                ret += variableName + " = reader.ReadBytes(count);";
                ret += "}}\n";
                return ret;
            }
            if (variableType == typeof(Guid))
            {
                return variableName + " = genReader.ReadGuid();";
            }
            if (variableType.IsEnum)
            {
                var castType = variableType.FullName;
                castType = castType.Replace("+", ".");
                if (System.Enum.GetUnderlyingType(variableType) == typeof(byte))
                {
                    return variableName + " = (" + castType + ")reader.ReadByte();";
                }
                if (System.Enum.GetUnderlyingType(variableType) == typeof(int))
                {
                    return variableName + " = (" + castType + ")reader.ReadInt32();";
                }
            }
            if (variableType.IsArray)
            {
                var ret = "{";
                ret += "var count = reader.ReadInt32();\n";
                ret += "if (count != -1 ) {\n";
                ret += variableName + " = new " + variableType.GetElementType().FullName + "[count];\n";
                ret += "for (int iArray = 0; iArray < count; iArray++) \n";
                ret += "{\n";
                ret += generateLoadVariable(variableName + "[iArray]", variableType.GetElementType());
                ret += "}}}\n";
                return ret;
            }


            throw new InvalidOperationException("This type is currently not supported for auto-generated factories! (" + variableType.FullName + ")");
        }

        private string generateSaveVariable(string variableName, Type variableType)
        {

            if (variableType == typeof(int)
                || variableType == typeof(string)
                || variableType == typeof(float)
                || variableType == typeof(bool)
                || variableType == typeof(string))
            {
                return "writer.Write(" + variableName + ");";
            }
            if (variableType == typeof(byte[]))
            {
                var ret = "";
                ret += "if (" + variableName + " == null) {\n";
                ret += "writer.Write((int)-1);\n";

                ret += "} else {\n";
                ret += "writer.Write(" + variableName + ".Length);";
                ret += "writer.Write(" + variableName + ");";
                ret += "}\n";
                return ret;

            }
            if (variableType.IsEnum)
            {
                var baseType = System.Enum.GetUnderlyingType(variableType);

                if (baseType == typeof(byte))
                {
                    return "writer.Write((byte)" + variableName + ");";
                }
                if (baseType == typeof(int))
                {
                    return "writer.Write((int)" + variableName + ");";
                }
            }
            if (variableType == typeof(Guid))
            {
                return "genWriter.Write(" + variableName + ");";
            }
            if (variableType.IsArray)
            {
                var ret = "";
                ret += "if (" + variableName + " == null) {\n";
                ret += "writer.Write((int)-1);\n";

                ret += "} else {\n";

                ret += "writer.Write(" + variableName + ".Length);\n";
                ret += "for (int iArray = 0; iArray < " + variableName + ".Length; iArray++) \n";
                ret += "{\n";
                ret += generateSaveVariable(variableName + "[iArray]", variableType.GetElementType());
                ret += "}\n";
                ret += "}\n";
                return ret;
            }


            throw new InvalidOperationException("This type is currently not supported for auto-generated factories! (" + variableType + ")");
        }

        private string generateLoadField(string instanceName, FieldInfo field)
        {
            return generateLoadVariable(instanceName + "." + field.Name, field.FieldType);
            string pre = instanceName + "." + field.Name + " = ";
            if (field.FieldType == typeof(int))
            {
                return pre + "reader.ReadInt32();";
            }
            if (field.FieldType == typeof(string))
            {
                return pre + "reader.ReadString();";
            }

            throw new InvalidOperationException("This type is currently not supported for auto-generated factories! (" + field.FieldType.FullName + ")");
        }

        private string generateSaveField(string instanceName, FieldInfo field)
        {
            return generateSaveVariable(instanceName + "." + field.Name, field.FieldType);

            string pre = instanceName + "." + field.Name + " = ";
            if (field.FieldType == typeof(int))
            {
                return "writer.Write(" + instanceName + "." + field.Name + ");";
            }
            if (field.FieldType == typeof(string))
            {
                return "writer.Write(" + instanceName + "." + field.Name + ");";
            }

            throw new InvalidOperationException("This type is currently not supported for auto-generated factories! (" + field.FieldType.FullName + ")");
        }

        private string generateLoadProperty(string instanceName, PropertyInfo info)
        {
            return generateLoadVariable(instanceName + "." + info.Name, info.PropertyType);
            string pre = instanceName + "." + info.Name + " = ";
            if (info.PropertyType == typeof(int))
            {
                return pre + "reader.ReadInt32();";
            }
            if (info.PropertyType == typeof(string))
            {
                return pre + "reader.ReadString();";
            }

            throw new InvalidOperationException("This type is currently not supported for auto-generated factories! (" + info.PropertyType.FullName + ")");
        }

        private string generateSaveProperty(string instanceName, PropertyInfo info)
        {
            return generateSaveVariable(instanceName + "." + info.Name, info.PropertyType);
            string pre = instanceName + "." + info.Name + " = ";
            if (info.PropertyType == typeof(int))
            {
                return "writer.Write(" + instanceName + "." + info.Name + ");";
            }
            if (info.PropertyType == typeof(string))
            {
                return "writer.Write(" + instanceName + "." + info.Name + ");";
            }

            throw new InvalidOperationException("This type is currently not supported for auto-generated factories! (" + info.PropertyType.FullName + ")");
        }

        private class HolderFactory<T> : INetworkPacketFactory<T> where T : INetworkPacket
        {
            private NetworkPacketFactoryCodeGenerater generater;

            private INetworkPacketFactory<T> internalFactory = null;

            public HolderFactory(NetworkPacketFactoryCodeGenerater generater)
            {
                this.generater = generater;
            }

            #region INetworkPacketFactory<T> Members

            public T FromStream(BinaryReader reader)
            {
                if (internalFactory == null) assignInternalFactory();
                return internalFactory.FromStream(reader);
            }

            private void assignInternalFactory()
            {
                internalFactory = generater.getGeneratedFactory<T>();
                if (internalFactory != null) return;
                generater.BuildFactoriesAssembly();

                internalFactory = generater.getGeneratedFactory<T>();
                if (internalFactory == null) throw new Exception("Error in generater!");

            }

            public void ToStream(BinaryWriter writer, T packet)
            {
                if (internalFactory == null) assignInternalFactory();
                internalFactory.ToStream(writer, packet);
            }

            #endregion
        }
    }

    public interface INetworkPacketFactoryFactory
    {
        INetworkPacketFactory<T> GetFactory<T>() where T : INetworkPacket;
    }


}
