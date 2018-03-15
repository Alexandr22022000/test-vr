using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTelecenes : MonoBehaviour {
    delegate void OnPressedReleased();

    class GamepadController
    {
        private OnPressedReleased onClick, onLongClick;
        private int id;
        private bool[] keys;
        private float timer = -1.0f;

        public GamepadController (OnPressedReleased onClick, OnPressedReleased onLongClick, int id, bool[] keys)
        {
            this.onClick = onClick;
            this.onLongClick = onLongClick;
            this.id = id;
            this.keys = keys;
        }

        public void OnUpdate (bool isPress, bool isRelese)
        {
            if (onLongClick != null && timer >= 0.0f) timer += Time.deltaTime;

            if ((isPress && keys[id]) || (isRelese && !keys[id]) || (!isPress && !isRelese)) return;

            if (isPress)
            {
                keys[id] = true;

                if (onLongClick == null)
                {
                    if (onClick != null) onClick();
                }
                else
                {
                    timer = 0.0f;
                }
            }
            else
            {
                keys[id] = false;

                if (timer >= 0.6f)
                    onLongClick();
                else
                    if (onClick != null) onClick();
            }
        }
    }

    private GameObject telekenesPoint, moveVectorController, player, telekenesObject = null;
    private bool[] keys = new bool[] {false, false, false, false, false, false, false};
    private GamepadController[] gamepadControllers;

	void Start ()
    {
        telekenesPoint = GameObject.Find("TelekenesPoint");
        moveVectorController = GameObject.Find("MoveVectorController");
        player = GameObject.Find("Player");

        gamepadControllers = new GamepadController[6];
        gamepadControllers[0] = new GamepadController(null, null, 0, keys);
        gamepadControllers[1] = new GamepadController(null, null, 1, keys);
        gamepadControllers[2] = new GamepadController(null, null, 2, keys);
        gamepadControllers[3] = new GamepadController(null, null, 3, keys);

        gamepadControllers[4] = new GamepadController(() => 
        {
            if (telekenesObject == null)
                UpTelekenesObject();
            else
                DownTelekenesObject();
        }, () => 
        {
            if (telekenesObject == null)
                UpTelekenesObject();
            else
                CastTeleckenesObject();
        }, 4, keys);

        gamepadControllers[5] = new GamepadController(() => {
            if (telekenesObject == null) InteractiveObject();
        }, null, 5, keys);
    }
	
	void Update ()
    {
        TestUpdate();  //TEST

        if (keys[5] && telekenesObject != null)
            RotationTelekenesObject();
        else
            PlayerMove();

        gamepadControllers[0].OnUpdate(Input.GetKeyDown(KeyCode.JoystickButton0), Input.GetKeyUp(KeyCode.JoystickButton0));
        gamepadControllers[1].OnUpdate(Input.GetKeyDown(KeyCode.JoystickButton1), Input.GetKeyUp(KeyCode.JoystickButton1));
        gamepadControllers[2].OnUpdate(Input.GetKeyDown(KeyCode.JoystickButton2), Input.GetKeyUp(KeyCode.JoystickButton2));
        gamepadControllers[3].OnUpdate(Input.GetKeyDown(KeyCode.JoystickButton3), Input.GetKeyUp(KeyCode.JoystickButton3));
        gamepadControllers[4].OnUpdate(Input.GetKeyDown(KeyCode.JoystickButton4), Input.GetKeyUp(KeyCode.JoystickButton4));
        gamepadControllers[5].OnUpdate(Input.GetKeyDown(KeyCode.JoystickButton5), Input.GetKeyUp(KeyCode.JoystickButton5));

        if (telekenesObject != null)
        {
            telekenesPoint.transform.LookAt(telekenesObject.transform);            

            if ((telekenesObject.transform.position - telekenesPoint.transform.position).magnitude <= 0.2)
            {
                telekenesObject.GetComponent<Rigidbody>().AddForce(telekenesPoint.transform.forward * -190);
            }
            else
            {
                telekenesObject.GetComponent<Rigidbody>().AddForce(telekenesPoint.transform.forward * -700);
            }
        }

        if (keys[0] && (telekenesPoint.transform.position - transform.position).magnitude < 1000000.0f)
        {
            telekenesPoint.transform.position = (telekenesPoint.transform.position - transform.position) / 100 + telekenesPoint.transform.position;
        }

        if (keys[2] && (telekenesPoint.transform.position - transform.position).magnitude > 1.7f)
        {
            telekenesPoint.transform.position = (telekenesPoint.transform.position - transform.position) / -100 + telekenesPoint.transform.position;
        }
    }

    private void UpTelekenesObject ()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward * 500000, out hit))
        {
            if (hit.collider.gameObject.GetComponent<TelekenrsObject>() != null)
            {
                telekenesPoint.transform.position = hit.point;
                telekenesObject = hit.collider.gameObject;
                telekenesObject.GetComponent<Rigidbody>().drag = 5.0f;
                telekenesObject.GetComponent<Rigidbody>().angularDrag = 2.0f;
            }
        }
    }

    private void DownTelekenesObject ()
    {
        telekenesObject.GetComponent<Rigidbody>().drag = 0.0f;
        telekenesObject.GetComponent<Rigidbody>().angularDrag = 0.0f;
        telekenesObject = null;
    }

    private void CastTeleckenesObject ()
    {
        telekenesObject.GetComponent<Rigidbody>().drag = 0.0f;
        telekenesObject.GetComponent<Rigidbody>().angularDrag = 0.0f;
        telekenesObject.GetComponent<Rigidbody>().AddForce(transform.forward * 40000);
        telekenesObject = null;
    }

    private void InteractiveObject ()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward * 500000, out hit))
        {
            if (hit.collider.gameObject.GetComponent<MyButton>() != null)
                hit.collider.gameObject.GetComponent<MyButton>().OnIterect();
        }
    }

    private void RotationTelekenesObject ()
    {
        telekenesObject.transform.Rotate(new Vector3(0, Input.GetAxis("Horizontal"), 0), Space.World);
        telekenesObject.transform.Rotate(new Vector3(Input.GetAxis("Vertical"), 0, 0), Space.World);
    }

    private void PlayerMove ()
    {
        Transform moveTranform = moveVectorController.transform;
        Quaternion rotation = transform.rotation;
        moveTranform.rotation = new Quaternion(transform.rotation.x, rotation.y, transform.rotation.z, rotation.w);

        Vector3 vector2D = Input.GetAxisRaw("Horizontal") * moveTranform.right * 1 + Input.GetAxisRaw("Vertical") * moveTranform.forward * 1;
        Rigidbody rigidbody = player.GetComponent<Rigidbody>();
        rigidbody.MovePosition(rigidbody.position + vector2D * 0.05f);
    }


    /*
    ---Функции для тестирования--- 
     */

    private void TestUpdate ()
    {
        transform.Rotate(new Vector3(Input.GetAxis("Mouse Y"), 0, 0), Space.Self);
        transform.Rotate(new Vector3(0, Input.GetAxis("Mouse X"), 0), Space.World);

        gamepadControllers[0].OnUpdate(Input.GetKeyDown(KeyCode.I), Input.GetKeyUp(KeyCode.I));
        gamepadControllers[1].OnUpdate(Input.GetKeyDown(KeyCode.L), Input.GetKeyUp(KeyCode.L));
        gamepadControllers[2].OnUpdate(Input.GetKeyDown(KeyCode.J), Input.GetKeyUp(KeyCode.J));
        gamepadControllers[3].OnUpdate(Input.GetKeyDown(KeyCode.K), Input.GetKeyUp(KeyCode.K));
        gamepadControllers[4].OnUpdate(Input.GetKeyDown(KeyCode.Mouse0), Input.GetKeyUp(KeyCode.Mouse0));
        gamepadControllers[5].OnUpdate(Input.GetKeyDown(KeyCode.Mouse1), Input.GetKeyUp(KeyCode.Mouse1));
    }
}