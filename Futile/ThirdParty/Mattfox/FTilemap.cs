using System;
using UnityEngine;
using System.Collections.Generic;

public class FTilemap : FContainer {
	public bool repeatX = false;
	public bool repeatY = false;
	
	protected string _baseName;
	//protected string _baseExtension;
	
	protected bool _skipZero;
	
	protected int _tilesWide;
	protected int _tilesHigh;
	
	protected float _tileWidth;
	protected float _tileHeight;
	
	protected FShader _shader;
	
	// list of all tiles
	protected int[] _tileArray;
	protected List<FSprite> _tiles;
	
	// size of clipping
	protected float _clipWidth;
	protected float _clipHeight;
	
	// node which is located at the center of the clipping size, most likely FCamObject
	protected FNode _clipNode;
	
	protected bool _clipToScreen = false;
	protected Vector2 _clipPos;
	
	[Obsolete("use FTilemap(string elementBase) instead")]
	public FTilemap (string elementBase, string elementExtension) : base()
	{
		_baseName = elementBase;
		
		_tiles = new List<FSprite>();
		
		_shader = FShader.defaultShader;
		
		this.ListenForUpdate(Update);
	}
	
	public FTilemap (string elementBase) : base() 
	{
		_baseName = elementBase;
		
		_tiles = new List<FSprite>();
		
		_shader = FShader.defaultShader;
		
		this.ListenForUpdate(Update);
	}
	
	virtual public void Update() {
		if (_clipNode != null && !_skipZero) {
			// get position of _clipNode relative to this
			Vector2 relPos = StageToLocal(_clipNode.GetPosition());
			
			float xMin = relPos.x - _clipWidth / 2 - _tileWidth * 0.75f;
			float yMin = relPos.y - _clipHeight / 2 - _tileHeight * 0.75f;
			
			// check if the _clipNode has moved enough to check tile positions
			if (Mathf.Round(xMin / _tileWidth) == _clipPos.x && Mathf.Round(yMin / _tileHeight) == _clipPos.y) {
				return;
			}
			_clipPos = new Vector2(Mathf.Round(xMin / _tileWidth), Mathf.Round(yMin / _tileHeight));
			
			float tileOffsetX = (Mathf.Floor(_clipWidth / _tileWidth) + 2) * _tileWidth;
			float tileOffsetY = (Mathf.Floor(_clipHeight / _tileHeight) + 2) * _tileHeight;
			
			float xMax = xMin + tileOffsetX;
			float yMax = yMin + tileOffsetY;
			
			foreach (FSprite tile in _tiles) {
				bool tileChangedX = false;
				bool tileChangedY = false;
				while (tile.x < xMin) {
					tile.x += tileOffsetX;
					tileChangedX = true;
				}
				if (!tileChangedX) {
					while (tile.x > xMax) {
						tile.x -= tileOffsetX;
						tileChangedX = true;
					}
				}
				while (tile.y < yMin) {
					tile.y += tileOffsetY;
					tileChangedY = true;
				} 
				if (!tileChangedY) {
					while (tile.y > yMax) {
						tile.y -= tileOffsetY;
						tileChangedY = true;
					}
				}
				
				if (tileChangedX || tileChangedY) {
					int tileX = Mathf.FloorToInt((tile.x - _tileWidth / 2) / _tileWidth);
					int tileY = Mathf.FloorToInt((-tile.y - _tileHeight / 2) / _tileHeight);
					
					if (repeatX) {
						while (tileX < 0) {
							tileX += _tilesWide;
						} 
						while (tileX >= _tilesWide) {
							tileX -= _tilesWide ;
						}
					} else if (tileX < 0 || tileX >= _tilesWide) { // display empty tile, outside of known data
						tileX = -1;
						tileY = -1;
					}
					if (repeatY) {
						while (tileY < 0) {
							tileY += _tilesHigh;
						} 
						while (tileY >= _tilesHigh) {
							tileY -= _tilesHigh;
						}
					} else if (tileY < 0 || tileY >= _tilesHigh) { // display empty tile, outside of known data
						tileX = -1;
						tileY = -1;
					}
					
					int frame = tileX + tileY * _tilesWide;
					if (frame >= 0 && frame < _tileArray.GetLength(0)) {
						int frameNum = _tileArray[frame];
						tile.element = Futile.atlasManager.GetElementWithName(_baseName+"_"+frameNum);
						tile.isVisible = true;
					} else {
						tile.isVisible = false;
					}
				}
			}
		}
	}
	
