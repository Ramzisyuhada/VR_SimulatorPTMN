using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ScoreController;

public class DoneButton : MonoBehaviour
{
    [SerializeField] private ScoreController scoreSys;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("space"))
        {
            Donebutton();
        }
    }
    public void Donebutton()
    {
   

        float delay = (float)scoreSys.PopulateScores() + 0.2f;

        LeanTween.value(gameObject, 0, 1, delay).setOnUpdate((float val) => {


        }).setOnComplete(() =>
        {
           
            WeldingScore scoreResult = scoreSys.GetResult();

        });

    }

}
