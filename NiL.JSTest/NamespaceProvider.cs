﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NiL.JSTest
{
    class NamespaceProvider : NiL.JS.Core.EmbeddedType
    {
        private static List<Type> types = new List<Type>();

        private static void addTypes(System.Reflection.Assembly assembly)
        {
            var types = assembly.GetTypes();
            for (var i = 0; i < types.Length; i++)
                NamespaceProvider.types.Add(types[i]);
        }

        static NamespaceProvider()
        {
            AppDomain.CurrentDomain.AssemblyLoad += new AssemblyLoadEventHandler(CurrentDomain_AssemblyLoad);
            var assms = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < assms.Length; i++)
                addTypes(assms[i]);
        }

        static void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            addTypes(args.LoadedAssembly);
        }

        public string Namespace { get; private set; }

        public NamespaceProvider(string @namespace)
        {
            Namespace = @namespace;
        }

        public override JS.Core.JSObject GetField(string name, bool fast, bool own)
        {
            string reqname = Namespace + "." + name;
            bool createSubNode = false;
            for (var j = 0; j < types.Count; j++)
            {
                if (types[j].FullName == reqname)
                    return NiL.JS.Core.TypeProxy.GetConstructor(types[j]);
                if (!createSubNode && types[j].Namespace != null && types[j].Namespace.Length >= reqname.Length && types[j].Namespace.IndexOf(reqname) == 0)
                    createSubNode = true;
            }
            if (createSubNode)
                return NiL.JS.Core.TypeProxy.Proxy(new NamespaceProvider(reqname));
            return new JS.Core.JSObject();
        }
    }
}
