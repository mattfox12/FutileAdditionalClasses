using UnityEngine;
using System;

public class FAnimation {
	
	protected string _name;
	protected int[] _frames;
	protected int _delay; // in milliseconds
	protected bool _looping;
	
	
	public FAnimation (string givenName, int[] givenFrames, int delayInMilliseconds, bool loop=false) 
	{
		_name = givenName;
		_frames = givenFrames;
		_delay = delayInMilliseconds;
		_looping = loop;
	}
	
	public string name {
		get { return _name; }
	}
	
	public int delay {
		get { return _delay; }
		set { _delay = value; }
	}
	
	public bool looping {
		get { return _looping; }
		set { _looping = value; }
	}
	
	public int[] frames {
		get { return _frames; }
	}
	
	public int totalFrames {
		get { return _frames.Length; }
	}
	
}
