using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoubleTap : MonoBehaviour {
 
    int TapCount;
    public float MaxDubbleTapTime;
    float NewTime;
    public AudioSource iniciar;
 
    void Start () {
        TapCount = 0;
        StartCoroutine(LembrarComoAbreTela());
    }

    IEnumerator LembrarComoAbreTela()
    {
        while (true)
        {
            yield return new WaitForSeconds(8);
            iniciar.Play();
        }
    }

    void Update () {
        if (Input.touchCount == 1) {
            Touch touch = Input.GetTouch (0);
            
            if (touch.phase == TouchPhase.Ended) {
                TapCount += 1;
            }

            if (TapCount == 1) {
                NewTime = Time.time + MaxDubbleTapTime;
            }else if(TapCount == 2 && Time.time <= NewTime){
                
                //Whatever you want after a dubble tap    
                print ("Double tap");
                    
                TapCount = 0;

                SceneManager.LoadScene(1);
            }
        }
        if (Time.time > NewTime) {
            TapCount = 0;
        }
    }
}