using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour
{
    public static bool isActivate = false;

    // 현재 장착된 Hand형 타입 무기.
    [SerializeField]
    private Hand currentHand;
    
    // 공격중?
    private bool isAttack = false;
    private bool isSwing = false;

    private RaycastHit hitInfo;

    void Update()
    {
        if (isActivate)
            TryAttack();
    }

    // 공격 여부 확인
    private void TryAttack()
    {
        if (Input.GetButton("Fire1")){
            if (!isAttack){
                // 코루틴 실행
                StartCoroutine(AttackCoroutine());
            }
        }
    }

    // 공격 진행 코루틴
    IEnumerator AttackCoroutine()
    {
        isAttack = true;

        currentHand.anim.SetTrigger("Attack");

        yield return new WaitForSeconds(currentHand.attackDelayA);
        isSwing = true;

        // 공격 활성화 시점.
        StartCoroutine(HitCoroutine());

        yield return new WaitForSeconds(currentHand.attackDelayB);
        isSwing = false;

        yield return new WaitForSeconds(currentHand.attackDelay - currentHand.attackDelayA - currentHand.attackDelayB);

        isAttack = false;
    }

    // 피격 여부 코루틴
    IEnumerator HitCoroutine()
    {
        while (isSwing){
            if (CheckObject()){
                isSwing = false;
                Debug.Log(hitInfo.transform.name);
            }
            yield return null;
        }
    }

    // Raycast를 통한 피격 확인
    private bool CheckObject()
    {
        if (Physics.Raycast(transform.position, transform.forward, out hitInfo, currentHand.range)){
            return true;
        }
        return false;
    }

    public void HandChange(Hand _hand)
    {
        if (WeaponManager.currentWeapon != null){
            WeaponManager.currentWeapon.gameObject.SetActive(false);
        }

        currentHand = _hand;
        WeaponManager.currentWeapon = currentHand.GetComponent<Transform>();
        WeaponManager.currentWeaponAnim = currentHand.anim;

        currentHand.transform.localPosition = Vector3.zero;
        currentHand.gameObject.SetActive(true);

        isActivate = true;
    }
}
