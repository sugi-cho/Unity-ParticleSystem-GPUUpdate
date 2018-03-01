using System.Runtime.InteropServices;
using UnityEngine;

public class MeshDataTest : MonoBehaviour {

    public Material bufferWriter;
    Mesh targetMesh;

    ComputeBuffer vDataBuffer;
    [Header("for debug")]
    [SerializeField] vData[] dataArray;

	// Use this for initialization
	void Start () {
        targetMesh = GetComponent<MeshFilter>().sharedMesh;
        var vCount = targetMesh.vertexCount;
        dataArray = new vData[vCount];
        vDataBuffer = new ComputeBuffer(vCount, Marshal.SizeOf(typeof(vData)));
	}

    private void OnDestroy()
    {
        vDataBuffer.Dispose();
    }

    // Update is called once per frame
    void Update () {
        Graphics.SetRandomWriteTarget(1, vDataBuffer);
        bufferWriter.SetPass(0);
        Graphics.DrawMeshNow(targetMesh, transform.localToWorldMatrix);
        Graphics.ClearRandomWriteTargets();

        vDataBuffer.GetData(dataArray);
	}

    [System.Serializable]
    public struct vData
    {
        public Vector3 pos;
        public Vector3 normal;
        public Vector2 uv;
    }
}
