using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfViewAngle : MonoBehaviour
{
    [SerializeField] private float viewAngle;       // 시야각      (120도)
    [SerializeField] private float viewDisatance;   // 시야 거리   (10m)
    [SerializeField] private LayerMask targetMask;  // 타겟 마스크 (Player)

    private Pig thePig;

    void Start() {
        thePig = GetComponent<Pig>();
    }

    void Update()
    {
        View();
    }

    private Vector3 BoundaryAngle(float _angle)
    {
        _angle += transform.eulerAngles.y;

        return new Vector3(Mathf.Sin(_angle * Mathf.Deg2Rad), 0f, Mathf.Cos(_angle * Mathf.Deg2Rad));
    }

    private void View()
    {
        Vector3 _leftBoundary = BoundaryAngle(-viewAngle * 0.5f); 
        Vector3 _rightBoundary = BoundaryAngle(viewAngle * 0.5f);

        Debug.DrawRay(transform.position + transform.up, _leftBoundary, Color.red);
        Debug.DrawRay(transform.position + transform.up, _rightBoundary, Color.red);

        Collider[] _target = Physics.OverlapSphere(transform.position, viewDisatance, targetMask);

        for (int i = 0; i < _target.Length; i++){
            Transform _targetTf = _target[i].transform;
            if (_targetTf.name == "Player"){
                Vector3 _direction = (_targetTf.position - transform.position).normalized;
                float _angle = Vector3.Angle(_direction, transform.forward);

                if (_angle < viewAngle * 0.5f){
                    RaycastHit _hit;
                    if (Physics.Raycast(transform.position + transform.up, _direction, out _hit, viewDisatance)){
                        if (_hit.transform.name == "Player"){
                            Debug.Log("플레이어가 돼지 시야 내에 있습니다.");
                            Debug.DrawRay(transform.position + transform.up, _direction, Color.blue);
                            thePig.Run(_hit.transform.position);
                        }
                    }
                }
            }
        }
    }
}
