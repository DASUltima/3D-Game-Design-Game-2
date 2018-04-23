using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Stat
{
	public BarScript bar;
	[SerializeField]
	private float maxVal;
	[SerializeField]
	private float currentVal;
	public float CurrentVal {
		get {
			return currentVal;}
		set {
			this.currentVal = Mathf.Clamp(value, 0, MaxVal);
            if (bar)
			    bar.Value = currentVal;}
	}
	public float MaxVal {
		get {
			return maxVal;
		}
		set {
			this.maxVal = value;
            if (bar)
			    bar.MaxValue = maxVal;
		}
	}

    public void Initialize()
    {
        if (bar != null)
        {
            this.MaxVal = maxVal;
            this.CurrentVal = currentVal;
        }
    }
}
