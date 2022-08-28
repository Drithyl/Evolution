using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour
{
    public LayerMask hitLayers;

    // Text info of the current target
    public TMPro.TextMeshProUGUI targetInfo;


    Camera mainCamera;
    ThirdPersonCamera thirdPersonMode;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        thirdPersonMode = mainCamera.GetComponent<ThirdPersonCamera>();
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        //Ray ray = mainCamera.ScreenPointToRay(new Vector2(Screen.width * 0.5f, Screen.height * 0.5f));
        //RaycastHit hitPoint;

        //Debug.DrawRay(ray.origin, ray.direction, Color.red);
        Debug.DrawLine(ray.origin, ray.direction * 100f, Color.red);

        if (Physics.Raycast(ray, out RaycastHit hitPoint, 100f, hitLayers) == false)
        {
            targetInfo.text = "";

            if (Input.GetButtonDown("Fire1") == true)
                thirdPersonMode.RemoveTarget();

            return;
        }

        targetInfo.text = hitPoint.transform.name;
        Debug.Log("Hit something! " + hitPoint.transform.name);

        Creature creature = hitPoint.transform.GetComponent<Creature>();

        if (creature == null)
        {
            thirdPersonMode.RemoveTarget();
            Debug.Log("Transform hit is not creature");
            return;
        }

        targetInfo.text = creature.name;

        if (Input.GetButtonDown("Fire1") == true)
            thirdPersonMode.SetTarget(hitPoint.transform);
    }
}
