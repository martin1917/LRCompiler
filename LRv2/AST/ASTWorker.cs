using LRv2.SyntaxAnalyzer.Nodes;
using System.Collections.Generic;

namespace LRv2.AST;

public class ASTWorker
{
	private readonly ProgramNode root;

	private Dictionary<string, bool> tableIdents = new();

	public ASTWorker(ProgramNode programNode)
	{
		root = programNode;
	}

    private List<string> GetErrors(BaseNode node)
    {
        List<string> errors = new();

        void GetErrorsInternal(BaseNode node, List<string> errors)
        {
            if (node is ScopeNode scopeNode)
            {
                scopeNode.Statements.ForEach(st => GetErrorsInternal(st, errors));
            }

            if (node is VariableNode variableNode)
            {
                if (!tableIdents.ContainsKey(variableNode.Ident))
                {
                    errors.Add($"Переменная {variableNode.Ident} не объявлена");
                }
            }

            if (node is AssignmentNode assignmentNode)
            {
                string ident = assignmentNode.Variable.Ident;
                if (!tableIdents.ContainsKey(ident))
                {
                    errors.Add($"Переменная {ident} не объявлена");
                }

                GetErrorsInternal(assignmentNode.Expression, errors);
            }

            if (node is BinOpNode binOpNode)
            {
                GetErrorsInternal(binOpNode.Left, errors);
                GetErrorsInternal(binOpNode.Right, errors);
            }

            if (node is UnaryOpNode unaryOpNode)
            {
                GetErrorsInternal(unaryOpNode.Node, errors);
            }

            if (node is ReadNode readNode)
            {
                var idents = readNode.Vars.Variables
                    .Select(v => v.Ident)
                    .ToList();

                var notExistingIdents = idents
                    .Where(id => !tableIdents.ContainsKey(id));

                errors.AddRange(notExistingIdents.Select(ident => $"Переменная {ident} не объявлена"));
            }

            if (node is WriteNode writeNode)
            {
                var idents = writeNode.Vars.Variables
                    .Select(v => v.Ident)
                    .ToList();

                var notExistingIdents = idents
                    .Where(id => !tableIdents.ContainsKey(id));

                errors.AddRange(notExistingIdents.Select(ident => $"Переменная {ident} не объявлена"));
            }

            if (node is IfElseNode ifElseNode)
            {
                GetErrorsInternal(ifElseNode.Predicate, errors);
                GetErrorsInternal(ifElseNode.TrueBranch, errors);
                GetErrorsInternal(ifElseNode.FalseBranch, errors);
            }
        }

        GetErrorsInternal(node, errors);
        return errors;
    }

	public void Proccess()
	{
        // объявление переменных
        ProccessVarNode(root.Vars);

        // поиск ошибок (использование не существующих переменных)
        var errors = GetErrors(root.Body);
        if (errors.Any())
        {
            throw new Exception(string.Join('\n', errors));
        }

        // основная работа
        ProccessScopeNode(root.Body);
    }

    private void ProccessVarNode(VarNode varNode)
    {
        varNode.Variables
            .ForEach(v => tableIdents.Add(v.Ident, false));
    }

    private void ProccessScopeNode(ScopeNode scopeNode)
    {
        scopeNode.Statements
            .ForEach(st => ProccessStatement(st));
    }

    private void ProccessStatement(BaseNode node)
    {
        if (node is AssignmentNode assignmentNode)
        {
            ProccessAssignmentNode(assignmentNode);
        }

        else if (node is ReadNode readNode)
        {
            ProccessReadNode(readNode);
        }

        else if (node is WriteNode writeNode)
        {
            ProccessWriteNode(writeNode);
        }

        else if (node is IfElseNode ifElseNode)
        {
            ProccessIfElseNode(ifElseNode);
        }
    }

    private void ProccessAssignmentNode(AssignmentNode assignmentNode)
	{
        string ident = assignmentNode.Variable.Ident;
        bool value = ProccessExpression(assignmentNode.Expression);
        tableIdents[ident] = value;
    }

    private void ProccessReadNode(ReadNode readNode)
	{
        var idents = readNode.Vars.Variables
                .Select(v => v.Ident)
                .ToList();

        var readingValues = Console.ReadLine()?.Trim()
            .Split(" ")
            .Select(v => v == "1")
            .ToList();

        if (readingValues == null || idents.Count != readingValues.Count)
        {
            throw new Exception($"Функции READ нужно передать значения для ВСЕХ переменных {string.Join(", ", idents)}");
        }

        idents.Zip(readingValues).ToList()
            .ForEach(pair => tableIdents[pair.First] = pair.Second);
    }
    
    private void ProccessWriteNode(WriteNode writeNode)
	{
        var idents = writeNode.Vars.Variables
                .Select(v => v.Ident)
                .ToList();

        Console.WriteLine(string.Join(" ", idents.Select(id => tableIdents[id])));
    }

    private void ProccessIfElseNode(IfElseNode ifElseNode)
    {
        var pred = ProccessExpression(ifElseNode.Predicate);
        if (pred)
        {
            ProccessScopeNode(ifElseNode.TrueBranch);
        }
        else
        {
            ProccessScopeNode(ifElseNode.FalseBranch);
        }
    }
    
    private bool ProccessExpression(BaseNode statemantNode)
	{
		if (statemantNode is VariableNode variableNode)
		{
            return tableIdents[variableNode.Ident];
        }

        if (statemantNode is ConstNode constNode)
		{
			return constNode.Value;
		}

        if (statemantNode is BinOpNode binOpNode)
		{
			var left = ProccessExpression(binOpNode.Left);
			var right = ProccessExpression(binOpNode.Right);

			return binOpNode.Operation switch
			{
				"or" => left || right,
				"and" => left && right,
				"equ" => left == right,
			};
        }

        if (statemantNode is UnaryOpNode unaryOpNode)
		{
			return !ProccessExpression(unaryOpNode.Node);
		}

		return false;
    }
}
