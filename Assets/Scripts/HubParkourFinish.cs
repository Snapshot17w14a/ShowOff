using UnityEngine;
using UnityEngine.VFX;

public class HubParkourFinish : MonoBehaviour
{
    [SerializeField] private GameObject LoloObject;
    [SerializeField] private VisualEffect partyParticle;
    private bool isPlayerOnMinecart;
    private int curPlayers;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            curPlayers++;
            isPlayerOnMinecart = true;
            Invoke("ShowLolo", 1);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            curPlayers--;

            if (curPlayers == 0)
            {
                isPlayerOnMinecart = false;
                LoloObject.SetActive(false);
                partyParticle.Stop();
            }
        }
    }

    private void ShowLolo()
    {
        if (isPlayerOnMinecart)
        {
            partyParticle.Play();
            LoloObject.SetActive(true);
        }
    }
}
