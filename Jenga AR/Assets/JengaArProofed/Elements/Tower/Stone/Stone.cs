using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stone : MonoBehaviour {

    private Transform ghostStone;
    public Material unselected;
    public Material selected;
    private Renderer render;

    private void Awake()
    {
        GetComponent<Rigidbody>().Sleep();
    }
    // Use this for initialization
    void Start () {
        render = GetComponent<Renderer>();
	}

	// Update is called once per frame
	void Update () {
        if (ghostStone == null) return;

        if (Vector3.Distance(transform.position, ghostStone.position) > transform.lossyScale.y * 0.2f)
        {
            transform.position =
                Vector3.Lerp(transform.position, ghostStone.position, 0.8f);
            transform.rotation =
                Quaternion.Lerp(transform.rotation, ghostStone.rotation, 0.8f);
        }
        else
            transform.position = ghostStone.position;
        transform.rotation = ghostStone.rotation;

    }

    public void ActivateGravity(Transform ghostStone)
    {

        this.ghostStone = ghostStone;

        if (ghostStone == null)
        {
            GetComponent<Rigidbody>().useGravity = true;
            render.material = unselected;
        }
        else
        {
            GetComponent<Rigidbody>().useGravity = false;
            render.material = selected;
        }
    }
}
