using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class XMLReader
{
    private static char TAG_START = '<';
    private static char TAG_END = '>';
    private static char SPACE = ' ';
    private static char QUOTE = '"';
    private static char QUESTION = '?';
    private static char SLASH = '/';
    private static char EQUALS = '=';
    private static String BEGIN_QUOTE = "" + EQUALS + QUOTE; 

    public XMLReader ()
    {

    }

    public XMLNode read( String xml )
    {
        int index = 0;
        int lastIndex = 0;
        XMLNode rootNode = new XMLNode();
        XMLNode currentNode = rootNode;

        while ( true )
        {
            index = xml.IndexOf(TAG_START, lastIndex );

            if ( index < 0 || index >= xml.Length )
            {
				//Debug.Log("XMLReader: Exit on index " + index);
                break;
            }
			
			
			// check for untagged text between index and lastIndex
			int lengthCheck = (index - lastIndex) - 1;
			if (lengthCheck > 0) {
				String xmlText = xml.Substring( lastIndex + 1, lengthCheck ).Trim();
				if (xmlText != "" && xmlText != "<") {
					//Debug.Log("XMLReader: Text Between indexes " + xmlText);
					currentNode.value = xmlText;
				}
			}
			
            index++;

            lastIndex = xml.IndexOf(TAG_END, index);
            if ( lastIndex < 0 || lastIndex >= xml.Length )
            {
				//Debug.Log("XMLReader: Exit on lastIndex " + lastIndex);
                break;
            }

            int tagLength = lastIndex - index;
            String xmlTag = xml.Substring( index, tagLength );
			
			//Debug.Log("XMLReader: Find text: " + xmlTag);
			
            // The tag starts with "<?"
            if (xmlTag[0] == QUESTION) {
				//Debug.Log("XMLReader: Question " + xmlTag);
                continue;
            }
            

            // if the tag starts with a </ then it is an end tag
            if (xmlTag[0] == SLASH)
            {		
				//Debug.Log("XMLReader: Slash " + xmlTag);		
                currentNode = currentNode.parentNode;
                continue;
            }

            bool openTag = true;

            // if the tag ends in /> the tag can be considered closed
            if ( xmlTag[tagLength - 1] == SLASH)
            {
                // cut away the slash
                xmlTag = xmlTag.Substring( 0, tagLength - 1 ); 
                openTag = false;
            } else {
				//Debug.Log("XMLReader: No open tag " + xmlTag);
			}

            XMLNode node = parseTag( xmlTag );
            node.parentNode = currentNode;
            currentNode.children.Add( node );

            if ( openTag )
            {				
                currentNode = node;
            }

        }

        return rootNode;
    }

    public XMLNode parseTag( String xmlTag )
    {
        XMLNode node = new XMLNode();

        int nameEnd = xmlTag.IndexOf(SPACE, 0);
        if ( nameEnd < 0 )
        {
            node.tagName = xmlTag;

            return node;
        }

        String tagName = xmlTag.Substring( 0, nameEnd );
        node.tagName = tagName;

        String attrString = xmlTag.Substring( nameEnd, xmlTag.Length - nameEnd );

        return parseAttributes( attrString, node );
    }

    public XMLNode parseAttributes( String xmlTag, XMLNode node )
    {
        int index = 0;
        int attrNameIndex = 0;
        int lastIndex = 0;

        while ( true )
        {

            index = xmlTag.IndexOf(BEGIN_QUOTE, lastIndex);

            if ( index < 0 || index > xmlTag.Length)
            {
                break;
            }

            attrNameIndex = xmlTag.LastIndexOf(SPACE, index );

            if ( attrNameIndex < 0 || attrNameIndex > xmlTag.Length )
            {
                break;
            }

            attrNameIndex++;
            String attrName = xmlTag.Substring( attrNameIndex, index - attrNameIndex );

            // skip the equal and quote character
            index +=2;

            lastIndex = xmlTag.IndexOf(QUOTE, index);

            if ( lastIndex < 0 || lastIndex > xmlTag.Length ) 
            {
                break;
            }

            int tagLength = lastIndex - index;
            String attrValue = xmlTag.Substring( index, tagLength );

            node.attributes[attrName] = attrValue;          

        }

        return node;

    }

    public void printXML( XMLNode node, int indent )
    {
        indent ++;

        foreach ( XMLNode n in node.children )
        {
            String attr = " ";
            foreach( KeyValuePair<String, String> p in n.attributes )
            {
                attr += "[" + p.Key + ": " + p.Value + "] ";
                //Debug.Log( attr );
            }

            String indentString = "";
            for ( int i=0; i< indent; i++ )
            {
                indentString += "-";
            }

            Debug.Log( "" + indentString + " " + n.tagName + attr );
            printXML(n, indent );
        }
    }   
	
}