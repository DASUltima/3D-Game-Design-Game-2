using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour {

    public Faction faction;
    public float speed;
    public float spawnCooldown;

    public Unit target;
    public int attackDamage;
    public float attackCooldown;
    float nextAttack;
    public Stat health;
    public LayerMask canSee;
    public float sightRange;
    RaycastHit hit;
    public Base destination;
    public Base spawnBase;

    void Start()
    {
        health.Initialize();
        GetComponent<NavMeshAgent>().speed = speed;
    }
    private void Update()
    {
        nextAttack += Time.deltaTime;
        Ray ray = new Ray(transform.position, transform.forward);

        if (Physics.Raycast(ray, out hit, sightRange, canSee))
        {
            if (faction == Faction.Player)
            {
                if (hit.collider.GetComponent<Unit>() && hit.collider.GetComponent<Unit>().faction == Faction.Enemy)
                {
                    GetComponent<NavMeshAgent>().enabled = false;
                    if (hit.collider.GetComponent<Unit>() && nextAttack >= attackCooldown)
                    {
                        hit.collider.GetComponent<Unit>().TakeDamage(attackDamage);
                        transform.Find("Model").GetComponent<Animator>().SetTrigger("Attack");
                        nextAttack = 0;
                    }
                }
                else if (hit.collider.GetComponent<Base>() && hit.collider.GetComponent<Base>().faction == Faction.Enemy)
                {
                    if (hit.collider.GetComponent<Base>().health.CurrentVal > 0)
                        GetComponent<NavMeshAgent>().enabled = false;
                    else if (hit.collider.GetComponent<Base>().health.CurrentVal <= 0)
                    {
                        GetComponent<NavMeshAgent>().enabled = true;
                        GetComponent<NavMeshAgent>().SetDestination(destination.transform.position);
                    }
                    if (hit.collider.GetComponent<Base>().health.CurrentVal > 0 && nextAttack >= attackCooldown)
                    {
                        hit.collider.GetComponent<Base>().TakeDamage(attackDamage);
                        transform.Find("Model").GetComponent<Animator>().SetTrigger("Attack");
                        nextAttack = 0;
                    }
                }
            }
            else if (faction == Faction.Enemy)
            {
                if (hit.collider.GetComponent<Unit>() && hit.collider.GetComponent<Unit>().faction == Faction.Player)
                {
                    GetComponent<NavMeshAgent>().enabled = false;
                    if (hit.collider.GetComponent<Unit>() && nextAttack >= attackCooldown)
                    {
                        hit.collider.GetComponent<Unit>().TakeDamage(attackDamage);
                        transform.Find("Model").GetComponent<Animator>().SetTrigger("Attack");
                        nextAttack = 0;
                    }
                }
                else if (hit.collider.GetComponent<Base>() && hit.collider.GetComponent<Base>().faction == Faction.Player)
                {
                    if (hit.collider.GetComponent<Base>().health.CurrentVal > 0)
                        GetComponent<NavMeshAgent>().enabled = false;
                    else if (hit.collider.GetComponent<Base>().health.CurrentVal <= 0)
                    {
                        GetComponent<NavMeshAgent>().enabled = true;
                        GetComponent<NavMeshAgent>().SetDestination(destination.transform.position);
                    }
                    if (hit.collider.GetComponent<Base>().health.CurrentVal > 0 && nextAttack >= attackCooldown)
                    {
                        hit.collider.GetComponent<Base>().TakeDamage(attackDamage);
                        transform.Find("Model").GetComponent<Animator>().SetTrigger("Attack");
                        nextAttack = 0;
                    }
                }
            }
        }
        else if (!Physics.Raycast(ray, out hit, sightRange, canSee))
        {
            if (!GetComponent<NavMeshAgent>().enabled)
            {
                GetComponent<NavMeshAgent>().enabled = true;
                if (destination)
                    GetComponent<NavMeshAgent>().SetDestination(destination.transform.position);
            }
        }
        Debug.DrawRay(ray.origin, ray.direction, Color.red);
    }
    public void TakeDamage(int damage)
    {
        if (damage >= health.CurrentVal)
        {
            // Unit die
            Destroy(gameObject);
        }
        else if (damage < health.CurrentVal)
        {
            health.CurrentVal -= damage;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (faction == Faction.Player)
        {
            if (other.CompareTag("Base"))
            {
                if (other.GetComponent<Base>().faction == Faction.Enemy || other.GetComponent<Base>().faction == Faction.None)
                {
                    other.GetComponent<Base>().meshRenderer.material = GameManager.Instance.playerBaseMaterial;
                    other.GetComponent<Base>().auraRenderer.material = GameManager.Instance.playerAuraMaterial;
                    other.GetComponent<Base>().faction = Faction.Player;
                    other.gameObject.layer = 8;
                    if (GameManager.Instance.enemyBases.Contains(other.GetComponent<Base>()))
                        GameManager.Instance.enemyBases.Remove(other.GetComponent<Base>());
                    other.GetComponent<Base>().health.CurrentVal = other.GetComponent<Base>().health.MaxVal;
                    Destroy(gameObject);
                }
                else if (other.GetComponent<Base>().faction == Faction.Player && other.GetComponent<Base>() != spawnBase)
                {
                    Destroy(gameObject);
                    GameManager.Instance.playerUnitCooldown = 0;
                }
            }
        }
        else if (faction == Faction.Enemy)
        {
            if (other.CompareTag("Base"))
            {
                if (other.GetComponent<Base>().faction == Faction.Player || other.GetComponent<Base>().faction == Faction.None)
                {
                    other.GetComponent<Base>().meshRenderer.material = GameManager.Instance.enemyBaseMaterial;
                    other.GetComponent<Base>().auraRenderer.material = GameManager.Instance.enemyAuraMaterial;
                    other.GetComponent<Base>().faction = Faction.Enemy;
                    other.gameObject.layer = 9;
                    GameManager.Instance.enemyBases.Add(other.GetComponent<Base>());
                    other.GetComponent<Base>().health.CurrentVal = other.GetComponent<Base>().health.MaxVal;
                    Destroy(gameObject);
                }
                else if (other.GetComponent<Base>().faction == Faction.Enemy && other.GetComponent<Base>() != spawnBase)
                {
                    Destroy(gameObject);
                    GameManager.Instance.enemyUnitCooldown = 0;
                }
            }
        }
    }
}
