using UnityEngine;
using System.Collections;

public class TmxExample : MonoBehaviour {
	
	protected float _time = 0.0f;
	
	protected FAnimatedSprite burglar;
	
	// Use this for initialization
	void Start () {
		FutileParams fparams = new FutileParams(true, true, false, false);
		fparams.AddResolutionLevel(480.0f, 2.0f, 1.0f, "");
		fparams.origin = new Vector2(0.5f, 0.5f);
		fparams.backgroundColor = new Color(0.15f, 0.15f, 0.3f);
		Futile.instance.Init(fparams);
		
		// load image atlas (within Resources/Atlases folder)
		Futile.atlasManager.LoadAtlas("Atlases/Burglar");
		
		// Add tilemap 
		FTmxMap tmx1 = new FTmxMap();
		tmx1.LoadTMX("CSVs/testTmx"); // load tmx text file (within Resources/CSVs folder)
		Futile.stage.AddChild(tmx1);
		
		// create burglar
		burglar = new FAnimatedSprite("Burglar");
		burglar.y = -44;
		int[] frames = { 1,1,2,1,1,1,10,1,11,1 }; // idle anim
		burglar.addAnimation(new FAnimation("idle", frames, 400, true));
		int[] frames2 = { 3,4,5,6,4,7 }; // run anim
		burglar.addAnimation(new FAnimation("run", frames2, 180, true));
		Futile.stage.AddChild(burglar); // add burglar to stage
		
		// load font atlas
		Futile.atlasManager.LoadAtlas("Atlases/Fonts");
		
		// Add large font text
		Futile.atlasManager.LoadFont("Large", "Large Font.png", "Atlases/Large Font");
		FLabel label1 = new FLabel("Large", "LARGE FONT");
		label1.y = 26;
		Futile.stage.AddChild(label1);
		
		// Add small font text
		Futile.atlasManager.LoadFont("Small", "Small Font.png", "Atlases/Small Font");
		FLabel label2 = new FLabel("Small", "Small Font");
		label2.y = 12;
		Futile.stage.AddChild(label2);
		
		// Add tiny font text
		Futile.atlasManager.LoadFont("Tiny", "Tiny Font.png", "Atlases/Tiny Font");
		FLabel label3 = new FLabel("Tiny", "Tiny Font");
		label3.y = 3;
		Futile.stage.AddChild(label3);
	}
	
	// Update is called once per frame
	void Update () {
		
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
