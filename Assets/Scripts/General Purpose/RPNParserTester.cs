//Author: Efe Ayan
//Editor: Xavier Austin
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

        //test for addition operator
        Debug.Log("Test 1 (5 3 +): " + RPNParser.Instance.DoParse("5 3 +")); // Expected: 8
        //test for subtraction operator
        Debug.Log("Test 3 (6 4 -): " + RPNParser.Instance.DoParse("6 4 -")); // Expected: 2
        //test for multiplication operator
        Debug.Log("Test 4 (3 5 *): " + RPNParser.Instance.DoParse("3 5 *")); // Expected: 15
        //test for modulo operator
        Debug.Log("Test 5 (7 3 %): " + RPNParser.Instance.DoParse("7 3 %")); // Expected: 1
        //test for division operator
        Debug.Log("Test 2 (10 2 /): " + RPNParser.Instance.DoParse("10 2 /")); // Expected: 5
        //test for integer arithmatic
        Debug.Log("Test 6 (7 3 /): " + RPNParser.Instance.DoParse("7 3 /")); // Expected: 2
        Debug.Log("Test 7 (7 3.0 /): " + RPNParser.Instance.DoParse("7 3.0 /")); // Expected: 2.33 (repeating)
        //test for pushing and poping a larger number of stack items
        Debug.Log("Test 8 (1 21 11 % 7 3 + / -): " + RPNParser.Instance.DoParse("1 21 11 % 7 3 + / -")); // Expected: 0
    }

    private void TestExpressionsWithVariables()
    {
        Debug.Log("==== Variable Tests ====");

        var vars = new Dictionary<string, float>
        {
            { "base", 20 },
            { "wave", 3 }
        };

        Debug.Log("Test 6 (base 5 wave * +): " + RPNParser.Instance.DoParse("base 5 wave * +", vars)); // Expected: 35
        Debug.Log("Test 7 (base wave + 2 *): " + RPNParser.Instance.DoParse("base wave + 2 *", vars)); // ((20)+(3))*2 = 46
        Debug.Log("Test 8 (wave base /): " + RPNParser.Instance.DoParse("wave base /", vars)); // (3)/(20) = 0.15
    }
}
