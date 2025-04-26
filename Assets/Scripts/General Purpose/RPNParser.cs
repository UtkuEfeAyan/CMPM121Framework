using UnityEngine;
using System;
using System.Collections.Generic;

public class RPNParser : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DoTestParse();
    }

    // Update is called once per frame
    void Update()
    {
        DoTestParse();
    }
	
	//not great practice imo but it allows for int and float returns which I like
	float DoParse(string str)
	{
		Stack<float> stack = new Stack<float>();
		//super wasteful memory-wise but uh :)
		Stack<bool> isIntStack = new Stack<bool>();
  
		//warning: extremely unoptimized (but super safe)
		for (int i = 0; i < str.Length; i ++){
			string 	substr = "";
			bool 	isInt = true;
			bool 	isFlt = false;
			bool 	isVar = true;
			bool 	isOpp = false;
			float 	intFromStr = 0;
			float	fltFromStr = 0;
			float 	fltDivisor = 1;
			//tokenize
			for (; (isInt || isFlt || isVar || isOpp) && (i < str.Length) && (str[i] != ' '); i ++){
				char temp = str[i];
				substr += temp;
				//calculate the divisor for floating point number
				if (isFlt)
					fltDivisor *= 10;
				//check for validity of expression
				isInt &= (temp >= '0' && temp <= '9');
				isFlt &= (temp >= '0' && temp <= '9'); //looks wrong but effectively checks number of .s
				isVar &= ((temp >= 'A' && temp <= 'Z') || (temp >= 'a' && temp <= 'z') || temp == '_');
				isOpp &= (temp == '%' || temp == '+' || temp == '-' || temp == '*' || temp == '/');
				//checks if the number is now a floating point number (also why line 45 works properly)
				if (temp == '.')
					isFlt = isInt;
				//calculate the numerator/integer from string
				else
					intFromStr = intFromStr * 10 + (temp - '0');
			}
			if (!(isInt || isFlt || isVar || isOpp))
				throw new ArgumentException("Invalid character at index ["+i+"] in expression \""+str+"\"");
			fltFromStr = intFromStr / fltDivisor;
			//push to stack
			if (isVar){
				/*				
				fltFromStr = varname;
				if (var is int) isInt = true;
				*/
				throw new ArgumentException("Variable name \""+substr+"\" not yet implemented");
			}
			isIntStack.Push(isInt);
			stack.Push(fltFromStr);
			if (!isOpp)
				continue;
			//evaluate
			if (stack.Count < 2)
				throw new ArgumentException("Too many opperators in expression \""+str+"\"");
			float opperand1 = stack.Pop();
			bool opp1Int = isIntStack.Pop();
			float opperand0 = stack.Pop();
			/*bool opp0Int = */isIntStack.Pop();
			switch (substr){
				case ("+"):
					stack.Push(opperand0+opperand1);
				break;
				case ("-"):
					stack.Push(opperand0-opperand1);
				break;
				case ("/"):
					if (opp1Int)
						stack.Push(opperand0/((int)opperand1));
					else
						stack.Push(opperand0/opperand1);
				break;
				case ("*"):
					stack.Push(opperand0*opperand1);
				break;
				case ("%"):
					stack.Push(opperand0%opperand1);
				break;
				default:
					throw new ArgumentException("Undefined opperator \""+substr+"\". Maybe missing a space?");
				break;
			}
		}
		return stack.Pop();
	}
    
	void DoTestParse(){
		Debug.Log("what");
	}
}
