using UnityEngine;
using System.Collections;

public class SuperBurgleGame : MonoBehaviour {
	
	protected float _time = 0.0f;
	
	protected FAnimatedSprite burglar;
	
	// Use this for initialization
	void Start () {
		FutileParams fparams = new FutileParams(true, true, false, false);
		
		fparams.AddResolutionLevel(480.0f, 1.0f, 1.0f, "");
		
		fparams.origin = new Vector2(0.5f, 0.5f);
		
		fparams.backgroundColor = new Color(0, 0, 0.2f);
		
		Futile.instance.Init(fparams);
		
		Futile.atlasManager.LoadAtlas("Atlases/Burglar");
		
		burglar = new FAnimatedSprite("Burglar");
		burglar.scale = 2.0f;
		// idle anim
		int[] frames = { 1,1,2,1,1,1,10,1,11,1 };
		burglar.addAnimation(new FAnimation("idle", frames, 400, true));
		// run anim
		int[] frames2 = { 3,4,5,6,4,7 };
		burglar.addAnimation(new FAnimation("run", frames2, 180, true));
		
		Futile.stage.AddChild(burglar);
		
	}
	
	// Update is called once per frame
	void Update () {
		burglar.Update();
		
		_time += Time.deltaTime;
		
		if (_time > 5.0f) {
			if (burglar.currentAnim.name == "run") {
				burglar.play ("idle");
			} else {
				burglar.play ("run");
			}
			
			_time -= 5.0f;
		}
	}
	
}
