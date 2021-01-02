using eDmitriyAssets.NavmeshLinksGenerator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshMakerTool : ATool
{
    public GameObject NavMeshGeneratorPref;

    private GameObject _lastGenerator = null;

    public override void Reload(){}

    public override void Reset(bool isRelease){}

    public override void Scroll(float delta){}

    public override void Shoot(Ray ray, bool isRelease = false, bool isRightClick = false)
    {
        if (isRelease || isRightClick)
            return;

        if(Physics.Raycast(ray, out RaycastHit hit))
        {
            new List<GravityObject>(FindObjectsOfType<GravityObject>()).ForEach(g => g.SetLocalGravity(-hit.normal * 9.81f));
            new List<BaseAI>(FindObjectsOfType<BaseAI>()).ForEach(ba => { ba.GravityChange(); });
            if (_lastGenerator)
                Destroy(_lastGenerator);
            _lastGenerator = Instantiate(NavMeshGeneratorPref);
            _lastGenerator.transform.position = hit.point;
            _lastGenerator.transform.up = hit.normal;
            _lastGenerator.GetComponent<NavMeshSurface>().BuildNavMesh();
            _lastGenerator.GetComponent<NavMeshLinks_AutoPlacer>().Generate();
        }
    }

}
