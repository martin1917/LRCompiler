using LRv2.Parser;

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
            var stateOnTopStack = stack.Peek().StateNumber;
            var parserOperation = fsm.Get(stateOnTopStack, lexems[i].TypeTerminal.Name);

            if (parserOperation.KindOperation is KindOperation.ERROR)
            {
                throw new Exception("Ошибка парсинга");
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

                            TreeNode child = item.TreeNode
                                ?? new TreeNode(
                                    value: item.Symbol,
                                    childs: item.Value != null
                                        ? new() { new TreeNode(item.Value) }
                                        : new());

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
