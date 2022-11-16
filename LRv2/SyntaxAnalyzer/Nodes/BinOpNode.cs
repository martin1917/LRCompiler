﻿namespace LRv2.SyntaxAnalyzer.Nodes;

public class BinOpNode : BaseNode
{
    public string Operation { get; set; }

    public BaseNode Left { get; set; }

    public BaseNode Right { get; set; }
}