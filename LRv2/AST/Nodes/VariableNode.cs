﻿namespace LRv2.SyntaxAnalyzer.Nodes;

public class VariableNode : BaseNode
{
    public string Ident { get; }

	public VariableNode(string ident)
	{
		Ident = ident;
	}
}
