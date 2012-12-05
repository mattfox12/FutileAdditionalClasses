using System;
using System.Collections;
using System.Collections.Generic;

public class XMLNode
{

    public String                           tagName;
    public XMLNode                          parentNode;
    public ArrayList                        children;
    public Dictionary<String, String>       attributes;
	public String							value;

    public XMLNode ()
    {
        tagName = "NONE";
        parentNode = null;
        children = new ArrayList();
        attributes = new Dictionary<String, String>();
		value = "";
    }
}