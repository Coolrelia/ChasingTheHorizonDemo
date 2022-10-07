using System.Collections;
using UnityEngine;
public class LeahEvent : Event
{
    [Header("Unique References")]
    public UnitLoader leahObjectNormalMode;
    public UnitLoader leahObjectHardMode;
    public MapDialogue leahDialogue;

    [Header("Normal Mode")]
    public Vector2 startPositionN;
    public Vector2 endPositionN;    

    [Header("Hard Mode")]
    public Vector2 startPositionH;
    public Vector2 endPositionH;

    public override void StartEvent()
    {
        StartCoroutine(MoveCamera());
    }

    private IEnumerator MoveCamera()
    {
        loseManager.gameObject.SetActive(false);

        string difficulty = PlayerPrefs.GetString("Difficulty", "Normal");
        Vector3 targetPosition = new Vector3(0, 0, 0);

        if(difficulty == "Normal"){
            targetPosition = new Vector3(startPositionN.x, startPositionN.y, -10);
        }
        else if(difficulty == "Hard"){
            targetPosition = new Vector3(startPositionH.x, startPositionH.y, -10);
        }

        if (targetPosition.x > cursor.cameraRight){
            targetPosition.x = cursor.cameraRight;
        }
        if(targetPosition.x < cursor.cameraLeft){
            targetPosition.x = cursor.cameraLeft;
        }
        if(targetPosition.y > cursor.cameraTop){
            targetPosition.y = cursor.cameraTop;
        }
        if(targetPosition.y < cursor.cameraBottom){
            targetPosition.y = cursor.cameraBottom;
        }

        if(difficulty == "Normal")        {
            leahObjectNormalMode.gameObject.SetActive(true);
            TurnManager.instance.allyUnits.Add(leahObjectNormalMode.GetComponent<UnitLoader>());
        }        
        else if(difficulty == "Hard")        {
            leahObjectHardMode.gameObject.SetActive(true);
            TurnManager.instance.allyUnits.Add(leahObjectHardMode.GetComponent<UnitLoader>());
        }

        while (mainCamera.transform.position.x != targetPosition.x)
        {
            Vector3 xTarget = new Vector3(targetPosition.x, mainCamera.transform.position.y, -10);
            mainCamera.transform.position = Vector3.MoveTowards(mainCamera.transform.position, xTarget, 10f * Time.deltaTime);
            yield return null;
        }
        while(mainCamera.transform.position.y != targetPosition.y)
        {
            Vector3 yTarget = new Vector3(mainCamera.transform.position.x, targetPosition.y, -10);
            mainCamera.transform.position = Vector3.MoveTowards(mainCamera.transform.position, yTarget, 10f * Time.deltaTime);
            yield return null;
        }
        yield return new WaitUntil(() => mainCamera.transform.position == targetPosition);

        if(difficulty == "Normal")
        {
            StartCoroutine(MoveUnit(new Vector2(endPositionN.x, endPositionN.y), leahObjectNormalMode));
            yield return new WaitUntil(() => (Vector2)leahObjectNormalMode.transform.localPosition == endPositionN);
            cursor.transform.localPosition = new Vector3(leahObjectNormalMode.transform.localPosition.x, leahObjectNormalMode.transform.localPosition.y, 0);
            cursor.currentPosition = new Vector3(leahObjectNormalMode.transform.localPosition.x, leahObjectNormalMode.transform.localPosition.y, 0);
        }
        else if(difficulty == "Hard")
        {
            StartCoroutine(MoveUnit(new Vector2(endPositionH.x, endPositionH.y), leahObjectHardMode));
            yield return new WaitUntil(() => (Vector2)leahObjectHardMode.transform.localPosition == endPositionH);
            cursor.transform.localPosition = new Vector3(leahObjectHardMode.transform.localPosition.x, leahObjectHardMode.transform.localPosition.y, 0);
            cursor.currentPosition = new Vector3(leahObjectHardMode.transform.localPosition.x, leahObjectHardMode.transform.localPosition.y, 0);
        }

        MapDialogueManager.instance.WriteSingle(leahDialogue);
        yield return new WaitForSeconds(1f);
        yield return new WaitUntil(() => !screenDim.gameObject.activeSelf);
        loseManager.gameObject.SetActive(true);
        finished = true;
    }
    private IEnumerator MoveUnit(Vector2 targetPosition, UnitLoader leahObject)
    {
        while(leahObject.transform.localPosition.x != targetPosition.x)
        {
            if(leahObject.transform.localPosition.x > targetPosition.x)
            {
                leahObject.animator.SetBool("Selected", true);
                leahObject.animator.SetBool("Left", true);
            }
            leahObject.transform.localPosition = Vector2.MoveTowards(leahObject.transform.localPosition, new Vector2(targetPosition.x, leahObject.transform.localPosition.y), 2.5f * Time.deltaTime); 
            yield return null;
        }
        while(leahObject.transform.localPosition.y != targetPosition.y)
        {
            if(leahObject.transform.localPosition.y < targetPosition.y)
            {
                leahObject.animator.SetBool("Selected", true);
                leahObject.animator.SetBool("Down", true);
            }
            leahObject.transform.localPosition = Vector2.MoveTowards(leahObject.transform.localPosition, new Vector2(leahObject.transform.localPosition.x, targetPosition.y), 2.5f * Time.deltaTime);
            yield return null;
        }
        leahObject.animator.SetBool("Selected", false);
        leahObject.animator.SetBool("Left", false);
        leahObject.animator.SetBool("Down", false);
        leahObject.animator.CrossFade("Idle", 0.3f);
    }
}
