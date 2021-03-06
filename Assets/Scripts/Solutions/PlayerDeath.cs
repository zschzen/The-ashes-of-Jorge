﻿using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDeath : MonoBehaviour {
    public GameObject pd, ak; // player death, press (any key)
    private bool IsDissolved;
    private Vector3 center;
    private Vector3 Distance => center - pd.transform.position;

    private void Awake() => center = new Vector3(Screen.width * .5f, Screen.height * .5f, 0);

    private void Update() {
        if (IsDissolved) { return; }

        // reached barely the center? if so, do some stuff and return
        if (Mathf.Abs(Distance.x) < 1f) {
            if (!ak.activeInHierarchy) { ak.SetActive(true); } 
            else if (Input.anyKey && pd) { InstanceDissolve(); }

            return;
        }

        Vector3 temp = pd.transform.position;
        temp.x += (Distance.x / Mathf.Abs(Distance.x)) * 2.5f;

        pd.transform.position = temp;
    }

    private void InstanceDissolve() {
        IsDissolved = true;

        GameObject c = Instantiate(Resources.Load<GameObject>("Prefab/PlayerAshes"));
        c.transform.SetParent(GameObject.Find("GameOver").transform, false);

        c.transform.position = pd.transform.position + new Vector3(0, 40f, 0);
        c.transform.rotation = pd.transform.rotation;

        Destroy(pd, .2f);

        StartCoroutine(Wait());
    }

    private IEnumerator Load() {
        AsyncOperation op = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);

        do { yield return null; } while (!op.isDone);
    }

    private IEnumerator Wait() {
        yield return new WaitForSeconds(2f);
        StartCoroutine(Load());
    }
}