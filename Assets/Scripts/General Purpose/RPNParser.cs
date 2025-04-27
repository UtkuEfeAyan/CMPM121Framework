
using UnityEngine;
using System;
using System.Collections.Generic;

public class RPNParser : MonoBehaviour
{
    public static RPNParser Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public float DoParse(string str, Dictionary<string, float> variables = null)
    {
        Stack<float> stack = new Stack<float>();
        int length = str.Length;

        for (int i = 0; i < length; i++)
        {
            if (str[i] == ' ')
                continue;

            string token = "";
            // Read a token (until next space)
            while (i < length && str[i] != ' ')
            {
                token += str[i];
                i++;
            }
            i--; // Because loop will increment again

            if (IsOperator(token))
            {
                if (stack.Count < 2)
                    throw new ArgumentException($"Too many operators or not enough operands in expression: \"{str}\"");

                float b = stack.Pop();
                float a = stack.Pop();

                switch (token)
                {
                    case "+": stack.Push(a + b); break;
                    case "-": stack.Push(a - b); break;
                    case "*": stack.Push(a * b); break;
                    case "/": stack.Push(a / b); break;
                    case "%": stack.Push(a % b); break;
                    default:
                        throw new ArgumentException($"Unknown operator '{token}'.");
                }
            }
            else if (variables != null && variables.ContainsKey(token))
            {
                stack.Push(variables[token]);
            }
            else if (float.TryParse(token, out float number))
            {
                stack.Push(number);
            }
            else
            {
                throw new ArgumentException($"Unknown token '{token}' in expression \"{str}\"");
            }
        }

        if (stack.Count != 1)
            throw new ArgumentException($"Malformed RPN expression: \"{str}\"");

        return stack.Pop();
    }

    private bool IsOperator(string token)
    {
        return token == "+" || token == "-" || token == "*" || token == "/" || token == "%";
    }
}
