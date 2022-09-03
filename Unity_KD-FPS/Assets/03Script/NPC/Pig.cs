using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pig : MonoBehaviour
{
    [SerializeField] private string animalName; // 동물의 이름
    [SerializeField] private int hp;            // 동물의 체력

    [SerializeField] private float walkSpeed;   // 걷기 스피드
    [SerializeField] private float runSpeed;    // 뛰기 스피드
    private float applySpeed;                   // 현재 스피드

    private Vector3 direction;  // 랜덤 방향 저장

    // 상태 변수
    private bool isAction;      // 행동 중인지 아닌지 판별
    private bool isWalking;     // 걷는지 안 걷는지 판별
    private bool isRunning;     // 뛰는지 판별
    private bool isDead;        // 죽었는지 판별

    [SerializeField] private float walkTime;    // 걷기 시간
    [SerializeField] private float waitTime;    // 대기 시간
    [SerializeField] private float runTime;     // 뛰기 시간
    private float currentTime;                  // 진행 시간

    // 필요한 컴포넌트
    [SerializeField] private Animator anim;
    [SerializeField] private Rigidbody rigid;
    [SerializeField] private BoxCollider boxCol;
    private AudioSource theAudio;

    [SerializeField] private AudioClip[] pig_sound_Nomal;
    [SerializeField] private AudioClip sound_pig_Hurt;
    [SerializeField] private AudioClip sound_pig_Dead;

    void Start()
    {
        theAudio = GetComponent<AudioSource>();
        currentTime = waitTime;
        isAction = true;
    }

    void Update()
    {
        if(!isDead){
            Move();         // 앞으로 움직이기
            Rotation();     // 회전 방향
            ElapseTime();   // 랜덤 애니메이션    
        }
        
    }

    private void Move()
    {
        if (isWalking || isRunning){
            rigid.MovePosition(transform.position + (transform.forward * applySpeed * Time.deltaTime));
        }
    }

    private void Rotation()
    {
        if (isWalking || isRunning){
            // pig의 방향이 한 프레임 마다 direction 값으로 0.01f 씩 가까워짐.
            Vector3 _rotation = Vector3.Lerp(transform.eulerAngles, new Vector3(0f, direction.y, 0f), 0.01f);

            // Quaternion.Euler(vector3 값) : 벡터값을 쿼터니언 값으로 바꿔준다.
            rigid.MoveRotation(Quaternion.Euler(_rotation));
        }
    }

    private void ElapseTime()
    {
        if (isAction){
            currentTime -= Time.deltaTime;
            if (currentTime <= 0)
                ReSet();
        }
    }

    // 초기화 후 애니메이션 실행
    private void ReSet() {
        isWalking = false; isRunning = false; isAction = true;
        anim.SetBool("Walking", isWalking); anim.SetBool("Running", isRunning);
        applySpeed = walkSpeed;
        direction.Set(0f, Random.Range(0f, 360f), 0f);
        RandomAction();
    }

    // 랜덤 애니메이션
    private void RandomAction()
    {
        RandomSound();
        int _random = Random.Range(0, 4);   // 대기, 풀뜯기, 두리번, 걷기

        if (_random == 0)
            Wait();             // 대기
        else if (_random == 1)
            Eat();              // 풀뜯기
        else if (_random == 2)
            Peek();             // 두리번 거리기
        else if (_random == 3)
            TryWalk();          // 걷기
    }

    private void Wait()
    {
        currentTime = waitTime;
        Debug.Log("대기");
    }

    private void Eat()
    {
        currentTime = waitTime;
        anim.SetTrigger("Eat");
        Debug.Log("풀뜯기");
    }

    private void Peek()
    {
        currentTime = waitTime;
        anim.SetTrigger("Peek");
        Debug.Log("두리번");
    }

    private void TryWalk()
    {
        isWalking = true;
        currentTime = walkTime;
        applySpeed = walkSpeed;
        anim.SetBool("Walking", isWalking);
        Debug.Log("걷기");
    }

    // 뛰기 (피격 될때만)
    public void Run(Vector3 _targetPos)
    {
        direction = Quaternion.LookRotation(transform.position - _targetPos).eulerAngles;

        applySpeed = runSpeed;
        currentTime = runTime;
        isWalking = false;
        isRunning = true;

        anim.SetBool("Running", isRunning);
    }

    // 피격 메소드
    public void Damage(int _dmg, Vector3 _targetPos)
    {
        if (!isDead){
            hp -= _dmg;

            if (hp <= 0){
                Dead();
                return;
            }

            PlaySE(sound_pig_Hurt);
            anim.SetTrigger("Hurt");
            Run(_targetPos);
        }
        
    }

    // 죽음 메소드
    private void Dead()
    {
        PlaySE(sound_pig_Dead);
        isWalking = false;
        isRunning = false;
        isDead = true;
        anim.SetTrigger("Dead");
    }

    // 랜덤 사운드
    private void RandomSound()
    {
        int random = Random.Range(0, 3);    // 일상 사운드 3개
        PlaySE(pig_sound_Nomal[random]);
    }

    // 사운드 호출
    private void PlaySE(AudioClip _clip)
    {
        theAudio.clip = _clip;
        theAudio.Play();
    }
}
