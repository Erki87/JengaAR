using UnityEngine;

using UnityEngine.Networking;
using UnityEngine.UI;

namespace Isobox

{

    public class IsoboxNetHUD : MonoBehaviour

    {

        public Text IpAddress;

        public string Port;

        public bool _started;



        public void Start()

        {

            _started = false;

        }

        public void StartHost()
        {
            _started = true;

            NetworkManager.singleton.networkPort = int.Parse(Port);

            NetworkManager.singleton.StartHost();
        }

        public void ConnectAsClient()
        {



            _started = true;

            NetworkManager.singleton.networkAddress = IpAddress.text;

            NetworkManager.singleton.networkPort = int.Parse(Port);

            NetworkManager.singleton.StartClient();

        }



        public void Disconnect()

        {

            _started = false;

            NetworkManager.singleton.StopHost();

        }

    }

}
