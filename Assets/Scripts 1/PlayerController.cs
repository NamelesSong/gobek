using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.InputSystem;
using UnityEditor.U2D;
using Unity.VisualScripting;

public class PlayerController : MonoBehaviour
{
    public Slider healthSlider;
    public Slider easeHealthSlider;
    public float maxHealth = 100f;
    public float currentHealth;
    private float lerpSpeed = 0.05f;


    public Rigidbody rb;
    public Transform cam;
    private Animator animator;
    /*private GameObject oyuncu;
    private GameObject düþman;*/

    //private Vector2 move;
    //public Vector3 InputKey;

    public float speed, jumpForce;
    public bool grounded;

    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    public bool isAttacking = false;
    public bool CanAttack = true;
    public float AttackCooldown = 1.0f;

    public Image StaminaBar;
    public float Stamina, MaxStamina;
    public float JumpCost;
    public float AttackCost;
    public float ChargeRate;

    private Coroutine recharge;

    /*public float dodgeDuration;
    private Vector3 dodgeDistance = Vector3.zero;
    float elapsedTime;*/


    //public EnemyScript bruh;
    //private GameObject enemy = GameObject.FindWithTag("Enemy");

    //public int attackDamage = 40;


    /*public void OnMove(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>(); // input sisteminden tamamen kurtulmam gerekebilir
    }*/

    /*public void OnLook(InputAction.CallbackContext context)
    {
        look = context.ReadValue<Vector2>();
    }*/

    /*public void OnJump(InputAction.CallbackContext context)
    {
        Jump();
    }*/
    //input Action'dan kurtul

    void Update()
    {
        /*Kýrmýzý.fillAmount = currentHealth / maxHealth;

        if (Kýrmýzý.fillAmount != currentHealth)
        {
            Kýrmýzý.fillAmount = currentHealth;
        }

        if (Kýrmýzý.fillAmount != Sarý.fillAmount)
        {
            Sarý.fillAmount = Mathf.Lerp(Sarý.fillAmount, currentHealth, lerpSpeed);
        }*/

        if (healthSlider.value != currentHealth)
        {
            healthSlider.value = currentHealth;
        }

        if (healthSlider.value != easeHealthSlider.value)
        {
            easeHealthSlider.value = Mathf.Lerp(easeHealthSlider.value, currentHealth, lerpSpeed);
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (CanAttack)
            {
                Attack();
            }
            Stamina -= AttackCost;
            if (Stamina < 0)
            {
                Stamina = 0;
            }
            StaminaBar.fillAmount = Stamina / MaxStamina;

            if (recharge != null)
            {
                StopCoroutine(recharge);
            }
            recharge = StartCoroutine(rechargeStamina());
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            Dodge();
            /*Vector3 startPosition = transform.position;
            Vector3 targetPosition = transform.position + new Vector3(5, 0, 0);

            var step = speed * Time.deltaTime * 20;

            transform.position = Vector3.MoveTowards(startPosition, targetPosition, step);*/
            //rb.velocity = new Vector3(5 * speed, 0, 0);

            /*Vector3 dash = new Vector3(20, 0, 0);
            rb.AddForce(dash*speed, ForceMode.Acceleration);*/
        }

        if (Input.GetKeyDown(KeyCode.Space) && Stamina > 0)
        {
            Stamina -= JumpCost;

            if (Stamina < 0)
            {
                Stamina = 0;
            }

            Jump();

            StaminaBar.fillAmount = Stamina / MaxStamina;

            if (recharge != null)
            {
                StopCoroutine(recharge);
            }

            recharge = StartCoroutine(rechargeStamina());
        }
    }

