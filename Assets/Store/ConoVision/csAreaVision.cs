using UnityEngine;
using System.Collections.Generic;

public class csAreaVision : MonoBehaviour {

	public int angulo = 45;
	public int rango  = 5;

	MeshFilter meshFilter;

	Vector3 oldPosition;
	Quaternion oldRotation;
	Vector3 oldScale;

	Mesh Cono(){
		
		Mesh _cono = new Mesh();
		List<Vector3> vertices = new List<Vector3>();
		List<Vector3> normals  = new List<Vector3>();
		List<Vector2> uv       = new List<Vector2>();

		Vector3 oldPosition,temp;
		oldPosition = temp = Vector3.zero;
		
		vertices.Add(Vector3.zero);
		normals.Add(Vector3.up);
		uv.Add(Vector2.one*0.5f);
		
		int w,s;
		
		for(w=0;w<angulo;w++){
			
			for(s=0;s<rango;s++){
				
				temp.x = Mathf.Cos(Mathf.Deg2Rad*w+Mathf.Deg2Rad*(s/rango))*rango;
				temp.z = Mathf.Sin(Mathf.Deg2Rad*w+Mathf.Deg2Rad*(s/rango))*rango;

				if(oldPosition!=temp){

					oldPosition=temp;
					vertices.Add(new Vector3(temp.x,temp.y,temp.z));
					normals.Add(Vector3.up);
					uv.Add(new Vector2((rango+temp.x)/(rango*2),(rango+temp.z)/(rango*2)));

				}

			}
			
		}
		
		int[] triangles = new int[(vertices.Count-2)*3];
		s = 0;
		
		for(w=1;w<(vertices.Count-2);w++){
			
			triangles[s++] = w+1;
			triangles[s++] = w;
			triangles[s++] = 0;
			
		}
		
		_cono.vertices = vertices.ToArray();
		_cono.normals = normals.ToArray();
		_cono.uv = uv.ToArray();
		_cono.triangles = triangles;
		
		return _cono;
		
	}

	Vector3[] initialPosition;
	Vector2[] initialUV;

	// Use this for initialization
	void Start () {

		meshFilter = this.gameObject.GetComponent<MeshFilter>();
		meshFilter.mesh = Cono();
		initialPosition = meshFilter.mesh.vertices;
		initialUV = meshFilter.mesh.uv;
	
	}

	Mesh areaMesh(Mesh mesh){

		Mesh _mesh = new Mesh();

		Vector3[] vertices = new Vector3[mesh.vertices.Length];
		Vector2[] uv       = new Vector2[mesh.uv.Length];

		Vector3 center   = transform.localToWorldMatrix.MultiplyPoint3x4(initialPosition[0]);
		uv[0] = initialUV[0];
		Vector3 worldPoint;

		RaycastHit hit = new RaycastHit();

		for(int i=1;i<vertices.Length;i++){

			worldPoint = transform.localToWorldMatrix.MultiplyPoint3x4(initialPosition[i]);

			if(Physics.Linecast(center,worldPoint, out hit)){

				vertices[i] = transform.worldToLocalMatrix.MultiplyPoint3x4(hit.point);
				uv[i] = new Vector2((rango+vertices[i].x)/(rango*2),(rango+vertices[i].z)/(rango*2));

			} else {

				vertices[i] = initialPosition[i];
				uv[i]       = initialUV[i];

			}

		}

		_mesh.vertices  = vertices;
		_mesh.uv        = uv;
		_mesh.normals   = mesh.normals;
		_mesh.triangles = mesh.triangles;

		return _mesh;

	}
	
	// Update is called once per frame
	void FixedUpdate () {

		if(oldPosition!=transform.position || oldRotation!=transform.rotation || oldScale != transform.localScale){

			oldPosition = transform.position;
			oldRotation = transform.rotation;
			oldScale    = transform.localScale;

			meshFilter.mesh = areaMesh(meshFilter.mesh);

		}
	
	}

}