	public void LoadText (string text, bool skipZero=true) {
		
		int zeroCount = 0;
		
		// remember for later
		_skipZero = skipZero;
		
		string[] lines = text.Split('\n');
		int i = 0;
		int j = 0;
		
		// set width/height
		string[] firstLine = lines[0].Split(',');
		_tilesWide = firstLine.GetLength(0);
		if (firstLine[firstLine.GetLength(0)-1] == "") {
			_tilesWide -= 1;
		}
		_tilesHigh = lines.GetLength(0);
		
		// set array
		_tileArray = new int[_tilesWide * _tilesHigh];
		
		foreach (string line in lines) {
			if (line != "") { // skip empty rows
				
				// split into individual numbers
				string[] frames = line.Split(',');
				
				i = 0;
				foreach (string frame in frames) {
					if (frame != "") { 
						// keep track of all frames
						int frameNum = int.Parse(frame);
						_tileArray[i+(j*_tilesWide)] = frameNum;
						
						if (frameNum == 0) {
							zeroCount++;
						}
					
						i++;
					}
				}
				
				j++;
			}
		}
		
		// get tile width/height
		FAtlasElement element =	Futile.atlasManager.GetElementWithName(_baseName+"_1");
		_tileWidth = element.sourceSize.x;
		_tileHeight = element.sourceSize.y;
		
		// warning if skipZero would give better results
		if (_clipWidth > 0 && _clipHeight > 0 && _clipNode != null) {
			int clipTilesWide = Mathf.CeilToInt(_clipWidth / _tileWidth) + 2;
			int clipTilesHigh = Mathf.CeilToInt(_clipHeight / _tileHeight) + 2;
			if (zeroCount > clipTilesWide * clipTilesHigh) {
				Debug.Log ("FTilemap would use less memory if _skipZero was true.");
				_skipZero = true;
				clipToScreen = false;
			}
		}
		
		buildTiles();
	}
	
	// skip zero lets us save resources by not adding tiles for the number 0
	// if you need to be able to change empty tiles later, set skipZero to false
	public void LoadCSV (string textFile, bool skipZero=true) {
		TextAsset dataAsset = (TextAsset) Resources.Load (textFile, typeof(TextAsset));
		if(!dataAsset) {
			Debug.Log ("FTilemap: Couldn't load the atlas data from: " + textFile);
		}
		string fileContents = dataAsset.ToString();
		Resources.UnloadAsset(dataAsset);
		
		this.LoadText(fileContents, skipZero);
	}
	
