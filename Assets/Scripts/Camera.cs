using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Cinemachine;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

public class Camera : MonoBehaviour
{
    [SerializeField] private bool isInMenu = false;
    
    // Allows us to refer to the specific position of both player 1 and 2
    GameObject player1;
    GameObject player2;

    float xPosition;
    float yPosition;

    float player1x;
    float player1y;

    float player2x;
    float player2y;

    [SerializeField]float distance; // Distance between the two players

    [SerializeField]float maxPlayerDistance = 12f;

    [SerializeField]float cameraAdditionAdjustment = 2f;

    [SerializeField]float cameraScaleFactor = 1.5f;

    float maxCameraFov = 120f;

    [SerializeField]GameObject mainCameraObject;

    //[SerializeField]GameObject camera2;

    //[SerializeField]GameObject camera3;

    CinemachineBrain cameraBrain;

    CinemachineVirtualCamera virtualCamera;

    float baseOrtho;

    bool playersFar;

    Vector2 cameraPointPosition;
    // Start is called before the first frame update
    void Start()
    {
        if (isInMenu)
            return;
        
        cameraPointPosition = transform.position;

        player1 = GameManager.Instance.player1;
        player2 = GameManager.Instance.player2;

        distance = 0;

        player1x = player1.transform.position.x;
        player1y = player1.transform.position.y;

        player2x = player2.transform.position.x;
        player2y = player2.transform.position.y;

        virtualCamera = mainCameraObject.GetComponent<CinemachineVirtualCamera>();
        

        playersFar = false;

        baseOrtho = virtualCamera.m_Lens.OrthographicSize;

    }

    // Update is called once per frame
    void Update()
    {
        if (isInMenu)
            return;
        
        player1x = player1.transform.position.x;
        player1y = player1.transform.position.y;

        player2x = player2.transform.position.x;
        player2y = player2.transform.position.y;

        getMidPoint();

        distanceBetweenPlayers();
        
        cameraChanges();

        transform.position = cameraPointPosition;
    }

    void getMidPoint(){

        xPosition = (player1x + player2x)/2;
        yPosition = (player1y + player2y)/2;

        cameraPointPosition = new Vector2(xPosition,yPosition + 1.2f);
    }

    void distanceBetweenPlayers(){
        distance = (float)System.Math.Sqrt(squared(player2x - player1x) + squared(player2y - player1y));

    }

    float squared(float x){
        return x*x;

    }

    // For now this works but change at a later date
    void cameraChanges(){
        if(distance > maxPlayerDistance){
            playersFar = true;
            changeCameraOrtho();
        }
        else{
            virtualCamera.m_Lens.OrthographicSize = baseOrtho;
            playersFar = false;
        }

    }

    void changeCameraOrtho(){
        virtualCamera.m_Lens.OrthographicSize = baseOrtho + (distance - maxPlayerDistance)*cameraScaleFactor;
    }
}
