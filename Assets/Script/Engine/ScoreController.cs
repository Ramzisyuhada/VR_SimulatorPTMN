using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static WeldingPanel;

public class ScoreController : MonoBehaviour
{

    [SerializeField] private WeldingPanel currentpane;
    [SerializeField] private GameObject[] panelPrefab;
    [SerializeField] private AudioClip panelDropSound;

    private int currentIndex;
    private AudioSource audioSource;


    public struct WeldingScore
    {
        public int uniformity;
        public int coverage;
        public int travel;
    }

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

    }

    internal int PopulateScores()
    {
        currentpane.PopulateWeldingStats(out int delay);

        return delay;
    }

    internal WeldingScore GetResult() {
        WeldingScore score = new WeldingScore();

        if (currentpane.GetWeldResult(out WeldingStats weldingResults))
        {
            if (weldingResults.holesCount > 0) score.uniformity = 0;
            else score.uniformity = Mathf.Clamp((int)Mathf.Round(weldingResults.uniformity * 100) - weldingResults.badweldCount, 0, 100);
            score.coverage = (int)Mathf.Round((weldingResults.coveragePercent * 100));
            score.travel = (int)Mathf.Round((weldingResults.travel * 100));
        }
        
        return score;
    }


    internal void ShowPanel(bool show)
    {
        currentpane.gameObject.SetActive(show);
        if (show)
        {
            AnimatePanel();
        }
    }

    internal void ResetPanel()
    {
        currentpane.ResetWeldTravel();
        AnimatePanel();
    }

    internal void AnimatePanel()
    {
        currentpane.transform.position = transform.position + Vector3.up * 0.06f;

        LeanTween.move(currentpane.gameObject, transform.position, 0.2f).setEase(LeanTweenType.easeInSine).setOnComplete(() =>
        {
            audioSource.PlayOneShot(panelDropSound);
        });

    }
    
}
