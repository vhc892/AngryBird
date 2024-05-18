using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class SlingShotHandler : MonoBehaviour
{
    [Header("Line Renderers")]
    [SerializeField] private LineRenderer leftLineRenderer;
    [SerializeField] private LineRenderer rightLineRenderer;

    [Header("Transform References")]
    [SerializeField] private Transform leftStartPosition;
    [SerializeField] private Transform rightStartPosition;
    [SerializeField] private Transform centerPosition;
    [SerializeField] private Transform idlePosition;
    [SerializeField] private Transform elasticTransform;

    [Header("Stats")]
    [SerializeField] private float maxDistance = 3.5f;
    [SerializeField] private float shotForce = 5f;
    [SerializeField] private float timeBetweenBirdRespawn = 2f;
    [SerializeField] private float elasticDivier = 1.2f;
    [SerializeField] private float maxAnimationTime = 1f;

    [Header("Script")]
    [SerializeField] private SlingShotArea slingShotArea;
    [SerializeField] private CameraManager cameraManager;

    [Header("Bird")]
    [SerializeField] private AngieBird angieBirdPrefab;
    [SerializeField] private float angieBirdPositionOffset = 0.2f;

    [Header("Sounds")]
    [SerializeField] private AudioClip elasticPulledClip;
    [SerializeField] private AudioClip[] elasticReleasedClips;
    private Vector2 slingShotLinesPosition;
    private Vector2 direction;
    private Vector2 directionNormalized;

    private bool clickWithinArea;
    private bool birdOnSlingshot;
    private AngieBird spawnedAngieBird;
    private AudioSource audioSource;

    

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        leftLineRenderer.enabled = false;
        rightLineRenderer.enabled = false;
        SpawnAgieBird();
    }
    private void Update()
    {
        /*if (Mouse.current.leftButton.wasPressedThisFrame && slingShotArea.IsWithinSlingshotArea())
        {
            clickWithinArea = true;
        }*/
        if (InputManager.wasLeftMouseButtonPressed && slingShotArea.IsWithinSlingshotArea())
        {
            clickWithinArea = true;
            if (birdOnSlingshot)
            {
                SoundManager.instance.PlayClip(elasticPulledClip, audioSource);
                cameraManager.SwitchToFollowCam(spawnedAngieBird.transform);
            }
        }
        if (InputManager.isLeftMousePressed && clickWithinArea/*==true*/ && birdOnSlingshot)
        {
            DrawSlingShot();
            PositionAndRotateAngieBird();
        }
        if (InputManager.wasLeftMouseButtonReleased && birdOnSlingshot && clickWithinArea)
        {
            if (GameManager.instance.HasEnoughShots())
            {
                clickWithinArea = false;
                birdOnSlingshot = false;

                spawnedAngieBird.LaunchBird(direction, shotForce);
                SoundManager.instance.PlayRandomClip(elasticReleasedClips, audioSource);
                GameManager.instance.UseShot();
                AnimateSlingShot();
                
                if(GameManager.instance.HasEnoughShots()) {
                   StartCoroutine(SpawnAngieBirdAfterTime());
                }
            }
            
        }
    }
    private void DrawSlingShot()
    {

        Vector3 touchPosition = Camera.main.ScreenToWorldPoint(InputManager.mousePosition);
        //max length slingshot
        slingShotLinesPosition = centerPosition.position + Vector3.ClampMagnitude(touchPosition - centerPosition.position, maxDistance);
        SetLines(slingShotLinesPosition);

        direction = (Vector2)centerPosition.position - slingShotLinesPosition;
        directionNormalized = direction.normalized;
    }
    private void SetLines(Vector2 position)
    {
        if (!leftLineRenderer.enabled && !rightLineRenderer.enabled)
        {
            leftLineRenderer.enabled = true;
            rightLineRenderer.enabled = true;

        }
        leftLineRenderer.SetPosition(0, position);
        leftLineRenderer.SetPosition(1, leftStartPosition.position);

        rightLineRenderer.SetPosition(0, position);
        rightLineRenderer.SetPosition(1, rightStartPosition.position);

    }
    private void SpawnAgieBird()
    {
        elasticTransform.DOComplete();
        SetLines(idlePosition.position);
        Vector2 dir = (centerPosition.position - idlePosition.position).normalized;
        Vector2 spawnPosition = (Vector2)idlePosition.position + dir * angieBirdPositionOffset;

        spawnedAngieBird = Instantiate(angieBirdPrefab, spawnPosition, Quaternion.identity);
        spawnedAngieBird.transform.right = dir;
        birdOnSlingshot = true;
    }
    private void PositionAndRotateAngieBird()
    {
        spawnedAngieBird.transform.position = slingShotLinesPosition + directionNormalized * angieBirdPositionOffset;
        spawnedAngieBird.transform.right = directionNormalized;
    }
    private IEnumerator SpawnAngieBirdAfterTime()
    {
        yield return new WaitForSeconds(timeBetweenBirdRespawn);
        SpawnAgieBird();
        cameraManager.SwitchToIdleCam();
    }
    private void AnimateSlingShot()
    {
        elasticTransform.position = leftLineRenderer.GetPosition(0);
        float dist = Vector2.Distance(elasticTransform.position, centerPosition.position);
        float time = dist / elasticDivier;

        elasticTransform.DOMove(centerPosition.position, time).SetEase(Ease.OutElastic);
        StartCoroutine(AnimatesSlingShotLines(elasticTransform, time));
    }
    private IEnumerator AnimatesSlingShotLines(Transform trans, float time)
    {
        float elapsedTime = 0f;
        while (elapsedTime < time && elapsedTime < maxAnimationTime)
        {
            elapsedTime += Time.deltaTime;
            SetLines(trans.position);
            yield return null;
        }
    }

}