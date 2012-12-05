using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FTmxMap : FContainer {
	
	private List<XMLNode> _tilesets;
	
	public int objectStartInt = 1;

	public FTmxMap ()
	{
		_tilesets = new List<XMLNode>();
		
		// starts tilemap in upper left corner
		this.x = -Futile.screen.halfWidth;
		this.y = Futile.screen.halfHeight;
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
				// get csv content
				XMLNode csvData = child.children[0] as XMLNode;
				if (csvData.attributes["encoding"] != "csv") {
					Debug.Log ("FTiledScene: Could not render layer data, encoding set to: " + csvData.attributes["encoding"]);
					break;
				}
				
				// get text for csv data
				string csvText = csvData.value;
				string firstFrame = csvText.Substring( 0, csvText.IndexOf(',') );
				int firstID = int.Parse(firstFrame);
				
				// find name of tileset being used
				string baseName = this.getTilesetNameForID(firstID);
				string baseExtension = this.getTilesetExtensionForID(firstID);
				
				// create tilemap
				FTilemap tilemap = new FTilemap(baseName, baseExtension);
				tilemap.LoadText(csvText, true);
				AddChild(tilemap);
			}
			
			// create FContainers for layer nodes
			if (child.tagName == "objectgroup") {
				// add objects to FContainers
				FContainer objectGroup = new FContainer();
				
				foreach (XMLNode fObject in child.children) {
					if (fObject.attributes.ContainsKey("gid")) {
						// get id numbers needed
						int id = int.Parse(fObject.attributes["gid"]);
						int firstID = this.getTilesetFirstIDForID(id);
						
						// find parts of source image
						string baseName = this.getTilesetNameForID(id);
						int actualFrame = id - firstID + objectStartInt;
						string baseExtension = this.getTilesetExtensionForID(id);
						
						// assemble whole name
						string wholeName = baseName + "_" + actualFrame + "." + baseExtension;
						
						// get x,y
						int x = int.Parse(fObject.attributes["x"]);
						int y = int.Parse(fObject.attributes["y"]);
						
						// create FSprite (override that function for specific class changes)
						objectGroup.AddChild(this.createTileObject(wholeName, x, y));
					} else if (fObject.attributes.ContainsKey("type")) {
						// type
						string type = fObject.attributes["type"];
						
						// get x,y
						int x = int.Parse(fObject.attributes["x"]);
						int y = int.Parse(fObject.attributes["y"]);
						
						objectGroup.AddChild(this.createObject(type, x, y));
					}
				}
				
				// add to self
				AddChild(objectGroup);
			}
		}
		
	}
	
	private string getTilesetNameForID(int num) {
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
	
	private string getTilesetExtensionForID(int num) {
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
	
	private int getTilesetFirstIDForID(int num) {
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
	
	virtual protected FNode createTileObject(string name, int givenX, int givenY) 
	{
		FSprite sprite = new FSprite(name);
		sprite.x = givenX + sprite.width / 2;
		sprite.y = -givenY + sprite.height / 2;
		
		return sprite;
	}
	
	virtual protected FNode createObject(string type, int givenX, int givenY) 
	{
		FSprite sprite = new FSprite(type);
		sprite.x = givenX + sprite.width / 2;
		sprite.y = -givenY + sprite.height / 2;
		
		return sprite;
	}
	
}
