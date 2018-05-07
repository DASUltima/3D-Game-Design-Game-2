using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;

    [Header("Player")]
    public GameObject unitMenu;
    public LayerMask playerBaseLayer;
    public List<GameObject> units;
    public Stat playerUnitCooldown;
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
    bool gameEnded;

    public GameObject gameEndMenu;
    public Material playerBaseMaterial;
    public Material playerAuraMaterial;
    public Material enemyBaseMaterial;
    public Material enemyAuraMaterial;
    public Sprite loseSprite;
    public Sprite winSprite;

    // Use this for initialization
    void Start()
    {
        Instance = this;
        lineRenderer = GetComponent<LineRenderer>();
        playerUnitCooldown.Initialize();
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (enemyHomeBase.health.CurrentVal == 0 && !gameEnded)
        {
            gameEnded = true;
            gameEndMenu.SetActive(true);
            gameEndMenu.transform.Find("GameEndText").GetComponent<Text>().text = "You Win!";
            gameEndMenu.GetComponent<Image>().sprite = winSprite;
        }
        else if (playerHomeBase.health.CurrentVal == 0 && !gameEnded)
        {
            gameEnded = true;
            gameEndMenu.SetActive(true);
            gameEndMenu.transform.Find("GameEndText").GetComponent<Text>().text = "You Lose!";
            gameEndMenu.GetComponent<Image>().sprite = loseSprite;
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (playerUnitCooldown.CurrentVal >= playerUnitCooldown.MaxVal)
            {
                GameObject testunit = Instantiate(units[0], currentBase.transform.position, Quaternion.identity);
                testunit.GetComponent<NavMeshAgent>().SetDestination(currentBase.connectingBase[currentPath].transform.position);
                testunit.GetComponent<Unit>().destination = currentBase.connectingBase[currentPath];
                testunit.GetComponent<Unit>().spawnBase = currentBase;
                playerUnitCooldown.CurrentVal = 0;
                playerUnitCooldown.MaxVal = units[0].GetComponent<Unit>().spawnCooldown;
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (playerUnitCooldown.CurrentVal >= playerUnitCooldown.MaxVal)
            {
                GameObject testunit = Instantiate(units[0], currentBase.transform.position, Quaternion.identity);
                testunit.GetComponent<NavMeshAgent>().SetDestination(currentBase.connectingBase[currentPath].transform.position);
                testunit.GetComponent<Unit>().destination = currentBase.connectingBase[currentPath];
                testunit.GetComponent<Unit>().spawnBase = currentBase;
                playerUnitCooldown.CurrentVal = 0;
                playerUnitCooldown.MaxVal = units[0].GetComponent<Unit>().spawnCooldown;
            }
        }
        EnemyAI();
        playerUnitCooldown.CurrentVal += Time.deltaTime;
        for (int buttonIndex = 0; buttonIndex < unitMenu.transform.childCount; buttonIndex++)
        {
            if (playerUnitCooldown.CurrentVal < playerUnitCooldown.MaxVal)
                unitMenu.transform.GetChild(buttonIndex).GetComponent<Button>().interactable = false;
            else if (playerUnitCooldown.CurrentVal >= playerUnitCooldown.MaxVal)
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
                    if (currentBase)
                        currentBase.selector.SetActive(false);
                    currentBase = hit.collider.GetComponent<Base>();
                    currentBase.selector.SetActive(true);
                    destinationBase = currentBase.connectingBase[0];
                    lineRenderer.enabled = true;
                    DrawPathBetweenBases();
                }
                else if (!Physics.Raycast(ray, out hit, 100, playerBaseLayer))
                {
                    unitMenu.SetActive(false);
                    if (currentBase)
                        currentBase.selector.SetActive(false);
                    currentBase = null;
                    destinationBase = null;
                    currentPath = 0;
                    lineRenderer.enabled = false;
                }
            }
        }
        if (currentBase)
        {
            if (Input.GetKeyDown(KeyCode.Q))
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
            if (Input.GetKeyDown(KeyCode.E))
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
    public void ButtonMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void ButtonSpawnUnit()
    {
        int unitIndex = EventSystem.current.currentSelectedGameObject.transform.GetSiblingIndex();
        GameObject testunit = Instantiate(units[unitIndex], currentBase.transform.position, Quaternion.identity);
        testunit.GetComponent<NavMeshAgent>().SetDestination(currentBase.connectingBase[currentPath].transform.position);
        testunit.GetComponent<Unit>().destination = currentBase.connectingBase[currentPath];
        testunit.GetComponent<Unit>().spawnBase = currentBase;
        playerUnitCooldown.CurrentVal = 0;
        playerUnitCooldown.MaxVal = units[0].GetComponent<Unit>().spawnCooldown;
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
                if (enemyBases[currentBase] && enemyBases[currentBase].connectingBase[pathIndex] && enemyBases[currentBase].connectingBase[pathIndex].faction == Faction.None || enemyBases[currentBase].connectingBase[pathIndex].faction == Faction.Player)
                    possibleTargets.Add(enemyBases[currentBase].connectingBase[pathIndex]);

            int target = Random.Range(0, possibleTargets.Count);
            destinationBase = possibleTargets[target];

            GameObject testunit = Instantiate(enemyUnits[currentUnit], enemyBases[currentBase].transform.position, Quaternion.identity);
            testunit.GetComponent<NavMeshAgent>().SetDestination(destinationBase.transform.position);
            testunit.GetComponent<Unit>().spawnBase = enemyBases[currentBase];
            testunit.GetComponent<Unit>().destination = destinationBase;
            enemyUnitCooldown = enemyUnits[currentUnit].GetComponent<Unit>().spawnCooldown;
        }
    }
}