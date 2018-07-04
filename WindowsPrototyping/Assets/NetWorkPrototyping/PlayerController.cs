using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour{

    public GameObject bulletPrefab;
    public Transform bulletSpawn;

	void Update () {

        //zuständig um unseren spieler auf unseren rechner zu kontrollieren

        if (!isLocalPlayer) {
            Debug.Log("notLocal");
            return;
        }
        Debug.Log("Local");

        var x = Input.GetAxis("Horizontal") * Time.deltaTime *  150.0f;
        var z = Input.GetAxis("Vertical") * Time.deltaTime * 3.0f;

        transform.Rotate(0, x, 0);
        transform.Translate(0, 0, z);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            CmdFire();
        }

    }


    //Command bewirkt das der code vom Client ausgeführt wird aber es läuft über den Server
    [Command]
    void CmdFire()
    {

        //Creat bullet from the bullet prefab
        var bullet = (GameObject)Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
        //Add velocity to the bullet
        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * 6;


        //Spawn the bullet on th clients
        NetworkServer.Spawn(bullet);

        //Destroy Bullets after 2 seconds
        Destroy(bullet, 2.0f);
    }

    // nur vom localen player ausgeführt
    public override void OnStartLocalPlayer ()
    {
        GetComponent<MeshRenderer>().material.color = Color.blue;
    }

}
