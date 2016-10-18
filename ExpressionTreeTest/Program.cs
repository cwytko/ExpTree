using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionTreeTest
{
    class Node
    {
        double Eval() { return 0; }
    }
    class constNode : Node
    {
        public double value;
        public constNode(double assign)
        {
            value = assign;
        }
        double Eval() { return value; }
    }

    // Will this rely on the existence on an ExpTree object in order to see
    // variables and their associated value?
    class VarNode : Node
    {
        public string Name;
        public VarNode(string expression)
        {
            Name = expression;
        }
        // reference the variable from the ExpTree class
        //private Dictionary<string, double> m_vars;
        //public double Eval()
        //{
        //    return ExpTree.GetVarValue(Name);
        //}
    }
    class OpNode : Node
    {
        public OpNode(char NewOp)
        {
            Op = NewOp;
        }

        public char Op;
        public Node Left, Right;
    }

    public class ExpTree
    {
        // The tuple contains the op, the ops precedence, its index location,
        // and the amount of parentheses the operator is contained within
        List<Tuple<char, OP, int, int>> Ops = new 
            List<Tuple<char, OP, int, int>>();
        // this will hold either the 'last to be evaluated' operator of the 
        // expression or a single variable OR single value
        private Node m_root;
        // A helper property for the Ops List Tuple 
        enum OP
        {
            minus,  // lowest precedence
            plus,
            mult,
            div,    // highest precedence
        };

        // When this function is called each time, the list must remove the
        // entry that has be turned into a node
        Tuple<char, OP, int, int> OpPrecedence(List<Tuple<char, OP, int, int>> 
            Candidates)
        {
            // This would mean that there is only a var or const in the exp
            if(Candidates.Count == 0)
            {
                return null;
            }

            Tuple<char, OP, int, int> leastPrecedent = new 
                Tuple<char, OP, int, int>(Candidates[0].Item1, 
                Candidates[0].Item2, Candidates[0].Item3, Candidates[0].Item4);
            // Here we build mini trees and link them based off of their order 
            // and their precedence
            foreach (Tuple<char, OP, int, int> candidate in Candidates)
            {
                // In here I need to determine the least 'influential' operator
                // it will be the last to evaluate the expression tree
                // therefore this will be the root
                if (candidate.Item2 < leastPrecedent.Item2 && 
                    candidate.Item4 < leastPrecedent.Item4)
                    leastPrecedent = candidate;
            }
            return leastPrecedent;
        }

        // force VarNodes to grab the double value from this dictionary
        private Dictionary<string, double> m_vars = new Dictionary<string, 
            double>();

        public double GetVarValue(string name)
        {
            return m_vars[name];
        }

        public void SetVar(string varName, double varValue)
        {
            m_vars[varName] = varValue;
        }

        

        public ExpTree(string expression_yo)
        {
            //m_root = Compile(expression_yo);
        }

        // This function builds the node tree
        // I grab the least precedent tuple that holds the operator and its
        // location and place it on the tree, I go through the whole list
        // until it's exhausted, I will then add the variables/constants 
        Node Compile(string exp)
        {
            OpNode traverse;
            // If this is null that means we have no operators thus we only
            // have a constant or a variable
            if (OpPrecedence(Ops) == null)
                return BuildSimple(exp);
            Tuple<char, OP, int, int> ToBeNode = OpPrecedence(Ops);
            // This will grab the least precedent Op
            // 
            OpNode root = new OpNode(ToBeNode.Item1);
            // Now that the 'root' node has been established I can traverse 
            // and determine how 

            //int index = 0;// GetOpIndex(exp);
            //if (-1 == index)
            //    return BuildSimple(exp);
            //OpNode root = new OpNode(exp[index]);
            //root.Left = Compile(exp.Substring(0, index));
            //root.Right = Compile(exp.Substring(index + 1));
            //return root;
        }

       
        // We reach this function when we have encountered no more operators
        // There is only a variable or constant left
        // This will determine if the leaf is a constant or a var
        private Node BuildSimple(string exp)
        {
            double num = 0;
            if (double.TryParse(exp, out num))
                return new constNode(num);
            // During the construction of this VarNode I need to populate the 
            // dictionary with the exp as the Name, how do I know if the Name
            // has a value association?
            else
            {
                SetVar(exp, 0);             
                return new VarNode(exp);
            }
        }

        // This generate the list of Operators found within the string
        // This function will be called once to generate the list, the funciton
        // OpPrecedence will then extract the least precedent operator each
        // time to build the tree from the root down (as in the last operation
        // to be done in the expression will be the root node)
        private void GetOpIndices(string exp)
        {
            
            int parenCount = 0;
            for (int i = exp.Length - 1; i > 0; i--)
            {
                switch(exp[i])
                {
                    case '+':
                        Tuple<char, OP, int, int> plus = new Tuple<char, OP, 
                            int, int>(exp[i], OP.plus, i, parenCount);
                        Ops.Add(plus);
                        break;
                    case '-':
                        Tuple<char, OP, int, int> minus = new Tuple<char, OP, 
                            int, int>(exp[i], OP.minus, i, parenCount);
                        Ops.Add(minus);
                        break;
                    case ')':
                        parenCount++;
                        break;
                    case '(':
                        parenCount--;
                        break;
                    case '*':
                        Tuple<char, OP, int, int> mult = new Tuple<char, OP, 
                            int, int>(exp[i], OP.mult, i, parenCount);
                        Ops.Add(mult);
                        break;
                    case '/':
                        Tuple<char, OP, int, int> div = new Tuple<char, OP, 
                            int, int>(exp[i], OP.div, i, parenCount);
                        Ops.Add(div);
                        break;
                    default: // if we're here this means we either found a var 
                             // or a const so we don't do anything
                        break;
                }
            }
        }

        // This will render the tree
        // Here's an example on how the tree can be read 
        // so this function ONLY reads the end result after all is said and 
        // done
        double resolve(Node traverse)
        {
            double evaluation = 0;
            if (traverse is constNode)
                return (traverse as constNode).value;
            else if (traverse is VarNode)
                return m_vars[(traverse as VarNode).Name];
            // The only other thing traverse could be would be an OpNode
            else
            {
                OpNode on = (traverse as OpNode);
                if (on.Op == '+')
                    evaluation += resolve(on.Left) + resolve(on.Right);
                else if (on.Op == '-')
                    evaluation += resolve(on.Left) - resolve(on.Right);
                else if (on.Op == '*')
                    evaluation += resolve(on.Left) * resolve(on.Right);
                else
                    evaluation += resolve(on.Left) / resolve(on.Right);
            }
            return evaluation;
        }
        
        public double Eval()
        {
            double evaluation = 0;

            evaluation = resolve(m_root);

            return evaluation;
        }
    }

class Program
    {
        static void Main(string[] args)
        {

            ExpTree test = new ExpTree("2*(2-9)");
            double num = test.Eval();
            Console.WriteLine(num);
            Console.ReadLine();

        }
    }
}