    public void TakeDamage(float damage) //silah script ine  taþýmaya çalýþ
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }

        void Die()
        {
            Debug.Log("Ölü");
            //ölme animasyonu
        }
    }
    void Attack()
    {
        animator.SetTrigger("Attack");
        isAttacking = true;
        CanAttack = false;
        StartCoroutine(ResetAttackCooldown());
        //bruh.GetComponent<EnemyScript>().TakeDamage(attackDamage);
    }
    IEnumerator Iframe()
    {
        Physics.IgnoreLayerCollision(3, 6);
        //transform.position = Vector3.MoveTowards(transform.position, target.position, step);
        yield return new WaitForSeconds(1f);
        Physics.IgnoreLayerCollision(3, 6, false);
        //jumpwait i kopyala
    }
    void Move()
    {
        float horInput = Input.GetAxisRaw("Horizontal") * speed;
        float verInput = Input.GetAxisRaw("Vertical") * speed;

        Vector3 camForward = cam.forward.normalized;
        Vector3 camRight = cam.right.normalized;

        camForward.y = 0;
        camRight.y = 0;

        Vector3 forwardRelative = verInput * camForward;
        Vector3 rightRelative = horInput * camRight;

        Vector3 moveDir = forwardRelative + rightRelative;

        float inputMagnitude = Mathf.Clamp01(moveDir.magnitude);

        if (Input.GetKey(KeyCode.LeftShift) == false)
        {
            inputMagnitude /= 2;
        }

        if (Input.GetKey(KeyCode.LeftShift) == true)
        {
            moveDir *= 2;
        }

        animator.SetFloat("Input Magnitude", inputMagnitude, 0.05f, Time.deltaTime);

        rb.velocity = new Vector3(moveDir.x, rb.velocity.y, moveDir.z); //movePosition koymalý mý?

        //transform.forward = new Vector3(rb.velocity.x, 0, rb.velocity.z);

        //rb.AddForce(moveDir, ForceMode.Force);
        //rb.AddForce(moveDir * speed); // bu kod çalýþýyor ve mümkünse bunun bir varyasyonunu zýplama sýrasýnda momentum korumak için kullanmalý 
        //ya da en azýndan momentum illüzyonu vermek için kullanýlabilir
    }
    IEnumerator JumpWait()
    {
        yield return new WaitForSeconds(0.4f);

        Vector3 jumpForces = Vector3.zero;

        if (grounded)
        {
            jumpForces = Vector3.up * jumpForce;
        }

        rb.AddForce(jumpForces, ForceMode.VelocityChange);
    }
    public void Jump()
    {
        animator.SetTrigger("Jump");

        StartCoroutine(JumpWait());
    }

    IEnumerator ResetAttackCooldown()
    {
        StartCoroutine(ResetAttackBool());
        yield return new WaitForSeconds(AttackCooldown);
        CanAttack = true;
    }

    IEnumerator ResetAttackBool()
    {
        yield return new WaitForSeconds(1.0f);
        isAttacking = false;
    }

    private IEnumerator rechargeStamina()
    {
        yield return new WaitForSeconds(1f);

        while (Stamina < MaxStamina)
        {
            Stamina += ChargeRate / 10f;
            if (Stamina > MaxStamina)
            {
                Stamina = MaxStamina;
            }
            StaminaBar.fillAmount = Stamina / MaxStamina;
            yield return new WaitForSeconds(.1f);
        }

    }


    private void FixedUpdate()
    {
        if (grounded)
        {
            Move(); // momentumu korumak gibi zorunluluk yok, koþma hýzýndan sonra zýplamaya bir mesafe tanýmlanabilir
        }

        /*if (Input.GetKeyDown(KeyCode.F))
        {
            rb.velocity = new Vector3(5 * speed, 0, 0);
        }*/

    }


    void Look()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 movementInput = new Vector3(horizontalInput, 0, verticalInput).normalized;

        if (movementInput != Vector3.zero)
        {
            float targetAngle = Mathf.Atan2(movementInput.x, movementInput.z) * Mathf.Rad2Deg + cam.eulerAngles.y; //karakteri kamerayý baz alarak hareket ettirmek lazým
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime); //onun için de buna benzer bir mantýk lazým?

            rb.MoveRotation(Quaternion.Euler(0f, angle, 0f));
        }
    }

    public void Dodge()
    {
        animator.SetTrigger("IFrame");
       
        StartCoroutine(Iframe());
    }
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
        //bruh = GameObject.FindGameObjectWithTag("Enemy").GetComponent<Enemy>();
        /*düþman = GameObject.FindWithTag("Enemy");
        oyuncu = GameObject.FindWithTag("Player");*/
    }

    void LateUpdate()
    {
        Look();
    }

    public void SetGrounded(bool state)
    {
        grounded = state;
    }
}