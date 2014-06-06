﻿using NiL.JS.Core;
using NiL.JS.Statements;
using NiL.JS.Core.BaseTypes;
using System;
using System.Threading;

namespace NiL.JS
{
    /// <summary>
    /// Управляет выполнением скрипта на языке JavaScript.
    /// </summary>
    [Serializable]
    public sealed class Script
    {
        private Statement root;
        public CodeBlock Root { get { return root as CodeBlock; } }
        /// <summary>
        /// Исходный код скрипта, переданный при создании объекта.
        /// </summary>
        public string Code { get; private set; }
        /// <summary>
        /// Корневой контекст выполнения скрипта.
        /// </summary>
        public Context Context { get; private set; }

        /// <summary>
        /// Инициализирует объект типа Script и преобрзует код скрипта во внутреннее представление.
        /// </summary>
        /// <param name="code">Код скрипта на языке JavaScript</param>
        public Script(string code)
        {
            Code = code;
            int i = 0;
            root = CodeBlock.Parse(new ParsingState(Tools.RemoveComments(code), Code), ref i).Statement;
            if (i < code.Length)
                throw new System.ArgumentException("Invalid char");
            Parser.Optimize(ref root, new System.Collections.Generic.Dictionary<string, VaribleDescriptor>());
            Context = new Context(NiL.JS.Core.Context.globalContext, root);
        }

        /// <summary>
        /// Запускает выполнение скрипта.
        /// </summary>
        public void Invoke()
        {
            var lm = System.Runtime.GCSettings.LatencyMode;
            System.Runtime.GCSettings.LatencyMode = System.Runtime.GCLatencyMode.Interactive;
            try
            {
                Context.ValidateThreadID();
                root.Invoke(Context);
            }
            finally
            {
                System.Runtime.GCSettings.LatencyMode = lm;
                Context.currentRootContext = null;
            }
        }
    }
}