using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    private Vector2 m_Move;
    private bool is_parachute;
    private Rigidbody rb;
    private AudioSource ParaAudio;
    private AudioSource WindAudio;
    private AudioSource RipAudio;
    private int para_num = 0;
    private bool is_auto;       // 자동조정
    private bool is_invincible; // 절대무적
    private float preset_y = 0.5f;

    public Vector2 left_con_pos;
    public Vector2 right_con_pos;

    public float force=10f;
    public float rotate_force = 60f;
    public GameObject EventSystem;
    public GameObject ParaEffect;
    public GameObject WindEffect;
    public GameObject RipEffect;
    // Start is called before the first frame update
    void Start()
    {
        is_parachute = false;
        is_auto = false;
        is_invincible = false;
        rb = gameObject.GetComponent<Rigidbody>();
        ParaAudio = ParaEffect.GetComponent<AudioSource>();
        WindAudio = WindEffect.GetComponent<AudioSource>();
        RipAudio = RipEffect.GetComponent<AudioSource>();
        WindAudio.Play();
    }

    public void OnMove(InputValue value)
    {

        m_Move = value.Get<Vector2>();

    }

    public void OnFire()
    {
        Debug.Log("fire");
        if (para_num > 0 && !(is_parachute))
        {

            para_num--;
            is_parachute = true;
            ParaAudio.Play();
            Debug.Log("para open");
        }
        else if (is_parachute) {

            is_parachute = false;
            RipAudio.Play();
        }
    }

    public void OnEscape()
    {
        EventSystem.GetComponent<EventSystem>().Over();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        WindAudio.volume = (Mathf.Abs(rb.velocity.y)+10) / 70;
        if (left_con_pos != new Vector2(0, 0) || right_con_pos != new Vector2(0, 0)) {

            m_Move = left_con_pos + right_con_pos;

        }


        rb.AddForce(transform.forward * force * m_Move.y);

        



        if (is_parachute)
        {
            rb.AddForce(0, -rb.velocity.y, 0);
        }
        else {
            rb.AddForce(0, -rb.velocity.y*0.1f, 0);
        }


        if (m_Move.x < 0)
        {
            transform.Rotate(0, -rotate_force*Time.deltaTime, 0);
        }
        if(m_Move.x>0) {

            transform.Rotate(0, rotate_force * Time.deltaTime, 0);
        }

        //버그방지
        if (transform.position.y < 2f) {

            transform.position = new Vector3(transform.position.x,2, transform.position.z);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.tag);

        if (other.gameObject.CompareTag("Parachute"))
        {
            para_num++;
            EventSystem.GetComponent<EventSystem>().Point(100);
            other.gameObject.SetActive(false);
        }
        else if (other.gameObject.CompareTag("Point"))
        {
            EventSystem.GetComponent<EventSystem>().Point(200);
            other.gameObject.SetActive(false);
        }
        else if (other.gameObject.CompareTag("Auto"))
        {
            EventSystem.GetComponent<EventSystem>().Point(100);
            is_auto = true;
            other.gameObject.SetActive(false);
            // 위치 자동 조정 (아이템을 거쳐서 조정? 자석?)
        }
        else if (other.gameObject.CompareTag("Invincible"))
        {
            EventSystem.GetComponent<EventSystem>().Point(100);
            is_invincible = true;
            other.gameObject.SetActive(false);
        }
        else if (other.gameObject.CompareTag("Obstacle"))
        {
            if (!is_invincible) // &&!(is_auto)?
            {
                // EventSystem.GetComponent<EventSystem>().fail(); // not clear not over but fail
            }
            else
            {
                is_invincible = false;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Clear")&& is_parachute)
        {

            EventSystem.GetComponent<EventSystem>().Clear();

        }
        else
        {

            EventSystem.GetComponent<EventSystem>().Fail();
        }
    }

}
