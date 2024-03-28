using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraSwitch : MonoBehaviour
{
    public Camera camera;
    private int Switch;
    private readonly int[] _playerId = new int[2];
    public float MoveSpeed;
    public float RotateSpeed;
    public const float FreeMaxPitch = 80;
    public const float MinClickInterval = 0.2f;

    public float clickLastTime;
    private void Start()
    {
        camera = GetComponent<Camera>();
        MoveSpeed = 5f;
        RotateSpeed = 300f;
    }
    private void Update()
    {
        SwitchCamera();
    }
    public void SwitchCamera()
    {

        if (Controller.PlayerSteve.Keys.Count<1)
        {
            return;
        }

        int i = 0;
        foreach (int key in Controller.PlayerSteve.Keys)
        {
            _playerId[i] = key;
            i++;
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (Time.time - clickLastTime > MinClickInterval) {
                Switch++;
                if (Switch > 2)
                {
                    Switch = 0;
                }
                clickLastTime = Time.time;
            }
        }
        if (Switch == 0)
        {
            CameraFollowingFirst();
        }
        else if (Switch == 1)
        {
            CameraFollowingSecond();
        }
        else if (Switch == 2)
        {
            CameraMove();
            CameraRotate();
        }


    }

    public void CameraFollowingFirst()
    {
        CameraSurround(Controller.PlayerSteve[_playerId[0]].transform.position);
    }

    public void CameraFollowingSecond()
    {
        CameraSurround(Controller.PlayerSteve[_playerId[1]].transform.position);
    }


    public void CameraMove() 
    {
        float horizontal= Input.GetAxis("Horizontal");
        float vertical= Input.GetAxis("Vertical");

        // Move when "w a s d" is pressed
        if (Mathf.Abs(vertical) > 0.01)
        {
            Vector3 fowardVector = transform.forward;
            fowardVector = new Vector3(fowardVector.x, 0, fowardVector.z).normalized;
            // move forward
            transform.Translate(MoveSpeed * Time.deltaTime * vertical * fowardVector, Space.World);
        }
        if (Mathf.Abs(horizontal) > 0.01)
        {
            Vector3 rightVector = transform.right;
            rightVector = new Vector3(rightVector.x, 0, rightVector.z).normalized;
            // move aside 
            transform.Translate(MoveSpeed * Time.deltaTime * horizontal * rightVector, Space.World);
        }

        // Fly up if space is clicked
        if (Input.GetKey(KeyCode.Space))
        {
            transform.Translate(MoveSpeed * Time.deltaTime * Vector3.up, Space.World);
        }
        // Fly down if left shift is clicked
        if (Input.GetKey(KeyCode.LeftShift))
        {
            transform.Translate(MoveSpeed * Time.deltaTime * Vector3.down, Space.World);
        }


    }

    public void CameraRotate()
    {
        float MouseX = Input.GetAxis("Mouse X");
        float MouseY = Input.GetAxis("Mouse Y");
        if ((Mathf.Abs(MouseX) > 0.01 || Mathf.Abs(MouseY) > 0.01))
        {
            transform.Rotate(new Vector3(0, MouseX * RotateSpeed * Time.deltaTime, 0), Space.World);

            float rotatedPitch = transform.eulerAngles.x - MouseY * RotateSpeed * Time.deltaTime * 1f;
            if (Mathf.Abs(rotatedPitch > 180 ? 360 - rotatedPitch : rotatedPitch) < FreeMaxPitch)
            {
                transform.Rotate(new Vector3(-MouseY * RotateSpeed * Time.deltaTime * 1f, 0, 0));
            }
            else
            {
                if (transform.eulerAngles.x < 180)
                    transform.eulerAngles = new Vector3((FreeMaxPitch - 1e-6f), transform.eulerAngles.y, 0);
                else
                    transform.eulerAngles = new Vector3(-(FreeMaxPitch - 1e-6f), transform.eulerAngles.y, 0);
            }
        }
        
    }

    public void CameraSurround(Vector3 playerPosition)
    {

    }
}
