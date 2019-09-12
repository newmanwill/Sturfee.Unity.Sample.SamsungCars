using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Test : MonoBehaviour {

    public int GlobalInt = 4;

	// Use this for initialization
	void Start () {
        int localInt = 2;

        print($"The {GlobalInt} is 4");
        print($"The {localInt} is 2");

        //Console.WriteLine("The {0} = 4", GlobalInt);
        //Console.WriteLine("The {1} = 2 and {0} = 4", GlobalInt, localInt);


        StringBuilder strBuilder = new StringBuilder("Greg");
        //strBuilder

        print(string.Format("Srting Format: {0} = 4", GlobalInt));

    }

}
