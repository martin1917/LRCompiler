﻿using LRv2.Parser;

namespace LRv2.AST;

public class ASTGenerator
{
    public static TreeNode Generate(LRTable fsm, List<Lexem> lexems)
    {
        bool accept = false;
        int i = 0;

        var stack = new Stack<StackItem>();
        stack.Push(new StackItem(0, ""));

        while (!accept)
        {
            var stateOnTopStack = stack.Peek();
            var parserOperation = fsm.Get(stateOnTopStack.StateNumber, lexems[i].TypeTerminal.Name);

            if (parserOperation.KindOperation is KindOperation.ERROR)
            {
                var follow = ParserHelpers.FollowLexemsFor(stateOnTopStack.Symbol);

                var message = 
                    $"После '{stateOnTopStack.Symbol}' должны быть следующие символы [{string.Join(", ", follow)}]\n" +
                    $"А никак НЕ '{lexems[i].TypeTerminal.Name}'";

                throw new Exception(message);
            }

            switch (parserOperation.KindOperation)
            {
                case KindOperation.ACCEPT:
                    {
                        accept = true;
                    }
                    break;

                case KindOperation.SHIFT:
                    {
                        var nextStateNumber = parserOperation.Number;

                        StackItem stackItem = lexems[i].IsVariableOrConst()
                            ? new StackItem(nextStateNumber, lexems[i].TypeTerminal.Name, lexems[i].Value)
                            : new StackItem(nextStateNumber, lexems[i].TypeTerminal.Name);

                        stack.Push(stackItem);
                        i++;
                    }
                    break;

                case KindOperation.REDUCE:
                    {
                        var rule = Rule.AllRules.First(r => r.NumberRule == parserOperation.Number);

                        var childs = new List<TreeNode?>();
                        for (int k = 0; k < rule.Right.Length; k++)
                        {
                            StackItem item = stack.Pop();

                            TreeNode? child = item.TreeNode;
                            child ??= item.Value == null
                                ? new TreeNode(item.Symbol)
                                : new TreeNode(item.Symbol, new() { new TreeNode(item.Value) });

                            childs.Insert(0, child);
                        }

                        var stateAferReducing = stack.Peek().StateNumber;
                        var operation = fsm.Get(stateAferReducing, rule.Left);
                        var nextStateNumber = operation.Number;
                        stack.Push(new StackItem(nextStateNumber, rule.Left, new TreeNode(rule.Left, childs)));
                    }
                    break;
            }
        }

        return stack.Pop().TreeNode!;
    }
}
