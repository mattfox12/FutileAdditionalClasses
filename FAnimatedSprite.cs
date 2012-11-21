using UnityEngine;
using System;
using System.Collections.Generic;

public class FAnimatedSprite : FSprite {
	
	protected bool _pause = false;
	protected float _time = 0;
	protected int _currentFrame = 0;
	
	protected string _baseName;
	protected string _baseExtension;
	
	protected List<FAnimation> _animations;
	
	protected FAnimation _currentAnim;
	
	protected FAnimatedSprite() : base() //for overriding
	{
		
	}
	
	public FAnimatedSprite (string elementBase, string elementExtension="png") : base()
	{
		_baseName = elementBase;
		_baseExtension = elementExtension;
		
		// default to first frame, no animation
		Init(Futile.atlasManager.GetElementWithName(_baseName+"_1."+_baseExtension),1); // expects individual frames, in convention of NAME_#.EXT
		_isAlphaDirty = true;
		UpdateLocalVertices();
		
		_animations = new List<FAnimation>();
	}
	
	// Update is called once per frame
	public void Update () {
		if (_currentAnim != null && !_pause) {
			_time += Time.deltaTime;
		
			while (_time > (float)_currentAnim.delay / 1000.0f) { // looping this way will skip frames if needed
				_currentFrame++;
				if (_currentFrame >= _currentAnim.totalFrames) {
					if (_currentAnim.looping) {
						_currentFrame = 0;
					} else {
						_currentFrame = _currentAnim.totalFrames - 1;
					}
				}
				
				element = Futile.atlasManager.GetElementWithName(_baseName+"_"+_currentAnim.frames[_currentFrame]+"."+_baseExtension);
				
				_time -= (float)_currentAnim.delay / 1000.0f;
			}
		}
	}
	
	public void addAnimation(FAnimation anim) {
		_animations.Add(anim);
		
		if (_currentAnim == null) {
			_currentAnim = anim;
			_currentFrame = 0;
			_pause = false;
		}
	}
	
	public void play(string animName, bool forced=false) {
		// check if we are giving the same animation that is currently playing
		if (forced && _currentAnim.name == animName) {
			// restart at first frame
			_currentFrame = 0;
			_time = 0;
			
			// redraw
			element = Futile.atlasManager.GetElementWithName(_baseName+"_"+_currentAnim.frames[0]+"."+_baseExtension);
			
			return;
		} else if (!forced && _currentAnim.name == animName) {
			// do nothing!
			return;
		}
		
		// find the animation with the name given, no change if not found
		foreach (FAnimation anim in _animations) {
			if (anim.name == animName) {
				_currentAnim = anim;
				_currentFrame = 0;
				_time = 0;
				
				// force redraw to first frame
				element = Futile.atlasManager.GetElementWithName(_baseName+"_"+anim.frames[0]+"."+_baseExtension);
				
				break;
			}
		}
	}
	
	public void pause(bool forced=false) {
		if (forced) {
			_pause = true;
		} else {
			_pause = !_pause;
		}
	}
	
	public string baseName {
		get { return _baseName; }
		set { _baseName = value; }
	}
	
	public string baseExtension {
		get { return _baseExtension; }
		set { _baseExtension = value; }
	}
	
	public FAnimation currentAnim {
		get { return _currentAnim; }
	}
	
	public bool isPaused {
		get { return _pause; }
	}
}
