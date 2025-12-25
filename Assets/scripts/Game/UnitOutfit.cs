using UnityEngine;

public class UnitOutfit : MonoBehaviour
{
    public GameObject ExplodeGameObject;
    public void SpawnExplode()
    {
        Instantiate(ExplodeGameObject, transform.position, transform.rotation);
    }
}
