﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITI.Parser
{
    public class ErrorNode : Node
    {
        public ErrorNode(string message)
        {
            Message = message;
        }

        public string Message { get; }

        [DebuggerStepThrough]
        internal override void Accept(NodeVisitor visitor) => visitor.Visit(this);

        public override int Count => 1;
        public override int Depth => 1;

        public override string ToString()
        {
            return $"Error: {Message}";
        }
    }
}