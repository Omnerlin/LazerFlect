using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    [Tooltip("The bullet that will be instantiated when firing")]
    public GameObject bulletPrefab;

    [Tooltip("What will rotate with our mouse.")]
    public Transform barrel;

    [Tooltip("Where bullets will be ejected from")]
    public Transform firePoint;

    [Tooltip("How far the preview line will be drawn")]
    public float rayLength = 7;

    [Tooltip("Layers that the line preview should ignore when raycasting.")]
    public LayerMask lineMask;

    [Tooltip("Whether or not to draw the shot preview line")]
    public bool showLine = true;

    private LineRenderer lineRenderer;
    private Camera cam; // Used for finding out how our turret should be rotated

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.SetPositions(new Vector3[50]);
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if(showLine)
        {
            DrawPreviewLine();
        }
        UpdateBarrelRotation();

        if(Input.GetMouseButtonDown(0))
        {
            Bullet bullet = Instantiate(bulletPrefab, firePoint.transform.position, Quaternion.LookRotation(firePoint.forward)).GetComponent<Bullet>();
            bullet.Fire(firePoint.position, firePoint.forward);
        }
    }

    private void DrawPreviewLine()
    {
        int hitCount = 0;
        float lengthLeft = rayLength;
        bool hit = true;
        Vector3 rayOrigin = firePoint.position;
        Vector3 rayDirection = firePoint.forward;
        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(0, rayOrigin);

        while(hit)
        {
            Ray lineRay = new Ray(rayOrigin, rayDirection);
            RaycastHit hitinfo;
            hit = Physics.Raycast(lineRay, out hitinfo, 500, lineMask);
            hitinfo.normal = new Vector3(hitinfo.normal.x, 0, hitinfo.normal.z);
            if (!hit) hitinfo.distance = 500;
            lineRenderer.positionCount++;
            lineRenderer.SetPosition(++hitCount, rayOrigin + lineRay.direction * (hitinfo.distance > lengthLeft ? lengthLeft : hitinfo.distance));
            lengthLeft -= hitinfo.distance;
            if(lengthLeft <= 0 || !hit || !hitinfo.collider.CompareTag("Reflector"))
            {
                hit = false;
            }
            else
            {
                rayOrigin = hitinfo.point;
                rayDirection = Vector3.Reflect(rayDirection, hitinfo.normal);
            }
        }

        lineRenderer.positionCount = hitCount + 1;
    }

    // Rotate barrel to face cursor.
    private void UpdateBarrelRotation()
    {
        Vector3 turret2DPos = cam.WorldToScreenPoint(transform.position);
        Vector3 mousePos = Input.mousePosition;

        Vector3 targetDir = mousePos - turret2DPos;
        float angle = -Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg + 90;

        barrel.rotation = Quaternion.AngleAxis(angle, transform.up);
    }
}
