using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Donnees techniques")]
    [HideInInspector] public int idPlayer;
    public typeOfPerso type;
    public float speed;
    public float jumpPower;
    public float cdShoot1;
    private float tickShoot1;
    public float cdShoot2;
    private float tickShoot2;
    private float tickToStopVelocity;
    public float timeToStopVelocity;
    private float controlForced;
    [Header("Robot")]
    public float timeToStopRecul;
    private float tickToStopRecul;
    [Header("Link")]
    [SerializeField] private GameObject grappin;
    [SerializeField] private GameObject chainGrappin;
    private GameObject grappinVisual;
    private bool isAccroched;
    private Vector3 grappinForce;
    private Vector3 accroche;
    [SerializeField] private float grappinCoefForce;
    [Header("Ninja")]
    [SerializeField] private float distanceDash;
    [SerializeField] private float cdAcessoire;
    private float tickAcessoire;
    [Header("Objet secondaire")]
    public OnGround onground;
    public OnWall leftWall;
    public OnWall rightWall;
    [SerializeField] private GameObject bullet1;
    [SerializeField] private GameObject bullet2;
    [SerializeField] private GameObject visualBody;
    [SerializeField] private GameObject visualWeapon;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.gameEnCours)
        {
            if (type == typeOfPerso.link && isAccroched)
                MouvOfGrappin();
            else
                Mouv();

            if (type == typeOfPerso.robot)
                ShootRobot();
            if (type == typeOfPerso.link)
            {
                ShootLink();
                Grappin();
            }
            if (type == typeOfPerso.ninja)
            {
                ShootNinja();
                DashNinja();
            }

            VisualCharacter();
            Jump();
        }

    }
    /// <summary>
    /// permet de savoir si une arme est activé et activable
    /// </summary>
    /// <param name="weapon2"></param>
    /// <returns></returns>
    public bool IsWeaponDeclenchement(bool weapon2)
    {
        if (weapon2 && ((idPlayer == 1 && Input.GetAxis("Shoot2P" + idPlayer) == 1) || (idPlayer == 2 && Input.GetAxis("Shoot2P" + idPlayer) == 1))
       && Time.time > tickShoot2 && (Input.GetAxis("AimHorizontalP" + idPlayer) != 0 || Input.GetAxis("AimVerticalP" + idPlayer) != 0))
            return true;
        if (!weapon2 && ((idPlayer == 1 && Input.GetAxis("ShootP" + idPlayer) == 1) || (idPlayer == 2 && Input.GetAxis("ShootP" + idPlayer) == 1))
               && Time.time > tickShoot1 && (Input.GetAxis("AimHorizontalP" + idPlayer) != 0 || Input.GetAxis("AimVerticalP" + idPlayer) != 0))
            return true;
        return false;
    }
    public void ShootRobot()
    {
        //Tir de Sniper
        if (IsWeaponDeclenchement(false))
        {
            tickShoot1 = Time.time + cdShoot1;
            float inputHorizontal = Input.GetAxis("AimHorizontalP" + idPlayer);
            float inputVertical = Input.GetAxis("AimVerticalP" + idPlayer);
            Vector3 shootDirection = new Vector3(inputHorizontal, inputVertical, 0).normalized;
            Quaternion rotation = Quaternion.LookRotation(shootDirection, Vector3.right);
            GameObject projectile = Instantiate(bullet1, transform.position, rotation);
            projectile.GetComponent<MoveBullet>().Initialisation(shootDirection);
            GetComponent<Rigidbody>().AddForce(shootDirection * (-jumpPower));
            tickToStopRecul = Time.time + timeToStopRecul;
        }
        //Tir de shootgun
        if (IsWeaponDeclenchement(true))
        {
            tickShoot2 = Time.time + cdShoot2;
            float inputHorizontal = Input.GetAxis("AimHorizontalP" + idPlayer);
            float inputVertical = Input.GetAxis("AimVerticalP" + idPlayer);
            Vector3 shootDirection = new Vector3(inputHorizontal, inputVertical, 0).normalized;
            GetComponent<Rigidbody>().AddForce(shootDirection * (-jumpPower));
            tickToStopRecul = Time.time + timeToStopRecul;

            for (int i = 0; i < 4; i++)
            {
                Quaternion aleaRotation = Quaternion.AngleAxis(UnityEngine.Random.Range(-10, 10), Vector3.forward);
                Vector3 shootReel = (aleaRotation * shootDirection).normalized;
                Quaternion rotation = Quaternion.LookRotation(shootReel, Vector3.right);
                GameObject projectile = Instantiate(bullet2, transform.position + (shootReel * 1.4f), rotation);
                projectile.GetComponent<MoveBullet>().Initialisation(shootReel);
            }
        }
    }

    public void ShootLink()
    {
        //Tir de Booomerang
        if (IsWeaponDeclenchement(false))
        {
            tickShoot1 = Time.time + cdShoot1;
            float inputHorizontal = Input.GetAxis("AimHorizontalP" + idPlayer);
            float inputVertical = Input.GetAxis("AimVerticalP" + idPlayer);
            Vector3 shootDirection = new Vector3(inputHorizontal, inputVertical, 0).normalized;
            Quaternion rotation = Quaternion.LookRotation(shootDirection, Vector3.right);
            GameObject projectile = Instantiate(bullet1, transform.position + (shootDirection * 0.8f), rotation);
            projectile.GetComponent<MoveBullet>().Initialisation(shootDirection);
        }
        //Tir de Bomb
        if (IsWeaponDeclenchement(true))
        {
            tickShoot2 = Time.time + cdShoot2;
            float inputHorizontal = Input.GetAxis("AimHorizontalP" + idPlayer);
            float inputVertical = Input.GetAxis("AimVerticalP" + idPlayer);
            Vector3 shootDirection = new Vector3(inputHorizontal, inputVertical, 0).normalized;
            Quaternion rotation = Quaternion.LookRotation(shootDirection, Vector3.right);
            GameObject projectile = Instantiate(bullet2, transform.position + (shootDirection * 1.8f), rotation);
            projectile.GetComponent<MoveBullet>().Initialisation(shootDirection);
        }
    }

    public void ShootNinja()
    {
        //Tir de Kunais
        if (IsWeaponDeclenchement(false))
        {
            tickShoot1 = Time.time + cdShoot1;
            float inputHorizontal = Input.GetAxis("AimHorizontalP" + idPlayer);
            float inputVertical = Input.GetAxis("AimVerticalP" + idPlayer);
            Vector3 shootDirection = new Vector3(inputHorizontal, inputVertical, 0).normalized;
            StartCoroutine(LaunchKunai(shootDirection, 0));
        }
        //Tir épée
        if (IsWeaponDeclenchement(true))
        {
            tickShoot2 = Time.time + cdShoot2;
            float inputHorizontal = Input.GetAxis("AimHorizontalP" + idPlayer);
            float inputVertical = Input.GetAxis("AimVerticalP" + idPlayer);
            Vector3 shootDirection = new Vector3(inputHorizontal, inputVertical, 0).normalized;
            Quaternion rotation = Quaternion.LookRotation(shootDirection, Vector3.right);
            GameObject projectile = Instantiate(bullet2, transform.position + (shootDirection * 1.8f), rotation);
            projectile.GetComponent<SlashSword>().idCaster = idPlayer;
        }
    }
    public IEnumerator LaunchKunai(Vector3 direction, int number)
    {
        int coef = 5;
        if (visualBody.transform.rotation.y == 0)
            coef *= -1;
        Quaternion upKunai = Quaternion.AngleAxis(coef * number, Vector3.forward);
        Vector3 shootReel = (upKunai * direction).normalized;
        Quaternion rotation = Quaternion.LookRotation(shootReel, Vector3.right);
        GameObject projectile = Instantiate(bullet1, transform.position + (shootReel * 1.8f), rotation);
        projectile.GetComponent<MoveBullet>().Initialisation(shootReel);

        yield return new WaitForSeconds(0.1f);
        if (number < 2)
            StartCoroutine(LaunchKunai(direction, number + 1));
    }


    public void Mouv()
    {
        float inputHorizontal = Input.GetAxis("HorizontalP" + idPlayer);
        if (Time.time < tickToStopVelocity)
            inputHorizontal = controlForced;
        float mouvementFrame = inputHorizontal * speed * Time.deltaTime;
        if ((leftWall.isInTheWall && mouvementFrame < 0) || (rightWall.isInTheWall && mouvementFrame > 0))
                mouvementFrame = 0;
        Vector3 mouvVector3 = new Vector3(mouvementFrame, 0, 0);
        transform.Translate(mouvVector3);
        if (inputHorizontal < 0 && GetComponent<Rigidbody>().velocity.x > 0 && Time.time > tickToStopRecul)
            GetComponent<Rigidbody>().velocity = new Vector3(0, GetComponent<Rigidbody>().velocity.y, 0);
        else if (inputHorizontal > 0 && GetComponent<Rigidbody>().velocity.x < 0 && Time.time > tickToStopRecul)
            GetComponent<Rigidbody>().velocity = new Vector3(0, GetComponent<Rigidbody>().velocity.y, 0);
    }
    public void VisualCharacter()
    {
        float inputHorizontal = Input.GetAxis("AimHorizontalP" + idPlayer);
        float inputVertical = Input.GetAxis("AimVerticalP" + idPlayer);
        if (inputHorizontal < 0)
            visualBody.transform.rotation = Quaternion.LookRotation(Vector3.forward);
        else if (inputHorizontal > 0)
            visualBody.transform.rotation = Quaternion.LookRotation(Vector3.back);

        if (inputHorizontal != 0 || inputVertical != 0)
        {
            int x = 0;
            if (inputHorizontal < 0)
                x = 180;
            else if (inputHorizontal > 0)
                x = 0;
            Vector3 currentEulerAngles = new Vector3(0, x, inputVertical * 90) ;
            visualWeapon.transform.eulerAngles = currentEulerAngles;
        }

    }
    public void Jump()
    {
        if (((idPlayer == 1 && Input.GetButtonDown("JumpP" + idPlayer)) || (idPlayer == 2 && Input.GetButtonDown("JumpP" + idPlayer))) && onground.isInTheSol == true)
        {
            float inputHorizontal = Input.GetAxis("HorizontalP" + idPlayer);
            GetComponent<Rigidbody>().AddForce(Vector3.right * jumpPower * inputHorizontal / 3);
            GetComponent<Rigidbody>().AddForce(Vector3.up * jumpPower);
        }
        else if (((idPlayer == 1 && Input.GetButtonDown("JumpP" + idPlayer)) || (idPlayer == 2 && Input.GetButtonDown("JumpP" + idPlayer))) && (leftWall.isInTheWall || rightWall.isInTheWall))
        {
            float coef = 1;
            visualBody.transform.rotation = Quaternion.LookRotation(Vector3.back);
            if (rightWall.isInTheWall)
            {
                coef = -1;
                visualBody.transform.rotation = Quaternion.LookRotation(Vector3.forward);
            }
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            GetComponent<Rigidbody>().AddForce(Vector3.right * jumpPower * coef / 3);
            GetComponent<Rigidbody>().AddForce(Vector3.up * jumpPower);
            tickToStopVelocity = Time.time + timeToStopVelocity;
            controlForced = coef;
        }

    }

    public void DashNinja()
    {
        if (((idPlayer == 1 && Input.GetButtonDown("AccessoireP" + idPlayer)) || (idPlayer == 2 && Input.GetButtonDown("AccessoireP" + idPlayer)))
       && Time.time > tickAcessoire && (Input.GetAxis("AimHorizontalP" + idPlayer) != 0 || Input.GetAxis("AimVerticalP" + idPlayer) != 0))
        {
            tickAcessoire = Time.time + cdAcessoire;
            Vector3 deplacement = new Vector3(Input.GetAxis("AimHorizontalP" + idPlayer), Input.GetAxis("AimVerticalP" + idPlayer), 0).normalized;
            transform.Translate(deplacement * distanceDash);
        }
    }
    public void Grappin()
    {
        //Tir de Grappin de Link
        if (((idPlayer == 1 && Input.GetButtonDown("AccessoireP" + idPlayer)) || (idPlayer == 2 && Input.GetButtonDown("AccessoireP" + idPlayer)))
       && Time.time > tickAcessoire && (Input.GetAxis("AimHorizontalP" + idPlayer) != 0 || Input.GetAxis("AimVerticalP" + idPlayer) != 0))
        {
            if (grappinVisual != null)
                Destroy(grappinVisual);
            tickAcessoire = Time.time + cdAcessoire;
            float inputHorizontal = Input.GetAxis("AimHorizontalP" + idPlayer);
            float inputVertical = Input.GetAxis("AimVerticalP" + idPlayer);
            Vector3 shootDirection = new Vector3(inputHorizontal, inputVertical, 0).normalized;
            Quaternion rotation = Quaternion.LookRotation(shootDirection, Vector3.right);
            GameObject projectile = Instantiate(grappin, transform.position + (shootDirection * 0.5f), rotation);
            projectile.GetComponent<MoveGrappin>().Initialisation(shootDirection, gameObject);
        }
        //Se decrocher
        else if (isAccroched && (idPlayer == 1 && Input.GetButtonUp("AccessoireP" + idPlayer)) || (idPlayer == 2 && Input.GetButtonUp("AccessoireP" + idPlayer)))
        {
            isAccroched = false;
            Destroy(grappinVisual);
            GetComponent<Rigidbody>().useGravity = true;
        }
    }
    public void OnDestroy()
    {
        if (grappinVisual != null)
            Destroy(grappinVisual);
    }
    /// <summary>
    /// Pour Link, le grappin accroche un mur
    /// </summary>
    public void Aggrippe(Vector3 pointOfAttach)
    {
        if ((idPlayer == 1 && Input.GetButton("AccessoireP" + idPlayer)) || (idPlayer == 2 && Input.GetButton("AccessoireP" + idPlayer)))
        {
            GetComponent<Rigidbody>().useGravity = false;
            GetComponent<Rigidbody>().AddForce(Vector3.right * (Input.GetAxis("HorizontalP" + idPlayer) * speed * Time.deltaTime));
            accroche = pointOfAttach;
            isAccroched = true;
            Quaternion rotation = Quaternion.LookRotation(accroche - transform.position, Vector3.up);
            grappinVisual = Instantiate(chainGrappin, (transform.position + accroche) / 2, rotation);
            grappinVisual.transform.localScale = new Vector3(1, 1, Vector3.Magnitude(accroche - transform.position));
        }
    }
    /// <summary>
    /// Pour Link, ça remplace le mouv normal quand il est accroché avec le grappin
    /// </summary>
    public void MouvOfGrappin()
    {

        Vector3 vectorLinkToPointAccroche = accroche - transform.position;
        grappinForce = new Vector3(vectorLinkToPointAccroche.x, vectorLinkToPointAccroche.y, 0).normalized;
        Vector3 mouvVector3 = grappinForce * grappinCoefForce * Time.deltaTime;
        //GetComponent<Rigidbody>().velocity = Vector3.zero;
        transform.Translate(mouvVector3);
        grappinVisual.transform.position = (transform.position + accroche) / 2;
        grappinVisual.transform.rotation = Quaternion.LookRotation(accroche - transform.position, Vector3.up);
        grappinVisual.transform.localScale = new Vector3(1, 1, Vector3.Magnitude(accroche - transform.position));

    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("deadZone") && GameManager.instance.gameEnCours)
        {
            GameManager.instance.EndOfGame(this);
        }
        //vérifie si le collider est sa propre épée
        if (other.GetComponent<SlashSword>() != null && other.GetComponent<SlashSword>().idCaster == idPlayer)
            return;
        if (other.gameObject.CompareTag("bullet") && GameManager.instance.gameEnCours)
        {
            GameManager.instance.EndOfGame(this);
        }
    }
}