	protected void buildTiles() {
		// clear pos so next update fixes tiles
		_clipPos = Vector2.zero;
		
		// figure out needed size
		int clipTilesWide = Mathf.CeilToInt(_clipWidth / _tileWidth) + 2;
		if (_tilesWide < clipTilesWide || _clipWidth == 0 || _skipZero) {
			clipTilesWide = _tilesWide;
		}
		int clipTilesHigh = Mathf.CeilToInt(_clipHeight / _tileHeight) + 2;
		if (_tilesHigh < clipTilesHigh || _clipHeight == 0 || _skipZero) {
			clipTilesHigh = _tilesHigh;
		}
		
		// update count of tiles
		if (!_skipZero) {
			int totalNeeded = clipTilesWide * clipTilesHigh;
		
			// make sure we have the right amount of tiles for the current clip size
			if (_tiles.Count <= 0) {
				for (int i = 0; i < totalNeeded; i++) {
					FSprite sprite = new FSprite(_baseName + "_1"); // set to 1
					sprite.shader = _shader;
					
					// add to this collection
					_tiles.Add(sprite);
					AddChild(sprite);
				}
			} else if (_tiles.Count < totalNeeded) {
				int start = _tiles.Count;
				for (int i = start; i < totalNeeded; i++) {
						FSprite sprite = new FSprite(_baseName + "_1");
						sprite.shader = _shader;
						
						// add to this collection
						_tiles.Add(sprite);
						AddChild(sprite);
				}
			} else if (_tiles.Count > totalNeeded) {
				int removeThisMany = _tiles.Count - totalNeeded;
				for (int i = 0; i < removeThisMany; i++) {
					// remove from the beginning of array
					RemoveChild(_tiles[0]);
					_tiles.RemoveAt(0);
				}
			}
		}
		
		// set clipWidth to the whole size if it doesn't exist
		if (_clipWidth == 0) {
			_clipWidth = _tilesWide * _tileWidth;
		}
		if (_clipHeight == 0) {
			_clipHeight = _tilesHigh * _tileHeight;
		}
		
		// make array of sprites
		for (int i = 0; i < clipTilesWide; i++) {
			for (int j = 0; j < clipTilesHigh; j++) {
				int frame = _tileArray[i + (j*_tilesWide)];
				
				if (!_skipZero || frame > 0) {
					FSprite sprite;
					if (_skipZero) {
						sprite = new FSprite(_baseName + "_"+frame);
						sprite.shader = _shader;
						AddChild(sprite);
					} else {
						sprite = _tiles[i + (j*clipTilesWide)];
						sprite.element = Futile.atlasManager.GetElementWithName(_baseName+"_"+frame);
					}
					
					// offset sprite coordinates
					sprite.x = i * _tileWidth + _tileWidth / 2;
					sprite.y = -j * _tileHeight - _tileHeight / 2;
					
					if (frame == 0) {
						sprite.isVisible = false;
					} else {
						sprite.isVisible = true;
					}
				}
			}
		}
		
	}
	
	public float getLeft() {
		float returnX = width;
		foreach (FSprite sprite in _childNodes) {
			if (returnX > sprite.x) {
				returnX = sprite.x;
			}
		}
		
		return returnX;
	}
	
	public float getRight() {
		float returnX = 0;
		foreach (FSprite sprite in _childNodes) {
			if (returnX < sprite.x + sprite.width) {
				returnX = sprite.x + sprite.width;
			}
		}
		
		return returnX;
	}
	
	public int getFrameNum(int givenX, int givenY) {
		return _tileArray[(givenX % _tilesWide) + (givenY * _tilesWide)];
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
				
				float compareX = Mathf.Floor((sprite.x - _tileWidth/2) / (float)_tileWidth);
				float compareY = -Mathf.Floor((sprite.y + _tileHeight/2) / (float)_tileHeight);
				
				if (checkX == compareX && checkY == compareY) {
					return sprite as FSprite;
				}
			}
			
			Debug.Log ("FTilemap: index [" + checkX + ", " + checkY + "] not found.");
			return null;
		}
	}
	
	virtual public void HandleResize(bool orientationChange) {
		if (clipToScreen) {
			// update width/height
			_clipWidth = Futile.screen.width;
			_clipHeight = Futile.screen.height;
			
			// reset tiles
			buildTiles();
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
	
	virtual public float clipWidth
	{
		get { return _clipWidth; }
		set { _clipWidth = value; }
	}
	
	virtual public float clipHeight
	{
		get { return _clipHeight; }
		set { _clipHeight = value; }
	}
	
	virtual public FNode clipNode {
		get { return _clipNode; }
		set { _clipNode = value; }
	}
	
	public bool clipToScreen {
		get { return _clipToScreen; }
		set {
			if (value) {
				Futile.screen.SignalResize += this.HandleResize;
				_clipWidth = Futile.screen.width;
				_clipHeight = Futile.screen.height;
			} else if (_clipToScreen == true) {
				Futile.screen.SignalResize -= this.HandleResize;
			}
			
			_clipToScreen = value;
		}
	}
	
	virtual public int widthInTiles
	{
		get { return _tilesWide; }
	}
	
	virtual public int heightInTiles
	{
		get { return _tilesHigh; }
	}
	
	public FShader shader
	{
		get { return _shader;}
		set {
			if(_shader != value) {
				_shader = value;
			
				// update shader of each tile
				foreach (FFacetNode tile in _childNodes) {
					tile.shader = _shader;
				}
			}
		}
	}
}