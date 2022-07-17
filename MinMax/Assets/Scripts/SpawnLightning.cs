using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnLightning : MonoBehaviour
{
    public GameObject lightingGraphic;
    private GameObject spawnedGraphic;

    private Bullet bullet;

    // Start is called before the first frame update
    void Start()
    {
        bullet = GetComponent<Bullet>();
        Vector3 dir = (bullet.targetPosition - bullet.startPosition).normalized;
        float totalDist = (bullet.targetPosition - bullet.startPosition).magnitude;
        float posMod = totalDist / 20;
        Vector3[] lightningKinks = new Vector3[20];
        for(int i=0; i<20; i++)
        {
            Vector3 newPos = bullet.startPosition + (dir * posMod * i);
            newPos = newPos + new Vector3(Random.Range(-20,20), Random.Range(-20, 20), Random.Range(-20, 20));
            lightningKinks[i] = newPos;
        }

        var newLightningGraphic = Instantiate(lightingGraphic, bullet.startPosition, Quaternion.identity, null);
        newLightningGraphic.GetComponent<LineRenderer>().SetPositions(lightningKinks);
        spawnedGraphic = newLightningGraphic;

        //if bullet diff is lees than threshold kill the lightning;
    }

    private IEnumerator KillLighting()
    {
        yield return new WaitForSeconds(.25f);
        Destroy(spawnedGraphic);
    }

}
