using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.AI;

public class GameManager : MonoBehaviour {

    public static GameManager Instance;

    [Header("Player")]
    public GameObject unitMenu;
    public LayerMask playerBaseLayer;
    public List<GameObject> units;
    public float playerUnitCooldown;
    public Base currentBase;
    public Base playerHomeBase;

    public Base destinationBase;

    LineRenderer lineRenderer;
    int currentPath;

    [Header("Enemy")]
    public Base enemyHomeBase;
    public List<GameObject> enemyUnits;
    public float enemyUnitCooldown;
    public List<Base> enemyBases;

    public GameObject gameEndMenu;

    // Use this for initialization
    void Start () {
        Instance = this;
        lineRenderer = GetComponent<LineRenderer>();
	}

    // Update is called once per frame
    void Update()
    {
        EnemyAI();
        playerUnitCooldown -= Time.deltaTime;
        for (int buttonIndex = 0; buttonIndex < unitMenu.transform.childCount; buttonIndex++)
        {
            if (playerUnitCooldown > 0)
                unitMenu.transform.GetChild(buttonIndex).GetComponent<Button>().interactable = false;
            else if (playerUnitCooldown < 0)
                unitMenu.transform.GetChild(buttonIndex).GetComponent<Button>().interactable = true;
        }
        if (Input.GetMouseButtonDown(0))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                // Shoot out a ray
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                // If we hit
                if (Physics.Raycast(ray, out hit, 100, playerBaseLayer))
                {
                    unitMenu.SetActive(true);
                    currentBase = hit.collider.GetComponent<Base>();
                    destinationBase = currentBase.connectingBase[0];
                    lineRenderer.enabled = true;
                    DrawPathBetweenBases();
                }
                else if (!Physics.Raycast(ray, out hit, 100, playerBaseLayer))
                {
                    unitMenu.SetActive(false);
                    currentBase = null;
                    destinationBase = null;
                    currentPath = 0;
                    lineRenderer.enabled = false;
                }
            }
        }
        if (currentBase)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (currentPath > 0)
                {
                    currentPath--;
                    destinationBase = currentBase.connectingBase[currentPath];
                }
                else
                {
                    destinationBase = currentBase.connectingBase[currentBase.connectingBase.Count - 1];
                    currentPath = currentBase.connectingBase.Count - 1;
                }
                DrawPathBetweenBases();
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (currentPath + 1 < currentBase.connectingBase.Count)
                {
                    currentPath++;
                    destinationBase = currentBase.connectingBase[currentPath];
                }
                else
                {
                    destinationBase = currentBase.connectingBase[0];
                    currentPath = 0;
                }
                DrawPathBetweenBases();
            }
        }
    }
    public void ButtonSpawnUnit()
    {
        for (int unitIndex = 0; unitIndex < units.Count; unitIndex++)
        {
            GameObject testunit = Instantiate(units[unitIndex], currentBase.transform.position, Quaternion.identity);
            testunit.GetComponent<NavMeshAgent>().SetDestination(currentBase.connectingBase[currentPath].transform.position);
            testunit.GetComponent<Unit>().destination = currentBase.connectingBase[currentPath];
            playerUnitCooldown = units[unitIndex].GetComponent<Unit>().spawnCooldown;
        }
    }
    void DrawPathBetweenBases()
    {
        lineRenderer.SetPosition(0, currentBase.transform.position);
        lineRenderer.SetPosition(1, destinationBase.transform.position);
    }
    void EnemyAI()
    {
        enemyUnitCooldown -= Time.deltaTime;
        if (enemyUnitCooldown < 0)
        {
            int currentBase = Random.Range(0, enemyBases.Count);
            int currentUnit = Random.Range(0, enemyUnits.Count);
            Base destinationBase = null;
            List<Base> possibleTargets = new List<Base>();
            for (int pathIndex = 0; pathIndex < enemyBases[currentBase].connectingBase.Count; pathIndex++)
                if (enemyBases[currentBase].connectingBase[pathIndex].faction == Faction.None || enemyBases[currentBase].connectingBase[pathIndex].faction == Faction.Player)
                    possibleTargets.Add(enemyBases[currentBase].connectingBase[pathIndex]);

            int target = Random.Range(0, possibleTargets.Count);
            destinationBase = possibleTargets[target];

            GameObject testunit = Instantiate(enemyUnits[currentUnit], enemyBases[currentBase].transform.position, Quaternion.identity);
            testunit.GetComponent<NavMeshAgent>().SetDestination(destinationBase.transform.position);
            testunit.GetComponent<Unit>().destination = destinationBase;
            enemyUnitCooldown = enemyUnits[currentUnit].GetComponent<Unit>().spawnCooldown;
        }
    }
}
