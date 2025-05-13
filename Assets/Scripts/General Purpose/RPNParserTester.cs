//Author: Efe Ayan
//Editor: Xavier Austin (added a few more test cases)
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Simple tester for RPNParser. Attach this to a GameObject and test parsing expressions.
/// </summary>
public class RPNParserTester : MonoBehaviour
{
    void Start()
    {
        TestSimpleExpressions();
        TestExpressionsWithVariables();
    }

    private void TestSimpleExpressions()
    {
        Debug.Log("==== Simple Tests ====");

        //basic character conversion test
        Debug.Log("Test 0 (5): " + RPNParser.DoParse("5")); // Expected 5
        //test for addition operator
        Debug.Log("Test 1 (5 3 +): " + RPNParser.DoParse("5 3 +")); // Expected: 8
        //test for subtraction operator
        Debug.Log("Test 2 (6 4 -): " + RPNParser.DoParse("6 4 -")); // Expected: 2
        //test for multiplication operator
        Debug.Log("Test 3 (3 5 *): " + RPNParser.DoParse("3 5 *")); // Expected: 15
        //test for modulo operator
        Debug.Log("Test 4 (7 3 %): " + RPNParser.DoParse("7 3 %")); // Expected: 1
        //test for division operator
        Debug.Log("Test 5 (10 2 /): " + RPNParser.DoParse("10 2 /")); // Expected: 5
        //test for integer arithmatic
        Debug.Log("Test 6 (7 3 /): " + RPNParser.DoParse("7 3 /")); // Expected: 2
        Debug.Log("Test 7 (7 3.0 /): " + RPNParser.DoParse("7 3.0 /")); // Expected: 2.33 (repeating)
        //test for pushing and poping a larger number of stack items
        Debug.Log("Test 8 (1 21 11 % 7 3 + / -): " + RPNParser.DoParse("1 21 11 % 7 3 + / -")); // Expected: 0
    }

    private void TestExpressionsWithVariables()
    {
        Debug.Log("==== Variable Tests ====");

        var vars = new Dictionary<string, float>
        {
            { "base", 20 },
            { "wave", 3 }
        };

        Debug.Log("Test 0 (base 5 wave * +): " + RPNParser.DoParse("base 5 wave * +", vars)); // Expected: 35
        Debug.Log("Test 1 (base wave + 2 *): " + RPNParser.DoParse("base wave + 2 *", vars)); // ((20)+(3))*2 = 46
        Debug.Log("Test 2 (wave base /): " + RPNParser.DoParse("wave base /", vars)); // (3)/(20) = 0.15
    }
}
