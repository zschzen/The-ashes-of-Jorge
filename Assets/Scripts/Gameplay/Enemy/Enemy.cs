﻿using UnityEngine;

public class Enemy : CharacterBase {
    #region "Vars"
    protected float walkTimer;
    public float stopDistance = 1f;
    protected int lastDamage;

    protected float forcaZ, forcaH;

    private int num;
    protected Transform Target => GameObject.FindGameObjectsWithTag("EnemyPoints")[num].transform;
    public Vector3 PointDistance => Target.position - transform.position;
    public Vector3 PlayerDistance => player.transform.position - transform.position;


    protected GameObject HB;
    private bool HB_show;
    protected int posLife;

    public bool Attacking;

    private void HealthBar() {
        if (HB_show) { return; }
        HB_show = true;
        MountHealthBar();
    }
    #endregion

    #region "Code"
    private void Start() {
        num = Random.Range(0, 2);

        //GetComponentInChildren<HealthBar>().MaxHealthPoints = maxHealth;
        //GetComponentInChildren<HealthBar>().Hurt(0);

        maxHealth = 3;
        //nextAttack = 3.5f;
        currentHealth = maxHealth;
    }

    private void Update() {
        AirUpdate();
    }

    private void FixedUpdate() {
        if (isDead) { return; }

        BasicMove();

        if (!Attacking) { return; }

        BasicAttack();
    }

    private void OnCollisionEnter(Collision other) {
        if (!isDead || !other.gameObject.name.Equals("Ground")) { return; }

        GetComponent<CapsuleCollider>().enabled = false;
        rb.isKinematic = true;

        //if (posLife >= 2) { anim.SetTrigger("Destroy"); return; }

        string triggerName = Random.value <= .2f ? "Destroy" : "Revive";

        anim.SetTrigger(triggerName);
    }

    protected override void OnHit(int damage) {
        HealthBar();
        HB.GetComponentInChildren<HealthBar>().Hurt(damage);
        walkTimer = 0;
    }
    //protected override void OnRecover(int damage) { GetComponentInChildren<HealthBar>().Recover(damage); }
    #endregion

    #region "Animation events"
    public void Recover() { return; } //damaged = damaged; }

    public void Destroy() { Destroy(gameObject, Random.Range(1f, 3f)); Destroy(HB, .525f); }

    public void Revive() {
        posLife++;
        GetComponent<CapsuleCollider>().enabled = true;
        currentSpeed = maxSpeed;
        currentHealth = maxHealth;
        isDead = false;
        Destroy(HB);
        rb.isKinematic = false;

        MountHealthBar();
    }
    #endregion

    #region "HealthBar releted"
    public void ShowLifeBar() {
        if (HB) { return; }
        HB.SetActive(true);
    }

    protected void MountHealthBar() {
        GameObject prefab = (GameObject)Resources.Load("Prefab/HealthBar");
        Transform WS = GameObject.Find("-- World Space").transform;

        HB = Instantiate(prefab, WS);
        HB.transform.SetParent(WS, true);

        HB.GetComponentInChildren<HealthBar>().MaxHealthPoints = maxHealth;
        HB.transform.position = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, 3, 0));
    }
    #endregion

    #region "Meta"
    protected virtual void BasicMove() {
        if (!Attacking) {
            if (PlayerDistance.sqrMagnitude < 100 && walkTimer > Random.Range(1f, 2f)) {
                forcaZ = Random.Range(-1, 2);
                //forcaH = Random.Range(-.5f, 1.5f);
                walkTimer = 0;

            } else if (PlayerDistance.sqrMagnitude >= 100) {
                forcaH = PointDistance.x / Mathf.Abs(PointDistance.x);
            }
        } else {
            forcaH = PointDistance.x / Mathf.Abs(PointDistance.x);
            forcaZ = PointDistance.z / Mathf.Abs(PointDistance.z);

        }

        if (Mathf.Abs(PointDistance.x) <= stopDistance) { forcaH = 0; }

        MoveHandler(forcaH, forcaZ);
    }

    protected virtual void BasicAttack() {
        bool attack = Mathf.Abs(PointDistance.x) < 1.5f && Mathf.Abs(PointDistance.z) < 1f;
        if (attack && Time.time > nextAttack) {
            Attack();
        }
    }

    protected virtual void AirUpdate() {
        walkTimer += Time.deltaTime;
        anim.SetBool("isDead", isDead);

        if (!HB) { return; }
        HB.transform.position = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, 3, 0));
    }
    #endregion
}