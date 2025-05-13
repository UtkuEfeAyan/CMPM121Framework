
//Author: Xavier Austin
using UnityEngine;
using System;
using System.Collections.Generic;

public class RPNParser 
{
    public static float DoParse(string str, Dictionary<string, float> variables = null)
    {
        Stack<float> stack = new Stack<float>();
        //super wasteful memory-wise but uh :)
        Stack<bool> isIntStack = new Stack<bool>();
        if (str == "")
            throw new ArgumentException("Empty string");
  
        //warning: extremely unoptimized (but super safe)
        for (int i = 0; i < str.Length; i ++){
            string   substr = "";
            bool     isInt = true;
            bool     isFlt = false;
            bool     isVar = true;
            bool     isOpp = true;
            float    intFromStr = 0;
            float    fltFromStr = 0;
            float    fltDivisor = 1;
            //tokenize
            for (; (isInt || isFlt || isVar || isOpp) && (i < str.Length) && (str[i] != ' '); i ++){
                char temp = str[i];
                substr += temp;
                //calculate the divisor for floating point number
                if (isFlt)
                    fltDivisor *= 10;
                isFlt &= (temp >= '0' && temp <= '9'); //looks wrong but effectively checks number of .s
                //checks if the number is now a floating point number (also why above works properly)
                if (temp == '.')
                    isFlt = isInt;
                //check for validity of expression
                isInt &= (temp >= '0' && temp <= '9');
                isVar &= ((temp >= 'A' && temp <= 'Z') || (temp >= 'a' && temp <= 'z') || temp == '_');
                isOpp &= (temp == '%' || temp == '+' || temp == '-' || temp == '*' || temp == '/');
                //calculate the numerator/integer from string
                if (temp != '.')
                    intFromStr = intFromStr * 10 + (temp - '0');
            }
            if (!(isInt || isFlt || isVar || isOpp))
                throw new ArgumentException("Invalid character at index ["+i+"] in expression \""+str+"\"");
            fltFromStr = intFromStr / fltDivisor;
            if (substr == "")
                throw new ArgumentException("Too many spaces");
            //push to stack
            if (isVar){
                if (variables == null)
                    throw new ArgumentException("Variable found but no dictionary passed");
                if (!variables.ContainsKey(substr))
                    throw new ArgumentException("Variable name \""+substr+"\" not in passed dictionary");
                fltFromStr = variables[substr];
                //if (var is int) isInt = true; unfortunate sacrifice of nuance made to the c# gods
                //probably wont be important though
            }
            if (!isOpp){
                isIntStack.Push(isInt);
                stack.Push(fltFromStr);
                continue;
            }
            //evaluate
            if (stack.Count < 2)
                throw new ArgumentException("Too many opperators in expression \""+str+"\"");
            float opperand1 = stack.Pop();
            bool opp1Int = isIntStack.Pop();
            float opperand0 = stack.Pop();
            bool opp0Int = isIntStack.Pop();
            switch (substr){
                case ("+"):
                    stack.Push(opperand0+opperand1);
                    isIntStack.Push(opp1Int && opp0Int);
                break;
                case ("-"):
                    stack.Push(opperand0-opperand1);
                    isIntStack.Push(opp1Int && opp0Int);
                break;
                case ("/"):
                    //implicit type casts from floats to ints when dividing by ints are actually absent from c#
                    if (opp1Int)
                        stack.Push(Convert.ToInt32(opperand0/opperand1));
                    else
                        stack.Push(opperand0/opperand1);
                    isIntStack.Push(opp1Int);
                break;
                case ("*"):
                    stack.Push(opperand0*opperand1);
                    isIntStack.Push(opp1Int && opp0Int);
                break;
                case ("%"):
                    stack.Push(opperand0%opperand1);
                    isIntStack.Push(opp1Int && opp0Int);
                break;
                default:
                    throw new ArgumentException("Undefined opperator \""+substr+"\". Maybe expression is missing a space?");
            }
        }
        return stack.Pop();
    }
}