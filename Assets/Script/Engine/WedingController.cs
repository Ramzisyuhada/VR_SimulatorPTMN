using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;

public class WedingController : MonoBehaviour
{
    [SerializeField] private Transform weldBlobSet, weldHoleMask, weldingTip;
    [SerializeField] private MeshRenderer tipRenderer;

    [SerializeField] private GameObject glowEffect;

    private AudioSource audioSource;

    private Material tipOriginalMat;


    private float waktuwelder;
    private RaycastHit hit;
    private bool tunggu;
    private bool weldingLayer;

    private float trivetimer;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        tipOriginalMat = tipRenderer.material;
    }

    
    public void StartLash()
    {
        if (tunggu) return;

        waktuwelder += Time.deltaTime;

        if (waktuwelder >= 1)
        {
            if (weldingLayer)
            {
                ShowEffect(true);
                ShowBlow();
                trivetimer = Time.deltaTime;
            }
            else
            {
                ShowEffect(false);
                trivetimer = 0;
            }
        }
        
    }

    private void ShowEffect(bool kondisi)
    {
        glowEffect.SetActive(kondisi);
        if (kondisi && !audioSource.isPlaying)
        {
            audioSource.Play();
            tipRenderer.material = weldBlobSet.GetComponent<WeldingBlobSet>().blobHotMaterial;
        }
        else if (!kondisi && audioSource.isPlaying)
        {
            tipRenderer.material = tipOriginalMat;

            audioSource.Stop();
        }
    }

    private bool hasBlob = false;
    private Transform currentBlob;
    private float blobSizeTimer;
    private Transform currentPanel;
    public  Vector3 GetWorldPoint()
    {

        Vector3 point = weldingTip.position;
        if (Physics.Raycast(weldingTip.position, weldingTip.forward,out RaycastHit hit1))
        {
            if(hit1.transform.gameObject.layer == 6 || hit1.transform.gameObject.layer == 7)
            {
                hit = hit1;
              
                weldingLayer = true;
            }
            else
            {
               
                hit = new RaycastHit();
                weldingLayer = false;   
            }
            point = hit1.point;


            Debug.DrawLine(weldingTip.position, hit1.point, Color.red);


        }

        bool hasFrontPanel = false;

        Vector3 hasFrontPositionn = hit.point - weldingTip.forward * 0.013f;

        if(Physics.Raycast(hasFrontPositionn , -Vector3.forward, out RaycastHit hit2))
        {
            if ((hit2.transform.gameObject.layer == 6 || hit2.transform.gameObject.layer == 7) && hit2.distance < 0.02f)
            {
                hasFrontPanel = true;
                Debug.DrawLine(hasFrontPositionn, hit2.point, Color.green);
            }
            else
            {
                hasFrontPanel = false;
                Debug.DrawLine(hasFrontPositionn, hit2.point, Color.yellow);
            }

        }


        bool hasBottomPanel = false;
        if(Physics.Raycast(hasFrontPositionn,Vector3.back,out RaycastHit hit3))
        {
            if ((hit2.transform.gameObject.layer == 6 || hit2.transform.gameObject.layer == 7) && hit2.distance < 0.02f)
            {
                hasFrontPanel = true;
                Debug.DrawLine(hasFrontPositionn, hit3.point, Color.green);
            }
            else
            {
                hasBottomPanel = false;
                Debug.DrawLine(hasFrontPositionn, hit3.point, Color.green);
            }
        }

        if (hasFrontPanel && hasBottomPanel)
            isCornerWeld = true;
        else
            isCornerWeld = false;


        return point;
    }
    public bool isCornerWeld = false;

    private Transform currentblob;
    private Transform currentPane;
    private float BlobSizeTimer;
    private void ShowBlow()
    {
        float currentsize = 0.2f;
        if (!hasBlob)
        {
            if(hit.transform.gameObject.layer == 7) // hit panel
            {
                currentPanel = hit.transform;
                Quaternion rotatin = Quaternion.FromToRotation(Vector3.up, hit.normal);
                currentBlob = Instantiate(weldBlobSet,hit.point,rotatin);
                currentBlob.localScale = Vector3.one * currentsize;
                BlobThicknes(currentBlob);
            }
            else if(hit.transform.gameObject.layer == 6) // nge cek apakah raycast terkena blob lagi
            {
                currentBlob = hit.transform;
                currentBlob.parent = null; //Remove parent

                currentsize = currentBlob.localScale.x;
                currentBlob.GetComponent<WeldingBlobSet>().ShowGlow();


            }
            hasBlob = true;
            BlobSizeTimer = 0f;
        }

        if (hasBlob)
        {
            blobSizeTimer += Time.deltaTime * 0.2f;
            if(hit.transform == currentBlob)
            {
                if(currentBlob.localScale.magnitude < 0.7f) // Batas Ukuran
                {
                    currentBlob.localScale = Vector3.one * (currentsize + blobSizeTimer);
                    BlobThicknes(currentBlob);
                }
                else // Jika Las Nya terlalu Lama Bakal Mengakibatkan Bolong pada panel
                {
                    HolePanel(currentBlob);

                    StopWelding(false);
                    StartCoroutine(HoldWeldingRoutine(0.5f));

                }
            }
            else
            {
                ResetBlobSettings();
            }
        }
    }

    private void SetBlobTravelTime(WeldingPanel panel)
    {

        if (panel && trivetimer > 0)
        {
            //Debug.Log("Weld Travel: " + travelTimer);
          //  panel.AddWeldTravel(trivetimer);
            trivetimer = 0;
        }

    }
    private void StopWelding(bool resetime = true)
    {
        if (resetime)
        {
            tunggu = false;
            waktuwelder = 0;
            Debug.Log("Las Mati");
        }

        ShowEffect(false);
        ResetBlobSettings(true);
    }

    Transform prevblob;
    private void ResetBlobSettings(bool reset = false)
    {
        if (currentBlob)
        {
            currentBlob.parent = currentPanel;
            if (prevblob)
            {
                prevblob.LookAt(currentBlob, prevblob.up);
                prevblob.GetComponent<WeldingBlobSet>().tiltForward = true;

            }

            prevblob = currentBlob;
        }


        if (reset)
        {
            if (prevblob)
                prevblob.GetComponent<WeldingBlobSet>().tiltForward = false;

            currentPanel = null;
            currentBlob = null;
            prevblob = null;
        }


        hasBlob = false;
        blobSizeTimer = 0;
    }

    private IEnumerator HoldWeldingRoutine(float duration)
    {
        tunggu = true;
        yield return new WaitForSeconds(duration);
        tunggu = false;
    }
    private void HolePanel(Transform Blob)
    {
        Transform holeobject = Instantiate(weldHoleMask,Blob.position,Blob.rotation);
        holeobject.localScale = Vector3.one * 0.2f;

        float width = holeobject.localScale.x  + 0.4f;
        Vector3 final = new Vector3(width, holeobject.localScale.y + 0.7f, width);
        LeanTween.scale(holeobject.gameObject, final, 0.2f); //Animate


        Destroy(Blob.gameObject);

    }
    private void BlobThicknes(Transform blob)
    {
        if (isCornerWeld)
        {
            blob.localScale = new Vector3(blob.localScale.x, blob.localScale.y * 1.3f, blob.localScale.z * 1.3f);
        }
        else
        {
            blob.localScale = new Vector3(blob.localScale.x, blob.localScale.y / 3, blob.localScale.z);
        }
    }
}
