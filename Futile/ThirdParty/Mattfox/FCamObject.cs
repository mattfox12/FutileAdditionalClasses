using UnityEngine;
using System.Collections;

// Why is this an FContainer? -- So you can add HUD elements to it!

public class FCamObject : FContainer {
	
	// object to follow
	protected FNode _followObject;
	
	// bounds around follow character to allow dead space
	protected Rect _bounds;
	
	// world bounds, camera will not go past
	protected Rect _worldBounds;
	
	// shake vars
	protected Vector2 _shakeOffset;
	protected float _shakeDuration;
	protected float _shakeIntensity;
	protected bool _shakeIncludeHUD;
	
	public FCamObject () : base()
	{		
		this.ListenForUpdate(Update);
		this.ListenForResize(HandleResize);
		
		_bounds = new Rect(-5,-5,10,10);
		
		_worldBounds = new Rect(0,0,0,0);
		
		_shakeIncludeHUD = true;
		
	}
	
	override public void HandleRemovedFromStage()
	{
		// reset to zero
		_followObject = null;
		_worldBounds.width = 0;
		_worldBounds.height = 0;
		x = 0;
		y = 0;
		
		base.HandleRemovedFromStage();
	}
	
	virtual public void HandleResize(bool orientationChange) {
		// use this function to reposition objects when the screen size or orientation changes
	}
	
	virtual public void Update() {
		if (!_shakeIncludeHUD) {
			x += _shakeOffset.x;
			y += _shakeOffset.y;
		} else {
			x += _shakeOffset.x/2;
			y += _shakeOffset.y/2;
		}
		
		// move to keep object within bounds
		if (_followObject != null) {
			// try to keep player within bounds
			if (_bounds.width > 0) {
				if (_followObject.x > x + _bounds.xMax) {
					x = _followObject.x - _bounds.width / 2;
				} else if (_followObject.x < x + _bounds.xMin) {
					x = _followObject.x + _bounds.width / 2;
				}
			} else {
				// follow directly
				x = _followObject.x;
			}
			if (_bounds.height > 0) {
				if (_followObject.y > y + _bounds.yMax) {
					//Debug.Log(_followObject.y + " > " + (y + _bounds.yMax));
					y = _followObject.y - _bounds.height / 2;
				} else if (_followObject.y < y + _bounds.yMin) {
					//Debug.Log(_followObject.y + " < " + (y + _bounds.yMin));
					y = _followObject.y + _bounds.height / 2;
				}
			} else {
				// follow directly
				y = _followObject.y;
			}
		}
		
		
		// keep within world bounds
		if (_worldBounds.width > 0) {
			if (Futile.screen.width > _worldBounds.width) {
				x = (_worldBounds.width / 2) + _worldBounds.x;
			} else if (x < _worldBounds.x + Futile.screen.halfWidth) {
				x = _worldBounds.x + Futile.screen.halfWidth;
			} else if (x > _worldBounds.xMax - Futile.screen.halfWidth) {
				x = _worldBounds.xMax - Futile.screen.halfWidth;
			}
		}
		if (_worldBounds.height > 0) {
			if (Futile.screen.height > _worldBounds.height) {
				y = (_worldBounds.height / 2) + _worldBounds.y;
			} else if (y < _worldBounds.yMin + Futile.screen.halfHeight) {
				y = _worldBounds.yMin + Futile.screen.halfHeight;
			} else if (y > _worldBounds.yMax - Futile.screen.halfHeight) {
				y = _worldBounds.yMax - Futile.screen.halfHeight;
			}
		}
		
		// round values, sometimes needed for pixel art
		//x = Mathf.Round(x * Futile.displayScale) / Futile.displayScale;
		//y = Mathf.Round(y * Futile.displayScale) / Futile.displayScale;
		
		// shake
		_shakeOffset = new Vector2(0,0);
		if (_shakeDuration > 0) {
			_shakeDuration -= UnityEngine.Time.deltaTime;
			
			_shakeOffset.x = Mathf.RoundToInt(Random.value * _shakeIntensity * Futile.resourceScale);
			_shakeOffset.y = Mathf.RoundToInt(Random.value * _shakeIntensity * Futile.resourceScale);
		}
		
		Futile.stage.x = -x + _shakeOffset.x;
		Futile.stage.y = -y + _shakeOffset.y;
		
		if (!_shakeIncludeHUD) {
			x -= _shakeOffset.x;
			y -= _shakeOffset.y;
		} else {
			x -= _shakeOffset.x/2;
			y -= _shakeOffset.y/2;
		}
	}
	
	public void follow(FNode givenObject)
	{
		_followObject = givenObject;
	}
	
	// move to a coordinate
	public void moveToPoint(Vector2 point, float time=0.0f) {
		if (_followObject != null) {
			_followObject = null;
		}
		
		if (time == 0.0f) {
			// jump right to the given point
			x = point.x;
			y = point.y;
		} else {
			// tween to the given point
			Go.to(this, time, new TweenConfig().floatProp("x", point.x).floatProp("y", point.y).setEaseType(EaseType.QuadInOut));
		}
	}
	
	// move to an FNode object
	public void moveTo(FNode givenObject, float time=0.0f) 
	{
		if (_followObject != null) {
			_followObject = null;
		}
		
		if (time == 0.0f) {
			// jump right to the given object
			x = givenObject.x;
			y = givenObject.y;
		} else {
			// tween to the given point
			Go.to(this, time, new TweenConfig().floatProp("x", givenObject.x).floatProp("y", givenObject.y).setEaseType(EaseType.QuadInOut));
		}
	}
	
	public void shake(float intensity=1.0f, float duration=1.0f) {
		_shakeDuration = duration;
		_shakeIntensity = intensity;
	}
	
	public void setWorldBounds(Rect givenRect) 
	{
		_worldBounds = givenRect;
	}
	
	public Rect getWorldBounds() {
		return _worldBounds;
	}
	
	public void setBounds(Rect givenRect) 
	{
		_bounds = givenRect;
	}
	
	public bool shakeHUD {
		get { 
			return _shakeIncludeHUD;
		}
		set {
			_shakeIncludeHUD = value;
		}
	}
}
