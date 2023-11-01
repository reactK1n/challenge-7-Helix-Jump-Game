using System.Collections.Generic;
using UnityEngine;

public class Helix : MonoBehaviour
{
    private Vector2 lastTapPos;
    private Vector3 startRotation;

    [SerializeField]
    private Transform topTransform;

    [SerializeField]
    private Transform goalTransform;

    [SerializeField]
    private GameObject helixLevelPrefab;

    [SerializeField]
    private List<Stage> allStages;

    private float helixDistance;

    private List<GameObject> spawnedLevels = new();

    private Vector3 lastRotation;


    void Awake()
    {
        startRotation = transform.localEulerAngles;
        helixDistance = topTransform.localPosition.y - (goalTransform.localPosition.y + 0.1f);
        LoadStage(0);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.IsGameOver)
            return;

        if (Input.GetMouseButton(0))
        {
            Vector2 currentTapPos = Input.mousePosition;

            if (lastTapPos == Vector2.zero)
            {
                lastTapPos = currentTapPos;
            }

            // reduce delta to reduce the rotation speed
            float delta = (currentTapPos.x - lastTapPos.x) * .03f;
            transform.Rotate(Vector3.up * delta);
        }

        if (Input.GetMouseButtonUp(0))
        {
            lastTapPos = Vector2.zero;
        }
    }

    public void ResetHelixToPreviousState()
    {
        transform.localEulerAngles = lastRotation;
    }

    public void SetLastRotation()
    {
        lastRotation = transform.localEulerAngles;
    }

    public int AllStages()
    {
        return allStages.Count;
    }

    public void LoadStage(int stageNumber)
    {
        var stage = allStages[Mathf.Clamp(stageNumber, 0, allStages.Count - 1)];

        if (stage == null)
        {
            Debug.Log($"No stage {stageNumber} found in allStages List");
            return;
        }

        //set background color
        Camera.main.backgroundColor = allStages[stageNumber].stageBackgroundColor;

        //set ball color
        FindObjectOfType<BallController>().GetComponent<Renderer>().material.color = allStages[stageNumber].stageBallColor;

        //reset helix rotation
        transform.localEulerAngles = startRotation;

        //destroy old level if any
        foreach (var go in spawnedLevels)
        {
            Destroy(go);
        }

        //create new level / platforms
        float levelDistance = helixDistance / stage.levels.Count;
        var spawnPosY = topTransform.localPosition.y;

        for (int i = 0; i < stage.levels.Count; i++)
        {
            spawnPosY -= levelDistance;

            //created level within scene
            var level = Instantiate(helixLevelPrefab, transform);

            level.transform.localPosition = new Vector3(0, spawnPosY, 0);
            spawnedLevels.Add(level);

            //disable part
            const int totalPart = 12;
            int partsToDisable = totalPart - stage.levels[i].partCount;
            var disabledParts = new List<GameObject>();

            while (disabledParts.Count < partsToDisable)
            {
                //get child gameobject of a gameobject
                var randomPart = level.transform.GetChild(Random.Range(0, level.transform.childCount)).gameObject;
                if (disabledParts.Contains(randomPart))
                    continue;

                randomPart.SetActive(false);
                disabledParts.Add(randomPart);
            }

            //add color to remaining gameobject
            var leftParts = new List<GameObject>();
            foreach (Transform t in level.transform)
            {
                t.GetComponent<Renderer>().material.color = allStages[stageNumber].stageLevelPartColor;
                if (t.gameObject.activeInHierarchy)
                {
                    leftParts.Add(t.gameObject);
                }
            }

            //add death part
            int deathPartCount = stage.levels[i].deathPartCount;
            var deathParts = new List<GameObject>();

            while (deathPartCount > 0)
            {
                var randomPart = leftParts[Random.Range(0, leftParts.Count)].gameObject;
                if (deathParts.Contains(randomPart))
                    continue;

                randomPart.gameObject.AddComponent<DeathPart>();
                deathParts.Add(randomPart);
                deathPartCount--;
            }
        }
    }
}
