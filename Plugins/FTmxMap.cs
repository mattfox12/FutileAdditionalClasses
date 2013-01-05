using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FTmxMap : FContainer {
	
	private List<XMLNode> _tilesets;
	private List<string> _layerNames;
	
	public int objectStartInt = 1;

	public FTmxMap ()
	{
		_tilesets = new List<XMLNode>();
		_layerNames = new List<string>();
	}
	
	public void LoadTMX(string fileName) 
	{
		// load xml document
		TextAsset dataAsset = (TextAsset) Resources.Load (fileName, typeof(TextAsset));
		if(!dataAsset)
		{
			Debug.Log ("FTiledScene: Couldn't load the xml data from: " + fileName);
		}
		string fileContents = dataAsset.ToString();
		Resources.UnloadAsset(dataAsset);
		
		// parse xml string
		XMLReader parser = new XMLReader();
		XMLNode xmlNode = parser.read(fileContents);
		XMLNode rootNode = xmlNode.children[0] as XMLNode;
		
		// loop through all children
		foreach (XMLNode child in rootNode.children) {
			//Debug.Log ("FTiledScene: Node[]: " + child.tagName);
			
			// save references to tilesets
			if (child.tagName == "tileset") {
				_tilesets.Add(child);
			}
			
			// create FTilemap for layer nodes
			if (child.tagName == "layer" && child.children.Count > 0) {
				AddChild(this.createTilemap(child));
			}
			
			// create FContainers for layer nodes
			if (child.tagName == "objectgroup") {
				AddChild(this.createObjectLayer(child));
			}
		}
		
	}
	
	protected string getTilesetNameForID(int num) 
	{
		if (_tilesets.Count < 1) {
			Debug.Log("FTiledScene: No Tilesets found.");
			return "";
		}
		
		XMLNode wantedNode = _tilesets[0];
		
		// loop through tilesets
		foreach (XMLNode node in _tilesets) {
			// check if node attribute firstgid >= num
			int firstID = int.Parse (node.attributes["firstgid"]);
			if (firstID <= num) {
				wantedNode = node;
			}
		}
		
		// return the name of the file from wantedNode
		XMLNode imageNode = wantedNode.children[0] as XMLNode;
		string sourceString = imageNode.attributes["source"];
		int startIndex = sourceString.LastIndexOf('/') + 1;
		string returnValue = sourceString.Substring( startIndex , sourceString.LastIndexOf('.') - startIndex);
		
		return returnValue;
	}
	
	protected string getTilesetExtensionForID(int num) 
	{
		if (_tilesets.Count < 1) {
			Debug.Log("FTiledScene: No Tilesets found.");
			return "";
		}
		
		XMLNode wantedNode = _tilesets[0];
		
		// loop through tilesets
		foreach (XMLNode node in _tilesets) {
			// check if node attribute firstgid >= num
			int firstID = int.Parse (node.attributes["firstgid"]);
			if (firstID <= num) {
				wantedNode = node;
			}
		}
		
		// return the extension of the file from wantedNode
		XMLNode imageNode = wantedNode.children[0] as XMLNode;
		string sourceString = imageNode.attributes["source"];
		int startIndex = sourceString.LastIndexOf('.') + 1;
		string returnValue = sourceString.Substring( startIndex );
		
		return returnValue;
	}
	
	protected int getTilesetFirstIDForID(int num) 
	{
		if (_tilesets.Count < 1) {
			Debug.Log("FTiledScene: No Tilesets found.");
			return -1;
		}
		
		XMLNode wantedNode = _tilesets[0];
		
		// loop through tilesets
		foreach (XMLNode node in _tilesets) {
			// check if node attribute firstgid >= num
			int firstID = int.Parse (node.attributes["firstgid"]);
			if (firstID <= num) {
				wantedNode = node;
			}
		}
		
		// return the firstgid of the file from wantedNode
		int startIndex = int.Parse (wantedNode.attributes["firstgid"]);
		
		return startIndex;
	}
	
	virtual protected FNode createObjectLayer(XMLNode node)
	{
		// add objects to FContainers
		FContainer objectGroup = new FContainer();
		
		foreach (XMLNode fObject in node.children) {
			if (fObject.tagName == "object") {
				if (fObject.attributes.ContainsKey("gid")) {
					// create FSprite (override that function for specific class changes)
					objectGroup.AddChild(this.createTileObject(fObject));
				} else {
					objectGroup.AddChild(this.createObject(fObject));
				}
			}
		}
		
		// remember name 
		_layerNames.Add (node.attributes["name"]);
		
		// add to self
		return objectGroup;
	}
	
	virtual protected FNode createTilemap(XMLNode node) 
	{
		XMLNode csvData = new XMLNode();
		XMLNode properties = new XMLNode();
		foreach (XMLNode child in node.children) {
			if (child.tagName == "data") {
				csvData = child;
			} else if (child.tagName == "properties") {
				properties = child;
			}
		}
		
		// make sure encoding is set to csv
		if (csvData.attributes["encoding"] != "csv") {
			Debug.Log ("FTiledScene: Could not render layer data, encoding set to: " + csvData.attributes["encoding"]);
			return null;
		}
		
		// remember name 
		_layerNames.Add (node.attributes["name"]);
		
		// do stuff with properties
		foreach (XMLNode property in properties.children) {
			// check each property
			if (property.attributes["name"] == "something") {
				// do something with property.attributes["value"];
			}
		}
		
		// get text for csv data
		string csvText = csvData.value;
		string firstFrame = csvText.Substring( 0, csvText.IndexOf(',') );
		int firstID = int.Parse(firstFrame);
		
		// find name of tileset being used, assumes all tiles are from the same tileset
		string baseName = this.getTilesetNameForID(firstID);
		string baseExtension = this.getTilesetExtensionForID(firstID);
		
		// create tilemap
		FTilemap tilemap = new FTilemap(baseName, baseExtension);
		tilemap.LoadText(csvText, true);

		return tilemap;
	}
	
	virtual protected FNode createTileObject(XMLNode node) 
	{
		// get id numbers needed
		int id = int.Parse(node.attributes["gid"]);
		int firstID = this.getTilesetFirstIDForID(id);
		
		// find parts of source image
		string baseName = this.getTilesetNameForID(id);
		int actualFrame = id - firstID + objectStartInt;
		string baseExtension = this.getTilesetExtensionForID(id);
		
		// assemble whole name
		string name = baseName + "_" + actualFrame + "." + baseExtension;
		
		// get x,y
		int givenX = int.Parse(node.attributes["x"]);
		int givenY = int.Parse(node.attributes["y"]);
		
		FSprite sprite = new FSprite(name);
		sprite.x = givenX + sprite.width / 2;
		sprite.y = -givenY + sprite.height / 2;
		
		return sprite;
	}
	
	virtual protected FNode createObject(XMLNode node) 
	{
		// type
		string type = node.attributes["type"];
		
		// get x,y
		int givenX = int.Parse(node.attributes["x"]);
		int givenY = int.Parse(node.attributes["y"]);
		
		FSprite sprite = new FSprite(type);
		sprite.x = givenX + sprite.width / 2;
		sprite.y = -givenY + sprite.height / 2;
		
		return sprite;
	}
	
	public FNode getLayerNamed(string name) 
	{
		int i = 0;
		foreach (string check in _layerNames) {
			if (check == name) {
				return GetChildAt(i);
			}
			 i++;
		}
		//
		Debug.Log("No layer named " + name + " found.");
		return null;
	}
}
