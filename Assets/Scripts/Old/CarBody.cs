﻿using UnityEngine;
using System.Collections;
using Vectrosity;

public class CarBody : MonoBehaviour {

	VectorLine myline;
	VectorLine polyline;
	
	public int line_thickness;
	public bool finished = false;
	private bool killable = false;
	private bool killed = false;
	
	private static bool Less (Vector2 a, Vector2 b, Vector2 center) {
		if (a.x - center.x >= 0 && b.x - center.x <= 0)
			return true;
		if (a.x - center.x < 0 && b.x - center.x >= 0)
			return false;
		if (a.x - center.x == 0 && b.x - center.x == 0) {
			if (a.y - center.y >= 0 || b.y - center.y >= 0)
				return a.y  > b.y;
			return b.y > a.y;
		}

		float det = (a.x - center.x) * (b.y - center.y) - (b.x - center.x) * (a.y - center.y);
		if (det < 0)
			return true;
		if (det > 0)
			return false;

		float d1 = (a.x - center.x) * (a.x - center.x) + (a.y - center.y) * (a.y - center.y);
		float d2 = (b.x - center.x) * (b.x - center.x) + (b.y - center.y) * (b.y - center.y); 
		return d1 > d2;
	}

	public static Vector2[] CircleSort(Vector2[] list) {
		Vector2[] output = (Vector2[]) list.Clone();

		//Calculate center
		float x = 0.0f;
		float y = 0.0f;
		for (int i = 0; i<list.Length; i++) {
			x += list[i].x;
			y += list[i].y;
		}

		Vector2 center = new Vector2 (x/list.Length, y/list.Length);

		//Bubble because lazy...

		bool swapped = true;
		while (swapped) {
			swapped = false;
			for (int i = 0; i<list.Length-1; i++) {
				if (!Less (output[i], output[i+1], center)) {
					Vector2 temp = output[i+1];
					output[i+1] = output[i];
					output[i] = temp;
					swapped = true;
				}
			}
		}

		return output;
	}

	public static Vector2[] RandomPolygon(int verts, float scale_x = 1.0f, float scale_y = 1.0f) {
		Vector2[] points = new Vector2[verts];
		for (int i = 0; i<verts; i++) {
			points[i] = new Vector2(Random.Range (-scale_x, scale_x), Random.Range (-scale_y, scale_y));
		}

		return CircleSort (points);
	}
	
	public static Vector2[] PolyFromGenome (string genome) {
		Vector2[] points = new Vector2[genome.Length/8];
		
		Debug.Log (genome);
		Debug.Log (genome.Length);
		
		for (int i = 0; i<genome.Length/4; i+=2) {
			string x_gene = genome.Substring(i*4, 4);
			string y_gene = genome.Substring((i+1)*4, 4);
			
			Debug.Log ("x_gene is: " + x_gene);
			Debug.Log ("y_gene is: " + y_gene);
		}
		
		return points;
	}

	public void SetColor (Color c) {
		myline.SetColor (c);
		polyline.SetColor (c);
	}

	// Use this for initialization
	void Start () {
		//Drawing Portion
		Vector2[] points = GetComponent<PolygonCollider2D> ().points;

		float x = 0.0f;
		float y = 0.0f;
		for (int i = 0; i<points.Length; i++) {
			x += points[i].x;
			y += points[i].y;
		}
		
		Vector2 center = new Vector2 (x/points.Length, y/points.Length);

		Vector3[] points3 = new Vector3[points.Length];

		for (int i = 0; i<points.Length; i++)
			points3 [i] = (Vector3)points [i];

		Vector3[] p = new Vector3[points.Length + 1];
		myline = new VectorLine ("CarBody", p, null, line_thickness, LineType.Continuous, Joins.Weld);

		myline.MakeSpline (points3, points.Length, true);

		myline.SetColor (Color.blue);
		myline.drawTransform = transform;
		myline.Draw3D ();

		Vector3[] p2 = new Vector3[points.Length*2];

		for (int i = 0; i<points.Length*2; i+=2) {
			p2[i] = center;
			p2[i+1] = points[i/2];
		}

		polyline = new VectorLine ("BodyLines", p2, null, line_thickness/2.0f, LineType.Discrete, Joins.Weld);

		polyline.SetColor (Color.blue);
		polyline.drawTransform = transform;
		polyline.Draw3D ();

		GetComponent<Rigidbody2D> ().centerOfMass = center;
		
		finished = true;
	}

	// Update is called once per frame
	void Update () {
		if (!killable && GetComponent<Rigidbody2D> ().velocity.magnitude < 0.01) {
			killable = true;
			StartCoroutine (KillScheduled ());
		} else if (GetComponent<Rigidbody2D> ().velocity.magnitude > 0.01) {
			killable = false;
		}

		if (killed) {
			myline.active = false;
			polyline.active = false;
			foreach (DrawWheel w in GetComponentsInChildren<DrawWheel>())
				w.Deactivate();
			Destroy (gameObject);
		}
	}

	void LateUpdate() {
		myline.Draw3D ();
		polyline.Draw3D ();
	}

	public bool IsAlive() {
		return !killed;
	}

	IEnumerator KillScheduled() {
		yield return new WaitForSeconds(3.0f);
		if (killable)
			killed = true;
	}
}
