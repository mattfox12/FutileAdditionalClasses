using UnityEngine;
using System.Collections;

/* Use this FContainer to make a layer move at a different speed from the FCamObject
This container will move itself based on where the FCamObject is within the worldBounds. FCamObject must have worldBounds set. 
Size refers to a bounding box centered at x,y of 0,0
Examples: 
1. if size is less than the screen, it will always remain centered with the camera -- use for non-moving background
2. if size is larger than the screen & smaller than the worldBounds, it will move slower than the camera, for slow backgrounds
3. if size is larger than the worldBounds, it will move faster than the camera, for foregrounds
*/

public class FParallaxContainer : FContainer {
	
	protected bool _parallaxX = true;
	protected bool _parallaxY = true;
	
	protected Vector2 _size;
	
	private FCamObject _cam;
	
	
	public FParallaxContainer () : base()
	{
		// default to screen size, won't move then
		_size = new Vector2(Futile.screen.width, Futile.screen.height);
		
		ListenForLateUpdate(LateUpdate);
	}
	
	override public void HandleRemovedFromStage()
	{
		_cam = null;
		
		base.HandleRemovedFromStage();
	}
	
	virtual public void LateUpdate() {
		if (_cam != null) {
			Rect bounds = _cam.getWorldBounds();
			if (bounds.width > 0 && bounds.height > 0) {
				if (_parallaxX) {
					if (_size.x > Futile.screen.width) {
						float adjustedWidth = bounds.width - Futile.screen.width;
						float worldPercent = 1 - ((bounds.xMin + _cam.x - Futile.screen.halfWidth) / adjustedWidth);
						
						float screenWidth = _size.x - Futile.screen.width;
						x = _cam.x + screenWidth * worldPercent - screenWidth / 2;
					} else {
						x = _cam.x;
					}
				}
				if (_parallaxY) {
					if (_size.y > Futile.screen.height) {
						float adjustedHeight = bounds.height - Futile.screen.height;
						float worldPercent = 1 - ((-bounds.yMin + _cam.y - Futile.screen.halfHeight) / adjustedHeight);
						
						float screenHeight = _size.y - Futile.screen.height;
						y = _cam.y + screenHeight * worldPercent - screenHeight / 2;
					} else {
						y = _cam.y;
					}
				}
			}
			
		}
	}
	
	public FCamObject camObject {
		get { return _cam; }
		set { _cam = value; }
	}
	
	public Vector2 size {
		get { return _size; }
		set { _size = value; }
	}
	
}
