using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/* Gestion de déplacement et du saut du personnage à l'aide des touches : a, d et w      
* Gestion des détections de collision entre le personnage et les objets du jeu  
* Par: Vahik Toroussian
* Modifié: 5/12/2018
*/
public class ControlerPersonnage : MonoBehaviour
{
    float vitesseX;      //vitesse horizontale actuelle
    public float vitesseXMax;   //vitesse horizontale Maximale désirée
    float vitesseY;      //vitesse verticale 
    public float vitesseSaut;   //vitesse de saut désirée
    bool partieTerminee;

    public bool enAttaque;

    /* Détection des touches et modification de la vitesse de déplacement;
       "a" et "d" pour avancer et reculer, "w" pour sauter
    */
    void Update ()
    {

        if (!partieTerminee)
        {
            // déplacement vers la gauche
            if (Input.GetKey("a") || Input.GetKey(KeyCode.LeftArrow))
            {
                vitesseX = -vitesseXMax;
                GetComponent<SpriteRenderer>().flipX = true;

            }
            else if (Input.GetKey("d") || Input.GetKey(KeyCode.RightArrow))   //déplacement vers la droite
            {
                vitesseX = vitesseXMax;
                GetComponent<SpriteRenderer>().flipX = false;
            }
            else
            {
                vitesseX = GetComponent<Rigidbody2D>().velocity.x;  //mémorise vitesse actuelle en X
            }


            //print(Physics2D.OverlapCircle(transform.position, 0.5f)== true);

            // sauter l'objet à l'aide la touche "w"
            if (Input.GetKeyDown("w") && Physics2D.OverlapCircle(transform.position, 0.5f) || Input.GetKeyDown(KeyCode.UpArrow) && Physics2D.OverlapCircle(transform.position, 0.5f))
            {
                vitesseY = vitesseSaut;
                GetComponent<Animator>().SetBool("saut", true);
            }
            else
            {
                vitesseY = GetComponent<Rigidbody2D>().velocity.y;  //vitesse actuelle verticale
            }

            if (Input.GetKeyDown(KeyCode.Space) && !enAttaque)
            {
                enAttaque = true;
                GetComponent<Animator>().SetTrigger("attaque");
                GetComponent<Animator>().SetBool("saut", false);
                Invoke("arretAttaque", 0.4f);
                
            }

            if (enAttaque && Mathf.Abs(vitesseX) <=vitesseXMax)
            {
                vitesseX *= 5; 
            }

            //Applique les vitesses en X et Y
            GetComponent<Rigidbody2D>().velocity = new Vector2(vitesseX, vitesseY);


            //**************************Gestion des animaitons de course et de repos********************************
            //Active l'animation de course si la vitesse de déplacement n'est pas 0, sinon le repos sera jouer par Animator



            if (vitesseX > 0.1f || vitesseX < -0.1f)
            {

                GetComponent<Animator>().SetBool("course", true);
            }
            else
            {

                GetComponent<Animator>().SetBool("course", false);
            }


        }


    }
    void OnCollisionEnter2D(Collision2D infoCollision)
    {

       if(Physics2D.OverlapCircle(transform.position, 0.5f)){

            GetComponent<Animator>().SetBool("saut", false);
        }

        if (infoCollision.gameObject.name == "Bombe"){

            partieTerminee = true;

            GetComponent<Animator>().SetTrigger("mort");

            if (transform.position.x > infoCollision.transform.position.x) {

                GetComponent<Rigidbody2D>().velocity = new Vector2(10, 30);
             } else {
                GetComponent<Rigidbody2D>().velocity = new Vector2(-10, 30);

            }

            Invoke("recommencer", 3f);

        }
        else if(infoCollision.gameObject.tag == "mechant")
        {
            if (enAttaque)
            {
                Destroy(infoCollision.gameObject); //Mettre un delay si ya une animation de mort
            }
            else
            {
                partieTerminee = true;
                GetComponent<Animator>().SetTrigger("mort");
                Invoke("recommencer", 3f);
            }
        }
    

    }

    void recommencer(){

        SceneManager.LoadScene(0);
    }

    void arretAttaque()
    {
        enAttaque = false;
    }
}

