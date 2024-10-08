using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static WeldingPanel;

public class WeldingPanel : MonoBehaviour
{
    [SerializeField] Collider WeldingColider;
    [SerializeField] List<Transform> Panel;
    [SerializeField] Material Bloberror;
    [SerializeField] Material BlobGood;
    [SerializeField] GameObject WeldScanner;
    [SerializeField] Transform[] Checking;

    public int Timecheck = 2;
    private Transform checkerCapsule;
    private Vector3[] checkingPoints;

    public struct WeldingStats
    {
        public float uniformity;
        public float coveragePercent;
        public float travel;

        public int badweldCount;
        public int holesCount;

    }

    void Awake()
    {
        checkingPoints = new Vector3[Checking.Length];

        int i = 0;
        foreach (Transform t in Checking)
        {
            checkingPoints[i] = t.position;
            i++;
        }
    }

    bool isWeldingStatsDone = false;
    WeldingStats weldingstats;


    internal void PopulateWeldingStats( out int delay)
    {
        delay = Timecheck;
        isWeldingStatsDone = false ;
        weldingstats = new WeldingStats();
        if (checkerCapsule == null)checkerCapsule = Instantiate(WeldScanner, checkingPoints[0],Quaternion.identity).transform;

        checkerCapsule.rotation = Checking[0].rotation;

        weldingstats.uniformity = GetUniformty();
        weldingstats.travel = GetWeldTravelUniformity();
        weldingstats.badweldCount = GetBadWelds();
        weldingstats.holesCount  = GetweldHoles();

        int totalCount = 0;
        int blobCount = 0;

        LeanTween.move(checkerCapsule.gameObject, checkingPoints, Timecheck).setOnUpdate((Vector3 value) =>
        {
            bool hasBlob = RaycastCheckWeldl(checkerCapsule);
            totalCount++;
            if (hasBlob)
            {
                blobCount++;
                checkerCapsule.GetComponent<AudioSource>().pitch = 1f;
            }else
            {
                checkerCapsule.GetComponent <AudioSource>().pitch = 1.3f;     
            }
        }).setOnComplete(()  => { 
            if (checkerCapsule) Destroy(checkerCapsule.gameObject);
            weldingstats.coveragePercent = (float)blobCount / (float)totalCount;

            isWeldingStatsDone = true;

        });
    }

    private int GetweldHoles()
    {

        
        return GameObject.FindGameObjectsWithTag("WeldHole").Length;
    }

    internal bool GetWeldResult(out WeldingStats stats) 
    {
        stats = weldingstats;
        return isWeldingStatsDone;
    }
    private bool RaycastCheckWeldl(Transform checkPos)
    {
        bool hasBlob = false;

        Vector3 checkPosWithGap = checkPos.position + Vector3.up * 0.1f;
        if(Physics.Raycast(checkPosWithGap,Vector3.down, out RaycastHit hit))
        {
            if(hit.transform.gameObject.layer == 6)
            {
                hasBlob = true;
            }
            else
            {
                hasBlob = false;
            }
        }

        return hasBlob;
    }
    private int GetBadWelds()
    {
        int badWeldsCount = 0;

        foreach(Transform t in Panel)
        {
            WeldingBlobSet[] blobs = t.GetComponentsInChildren<WeldingBlobSet>();
            foreach (WeldingBlobSet blob in blobs)
            {
                blob.gameObject.layer = 7;
                LeanTween.value(0,1,Timecheck).setOnComplete(()=>
                {
                    blob.GetComponent<Renderer>().material = Bloberror;
                } );
            }
            badWeldsCount += blobs.Length;

        }

        //Good Welds

        WeldingBlobSet[] goodBlobs = WeldingColider.transform.GetComponentsInChildren<WeldingBlobSet>();

        foreach (WeldingBlobSet t in goodBlobs)
        {
           
                LeanTween.value(0, 1, Timecheck).setOnComplete(() =>
                {
                    t.GetComponent<Renderer>().material = BlobGood;
                });
            
        }
        return badWeldsCount;
    }
    private float GetUniformty()
    {
        float unifrmity = 0.0f;
        float smalletScale = Mathf.Infinity;
        float largestScale = 0;

        GameObject[] weldObjects = GameObject.FindGameObjectsWithTag("WeldObject");
        foreach (GameObject obj in weldObjects)
        {
            if (obj.transform.localScale.x < smalletScale) smalletScale = obj.transform.localScale.x;
            if (obj.transform.localScale.x > largestScale) largestScale = obj.transform.localScale.x;
            

            
        }

        unifrmity = ((smalletScale + largestScale) / 2) / largestScale;
        return unifrmity;
    }

    List<float> weldTravels = new List<float>();
    internal void AddWedlTravel(float wedlTravel)
    {
        weldTravels.Add(wedlTravel);
    }
    internal void ResetWeldTravel()
    {
        weldTravels.Clear();
    }
    private float GetWeldTravelUniformity()
    {
        if (weldTravels.Count <= 10)
        {
            return 0;
        }
        float idealTime = 0.419f;
        float avarageTime = weldTravels.Average();
        float travelPerf = 1 - (Mathf.Abs(idealTime - avarageTime )/ idealTime);

        return travelPerf;
    }
    void Update()
    {
        
    }
}
