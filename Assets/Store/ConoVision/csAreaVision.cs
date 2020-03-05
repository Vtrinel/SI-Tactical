using System.Collections.Generic;
using UnityEngine;

public class csAreaVision : MonoBehaviour {

    [SerializeField] BasicEnemy myBasicEnemy;

	float angle = 45;
    float range  = 5;

	MeshFilter meshFilter;

	Vector3 oldPosition;
	Quaternion oldRotation;
	Vector3 oldScale;

    MeshCollider myMeshCollider;

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
		
		for(w=0;w<angle;w++){
			
			for(s=0;s<range;s++){
				
				temp.x = Mathf.Cos(Mathf.Deg2Rad*w+Mathf.Deg2Rad*(s/range))*range;
				temp.z = Mathf.Sin(Mathf.Deg2Rad*w+Mathf.Deg2Rad*(s/range))*range;

				if(oldPosition!=temp){

					oldPosition=temp;
					vertices.Add(new Vector3(temp.x,temp.y,temp.z));
					normals.Add(Vector3.up);
					uv.Add(new Vector2((range+temp.x)/(range*2),(range+temp.z)/(range*2)));

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

        angle = myBasicEnemy.angleAttack;
        range = myBasicEnemy.attackRange;


        myMeshCollider = gameObject.GetComponent<MeshCollider>();

        meshFilter = gameObject.GetComponent<MeshFilter>();
		meshFilter.mesh = Cono();
		initialPosition = meshFilter.mesh.vertices;
		initialUV = meshFilter.mesh.uv;

        transform.parent.eulerAngles = new Vector3(transform.parent.eulerAngles.x, transform.parent.eulerAngles.y + angle / 2, transform.parent.eulerAngles.z);

        if(myMeshCollider != null)
        {
            myMeshCollider.sharedMesh = meshFilter.mesh;
        }
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
				uv[i] = new Vector2((range+vertices[i].x)/(range*2),(range+vertices[i].z)/(range*2));

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
}
