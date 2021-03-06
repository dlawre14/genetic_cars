﻿using UnityEngine;
using System.Collections;
using Vectrosity;

public class FloorMake : MonoBehaviour {

	public float difficulty;
	public PhysicsMaterial2D mat;

	// Use this for initialization
	void Start () {
		Vector3[] p = new Vector3[10000];
		VectorLine myline = new VectorLine("Floor", p, null, 5.0f, LineType.Continuous, Joins.Weld);

		Vector3[] points = new Vector3[500];
		Vector3 prev = new Vector3(-10,0,0);
		for (int i = 0; i<500; i++) {
		  if (i < 20)
		  	prev += new Vector3(1, 0, 0);
		  else
		    prev += new Vector3(1, Random.Range(-difficulty, difficulty), 0);

		  points[i] = prev;
		}

		myline.MakeSpline(points);
		myline.SetColor (Color.red);
		
		myline.physicsMaterial = mat;
		myline.collider = true;
		myline.Draw3D ();
	}

	// Update is called once per frame
	void Update () {

	}
}
