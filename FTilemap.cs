using System;
using UnityEngine;


public class FTilemap : FContainer
{
	
	protected string _baseName;
	protected string _baseExtension;
	
	protected bool _skipZero;
	
	protected int _tilesWide;
	protected int _tilesHigh;
	
	protected float _tileWidth;
	protected float _tileHeight;
	
	public FTilemap (string elementBase, string elementExtension="png") : base()
	{
		_baseName = elementBase;
		_baseExtension = elementExtension;
	}
	
	// skip zero lets us save resources by not adding tiles for the number 0
	// if you need to be able to change empty tiles later, set skipZero to false
	public void LoadCSV (string textFile, bool skipZero=true) {
		TextAsset dataAsset = (TextAsset) Resources.Load (textFile, typeof(TextAsset));
		if(!dataAsset)
		{
			Debug.Log ("Futile: Couldn't load the atlas data from: " + textFile);
		}
		string fileContents = dataAsset.ToString();
		Resources.UnloadAsset(dataAsset);
		
		// remember for later
		_skipZero = skipZero;
		
		string[] lines = fileContents.Split('\n');
		int i = 0;
		int j = 0;
		foreach (string line in lines) {
			if (line != "") { // skip empty rows
				
				// split into individual numbers
				string[] frames = line.Split(',');
				
				i = 0;
				foreach (string frame in frames) {
					if (frame != "") { // skip empty frames
						
						int frameNum = int.Parse(frame);
						if (frameNum > 0 || (!skipZero && frameNum == 0)) {
							// create sprite for this
							FSprite sprite = new FSprite(_baseName + "_" + frame + "." + _baseExtension);
							
							// remember width/height of tiles, each tile should be same size
							if (_tileWidth <= 0 || _tileHeight <= 0) {
								_tileWidth = sprite.width;
								_tileHeight = sprite.height;
							}
							
							// offset sprite coordinates
							sprite.x = i * _tileWidth;
							sprite.y = -j * _tileHeight;
							
							if (int.Parse(frame) == 0) {
								sprite.isVisible = false;
							}
							
							// add to this collection
							AddChild(sprite);
						}
					
						i++;
					}
				}
				
				j++;
			}
		}
		
		// set the wide/high tile count
		_tilesWide = i-1;
		_tilesHigh = j-1;
	}
	
	// returns FSprite at 
	public FSprite getTile(int givenX, int givenY) {
		if (!_skipZero) {
			int node = (givenX % _tilesWide) + givenY * _tilesWide;
			
			if (node < _tilesWide * _tilesHigh) {
				return _childNodes[node] as FSprite;
			} else {
				Debug.Log ("FTilemap: index [" + node + "] outside of range: " + (_tilesWide * _tilesHigh));
				return null;
			}
		} else {
			return getTileAt(givenX * _tileWidth, givenY * _tileHeight);
		}
	}
	
	public FSprite getTileAt(float givenX, float givenY) {
		if (!_skipZero) {
			int node = (int)((_tilesWide % Mathf.Floor(givenX / _tileWidth)) + Mathf.Floor(givenY / _tileHeight) * _tilesWide);
			
			if (node < _tilesWide * _tilesHigh) {
				return _childNodes[node] as FSprite;
			} else {
				Debug.Log ("FTilemap: index [" + node + "] outside of range: " + (_tilesWide * _tilesHigh));
				return null;
			}
		} else {
			float checkX = Mathf.Floor(givenX / _tileWidth);
			float checkY = Mathf.Floor(givenY / _tileHeight);
			
			// loop through the available sprites
			foreach (FNode sprite in _childNodes) {
				
				float compareX = Mathf.Floor(sprite.x / (float)_tileWidth);
				float compareY = -Mathf.Floor(sprite.y / (float)_tileHeight);
				
				if (checkX == compareX && checkY == compareY) {
					return sprite as FSprite;
				}
			}
			
			Debug.Log ("FTilemap: index [" + checkX + ", " + checkY + "] not found.");
			return null;
		}
	}
	
	public bool skipZero 
	{
		get { return _skipZero; }
	}
	
	virtual public float width
	{
		get { return _scaleX * _tilesWide * _tileWidth; }
	}
	
	virtual public float height
	{
		get { return _scaleY * _tilesHigh * _tileHeight; }
	}
	
	virtual public int widthInTiles
	{
		get { return _tilesWide; }
	}
	
	virtual public int heightInTiles
	{
		get { return _tilesHigh; }
	}
}