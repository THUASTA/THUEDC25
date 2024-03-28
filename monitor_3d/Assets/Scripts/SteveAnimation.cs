using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class SteveAnimation : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject Steve;
    public GameObject Arrow;
    Animator _ator;
    void Start()
    {
        Steve = gameObject;
        _ator = Steve.GetComponent<Animator>();
    }

    public void Run(Vector3 target,float speed)
    {
        Steve = gameObject;

        _ator = Steve.GetComponent<Animator>();
        if (Steve.transform.position != target)
        {
            float RotationSpeed = 1f;
            Steve.transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(target - Steve.transform.position), RotationSpeed);
            Steve.transform.position=Vector3.MoveTowards(transform.position,target, speed * Time.deltaTime);
            _ator.Play("SteveRun");
        }
    }
    public void Dig(Vector3 target)
    {
        float RotationSpeed = 1f;
        Steve.transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(target), RotationSpeed);
        _ator.Play("SteveDig");
    }
    public void PickUp()
    {
        _ator.Play("SteveDig");
    }
    public void PlaceBlock()
    {
        _ator.Play("SteveDig");
    }
    public void TryAttack()
    {
        _ator.Play("SteveAttack");
    }
    public void Attack(Vector3 target)
    {
        float RotationSpeed = 1f;
        Steve.transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(target), RotationSpeed);
        GameObject MyArrow = Instantiate(Arrow, Steve.transform.position, Quaternion.identity);
        _ator.Play("SteveAttack");
        StartCoroutine(ArrowFly(target,MyArrow));
    }

    IEnumerator ArrowFly(Vector3 target,GameObject MyArrow)
    {
        float speed = 10;
        bool isMove = true;
        float distanceToTarget = Vector3.Distance(MyArrow.transform.position, target);
        while (isMove)
        {
            MyArrow.transform.LookAt(target);
            float angle = Mathf.Min(1, Vector3.Distance(MyArrow.transform.position, target) / distanceToTarget) * 45;
            MyArrow.transform.rotation = MyArrow.transform.rotation * Quaternion.Euler(Mathf.Clamp(-angle, -42, 42), 0, 0);
            float currentDistance = Vector3.Distance(MyArrow.transform.position, target);
            if (currentDistance < 0.5f)
            {
                isMove = false;
            }
            MyArrow.transform.Translate(Vector3.forward * Mathf.Min(speed * Time.deltaTime, currentDistance));
            yield return null;
            if(isMove == false)
            {
                MyArrow.transform.position = target;
            }
        }
        Destroy(MyArrow);
    }
}
