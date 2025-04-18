using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hitEffectDeleteScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(deleteEffect());
    }

    IEnumerator deleteEffect(){
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
}
